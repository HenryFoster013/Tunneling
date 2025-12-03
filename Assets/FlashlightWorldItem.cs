using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashlightWorldItem : MonoBehaviour{

    [SerializeField] WorldItem Manager;
    [SerializeField] GameObject OffModel;
    [SerializeField] GameObject OnModel;
    
    public void Start(){
        StartCoroutine(Setup());
    }

    IEnumerator Setup(){
        OffModel.SetActive(false);
        OnModel.SetActive(false);
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        OffModel.SetActive(Manager.GetInstance().stored_int != 1);
        OnModel.SetActive(Manager.GetInstance().stored_int == 1);
    }
}
