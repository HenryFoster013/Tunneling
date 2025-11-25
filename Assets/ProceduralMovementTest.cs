using System.Collections;
using UnityEngine;

public class ProceduralMovementTest : MonoBehaviour
{
    [Header("Torso Object")]
    public GameObject torso;

    [Header("Ground Detection")]
    public LayerMask groundLayer;
    public float torsoHeightOffset = 0f;
    public float heightSmoothSpeed = 5f;
    private float currentTorsoY;

    [Header("Leg IK Targets")]
    public Transform frontLeftTarget;
    public Transform frontRightTarget;
    public Transform backLeftTarget;
    public Transform backRightTarget;

    [Header("Leg Distance Pointers (Follow Torso)")]
    public Transform frontLeftDistance;
    public Transform frontRightDistance;
    public Transform backLeftDistance;
    public Transform backRightDistance;
    public float legFollowThreshold = 0.5f;

    [Header("Step Settings")]
    public float stepHeight = 0.3f;
    public float stepSpeed = 2f;

    private bool isStepping = false;

    private bool flStepping = false;
    private bool frStepping = false;
    private bool blStepping = false;
    private bool brStepping = false;

    private int stepIndex = 0; // FL → BL → FR → BR

    private Vector3 FL_position;
    private Vector3 FR_position;
    private Vector3 BL_position;
    private Vector3 BR_position;


    void Start()
    {
        currentTorsoY = torso.transform.position.y;

        FL_position = frontLeftTarget.position;
        FR_position = frontRightTarget.position;
        BL_position = backLeftTarget.position;
        BR_position = backRightTarget.position;

        //initialize distance pointers relative to the torso
        frontLeftDistance.position = frontLeftTarget.position;
        frontRightDistance.position = frontRightTarget.position;
        backLeftDistance.position = backLeftTarget.position;
        backRightDistance.position = backRightTarget.position;

    }

    void MaintainLegPosition(){
        frontLeftTarget.position = FL_position;
        frontRightTarget.position = FR_position;
        backLeftTarget.position = BL_position;
        backRightTarget.position = BR_position;
    }

    void Update()
    {
        MaintainTorsoHeight();
        if (!isStepping){ MaintainLegPosition();}
        UpdateDistancePointers();
        BodyDrivenLegStep();
    }

    //maintain torso height above ground
    void MaintainTorsoHeight()
    {
        RaycastHit hit;
        if (Physics.Raycast(torso.transform.position, Vector3.down, out hit, 10f, groundLayer))
        {
            float targetY = hit.point.y + torsoHeightOffset;
            currentTorsoY = Mathf.Lerp(currentTorsoY, targetY, Time.deltaTime * heightSmoothSpeed);

            Vector3 pos = torso.transform.position;
            pos.y = currentTorsoY;
            torso.transform.position = pos;
        }
    }

    // move distance pointers with torso
    void UpdateDistancePointers()
    {
        // Example: maintain initial local offsets from torso
        frontLeftDistance.position = torso.transform.TransformPoint(torso.transform.InverseTransformPoint(frontLeftDistance.position));
        frontRightDistance.position = torso.transform.TransformPoint(torso.transform.InverseTransformPoint(frontRightDistance.position));
        backLeftDistance.position = torso.transform.TransformPoint(torso.transform.InverseTransformPoint(backLeftDistance.position));
        backRightDistance.position = torso.transform.TransformPoint(torso.transform.InverseTransformPoint(backRightDistance.position));
    }

    //step legs when pointer exceeds threshold
    void BodyDrivenLegStep()
    {
        //if (isStepping) return;

        switch (stepIndex)
        {
            case 0:
                if (Vector3.Distance(frontLeftTarget.position, frontLeftDistance.position) > legFollowThreshold)
                    StartCoroutine(StepLeg(frontLeftTarget, frontLeftDistance));
                break;
            case 1:
                if (Vector3.Distance(backLeftTarget.position, backLeftDistance.position) > legFollowThreshold)
                    StartCoroutine(StepLeg(backLeftTarget, backLeftDistance));
                break;
            case 2:
                if (Vector3.Distance(frontRightTarget.position, frontRightDistance.position) > legFollowThreshold)
                    StartCoroutine(StepLeg(frontRightTarget, frontRightDistance));
                break;
            case 3:
                if (Vector3.Distance(backRightTarget.position, backRightDistance.position) > legFollowThreshold)
                    StartCoroutine(StepLeg(backRightTarget, backRightDistance));
                break;
        }
    }

    //move leg to pointer (pointer stays with torso)
    IEnumerator StepLeg(Transform foot, Transform distancePointer)
    {
         if (foot == frontLeftTarget) flStepping = true;
        else if (foot == frontRightTarget) frStepping = true;
        else if (foot == backLeftTarget) blStepping = true;
        else if (foot == backRightTarget) brStepping = true;

        Vector3 startPos = foot.position;
        Vector3 targetPos = distancePointer.position; // pointer moves with torso

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * stepSpeed; // control speed here
            t = Mathf.Min(t, 1f); // ensure it never exceeds 1

            Vector3 flatMove = Vector3.Lerp(startPos, targetPos, Mathf.SmoothStep(0f, 1f, t));
            float vertical = Mathf.Sin(t * Mathf.PI) * stepHeight;

            foot.position = flatMove + Vector3.up * vertical;

            yield return null;
        }


        // the pointer stays in its current position
        stepIndex = (stepIndex + 1) % 4;

         if (foot == frontLeftTarget) { FL_position = targetPos; flStepping = false; }
        else if (foot == frontRightTarget) { FR_position = targetPos; frStepping = false; }
        else if (foot == backLeftTarget) { BL_position = targetPos; blStepping = false; }
        else if (foot == backRightTarget) { BR_position = targetPos; brStepping = false; }

        isStepping = false;
    }
}