using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SoundUtils;
using UnityEngine.Events;

public class Interactable : MonoBehaviour{
    
    [Header(" - To Call - ")]
    public UnityEvent OnInteract;
    [Header(" - Additionals - ")]
    [SerializeField] string DisplayText;
    [SerializeField] bool UseOnce;

    bool used;

    void Start(){
        this.transform.tag = "Interactable";
    }
    
    // Virtuals //
    
    public virtual void Activate(){
        OnInteract.Invoke();
    }

    public virtual string InteractText() {return DisplayText;}

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
