using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessStaircase : MonoBehaviour
{
    
    [Header("Teleportation")]
    [SerializeField] PlayerController Player;

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
    [SerializeField] GameObject Music;
    [SerializeField] AudioSource Wind;
    float wind;
    [SerializeField] AudioSource Machinery;
    float machinery;
    [SerializeField] AudioSource Heartbeat;
    float heartbeat;

    [Header("Additonals")]
    [SerializeField] GameObject[] Additonals;
    [SerializeField] GameObject[] EnabledOnDefault;
    [SerializeField] GameObject[] DisabledOnDefault;
    [SerializeField] GameObject[] InitialEntryEnable;
    [SerializeField] GameObject[] InitialEntryDisable;
    [SerializeField] GameObject[] EnableOnEnding;
    [SerializeField] GameObject[] DisableOnEnding;
 
    int current_floor = 0;
    float lerped_floor = 0;
    float lerped_transitional;
    bool initial_entry = false;
    bool buffered = false;
    bool in_ending = false;
    bool ending_buffer = false;

    const int music_floor = 2;
    const int shading_offset = 2;
    const float transition_speed = 1f;
    const float light_levels = 6;
    const float floor_distance = 8.1f;

    void Start(){
        SetupAudio();
        SetupToggles();
    }

    void SetupToggles(){
        foreach(GameObject g in EnabledOnDefault)
            g.SetActive(true);
        foreach(GameObject g in DisabledOnDefault)
            g.SetActive(false);
    }

    void SetupAudio(){
        Music.SetActive(false);
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
        lerped_transitional = Mathf.Clamp(((lerped_floor - shading_offset) / light_levels), 0f, 1f);
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
        if(Buffered())
            return;

        CheckInitial();
        if(!in_ending)
            Player.Teleport(new Vector3(0, -1f, 0) * floor_distance);
        else{
            if(ending_buffer)
                Player.Teleport(new Vector3(0, -1f, 0) * floor_distance);
            ending_buffer = true;
        }
    }

    public void BottomColliderEntry(){
        
        if(Buffered())
            return;

        Player.Teleport(new Vector3(0, 1f, 0) * floor_distance);
        if(!in_ending){
            current_floor++;
            CheckEnding();
        }
    }

    void CheckEnding(){
        Music.SetActive(current_floor >= music_floor);
        if(current_floor == Additonals.Length - 1){
            foreach(GameObject g in EnableOnEnding)
                g.SetActive(true);
            foreach(GameObject g in DisableOnEnding)
                g.SetActive(false);
            foreach(GameObject g in Additonals){
                if(g != null)
                    g.SetActive(false);
            }
            in_ending = true;
        }
    }
}
