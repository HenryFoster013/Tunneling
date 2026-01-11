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

    [Header("Materials")]
    [SerializeField] Material StationMatOne;
    [SerializeField] Material StationMatTwo;

    [Header("Audio")]
    [SerializeField] AudioSource Wind;
    float wind;
    [SerializeField] AudioSource Machinery;
    float machinery;
    [SerializeField] AudioSource Heartbeat;
    float heartbeat;

    [Header("Additonals")]
    [SerializeField] GameObject[] Additonals;

    int current_floor = 0;
    float lerped_floor = 0;
    float lerped_transitional;
    bool initial_entry = false;
    bool buffered = false;

    const float transition_speed = 1f;
    const float light_levels = 7;
    const float floor_distance = 8.1f;

    void Start(){
        wind = Wind.volume;
        machinery = Machinery.volume;
        heartbeat = Heartbeat.volume;
    }

    void Update(){
        LerpForwards();
        SetLights();
        SetMaterials();
        SetAudios();
        SetAdditionals();
    }

    // ANIMATION //

    void LerpForwards(){
        lerped_floor = Mathf.Lerp(lerped_floor, (float)current_floor, Time.deltaTime * transition_speed);
        lerped_transitional = Mathf.Clamp(lerped_floor / light_levels, 0f, 1f);
    }

    void SetLights(){
        Color light_colour = Color.Lerp(StartLights, EndLights, lerped_transitional);
        foreach(Light light in Lights)
            light.color = light_colour;

        RenderSettings.ambientLight = Color.Lerp(StartAmbient, EndAmbient, lerped_transitional);
    }

    void SetMaterials(){
        StationMatOne.SetFloat("_Blend", lerped_transitional * 0.6f);
        StationMatTwo.SetFloat("_Blend", lerped_transitional * 0.6f);
    }

    void SetAudios(){
        Wind.volume = Mathf.Lerp(0f, wind, lerped_transitional);
        Machinery.volume = Mathf.Lerp(0f, machinery, lerped_transitional);
        Heartbeat.volume = Mathf.Lerp(0f, heartbeat, lerped_transitional);
    }

    void SetAdditionals(){
        if(Additonals.Length == 0)
            return;

        foreach(GameObject g in Additonals){
            if(g != null)
                g.SetActive(false);
        }

        if(current_floor < 0 || current_floor >= Additonals.Length)
            return;
        
        if(Additonals[current_floor] != null){
            Additonals[current_floor].SetActive(true);
        }
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
