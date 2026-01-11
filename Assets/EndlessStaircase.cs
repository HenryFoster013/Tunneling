using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessStaircase : MonoBehaviour
{
    
    [SerializeField] PlayerController Player;
    [SerializeField] GameObject[] InitialEntryEnable;
    [SerializeField] GameObject[] InitialEntryDisable;
    [SerializeField] float FloorDistance = 8.1f;

    bool initial_entry = false;
    bool buffered = false;

    bool Buffered(){
        if(buffered){
            buffered = false;
            return true;
        }
        else{
            buffered = true;
            StartCoroutine(BufferUp());
        }
        return false;
    }

    IEnumerator BufferUp(){
        for(int i = 0; i < 4; i++)
            yield return new WaitForEndOfFrame();
        buffered = false;
    }

    bool CheckInitial(){
        if(!initial_entry){
            initial_entry = true;
            foreach(GameObject g in InitialEntryEnable)
                g.SetActive(true);
            foreach(GameObject g in InitialEntryDisable)
                g.SetActive(false);
            return true;
        }
        return false;
    }

    public void TopColliderEntry(){
        if(CheckInitial())
            return;
        if(Buffered())
            return;

        print("Top Trigger");
        Player.Teleport(new Vector3(0, -1f, 0) * FloorDistance);
    }

    public void BottomColliderEntry(){
        if(Buffered())
            return;

        print("Bottom Trigger");
        Player.Teleport(new Vector3(0, 1f, 0) * FloorDistance);
    }
}
