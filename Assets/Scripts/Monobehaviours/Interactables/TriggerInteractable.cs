using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerInteractable : MonoBehaviour
{
    public UnityEvent OnInteract;
    
    void OnTriggerEnter(Collider other){
        if(other.transform.tag == "Player")
            OnInteract.Invoke();
    }
}
