using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SoundUtils;
using static GenericUtils;

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
    public ItemType Type;
    [SerializeField] GameObject[] AllViewmodels;

    public enum ItemType{
        None, Flashlight, Irish_Beverage
    }

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

    // Main //

    void Start(){
        EquipItem(Type);
    }

    public void SetItem(ItemType type){
        Type = type;
    }

    // Gestures //

    public void ThumbsUp(){
        DisableAll(ref AllGestures);
        G_ThumbsUp.SetActive(true);
    }

    public void FlipOff(){
        DisableAll(ref AllGestures);
        G_FlipOff.SetActive(true);
    }

    public void PointThere(){
        DisableAll(ref AllGestures);
        G_There.SetActive(true);
    }

    public void PointHere(){
        DisableAll(ref AllGestures);
        G_Here.SetActive(true);
    }

    // Search Actions //

    public void EquipItem(ItemType item){
        Type = item;
        DisableAll(ref AllViewmodels);
        switch(Type){
            case ItemType.Flashlight:
                DefaultFlashlight();
                return;
            case ItemType.Irish_Beverage:
                DefaultIrishBeverage();
                return;
        }
    }

    public void UseItem(){
        switch(Type){
            case ItemType.Flashlight:
                Flashlight();
                return;
            case ItemType.Irish_Beverage:
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
    }

    void DefaultFlashlight(){
        flashlight_on = false;
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
        PlaySFX("Sip_Beer", SFX_Lookup);
        yield return new WaitForSeconds(irish_beverage_time);
        HeadsUp.SetRecoil(new Vector3(0, -0.5f, -0.5f), 7f, true);
        irish_beverage_drinking = false;
        RefreshIrishBeverage();
    }

    void DefaultIrishBeverage(){
        IrishBeverageViewmodel.SetActive(true);
        irish_beverage_drinking = false;
        PlaySFX("Open_Beer", SFX_Lookup);
        RefreshIrishBeverage();
    }

    void RefreshIrishBeverage(){
        IrishBeverageDrink.SetActive(irish_beverage_drinking);
        IrishBeverageHeld.SetActive(!irish_beverage_drinking);
    }
}