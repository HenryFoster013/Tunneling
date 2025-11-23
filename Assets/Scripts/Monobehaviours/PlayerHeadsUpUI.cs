using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHeadsUpUI : MonoBehaviour{

    [Header(" - Main - ")]
    [SerializeField] Transform Reference;
    
    const float position_lerp_speed = 38f;
    const float rotation_lerp_speed = 22f;

    void LateUpdate(){
        transform.rotation = Quaternion.Lerp(transform.rotation, Reference.rotation, rotation_lerp_speed * Time.deltaTime);
        transform.position = new Vector3(transform.position.x, Mathf.Lerp(transform.position.y, Reference.position.y, position_lerp_speed * Time.deltaTime), transform.position.z);
    }
}
