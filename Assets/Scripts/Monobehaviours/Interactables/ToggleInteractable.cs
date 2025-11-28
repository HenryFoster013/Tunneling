using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SoundUtils;

public class ToggleInteractable : Interactable{
    
    [Header(" - Toggle - ")]
    [SerializeField] GameObject ToToggle;
    [SerializeField] SoundEffect SFX;
    [SerializeField] string DisplayText = "Toggle";
    
    public override void Reset(){ }
    
    public override void Acivate(){
        PlaySFX(SFX);
        ToToggle.SetActive(!ToToggle.activeSelf);
    }

    public override string InteractText(){
        return DisplayText;
    }
}
