using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTrigger : MonoBehaviour{
    
    [SerializeField] DoorController Controller;
    [SerializeField] bool Peek;

    public void OnTriggerEnter(Collider other){
        if(Peek)
            Controller.EnterPeek(other.transform);
        else
            Controller.EnterOpen(other.transform);
    }

    public void OnTriggerExit(Collider other){
        if(Peek)
            Controller.ExitPeek(other.transform);
    }

}
