using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SoundUtils;
using ItemUtils;

public class WorldItem : MonoBehaviour{
    
    [SerializeField] ItemDefinition Definition;
    [SerializeField] Rigidbody _Rigidbody;

    ItemInstance item_instance;
    bool useable;

    // Setup //

    void Start(){
        MarkUseless();
        StartCoroutine(SetUsableDelay());
    }

    public void LoadInstance(ItemInstance passed_instance){
        item_instance = passed_instance;
    }

    public void LoadDefinition(){
        if(item_instance == null && Definition != null)
            LoadInstance(new ItemInstance(Definition));
    }

    IEnumerator SetUsableDelay(){
        yield return new WaitForEndOfFrame();
        LoadDefinition();
        MarkUsable();
    }

    // Usablity //

    void MarkUseless(){this.transform.tag = "Untagged"; useable = false;}
    void MarkUsable(){this.transform.tag = "Item"; useable = true;}

    // Outside Interaction //

    public void SetVelocity(Vector3 velocity){
        if(_Rigidbody != null)
            _Rigidbody.velocity = velocity;
    }

    public ItemInstance PickUp(){
        if(!useable)
            return null;
        PlaySFX(item_instance.GetPickupSound());
        return item_instance;
    }

    public string InteractText(){
        if(!useable)
            return "";
        return "Take " + item_instance.Name();
    }

    void OnCollisionEnter(Collision other){
        if(!useable)
            return;
        PlaySFX(item_instance.GetCollisionSound());
    }
}