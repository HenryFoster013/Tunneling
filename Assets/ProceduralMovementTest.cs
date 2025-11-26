using System.Collections;
using UnityEngine;

public class ProceduralMovementTest : MonoBehaviour
{
    [System.Serializable] //each leg has this class
    public class ProceduralLeg
    {
        public Transform target;      
        public Transform distanceRef; 
        public Transform caster;

        [HideInInspector] public Vector3 horizontalOffset;

        [HideInInspector] public bool stepping = false;
        [HideInInspector] public float stepProgress = 0f;
        [HideInInspector] public Vector3 startPos;
        [HideInInspector] public Vector3 desiredPos;
        [HideInInspector] public Vector3 plantedPos; //keeps foot locked when not stepping

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
                distanceRef.position = desiredPos;
            }
        }
    }

    [Header("Torso")]
    public GameObject torso;
    public LayerMask groundLayer;
    public float torsoHeightOffset = 0f;
    public float heightSmoothSpeed = 5f;
    private float currentTorsoY;

    [Header("Legs")]
    public ProceduralLeg frontLeft;
    public ProceduralLeg frontRight;
    public ProceduralLeg backLeft;
    public ProceduralLeg backRight;

    [Header("Step Settings")]
    public float legFollowThreshold = 0.5f;
    public float stepSpeed = 2f;
    public float stepHeight = 0.3f;
    private bool isPairStepping = false;
    //step pairs for diagonal walk
    private ProceduralLeg[][] stepPairs;
    private int stepPairIndex = 0;

    [Header("Grounded RayCast Settings")]
    public float rayStartOffset;

    void Start()
    {
        currentTorsoY = torso.transform.position.y;

        //set planted positions
        frontLeft.plantedPos = frontLeft.target.position;
        frontRight.plantedPos = frontRight.target.position;
        backLeft.plantedPos = backLeft.target.position;
        backRight.plantedPos = backRight.target.position;

        //define diagonal walk pairs
        stepPairs = new ProceduralLeg[][]
        {
            new ProceduralLeg[] { frontLeft, backRight },
            new ProceduralLeg[] { frontRight, backLeft }
        };
    }

    void StoreOffsets(){
        // store horizontal offsets (might not be needed)
        frontLeft.horizontalOffset = frontLeft.distanceRef.position - torso.transform.position;
        frontRight.horizontalOffset = frontRight.distanceRef.position - torso.transform.position;
        backLeft.horizontalOffset = backLeft.distanceRef.position - torso.transform.position;
        backRight.horizontalOffset = backRight.distanceRef.position - torso.transform.position;
    }

    void Update()
    {
        MaintainTorsoHeightAndRotation();
        UpdateDistancePointers();
        GroundedChecker(rayStartOffset);
        KeepFeetPlanted();
        PlainRotationManager();

        //check if step pair should move
        TryStepPairs();
        UpdateStepMotion();
    }

    void GroundedChecker(float offset){
    // pass the torso's current surface normal to each leg
        Vector3 surfaceNormal = torso.transform.up; // or computed via your torso ray
        frontLeft.UpdateGroundPosition(groundLayer, offset, 10f, surfaceNormal);
        frontRight.UpdateGroundPosition(groundLayer, offset, 10f, surfaceNormal);
        backLeft.UpdateGroundPosition(groundLayer, offset, 10f, surfaceNormal);
        backRight.UpdateGroundPosition(groundLayer, offset, 10f, surfaceNormal);
}

    void KeepFeetPlanted(){
        //keep feet planted when not stepping
        MaintainPlantedFeet(frontLeft);
        MaintainPlantedFeet(frontRight);
        MaintainPlantedFeet(backLeft);
        MaintainPlantedFeet(backRight);
    }

    void UpdateStepMotion(){
        //update stepping motion
        UpdateLegStep(frontLeft);
        UpdateLegStep(frontRight);
        UpdateLegStep(backLeft);
        UpdateLegStep(backRight);
    }

    void PlainRotationManager()
    {
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
        //cast a long ray straight down from above the spider
        Vector3 rayOrigin = torso.transform.position + Vector3.up * 1f;
        Vector3 rayDirection = Vector3.down;
        float rayLength = 50f;

        if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hit, rayLength, groundLayer))
        {
            Debug.DrawRay(rayOrigin, rayDirection * rayLength, Color.red);

            //smoothly move torso Y to ground height + offset
            float targetY = hit.point.y + torsoHeightOffset;
            currentTorsoY = Mathf.Lerp(currentTorsoY, targetY, Time.deltaTime * heightSmoothSpeed);

            //optional: Align rotation to surface normal for slopes
            Vector3 forward = Vector3.ProjectOnPlane(torso.transform.forward, hit.normal).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(forward, hit.normal);
            torso.transform.rotation = Quaternion.Slerp(torso.transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
        else
        {
            //if nothing is hit, keep current Y or smoothly return to default height
            currentTorsoY = Mathf.Lerp(currentTorsoY, torso.transform.position.y, Time.deltaTime * heightSmoothSpeed);
            torso.transform.rotation = Quaternion.Slerp(torso.transform.rotation, Quaternion.identity, Time.deltaTime * 5f);
        }

        //smooth Y
        Vector3 pos = torso.transform.position;
        pos.y = currentTorsoY;
        torso.transform.position = pos;
    }

    //keep distance pointers following torso
    void UpdateDistancePointers()
    {
        frontLeft.distanceRef.position = torso.transform.TransformPoint(
            torso.transform.InverseTransformPoint(frontLeft.distanceRef.position));

        frontRight.distanceRef.position = torso.transform.TransformPoint(
            torso.transform.InverseTransformPoint(frontRight.distanceRef.position));

        backLeft.distanceRef.position = torso.transform.TransformPoint(
            torso.transform.InverseTransformPoint(backLeft.distanceRef.position));

        backRight.distanceRef.position = torso.transform.TransformPoint(
            torso.transform.InverseTransformPoint(backRight.distanceRef.position));
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

        ProceduralLeg[] currentPair = stepPairs[stepPairIndex];

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
        leg.stepping = true;
        leg.stepProgress = 0f;
        leg.startPos = leg.target.position;
        leg.desiredPos = leg.distanceRef.position;
    }

    //update stepping arc per frame
    void UpdateLegStep(ProceduralLeg leg)
    {
        if (!leg.stepping) return;

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
            foreach (ProceduralLeg legInPair in stepPairs[stepPairIndex])
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
                stepPairIndex = (stepPairIndex + 1) % stepPairs.Length;
            }
        }
    }
}