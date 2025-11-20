using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour{

    [Header(" - Main - ")]
    [SerializeField] CharacterController _CharacterController;
    [SerializeField] Transform Head;
    [SerializeField] Camera Cam;
    [SerializeField] Animator CameraAnim;

    [Header(" - Modifiers - ")]
    public float MouseSens = 500f;

    // Camera
    const float base_fov = 80;
    const float walk_fov = 82;
    const float sprint_fov = 100;
    const float fov_change = 6;
    const float camera_swivel_amplitude = 3.5f;
    const float camera_swivel_speed = 1.5f;
    const float base_height = 0.5f;
    const float crouch_height = -0.2f;
    const float head_height_speed = 18f;
    // Movement
    const float movement_speed = 5f;
    const float sprint_multipler = 1.8f;
    const float crouch_multiplier = 0.5f;
    const float acceleration = 10f;
    const float gravity = 10f;
    const float fall_speed = 10f;
    const float floor_force = 1f;
    // Misc
    const int frame_delay = 5;

    // Private Variables
    float x_rot, y_rot, head_tilt, head_height;
    bool grounded, walking, sprinting, crouching, camera_delayed;
    Vector3 target_velocity, true_velocity;

    // MAIN //

    void Start(){
        DefaultValues();
        StartCoroutine(CameraDelay());
    }

    void DefaultValues(){
        head_height = base_height;
    }

    IEnumerator CameraDelay(){ // Prevents 
        for(int i = 0; i < frame_delay; i++)
            yield return new WaitForEndOfFrame();
        camera_delayed = true;
    }

    void Update(){
        SetCursor();
        MouseLook();
        Movement();
        Animate();
    }

    // Camera //

    void MouseLook(){
        if(!camera_delayed)
            return;

        float x_mod = Input.GetAxis("Mouse X") * MouseSens * Time.deltaTime;
        float y_mod = -1 * Input.GetAxis("Mouse Y") * MouseSens * Time.deltaTime;
        x_rot = Mathf.Clamp(x_rot + y_mod, -90f, 90f);
        y_rot += x_mod;
    }

    void SetCursor(){
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Movement //

    public void Movement(){
        GatherBoolean();
        SetTargetVelocity();
        LerpVelocity();
        ApplyVelocity();
    }

    void GatherBoolean(){
        grounded = _CharacterController.isGrounded;
        walking = Mathf.Abs(true_velocity.x) > 0.5f || Mathf.Abs(true_velocity.z) > 0.5f;
        crouching = Input.GetKey(KeyCode.LeftControl);
        sprinting = !crouching && Input.GetKey(KeyCode.LeftShift) && Input.GetAxisRaw("Vertical") > 0.5f;
    }

    void SetTargetVelocity(){
        float mult = MovementSpeedMultiplier();
        target_velocity = new Vector3(MovementAxis("Horizontal") * mult, FallSpeed(), MovementAxis("Vertical") * mult);        
    }

    float MovementAxis(string name){
        return Input.GetAxisRaw(name) * movement_speed;
    }

    float MovementSpeedMultiplier(){
        if(crouching)
            return crouch_multiplier;
        if(sprinting)
            return sprint_multipler;
        return 1f;
    }

    float FallSpeed(){
        if(!grounded)
            return -fall_speed;
        return -floor_force;
    }

    void LerpVelocity(){
        float true_x = Mathf.Lerp(true_velocity.x, target_velocity.x, acceleration * Time.deltaTime);
        float true_y = Mathf.Lerp(true_velocity.y, target_velocity.y, gravity * Time.deltaTime);
        float true_z = Mathf.Lerp(true_velocity.z, target_velocity.z, acceleration * Time.deltaTime);
        true_velocity = new Vector3(true_x, true_y, true_z);
    }

    void ApplyVelocity(){
        Quaternion y_rot = Quaternion.Euler(0, Head.eulerAngles.y, 0);
        Vector3 move_dir = ((y_rot * Vector3.forward) * true_velocity.z + (y_rot * Vector3.right) * true_velocity.x);
        move_dir = move_dir + new Vector3(0, true_velocity.y, 0);
        _CharacterController.Move((move_dir) * Time.deltaTime);
    }

    // Animation ///

    void Animate(){
        LerpValues();
        ApplyAnimations();
    }

    // Camera

    void LerpValues(){
        head_tilt = Mathf.Lerp(head_tilt, GetHeadAngle(), Time.deltaTime * camera_swivel_speed);
        head_height = Mathf.Lerp(head_height, GetHeadHeight(), Time.deltaTime * head_height_speed);
        Cam.fieldOfView = Mathf.Lerp(Cam.fieldOfView, GetFOV(), fov_change * Time.deltaTime);
    }

    void ApplyAnimations(){
        Head.localPosition = new Vector3(0, head_height, 0);
        Head.localRotation = Quaternion.Euler(x_rot, y_rot, head_tilt);
        CameraAnim.SetInteger("speed", GetHeadBop());
    }

    // State Calculations

    float GetHeadAngle(){
        if(Input.GetAxisRaw("Horizontal") > 0.5f)
            return -camera_swivel_amplitude;
        if (Input.GetAxisRaw("Horizontal") < -0.5f)
            return camera_swivel_amplitude;
        return 0f;
    }

    float GetHeadHeight(){
        if(crouching)
            return crouch_height;
        return base_height;
    }

    float GetFOV(){
        if(sprinting)
            return sprint_fov;
        if(walking)
            return walk_fov;
        return base_fov;
    }

    int GetHeadBop(){
        if(sprinting)
            return 2;
        if(walking)
            return 1;
        return 0;
    }

}