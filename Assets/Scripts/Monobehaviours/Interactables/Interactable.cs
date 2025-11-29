using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SoundUtils;

public class Interactable : MonoBehaviour{
    
    [Header(" - Base - ")]
    [SerializeField] SoundEffectLookup SFX_Lookup;
    [SerializeField] bool UseOnce;

    bool used;

    void Start(){
        this.transform.tag = "Interactable";
    }
    
    // Virtuals //
    
    public virtual void Activate() { }

    public virtual string InteractText() {return "";}

    public virtual void Reset() { }

    // Bases//

    public bool BasePass(){
        if(CheckUsage())
            return false;
        return true;
    }

    bool CheckUsage(){
        if(UseOnce){
            if(used)
                return true;
            MarkDisabled();
        }
        return false;
    }

    void MarkDisabled(){
        used = true;
        this.transform.tag = "Untagged";
    }
}
