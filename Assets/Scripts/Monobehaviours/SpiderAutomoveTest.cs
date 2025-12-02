using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderAutomoveTest : MonoBehaviour
{
    public GameObject spiderBody;
    public GameObject player;
    public bool ChasePlayer;

    public float xCord, zCord; //target position
    private Vector3 movePosition;

    [SerializeField, Range(0f, 15f)]
    public float speedFactor = 5f; //movement speed
    [SerializeField, Range(0f, 180f)]
    public float rotationSpeed = 180f; //degrees per second
    [SerializeField, Range(0f, 5f)]
    public float alignThreshold = 1f; //how close rotation must be to start moving

    [Header("Spider Front")]
    [SerializeField] private FrontDirection front = FrontDirection.ZPlus;

    public enum FrontDirection
    {
        ZPlus,
        ZMinus,
        XPlus,
        XMinus
    }

    void AvoidObstacle(){
        //TODO
    }

    void Start()
    {
        movePosition = new Vector3(
            xCord,
            spiderBody.transform.position.y,
            zCord
        );
    }

    public void ManualSetPosition(Vector3 location)
    {
        movePosition = new UnityEngine.Vector3(
        location.x,
        spiderBody.transform.position.y,
        location.z
    );
    }

    public bool GetChasePlayer()
    {
        return ChasePlayer;
    }
    public void SetChasePlayer(bool val)
    {
        ChasePlayer = val;
    }

    void RotateAndChase()
    {
        if (ChasePlayer)
        {
            movePosition = new Vector3(
                player.transform.position.x,
                spiderBody.transform.position.y,
                player.transform.position.z
            );
        }

        Vector3 direction = movePosition - spiderBody.transform.position;
        float distance = direction.magnitude;
        
        if (distance < 0.01f) return; // <-- stop if almost at target

        // apply front offset
        Quaternion offset = Quaternion.identity;
        switch (front)
        {
            case FrontDirection.XPlus: offset = Quaternion.Euler(0, -90f, 0); break;
            case FrontDirection.XMinus: offset = Quaternion.Euler(0, 90f, 0); break;
            case FrontDirection.ZMinus: offset = Quaternion.Euler(0, 180f, 0); break;
            case FrontDirection.ZPlus: offset = Quaternion.identity; break;
        }
        Vector3 adjustedDir = offset * direction;

        // target rotation including slope tilt
        Quaternion slopeRotation = Quaternion.LookRotation(adjustedDir.normalized, Vector3.up);

        // horizontal-only rotation toward target (XZ plane)
        Vector3 flatDir = new Vector3(adjustedDir.x, 0f, adjustedDir.z);
        if (flatDir.sqrMagnitude > 0.001f)
        {
            Quaternion horizontalRotation = Quaternion.LookRotation(flatDir, Vector3.up);
            Vector3 slopeEuler = slopeRotation.eulerAngles;
            Vector3 horizontalEuler = horizontalRotation.eulerAngles;
            Vector3 finalEuler = new Vector3(
                slopeEuler.x,
                horizontalEuler.y,
                slopeEuler.z
            );

            spiderBody.transform.rotation = Quaternion.RotateTowards(
                spiderBody.transform.rotation,
                Quaternion.Euler(finalEuler),
                rotationSpeed * Time.deltaTime
            );
        }

        // move toward target only if distance above threshold
        if (distance > 0.01f)
        {
            spiderBody.transform.position = Vector3.MoveTowards(
                spiderBody.transform.position,
                movePosition,
                speedFactor * Time.deltaTime
            );
        }
    }

    void Update()
    {
        RotateAndChase();
    }

    //externally update target position
    public void SetTargetPosition(Vector3 newTarget)
    {
        movePosition = new Vector3(newTarget.x, spiderBody.transform.position.y, newTarget.z);
    }
}