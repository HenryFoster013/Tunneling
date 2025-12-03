using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SoundUtils;
using static GenericUtils;
using ItemUtils;

public class ViewModelController : MonoBehaviour{

    [Header(" - MAIN - ")]
    [SerializeField] PlayerHeadsUpUI HeadsUp;
    [SerializeField] SoundEffectLookup SFX_Lookup;

    [Header(" - LEFT HAND - ")]
    [SerializeField] GameObject[] AllGestures;
    [SerializeField] GameObject G_ThumbsUp;
    [SerializeField] GameObject G_FlipOff;
    [SerializeField] GameObject G_Here;
    [SerializeField] GameObject G_There;

    [Header(" - RIGHT HAND - ")]
    [SerializeField] GameObject[] AllViewmodels;
    ItemInstance equipped_item;

    [Header("Flashlight")]
    [SerializeField] GameObject FlashlightViewmodel;
    [SerializeField] GameObject FlashlightLight;
    bool flashlight_on;

    [Header("Irish Beverage")]
    [SerializeField] GameObject IrishBeverageViewmodel;
    [SerializeField] GameObject IrishBeverageHeld;
    [SerializeField] GameObject IrishBeverageDrink;
    const float irish_beverage_time = 1f;
    bool irish_beverage_drinking;
    float gesture_time;

    // Main //

    void Start(){
        DisableAll(ref AllViewmodels);
        DisableAll(ref AllGestures);
    }

    void Update(){
        GestureClock();
    }

    void GestureClock(){
        gesture_time -= Time.deltaTime;
        if(gesture_time < 0)
            DisableAll(ref AllGestures);
    }

    // Gestures //

    bool GeneralGesture(){
        if(gesture_time > 0.333f)
            return false;
        DisableAll(ref AllGestures);
        gesture_time = 1f;
        return true;
    }

    public void ThumbsUp(){
        if(!GeneralGesture())
            return;
        
        G_ThumbsUp.SetActive(true);
        PlaySFX("Grunt_Yes", SFX_Lookup);
    }

    public void FlipOff(){
        if(!GeneralGesture())
            return;
        G_FlipOff.SetActive(true);
        PlaySFX("Grunt_No", SFX_Lookup);
    }

    public void PointThere(){
        if(!GeneralGesture())
            return;
        G_There.SetActive(true);
        PlaySFX("Grunt_There", SFX_Lookup);
    }

    public void PointHere(){
        if(!GeneralGesture())
            return;
        G_Here.SetActive(true);
        PlaySFX("Grunt_Here", SFX_Lookup);
    }

    // Items //

    public void EquipItem(ItemInstance item){
        equipped_item = item;
        DisableAll(ref AllViewmodels);
        if(item == null)
            return;

        HeadsUp.SetRecoil(new Vector3(0, -0.5f, 0.1f), 10f, true);
        switch(equipped_item.GetTypeDef()){
            case "FLASHLIGHT":
                DefaultFlashlight();
                return;
            case "IRISH BEVERAGE":
                DefaultIrishBeverage();
                return;
        }
    }

    public bool DropItem(){
        bool valid = equipped_item != null;
        if(valid)
            EquipItem(null);
        return valid;
    }

    public ItemInstance EquippedItem(){return equipped_item;}

    public void UseItem(){
        if(equipped_item == null)
            return;
        switch(equipped_item.GetTypeDef()){
            case "FLASHLIGHT":
                Flashlight();
                return;
            case "IRISH BEVERAGE":
                IrishBeverage();
                return;
        }
    }

    // Flashlight //

    void Flashlight(){
        flashlight_on = !flashlight_on;
        FlashlightLight.SetActive(flashlight_on);
        HeadsUp.SetRecoil(new Vector3(0, -0.1f, -0.1f), 10f, true);
        PlaySFX("Flashlight", SFX_Lookup);
        if(flashlight_on)
            equipped_item.stored_int = 1;
        else
            equipped_item.stored_int = 0;
    }

    void DefaultFlashlight(){
        flashlight_on = (equipped_item.stored_int == 1);
        FlashlightLight.SetActive(flashlight_on);
        FlashlightViewmodel.SetActive(true);
    }

    // Irish Beverage //

    void IrishBeverage(){
        if(irish_beverage_drinking)
            return;
        StartCoroutine(DrinkIrishBeverage());
    }

    IEnumerator DrinkIrishBeverage(){
        irish_beverage_drinking = true;
        RefreshIrishBeverage();
        HeadsUp.SetRecoil(new Vector3(0, -0.3f, 0.3f), 2f, true);
        PlaySFX("Beer_Sip", SFX_Lookup);
        yield return new WaitForSeconds(irish_beverage_time);
        HeadsUp.SetRecoil(new Vector3(0, -0.5f, -0.5f), 7f, true);
        irish_beverage_drinking = false;
        RefreshIrishBeverage();
    }

    void DefaultIrishBeverage(){
        IrishBeverageViewmodel.SetActive(true);
        irish_beverage_drinking = false;
        RefreshIrishBeverage();
    }

    void RefreshIrishBeverage(){
        IrishBeverageDrink.SetActive(irish_beverage_drinking);
        IrishBeverageHeld.SetActive(!irish_beverage_drinking);
    }
}