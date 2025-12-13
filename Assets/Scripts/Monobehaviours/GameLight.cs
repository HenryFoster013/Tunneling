using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLight : MonoBehaviour{

    [SerializeField] MeshRenderer Lightbulb;
    [SerializeField] Light Point;
    float base_intensity = 5f;
    Material our_mat;

    void Start(){
        our_mat = new Material(Lightbulb.material);
        Lightbulb.material = our_mat;
        base_intensity = Point.intensity;
    }

    public void SetBrightness(float brightness){
        our_mat.SetFloat("_Brightness", brightness);
        Point.intensity = base_intensity * brightness;
    }
}