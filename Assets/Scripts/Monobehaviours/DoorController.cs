using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SoundUtils;

public class DoorController : MonoBehaviour{
    
    [Header(" - Main - ")]
    [SerializeField] Transform Hinge;
    [SerializeField] GameObject DoorCollider;
    [Header(" - SFX - ")]
    [SerializeField] SoundEffect PeekSFX;
    [SerializeField] SoundEffect OpenSFX;
    [SerializeField] SoundEffect CloseSFX;

    PlayerManager player;
    
    [SerializeField] float max_rotation = 110f;
    [SerializeField] float peek_rotation = 25f;
    [SerializeField] float hinge_speed = 5f;
    [SerializeField] float close_speed = 8f;
    [SerializeField] float slam_speed = 15f;

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
        target_rot = 0;
        open = false;
        players_in_peek = new List<Transform>();
        hinge_rot = 0f;
        current_speed = hinge_speed;
    }

    public void Close(){
        StartCoroutine(CloseLongSFX());
        current_speed = close_speed;
        target_rot = 0;
        open = false;
        players_in_peek = new List<Transform>();
    }

    IEnumerator CloseLongSFX(){
        PlaySFX(PeekSFX, transform.position);
        yield return new WaitForSeconds(0.3f);
        PlaySFX(CloseSFX, transform.position);
    }

    // Animate //

    void Update(){
        SetTarget();
        LerpRotation();
        CleanPeek();
        EnableCollider();
    }

    void EnableCollider(){
        bool close_enough = Mathf.Abs(hinge_rot - target_rot) < 1f;
        DoorCollider.SetActive(close_enough);
        if(open && close_enough)
            transform.tag = "Interactable";
        else
            transform.tag = "Untagged";
    }

    void SetTarget(){
        target_rot = 0f;
        if(open)
            target_rot = max_rotation * direction;
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
            if(players_in_peek.Count == 0){
                current_speed = hinge_speed;
                PlaySFX(PeekSFX, transform.position);
            }
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
            if(players_in_peek.Contains(t)){
                players_in_peek.Remove(t);
                if(players_in_peek.Count == 0)
                    PlaySFX(CloseSFX, transform.position);
            }
        }
    }

    public void EnterOpen(Transform t){
        if(open)
            return;
        if(t.tag == "Player"){
            open = true;
            current_speed = hinge_speed;
            PlaySFX(OpenSFX, transform.position);
            if(player != null)
                player.OpenDoor(this);
        }
    }

    public void Slammed(){
        current_speed = slam_speed;
    }
}
