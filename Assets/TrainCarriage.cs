using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainCarriage : MonoBehaviour
{
    [Header("States")]
    public bool Open = true;
    public bool Moving = false;

    [Header("References")]
    [SerializeField] Animator BaseAnim;
    [SerializeField] Jitterer JitterEffect;
    
    [Header("Lights")]
    [SerializeField] Material BaseMaterial;
    [SerializeField] Light LightFront;
    [SerializeField] Light LightBack;
    
    [Header("Left Doors")]
    [SerializeField] Animator LD_Animator;
    [SerializeField] GameObject LD_Collider;
    [SerializeField] GameObject LD_ColliderWatcher;

    [Header("Right Doors")]
    [SerializeField] Animator RD_Animator;
    [SerializeField] GameObject RD_Collider;
    [SerializeField] GameObject RD_ColliderWatcher;

    float light_intensity = 1f;
    float current_intensity = 1f;
    float light_clock;

    void Update(){
        Animate();
    }

    void Animate(){

        ManageLights();

        BaseAnim.SetBool("moving", Moving);
        LD_Animator.SetBool("open", Open);
        RD_Animator.SetBool("open", Open);
        JitterEffect.animate = Moving;
        RD_Collider.SetActive(RD_ColliderWatcher.activeSelf);
        LD_Collider.SetActive(LD_ColliderWatcher.activeSelf);
    }

    void ManageLights(){
        SetLightTarget();
        LerpLights();
    }

    void SetLightTarget(){
        light_clock -= Time.deltaTime;
        if(light_clock < 0){

            if(!Moving){
                light_clock = Random.Range(1f, 3f);
                light_intensity = Random.Range(0.3f, 1f);
            }
            else{
                light_clock = Random.Range(0.1f, 0.5f);
                light_intensity = Random.Range(0f, 1.2f);
            }
        }
    }

    void LerpLights(){
        float lerp_speed = 0.1f;
        if(Moving)
            lerp_speed = 5f;

        current_intensity = Mathf.Lerp(current_intensity, light_intensity, Time.deltaTime * lerp_speed);
        LightFront.intensity = current_intensity;
        LightBack.intensity = current_intensity;
        BaseMaterial.SetFloat("_Emissive_Intensity", current_intensity);
    }
}
