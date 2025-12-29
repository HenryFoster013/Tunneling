using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WebcamManager : MonoBehaviour{
    
    [SerializeField] RawImage Display;
    WebCamTexture cam_tex;
    
    void Start(){
        StartCoroutine(StartWebCam());
    }

    private IEnumerator StartWebCam(){
        if (WebCamTexture.devices.Length == 0)
            yield break;
        
        cam_tex = new WebCamTexture(WebCamTexture.devices[0].name, 1280, 720, 24);
        Display.texture = cam_tex;
        cam_tex.Play();
        
        yield return new WaitUntil(() => cam_tex.didUpdateThisFrame && cam_tex.isPlaying);

        if (!cam_tex.isPlaying)
            yield break;
    }

    void OnDisable(){
        if (cam_tex != null && cam_tex.isPlaying)
            cam_tex.Stop();
    }
}