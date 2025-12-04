using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SoundUtils;

public class DoorController : MonoBehaviour{
    
    [Header(" - Main - ")]
    [SerializeField] Transform Hinge;
    [SerializeField] GameObject DoorCollider;
    [SerializeField] float MaxRotation = 110f;
    [Header(" - SFX - ")]
    [SerializeField] SoundEffect PeekSFX;
    [SerializeField] SoundEffect OpenSFX;
    [SerializeField] SoundEffect CloseSFX;

    PlayerManager player;
     
    const float peek_rotation = 25f;
    const float hinge_speed = 5f;
    const float slam_speed = 15f;

    float hinge_rot, target_rot;
    float current_speed;
    float direction = 1f;
    bool open;
    List<Transform> players_in_peek = new List<Transform>();
    
    // Start //

    void Start(){
        StartCoroutine(GetPlayer());
        Defaults();
    }

    IEnumerator GetPlayer(){
        yield return new WaitForEndOfFrame();
        if(player == null)
            player = GameObject.FindGameObjectWithTag("Player Master").GetComponent<PlayerManager>();
    }

    void Defaults(){
        hinge_rot = 0f;
        target_rot = 0f;
        current_speed = hinge_speed;
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
            target_rot = MaxRotation * direction;
        else if(players_in_peek.Count > 0)
            target_rot = peek_rotation * direction;
    }

    void LerpRotation(){
        hinge_rot = Mathf.Lerp(hinge_rot, target_rot, current_speed * Time.deltaTime);
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
            if(players_in_peek.Count == 0)
                PlaySFX(PeekSFX, transform.position);
            if(!players_in_peek.Contains(t)){
                CheckDirection(t);
                players_in_peek.Add(t);
            }
        }
    }

    void CheckDirection(Transform t){
        if(players_in_peek.Count != 0)
            return;

        direction = 1f;
        if(Vector3.Dot(t.position - transform.position, transform.forward) < 0)
            direction = -1f;
    }

    public void ExitPeek(Transform t){
        if(open)
            return;
        if(t.tag == "Player"){
            if(players_in_peek.Contains(t))
                players_in_peek.Remove(t);
            if(players_in_peek.Count == 0)
                PlaySFX(CloseSFX, transform.position);
        }
    }

    public void EnterOpen(Transform t){
        if(open)
            return;
        if(t.tag == "Player"){
            open = true;
            PlaySFX(OpenSFX, transform.position);
            if(player != null)
                player.OpenDoor(this);
        }
    }

    public void Slammed(){
        current_speed = slam_speed;
    }
}
