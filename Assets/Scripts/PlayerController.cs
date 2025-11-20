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
    public float MouseSens = 1f;

    // Camera
    const float base_fov = 80;
    const float walk_fov = 82;
    const float sprint_fov = 100;
    const float fov_change = 6;
    const float camera_swivel_amplitude = 3.5f;
    const float camera_swivel_speed = 1.5f;
    // Movement
    const float movement_speed = 5f;
    const float sprint_multipler = 1.8f;
    const float acceleration = 10f;
    const float gravity = 10f;
    const float fall_speed = 10f;
    const float floor_force = 2f;
    // Misc
    const int frame_delay = 5;

    // Private Variables
    float x_rot, y_rot, head_tilt;
    bool grounded, walking, sprinting, crouching, camera_delayed;
    Vector3 target_velocity, true_velocity;

    // MAIN //

    void Start(){
        StartCoroutine(CameraDelay());
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
        ApplyVelocity();
        Animate();
    }

    // Camera //

    void MouseLook(){
        if(!camera_delayed)
            return;

        float x_mod = Input.GetAxis("Mouse X") * MouseSens;
        float y_mod = -1 * Input.GetAxis("Mouse Y") * MouseSens;
        x_rot = Mathf.Clamp(x_rot + y_mod, -90f, 90f);
        y_rot += x_mod;
        Head.localRotation = Quaternion.Euler(x_rot, y_rot, head_tilt);
    }

    void SetCursor(){
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Movement //

    public void Movement(){
        GatherBoolean();
        SetTargetVelocity();
        LerpVelocity();
    }

    void GatherBoolean(){
        grounded = _CharacterController.isGrounded;
        walking = Mathf.Abs(true_velocity.x) > 0.5f || Mathf.Abs(true_velocity.z) > 0.5f;
        crouching = Input.GetKeyDown(KeyCode.LeftControl);
        sprinting = !crouching && Input.GetKey(KeyCode.LeftShift) && Input.GetAxisRaw("Vertical") > 0.5f;
    }

    void SetTargetVelocity(){
        float mult = 1;
        if(sprinting)
            mult = sprint_multipler;
        float x = Input.GetAxisRaw("Horizontal") * movement_speed * mult;
        float z = Input.GetAxisRaw("Vertical") * movement_speed * mult;
        float y = -1f;
        if(!grounded)
            y = -fall_speed;
        target_velocity = new Vector3(x, y, z);        
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
        SetHeadBop();
        CameraFX();
    }

    // Camera

    void SetHeadBop(){
        int speed = 0;
        if(walking)
            speed = 1;
        if(sprinting)
            speed = 2;
        CameraAnim.SetInteger("speed", speed);
    }

    void CameraFX(){
        head_tilt = Mathf.Lerp(head_tilt, GetHeadAngle(), Time.deltaTime * camera_swivel_speed);
        Cam.fieldOfView = Mathf.Lerp(Cam.fieldOfView, GetFOV(), fov_change * Time.deltaTime);
    }

    float GetHeadAngle(){
        if(Input.GetAxisRaw("Horizontal") > 0.5f)
            return -camera_swivel_amplitude;
        if (Input.GetAxisRaw("Horizontal") < -0.5f)
            return camera_swivel_amplitude;
        return 0f;
    }

    float GetFOV(){
        if(sprinting)
            return sprint_fov;
        if(walking)
            return walk_fov;
        return base_fov;
    }
}