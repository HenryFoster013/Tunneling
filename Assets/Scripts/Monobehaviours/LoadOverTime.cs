using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadOverTime : MonoBehaviour{
    
    [SerializeField] string NextScene;
    [SerializeField] float Delay;

    void Start(){
        StartCoroutine(StartLoad());
    }

    IEnumerator StartLoad(){
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(NextScene);
        asyncOperation.allowSceneActivation = false;
        yield return new WaitForSeconds(Delay);
        asyncOperation.allowSceneActivation = true;
    }
}
