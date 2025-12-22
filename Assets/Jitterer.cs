using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GenericUtils;

public class Jitterer : MonoBehaviour{

    [SerializeField] Vector3 Axis;
    [SerializeField] float MinJitter;
    [SerializeField] float MaxJitter;
    [SerializeField] float MinTime = 0.1f;
    [SerializeField] float MaxTime = 0.5f;
    [SerializeField] float LerpSpeed = 3f;

    Vector3 default_local;
    float clock;
    Vector3 current_pos;
    
    [HideInInspector] public bool animate = false;

    void Start(){
        default_local = transform.localPosition;
    }

    void Update(){
        Clock();       
        transform.localPosition = Vector3.Lerp(transform.localPosition, current_pos, Time.deltaTime * LerpSpeed);
    }

    void Clock(){
        clock -= Time.deltaTime;
        if(clock < 0){
            clock = Random.Range(MinTime, MaxTime);

            if(!animate){
                current_pos = default_local;
                return;
            }

            current_pos = default_local + new Vector3(
                Axis.x * FlipFlopNegation() * Random.Range(MinJitter, MaxJitter),
                Axis.y * FlipFlopNegation() * Random.Range(MinJitter, MaxJitter),
                Axis.z * FlipFlopNegation() * Random.Range(MinJitter, MaxJitter)
                );
        }
    }
}