using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightManager : MonoBehaviour{

    public List<GameLight> Lights = new List<GameLight>();

    public float flicker_time;
    float flicker_randomiser = 0f;

    const float speed = 0.2f;
    const float scale = 0.5f;
    const float flicker_rate = 0.2f;

    void Update(){
        if(flicker_time <= 0){
            foreach(GameLight light in Lights)
                light.SetBrightness(CalculateBrightness(light));
        }
        else{
            flicker_randomiser -= Time.deltaTime;
            foreach(GameLight light in Lights)
                light.SetBrightness(Random.Range(0f, 1.2f));
            if(flicker_randomiser < 0)
                flicker_randomiser = flicker_rate;
        }

        flicker_time -= Time.deltaTime;
    }

    public float CalculateBrightness(GameLight light){
        return  Mathf.Clamp(Mathf.Sin((light.transform.position.x * scale) + (Time.time * speed)), 0f, 1f);
    }

    public void SetFlickerTime(float amount){
        flicker_time = amount;
    }
}