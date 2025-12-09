using System.Collections;
using System.Linq;
using UnityEngine;
using static SoundUtils;

public class ProceduralMovementAPI : MonoBehaviour
{
    [System.Serializable] //each leg has this class
    public class ProceduralLeg
    {
        public Transform target;      
        public Transform distanceRef; 
        public Transform customStepTarget;
        public Transform caster;

        [HideInInspector] public Vector3 horizontalOffset;

        [HideInInspector] public bool stepping = false;
        [HideInInspector] public float stepProgress = 0f;
        [HideInInspector]public float randomXOffset;
        [HideInInspector] public float randomZOffset;

        [HideInInspector] public Vector3 startPos;
        [HideInInspector] public Vector3 desiredPos;
        [HideInInspector] public Vector3 plantedPos; //keeps foot locked when not stepping

        [HideInInspector] public int customStepPhase = 0; // 0 = to custom target, 1 = to distanceRef
        [HideInInspector] public float customPhaseProgress = 0f;
        [HideInInspector] public Vector3 customDesiredPos;

        public void UpdateGroundPosition(LayerMask groundLayer, float rayStartOffset, float maxDistance = 10f, Vector3 surfaceNormal = default)
        {
            if (distanceRef == null) return;

            //if no surface normal provided, default to up
            if (surfaceNormal == default) surfaceNormal = Vector3.up;

            //ray starts slightly above the foot along the surface normal
            Vector3 rayOrigin = caster.position;
            Vector3 rayDirection = -surfaceNormal; // always toward the surface

            Debug.DrawRay(rayOrigin, rayDirection * (maxDistance + rayStartOffset), Color.green);

            if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hit, Mathf.Infinity, groundLayer))
            {
                desiredPos = hit.point;
                desiredPos.x += randomXOffset;
                desiredPos.z += randomZOffset;
                distanceRef.position = desiredPos;
            }
        }
    }
    [System.Serializable]
    public class LegPair
    {
        public int[] legIndices;
    }

    [Header("Torso")]
    public GameObject torso;
    private float torsoDefaultHeight;
    public LayerMask groundLayer;
    public float torsoHeightOffset = 0f;
    public float heightSmoothSpeed = 5f;
    private float currentTorsoY;

    [Header("Legs")]
    public ProceduralLeg[] legs;


    [Header("Step Settings")]
    public float legFollowThreshold = 0.5f;
    public float stepSpeed = 2f;
    public float stepHeight = 0.3f;
    private bool isPairStepping = false;
    //step pairs for diagonal walk
    public LegPair[] stepPairs;
    private int stepPairIndex = 0;
    public float offsetRange;

    [SerializeField] SoundEffect StepSound;

    [Header("Bobbing")]
    public bool Bobbing;
    private float bobOffset = 0f;
    private float bobTimer;
    private float bobDuration = 0.5f;
    private bool isBobbing = false;

    [Header("Climbing")]
    public bool CanClimb;
    private bool isClimbing;
    public SphereCollider climbField;
    private Orientation CurrentPlane = Orientation.Normal;
    private Vector3 climbStartForward;
    private float rotationSmoothSpeed = 10f; 


    [Header("Grounded RayCast Settings")]
    public float rayStartOffset;

    [Header("Movement Settings")]
    private Vector3 moveTarget;
    public float moveSpeed = 3f;

    //[Header("Collision Settings")]
    //TODO


    public enum Orientation{
        Normal, //default
        UpsideDown, //180 degrees
        Sideways //90 degrees
    }

    void Start()
    {
        torsoDefaultHeight = torsoHeightOffset;

        currentTorsoY = torso.transform.position.y;

        AutoAssignStepPair();
        //set planted positions
        foreach (ProceduralLeg leg in legs){
            leg.plantedPos = leg.target.position;
        }

    }


    public void SetTargetPosition(Vector3 pos)
    {
        moveTarget = pos;
    }

    public Orientation GetOrientation()
    {
        return CurrentPlane;
    }

    void StartBob()
    {
        if (!isBobbing)
        {
            bobOffset = Random.Range(0.1f, 0.15f); // pick once
            isBobbing = true;
            bobTimer = 0f;
        }
    }

    public float getHeightOffset()
    {
        return torsoHeightOffset;
    }
    void BobManager()
    {
        if (!isBobbing) return;

        bobTimer += Time.deltaTime;

        if (bobTimer >= bobDuration)
        {
            bobOffset = 0f;
            isBobbing = false;
        }
    }

    public bool getClimbing(){
        return isClimbing;
    }

    void climbingManager()
    {
        if (!CanClimb)
        {
            isClimbing = false;
            CurrentPlane = Orientation.Normal;
            return;
        }

        //detect climbable surfaces
        Collider[] hits = Physics.OverlapSphere(climbField.transform.position, climbField.radius);
        bool onClimbable = false;
        Vector3 climbNormal = Vector3.up;
        Vector3 closestPoint = torso.transform.position;
        float closestDistance = float.MaxValue;

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Climbable"))
            {
                onClimbable = true;
                
                //find and use the closest climbable surface
                Vector3 hitClosestPoint = hit.ClosestPoint(torso.transform.position);
                float distance = Vector3.Distance(torso.transform.position, hitClosestPoint);
                
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestPoint = hitClosestPoint;
                    
                    //normal pointing away from wall
                    climbNormal = (torso.transform.position - closestPoint).normalized;
                    if (climbNormal.magnitude < 0.01f) //counteract zero vector message/error
                        climbNormal = -hit.transform.forward;
                    
                    // decide orientation with smoother transitions
                    float dotUp = Vector3.Dot(climbNormal, Vector3.up);
                    
                    //use hysteresis to prevent rapid orientation switching
                    if (dotUp > 0.7f)
                        CurrentPlane = Orientation.Normal;
                    else if (dotUp < -0.7f)
                        CurrentPlane = Orientation.UpsideDown;
                    else
                        CurrentPlane = Orientation.Sideways;
                }
            }
        }

        //smooth transition when starting/stopping climbing
        if (onClimbable && !isClimbing)
        {
            // when starting to climb, store the current forward direction
            climbStartForward = torso.transform.forward;
        }
        
        isClimbing = onClimbable;

        if (isClimbing)
        {
            //apply rotation
            ApplyClimbRotationSmooth(climbNormal);
            
            //apply height offset along new normal
            Vector3 desiredPos = closestPoint + climbNormal * (torsoHeightOffset - bobOffset);
            torso.transform.position = Vector3.Lerp(torso.transform.position, desiredPos, Time.deltaTime * heightSmoothSpeed);
        }
        else
        {
            //smoothly return to normal orientation when not climbing
            torso.transform.rotation = Quaternion.Slerp(torso.transform.rotation, 
                Quaternion.LookRotation(torso.transform.forward, Vector3.up), 
                Time.deltaTime * rotationSmoothSpeed);
        }
    }

    void ApplyClimbRotationSmooth(Vector3 surfaceNormal)
    {
        //calculate forward direction with better handling for transitions
        Vector3 currentForward = torso.transform.forward;
        
        //when surface normal changes dramatically, preserve the character's intended movement direction
        Vector3 projectedForward = Vector3.ProjectOnPlane(currentForward, surfaceNormal);
        
        //if projection fails (vector too small), use a fallback
        if (projectedForward.magnitude < 0.1f)
        {
            //preserve horizontal movement direction
            Vector3 horizontalForward = Vector3.ProjectOnPlane(climbStartForward, Vector3.up);
            if (horizontalForward.magnitude > 0.1f)
            {
                projectedForward = Vector3.ProjectOnPlane(horizontalForward, surfaceNormal);
            }
            else
            {
                // worst case: use character's right cross surface normal
                projectedForward = Vector3.Cross(torso.transform.right, surfaceNormal);
            }
        }
        
        projectedForward = projectedForward.normalized;
        
        //smooth rotation transition
        Quaternion targetRotation = Quaternion.LookRotation(projectedForward, surfaceNormal);
        torso.transform.rotation = Quaternion.Slerp(torso.transform.rotation, 
            targetRotation, Time.deltaTime * rotationSmoothSpeed);
    }

    //smoother transition option
    void ApplyClimbRotationSmooth2(Vector3 surfaceNormal)
    {
        //blend between current up and target surface normal
        Vector3 blendedUp = Vector3.Slerp(torso.transform.up, surfaceNormal, Time.deltaTime * rotationSmoothSpeed);
        
        //preserve forward direction as much as possible
        Vector3 currentForward = torso.transform.forward;
        Vector3 right = Vector3.Cross(blendedUp, currentForward);
        
        if (right.magnitude < 0.1f)
        {
            currentForward = Vector3.Cross(torso.transform.right, blendedUp);
        }
        
        Vector3 forward = Vector3.Cross(right, blendedUp).normalized;
        
        Quaternion targetRotation = Quaternion.LookRotation(forward, blendedUp);
        torso.transform.rotation = targetRotation;
    }
    void StoreOffsets(){
        // store horizontal offsets (might not be needed)
        foreach (ProceduralLeg leg in legs){
            leg.horizontalOffset = leg.distanceRef.position - torso.transform.position;
        }
    }
    
    void fixOrientation(){
        Vector3 rot = torso.transform.eulerAngles;

        //normalize angles to -180..180
        float x = Mathf.DeltaAngle(0, rot.x);
        float z = Mathf.DeltaAngle(0, rot.z);

        //how close must the AI be to upright
        const float uprightThreshold = 10f;

        //if X and Z are both close to 0°, the spider is upright
        if (Mathf.Abs(x) < uprightThreshold && Mathf.Abs(z) < uprightThreshold)
        {
            SetOrientation(Orientation.Normal);
        }
    }

    public void SetOrientation(Orientation o){
        CurrentPlane = o;
    }

    void Update()
    {   
        MaintainTorsoHeightAndRotation();
        UpdateDistancePointers();
        GroundedChecker(rayStartOffset);
        KeepFeetPlanted();
        PlainRotationManager();
        BobManager();
        climbingManager();
        fixOrientation();

        //check if step pair should move
        TryStepPairs();
        UpdateStepMotion();

    }

    //automatically assign the steppairs if none have been set.
    void AutoAssignStepPair(){
        if (stepPairs == null || stepPairs.Length == 0)
        {
            stepPairs = new LegPair[legs.Length];
            for (int i = 0; i < legs.Length; i++)
            {
                stepPairs[i] = new LegPair
                {
                    legIndices = new int[] { i }
                };
            }
        }
    }

    void GroundedChecker(float offset){
    // pass the torso's current surface normal to each leg
        Vector3 surfaceNormal = torso.transform.up; 

        foreach (ProceduralLeg leg in legs){
            leg.UpdateGroundPosition(groundLayer, offset, 10f, surfaceNormal);
        }
}

    void KeepFeetPlanted(){
        //keep feet planted when not stepping
        foreach (ProceduralLeg leg in legs){
            MaintainPlantedFeet(leg);
        }
    }

    void UpdateStepMotion(){
        //update stepping motion
        foreach (ProceduralLeg leg in legs){
            UpdateLegStep(leg);
        }
    }

    void PlainRotationManager()
    {
        if (isClimbing) return;
        //directions to check for a surface: down/forward/back/left/right
        Vector3[] rayDirections = {
            Vector3.down,
            torso.transform.forward,
            -torso.transform.forward,
            torso.transform.right,
            -torso.transform.right
        };

        RaycastHit hit;
        Vector3 surfaceNormal = Vector3.up; //default rotation
        bool hitFound = false;

        foreach (Vector3 dir in rayDirections)
        {
            Vector3 rayOrigin = torso.transform.position + dir * 0.1f; //slightly offset from center
            if (Physics.Raycast(rayOrigin, dir, out hit, 2f, groundLayer))
            {
                surfaceNormal = hit.normal;
                hitFound = true;
                break; //use the first valid hit
            }
        }

        if (!hitFound)
        {
            surfaceNormal = Vector3.up; //fallback to world up
        }

        //project the forward direction along the detected surface
        Vector3 forward = Vector3.ProjectOnPlane(torso.transform.forward, surfaceNormal).normalized;

        //build rotation aligned to surface normal
        Quaternion targetRotation = Quaternion.LookRotation(forward, surfaceNormal);

        //smooth it out
        torso.transform.rotation = Quaternion.Slerp(torso.transform.rotation, targetRotation, Time.deltaTime * 5f);
    }

    // torso height/rotation maintainer
    void MaintainTorsoHeightAndRotation()
    {

        // cast the ray along the torso's local "down"
        Vector3 rayOrigin = torso.transform.position + torso.transform.up * 1f; // slightly above torso
        Vector3 rayDirection = -torso.transform.up; // local down
        float rayLength = 50f; // allow for walls and ceilings

        if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hit, rayLength, groundLayer))
        {
            Debug.DrawRay(rayOrigin, rayDirection * rayLength, Color.red);

            //if (CurrentPlane == Orientation.Normal){
                // smoothly move torso along local down direction
                float targetHeight = hit.point.y + torsoHeightOffset - bobOffset;
                currentTorsoY = Mathf.Lerp(currentTorsoY, targetHeight, Time.deltaTime * heightSmoothSpeed);
                //project torso position along torso.up to hit point for non-flat surfaces
                Vector3 desiredPos = hit.point + torso.transform.up * torsoHeightOffset - torso.transform.up * bobOffset;

                // smooth the position
                torso.transform.position = Vector3.Lerp(torso.transform.position, desiredPos, Time.deltaTime * heightSmoothSpeed);
                //}

            //align torso rotation to surface
            Vector3 forward = Vector3.ProjectOnPlane(torso.transform.forward, hit.normal).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(forward, hit.normal);
            if (!isClimbing){
                torso.transform.rotation = Quaternion.Slerp(torso.transform.rotation, targetRotation, Time.deltaTime * 5f);
                }
        }
        else
        {
            // if nothing hit, keep current position or return to default
            torso.transform.position = Vector3.Lerp(torso.transform.position, torso.transform.position, Time.deltaTime * heightSmoothSpeed);
            if (!isClimbing){
                torso.transform.rotation = Quaternion.Slerp(torso.transform.rotation, Quaternion.identity, Time.deltaTime * 5f);
            }
        }
    }

    //keep distance pointers following torso
    void UpdateDistancePointers()
    {
        foreach (ProceduralLeg legIndex in legs){
            legIndex.distanceRef.position = torso.transform.TransformPoint(
                torso.transform.InverseTransformPoint(legIndex.distanceRef.position));
        }
    }

    // keep planted feet when not stepping
    void MaintainPlantedFeet(ProceduralLeg leg)
    {
        if (!leg.stepping)
        {
            leg.target.position = leg.plantedPos;
        }
    }

    //start steps for the current diagonal pair

    void TryStepPairs()
    {
        if (isPairStepping)
            return; 

        LegPair pair = stepPairs[stepPairIndex];

        ProceduralLeg[] currentPair = new ProceduralLeg[pair.legIndices.Length];

        for (int i = 0; i < pair.legIndices.Length; i++)
        {
            currentPair[i] = legs[pair.legIndices[i]];
        }

        //check if any leg exceeds threshold
        bool shouldStep = false;
        foreach (ProceduralLeg leg in currentPair)
        {
            if (Vector3.Distance(leg.target.position, leg.distanceRef.position) > legFollowThreshold)
            {
                shouldStep = true;
                break;
            }
        }

        if (shouldStep)
        {
            //start all legs in the pair
            foreach (ProceduralLeg leg in currentPair)
                StartStep(leg);

            isPairStepping = true; // lock until pair finishes
        }
    }

    //initialize a leg step

    void StartStep(ProceduralLeg leg)
    {
        leg.randomXOffset = Random.Range (-offsetRange,offsetRange);
        leg.randomZOffset = Random.Range(-offsetRange,offsetRange);

        leg.stepping = true;
        leg.stepProgress = 0f;
        leg.startPos = leg.target.position;

        if (leg.customStepTarget != null)
        {
            // PHASE 0 target
            Vector3 customPos = leg.customStepTarget.position;
            leg.customDesiredPos = new Vector3(
                customPos.x + leg.randomXOffset,
                customPos.y,
                customPos.z + leg.randomZOffset
            );

            leg.customStepPhase = 0;
            leg.customPhaseProgress = 0f;
        }
        else
        {
            //normal step
            leg.desiredPos = leg.distanceRef.position;
            leg.desiredPos.x += leg.randomXOffset;
            leg.desiredPos.z += leg.randomZOffset;
        }

        if (Bobbing){
            StartBob();
        }
}

    //update stepping arc per frame
    void UpdateLegStep(ProceduralLeg leg)
    {
        if (!leg.stepping) return;

        if (leg.customStepTarget != null)
        {
            //determine start and end for the current phase
            Vector3 start, end;

            if (leg.customStepPhase == 0)
            {
                start = leg.startPos;
                end = leg.customStepTarget.position + new Vector3(leg.randomXOffset, 0, leg.randomZOffset);
            }
            else
            {
                start = leg.customStepTarget.position + new Vector3(leg.randomXOffset, 0, leg.randomZOffset);
                end = leg.distanceRef.position + new Vector3(leg.randomXOffset, 0, leg.randomZOffset);
            }

            //advance phase progress
            leg.customPhaseProgress += Time.deltaTime * stepSpeed;
            float t = Mathf.Clamp01(leg.customPhaseProgress);

            //horizontal lerp
            Vector3 horizontal = Vector3.Lerp(new Vector3(start.x, 0, start.z), new Vector3(end.x, 0, end.z), t);

            //vertical arc
            float arc = Mathf.Sin(t * Mathf.PI) * stepHeight;
            float y = Mathf.Lerp(start.y, end.y, t) + arc;

            leg.target.position = new Vector3(horizontal.x, y, horizontal.z);

            //check if phase is complete
            if (t >= 1f)
            {
                leg.customPhaseProgress = 0f; // reset for next phase
                leg.customStepPhase++;

                //both phases finished?
                if (leg.customStepPhase > 1)
                {
                    leg.stepping = false;
                    leg.plantedPos = end;
                    FinishStepPair(leg);
                }
            }

            return;
        }
        else{

            // NORMAL ONE-PHASE STEP

            //Debug.Log(leg.desiredPos);

            leg.stepProgress += Time.deltaTime * stepSpeed;
            float t = Mathf.Clamp01(leg.stepProgress);

            //horizontal motion
            Vector3 flat = Vector3.Lerp(leg.startPos, leg.desiredPos, t);

            //vertical arc
            float arc = Mathf.Sin(t * Mathf.PI) * stepHeight;

            leg.target.position = flat + Vector3.up * arc;


            //finish step
            if (t >= 1f)
            {
                leg.stepping = false;
                leg.plantedPos = leg.desiredPos;

                //check if entire pair has finished stepping
                bool pairDone = true;
                foreach (ProceduralLeg legInPair in stepPairs[stepPairIndex].legIndices
                .Select(i => legs[i])
                .ToArray())
                {
                    if (legInPair.stepping)
                    {
                        pairDone = false;
                        break;
                    }
                }

                if (pairDone)
                {
                    isPairStepping = false; // unlock for next pair
                    PlaySFX(StepSound, leg.plantedPos);
                    stepPairIndex = (stepPairIndex + 1) % stepPairs.Length;
                }
            }
        }
    }

    void FinishStepPair(ProceduralLeg leg)
    {
        bool pairDone = true;

        foreach (ProceduralLeg legInPair in stepPairs[stepPairIndex]
            .legIndices.Select(i => legs[i]))
        {
            if (legInPair.stepping)
            {
                pairDone = false;
                break;
            }
        }

        if (pairDone)
        {
            isPairStepping = false;
            PlaySFX(StepSound, leg.plantedPos);
            stepPairIndex = (stepPairIndex + 1) % stepPairs.Length;
        }
    }
}