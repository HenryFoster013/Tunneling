using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SoundUtils;
using static GenericUtils;
using ItemUtils;
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
    const float throw_speed = 5f;

    ItemManager item_manager;
    Transform interact_buffer;
    Interactable interact;
    WorldItem item;
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
        random_tag = random_seed.RandomString(9);
    }

    void DefaultValues(){
        gestures_open = false;
        GestureMenu.SetActive(false);
        item_manager = new ItemManager();
    }

    // Update //

    void Update(){
        SetWatermark();
        SetCursor();
        Gestures();
        DropItem();
        UseItem();
        WorldInteractions();
    }

    // UI
    
    void SetWatermark(){
        string timestamp = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        string fluff = " -0100\nAPOLLYON BODY 2 ";
        WatermarkText.text = timestamp + fluff + random_tag;
    }
    
    void SetCursor(){
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Interactions

    void UseItem(){
        if(Input.GetButtonDown("Use Item"))
            _ViewModelController.UseItem();
        if(RightTriggerDown())
            _ViewModelController.UseItem();
    }

    bool RightTriggerDown(){
        float trigger = Input.GetAxisRaw("Right Trigger");
        if(!right_trigger_lock){
            if(trigger > 0.75f){
                right_trigger_lock = true;
                return true;
            }
        }
        else{
            if(trigger < 0.25f)
                right_trigger_lock = false;
        }
        return false;
    }

    void WorldInteractions(){
        SearchInteracts();
        InteractUI();
        InteractInput();
    }

    void SearchInteracts(){
        interact_buffer = null;
        interact = null;
        item = null;
        RaycastHit hit;
        if(Physics.Raycast(HeadPoint.position, HeadPoint.forward, out hit, interact_distance, InteractLayers))
            MarkInteract(hit.transform);
    }

    void MarkInteract(Transform trans){
        
        if(trans.tag != "Interactable" && trans.tag != "Item")
            return;
        if(trans == interact_buffer) // buffering using a transform to avoid excessive GetComponent calls
            return;
        
        interact_buffer = trans;

        CheckTransform(trans);
        CheckTransform(trans.parent);
    }

    void CheckTransform(Transform trans){
        if(trans == null)
            return;
        if(trans.tag == "Interactable"){
            interact = trans.GetComponent<Interactable>();
            item = null;
        }
        else if(trans.tag == "Item"){
            interact = null;
            item = trans.GetComponent<WorldItem>();
        }
    }

    void InteractUI(){ 
        InteractIcon.SetBool("live", interact != null || item != null);
        if(interact != null)
            InteractText.text = interact.InteractText();
        else if(item != null)
            InteractText.text = item.InteractText();
    }

    void InteractInput(){
        if(Input.GetButtonDown("Interact")){
            if(interact != null)
                interact.Activate();
            if(item != null)
                _ViewModelController.EquipItem(item_manager.PickupWorldItem(item));
        }
    }

    // Items

    public void SpawnItem(ItemInstance item, float speed){
        item_manager.SpawnWorldItem(item, HeadPoint.position, HeadPoint.rotation, HeadPoint.forward * speed);
    }

    void DropItem(){
        if(!Input.GetButtonDown("Drop Item"))
            return;
        ItemInstance dropped_item = _ViewModelController.EquippedItem();
        if(!_ViewModelController.DropItem())
            return;
        SpawnItem(dropped_item, throw_speed);
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
        if(Input.GetButtonDown("Gesture Menu")){
            gestures_open = !gestures_open;
            GestureMenu.SetActive(gestures_open);
            PlaySFX("UI_Crack", SFX_Lookup);
        }
    }
}