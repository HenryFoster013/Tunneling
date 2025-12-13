using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightManager : MonoBehaviour{

    public List<GameLight> Lights = new List<GameLight>();

    const float speed = 0.2f;
    const float scale = 0.5f;

    void Update(){
        foreach(GameLight light in Lights)
            light.SetBrightness(CalculateBrightness(light));
    }

    public float CalculateBrightness(GameLight light){
        return  Mathf.Clamp(Mathf.Sin((light.transform.position.x * scale) + (Time.time * speed)), 0f, 1f);
    }
}