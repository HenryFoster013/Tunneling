using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour{
    
    [SerializeField] Transform Hinge;
    [SerializeField] GameObject DoorCollider;
    
    const float peek_rotation = 25f;
    const float open_rotation = 110f;
    const float hinge_speed = 5f;

    float hinge_rot, target_rot;
    bool open;
    List<Transform> players_in_peek = new List<Transform>();
    
    // Start //

    void Start(){
        Defaults();
    }

    void Defaults(){
        hinge_rot = 0f;
        target_rot = 0f;
        open = false;
        DoorCollider.SetActive(false);
    }

    // Animate //

    void Update(){
        SetTarget();
        LerpRotation();
        CleanPeek();
        EnableCollider();
    }

    void EnableCollider(){
        DoorCollider.SetActive(Mathf.Abs(hinge_rot - target_rot) < 1f);
    }

    void SetTarget(){
        target_rot = 0f;
        if(open)
            target_rot = open_rotation;
        else if(players_in_peek.Count > 0)
            target_rot = peek_rotation;
    }

    void LerpRotation(){
        hinge_rot = Mathf.Lerp(hinge_rot, target_rot, hinge_speed * Time.deltaTime);
        Hinge.localRotation = Quaternion.Euler(0f, hinge_rot, 0f);
    }

    void CleanPeek(){
        if(players_in_peek.Contains(null))
            players_in_peek.Remove(null);
    }

    // Interfacing //

    public void EnterPeek(Transform t){
        if(open)
            return;
        if(t.tag == "Player"){
            if(!players_in_peek.Contains(t))
                players_in_peek.Add(t);
        }
    }

    public void ExitPeek(Transform t){
        if(open)
            return;
        if(t.tag == "Player"){
            if(players_in_peek.Contains(t))
                players_in_peek.Remove(t);
        }
    }

    public void EnterOpen(Transform t){
        if(t.tag == "Player")
            open = true;
    }
}
