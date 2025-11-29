using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GenericUtils;

public class PlayerHeadsUpUI : MonoBehaviour{

    [Header(" - Main - ")]
    [SerializeField] Transform Reference;
    [SerializeField] Transform LeftHand;
    [SerializeField] Transform RightHand;
    
    const float position_lerp_speed = 38f;
    const float rotation_lerp_speed = 22f;

    const float sprint_swing_degree = 5f;
    const float sprint_swing_speed = 10f;
    const float swing_allowance = 2.2f;
    const float recoil_factor = 1f/2f;

    bool oriental_swing, sprinting;
    float current_swing, right_recoil_speed, left_recoil_speed;
    Vector3 right_recoil, right_original_pos, left_recoil, left_original_pos;

    // Setup //

    void Start(){
        SetupHands();
    }

    void SetupHands(){
        right_original_pos = RightHand.localPosition;
        left_original_pos = LeftHand.localPosition;
    }

    // Lerping Movement //

    void LateUpdate(){
        LerpSwing();
        LerpRecoil();
        transform.rotation = Quaternion.Lerp(transform.rotation, Reference.rotation * Quaternion.Euler(current_swing, 0, 0), rotation_lerp_speed * Time.deltaTime);
        transform.position = new Vector3(transform.position.x, Mathf.Lerp(transform.position.y, Reference.position.y, position_lerp_speed * Time.deltaTime), transform.position.z);
        RightHand.localPosition = (right_recoil * recoil_factor) + right_original_pos;
        LeftHand.localPosition = (left_recoil * recoil_factor) + left_original_pos;
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

    void LerpRecoil(){
        LerpToZero(ref right_recoil, ref right_recoil_speed);
        LerpToZero(ref left_recoil, ref left_recoil_speed);
    }

    public void SetRecoil(Vector3 recoil, float recoil_speed, bool right_hand){
        if(right_hand){
            right_recoil_speed = recoil_speed;
            right_recoil = recoil;
        }
        else{
            left_recoil_speed = recoil_speed;
            left_recoil = recoil;
        }
    }
}
