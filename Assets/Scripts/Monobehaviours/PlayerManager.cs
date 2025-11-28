using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SoundUtils;
using static GenericUtils;
using RandomUtils;
using TMPro;

public class PlayerManager : MonoBehaviour{

    [Header(" - References - ")]
    [SerializeField] SoundEffectLookup SFX_Lookup;
    [SerializeField] ViewModelController _ViewModelController;
    [Header(" - Interactions - ")]
    [SerializeField] Transform HeadPoint;
    [SerializeField] LayerMask InteractLayers;
    [Header(" - UI - ")]
    [SerializeField] TMP_Text WatermarkText;
    [SerializeField] GameObject GestureMenu;
    [SerializeField] Animator InteractIcon;
    [SerializeField] TMP_Text InteractText;

    const float interact_distance = 2f;

    bool gestures_open, right_trigger_lock;
    Seed random_seed;
    string random_tag;
    Transform interact_buffer;
    Interactable interact;

    // Start //

    void Start(){
        InitialRandomness();
        DefaultValues();
    }

    void InitialRandomness(){
        random_seed = new Seed();
        random_tag = random_seed.RandomString(8);
    }

    void DefaultValues(){
        gestures_open = false;
        GestureMenu.SetActive(false);
    }

    // Update //

    void Update(){
        SetWatermark();
        SetCursor();
        Gestures();
        UseItem();
        WorldInteractions();
    }

    // UI
    
    void SetWatermark(){
        string timestamp = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        string fluff = " -0100\nAPOLLYGON BODY 2 ";
        WatermarkText.text = timestamp + fluff + random_tag;
    }
    
    void SetCursor(){
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Interactions

    void UseItem(){
        if(Input.GetButtonDown("Use Item"))
            _ViewModelController.UseItem();
        RightTriggerDown(UseItem);
    }

    void RightTriggerDown(Action action){
        float trigger = Input.GetAxisRaw("Right Trigger");
        if(!right_trigger_lock){
            if(trigger > 0.75f){
                right_trigger_lock = true;
                action();
            }
        }
        else{
            if(trigger < 0.25f)
                right_trigger_lock = false;
        }
    }

    void WorldInteractions(){
        SearchInteracts();
        InteractUI();
        InteractInput();
    }

    void SearchInteracts(){
        RaycastHit hit;
        if(Physics.Raycast(HeadPoint.position, HeadPoint.forward, out hit, interact_distance, InteractLayers)){
            MarkInteract(hit.transform);
        }
        else{
            interact_buffer = null;
            interact = null;
        }
    }

    void MarkInteract(Transform trans){
        if(trans == interact_buffer) // buffering using a transform to avoid excessive getcomponent calls
            return;
        if(trans.tag != "Interactable")
            return;
        interact_buffer = trans;
        interact = trans.GetComponent<Interactable>();
    }

    void InteractUI(){ 
        
        InteractIcon.SetBool("live", interact != null);
        string inter_disp_text = "";

        if(interact != null){
            inter_disp_text = interact.InteractText();
        }

        InteractText.text = inter_disp_text;
    }

    void InteractInput(){
        if(interact == null)
            return;
        if(Input.GetButtonDown("Interact")){
            interact.Acivate();
        }
    }

    // Gestures

    void Gestures(){
        GestureButton();
        if(gestures_open){
            bool gestured = false;
            if(Input.GetKeyDown("1")){
                _ViewModelController.ThumbsUp();
                gestured = true;
            }
            if(Input.GetKeyDown("2")){
                _ViewModelController.FlipOff();
                gestured = true;
            }
            if(Input.GetKeyDown("3")){
                _ViewModelController.PointThere();
                gestured = true;
            }
            if(Input.GetKeyDown("4")){
                _ViewModelController.PointHere();
                gestured = true;
            }
            if(gestured){
                gestures_open = false;
                GestureMenu.SetActive(gestures_open);
            }
        }
    }

    void GestureButton(){
        if(Input.GetKeyDown("x")){
            gestures_open = !gestures_open;
            GestureMenu.SetActive(gestures_open);
            PlaySFX("UI_Crack", SFX_Lookup);
        }
    }
}