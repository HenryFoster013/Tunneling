using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHeadsUpUI : MonoBehaviour{

    [Header(" - Main - ")]
    [SerializeField] Transform Reference;
    
    const float position_lerp_speed = 38f;
    const float rotation_lerp_speed = 22f;

    const float sprint_swing_degree = 5f;
    const float sprint_swing_speed = 10f;
    const float swing_allowance = 2.2f;

    bool oriental_swing, sprinting;
    float current_swing;

    void LateUpdate(){
        LerpSwing();
        transform.rotation = Quaternion.Lerp(transform.rotation, Reference.rotation * Quaternion.Euler(current_swing, 0, 0), rotation_lerp_speed * Time.deltaTime);
        transform.position = new Vector3(transform.position.x, Mathf.Lerp(transform.position.y, Reference.position.y, position_lerp_speed * Time.deltaTime), transform.position.z);
    }

    public void SetSprinting(bool new_sprint){
        if(new_sprint != sprinting && new_sprint == true)
            oriental_swing = false;
        sprinting = new_sprint;
    }

    void LerpSwing(){
        if(SwingEnded(GetSwingDirection()))
            oriental_swing = !oriental_swing;
        current_swing = Mathf.Lerp(current_swing, GetSwingDirection(), sprint_swing_speed * Time.deltaTime);
    }

    bool SwingEnded(float swing_target){
        if(current_swing > swing_target && current_swing - swing_target < swing_allowance)
            return true;
        if(swing_target > current_swing && swing_target - current_swing < swing_allowance)
            return true;
        return false;
    }

    float GetSwingDirection(){
        if(!sprinting)
            return 0f;
        float mult = 1f;
        if(oriental_swing)
            mult = -1f;
        return sprint_swing_degree * mult;
    }
}
