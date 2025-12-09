using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationAPI : MonoBehaviour
{
    [Header("References")]
    public ProceduralMovementAPI proceduralMovementAPI;
    public Transform torso;

    [Header("Settings")]
    public bool canClimb;
    public bool useNavMesh;
    public bool isProcedural;

    [Header("Wall Finding")]
    public bool FindingWall = false;
    private Transform ShortestDistanceWall;
    public float YThreshold;
    public float scanRadius = 50f;
    public int rayCount = 36;

    public bool getFindingWall(){
        return FindingWall;
    }

    public void setYThreshold(float y){
        YThreshold = y;
    }

    void Start()
    {
        
    }

    public void FindWall()
    {
    //TODO
    }

    void Update()
    {
        
    }
}
