using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SoundUtils;

public class Interactable : MonoBehaviour{
    
    [Header(" - References - ")]
    [SerializeField] SoundEffectLookup SFX_Lookup;

    void Start(){
        this.transform.tag = "Interactable";
    }
    
    public virtual void Reset(){ }
    
    public virtual void Acivate() { }

    public virtual string InteractText() {return "";}
}
