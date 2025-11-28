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
    [Header(" - UI - ")]
    [SerializeField] TMP_Text WatermarkText;
    [SerializeField] GameObject GestureMenu;

    bool gestures_open, right_trigger_lock;
    Seed random_seed;
    string random_tag;

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