using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SoundUtils;

public class ViewModelController : MonoBehaviour{

    [Header(" - MAIN - ")]
    public Functionality Type;
    [SerializeField] PlayerHeadsUpUI HeadsUp;
    [SerializeField] SoundEffectLookup SFX_Lookup;
    [Header(" - ADDITIONALS - ")]
    [SerializeField] GameObject ObjectOne;
    
    bool toggle_marker;
    public enum Functionality{
        None,
        Flashlight
    }

    // Main //

    void Start(){
        Defaults();
    }

    // Search Actions //

    public void Defaults(){
        switch(Type){
            case Functionality.Flashlight:
                DefaultFlashlight();
                return;
        }
    }

    public void DoSomething(){
        switch(Type){
            case Functionality.Flashlight:
                Flashlight();
                return;
        }
    }

    // Flashlight //

    void Flashlight(){
        toggle_marker = !toggle_marker;
        ObjectOne.SetActive(toggle_marker);
        HeadsUp.SetRecoil(new Vector3(0, -0.1f, -0.1f));
        PlaySFX("Flashlight", SFX_Lookup);
    }

    void DefaultFlashlight(){
        toggle_marker = false;
        ObjectOne.SetActive(toggle_marker);
    }
}
