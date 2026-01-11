using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessStaircase : MonoBehaviour
{
    
    [Header("Teleportation")]
    [SerializeField] PlayerController Player;
    [SerializeField] GameObject[] InitialEntryEnable;
    [SerializeField] GameObject[] InitialEntryDisable;

    [Header("Lighting")]
    [SerializeField] Light[] Lights;
    public Color StartLights;
    public Color EndLights;
    public Color StartAmbient;
    public Color EndAmbient;

    float current_floor = 0;
    float lerped_floor = 0;
    bool initial_entry = false;
    bool buffered = false;

    const float transition_speed = 1f;
    const float light_levels = 5;
    const float floor_distance = 8.1f;


    void Update(){
        UpdateLights();
    }

    void UpdateLights(){
        lerped_floor = Mathf.Lerp(lerped_floor, current_floor, Time.deltaTime * transition_speed);
        float lerped_transitional = lerped_floor / light_levels;

        Color light_colour = Color.Lerp(StartLights, EndLights, lerped_transitional);
        foreach(Light light in Lights)
            light.color = light_colour;

        RenderSettings.ambientLight = Color.Lerp(StartAmbient, EndAmbient, lerped_transitional);
    }

    // TELEPORTATION //

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

        Player.Teleport(new Vector3(0, -1f, 0) * floor_distance);
        current_floor--;
        if(current_floor < 0)
            current_floor = 0;
    }

    public void BottomColliderEntry(){
        if(Buffered())
            return;
        
        Player.Teleport(new Vector3(0, 1f, 0) * floor_distance);
        current_floor++;
    }
}
