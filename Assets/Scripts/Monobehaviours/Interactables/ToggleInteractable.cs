using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SoundUtils;

public class ToggleInteractable : Interactable{
    
    [Header(" - Toggle - ")]
    [SerializeField] GameObject ToToggle;
    [SerializeField] SoundEffect SFX;
    
    public override void Reset(){ }
    
    public override void Activate(){
        if(!BasePass())
            return;
        PlaySFX(SFX);
        ToToggle.SetActive(!ToToggle.activeSelf);
    }
}
