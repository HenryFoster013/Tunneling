using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SoundUtils;

// This manages player movement only

public class PlayerController : MonoBehaviour{

    [Header(" - Main - ")]
    [SerializeField] CharacterController _CharacterController;
    [SerializeField] SoundEffectLookup SFX_Lookup;
    [SerializeField] LayerMask GroundLayer;
    [Header(" - Camera - ")]
    [SerializeField] Transform HeadHolder;
    [SerializeField] Transform Head;
    [SerializeField] Camera Cam;
    [SerializeField] Camera OverlayCam;
    [SerializeField] Animator CameraAnim;
    [Header(" - Modifiers - ")]
    public float MouseSens = 150f;
    public float ControllerSens = 50f;
    [HideInInspector] public bool CanLook = true;
    [HideInInspector] public bool CanMove = true;

    // Camera
    const float base_fov = 80;
    const float walk_fov = 82;
    const float sprint_fov = 100;
    const float overlay_walk_fov = 76;
    const float overlay_sprint_fov = 68;
    const float fov_change = 6;
    const float camera_swivel_amplitude = 5f;
    const float camera_swivel_speed = 2f;
    const float base_height = 1.5f;
    const float crouch_height = 0.8f;
    const float head_height_speed = 18f;
    const float cam_float_speed = 35f;
    // Movement
    const float movement_speed = 5f;
    const float sprint_multipler = 1.8f;
    const float crouch_multiplier = 0.5f;
    const float acceleration = 10f;
    const float fall_speed = 10f;
    const float floor_force = 1f;
    // SFX
    const float walk_ftsp_period = 0.5f;
    // Misc
    const int frame_delay = 5;

    // Private Variables
    float x_rot, y_rot, displayed_x_rot, displayed_y_rot, head_tilt, head_height, footstep_timer;
    bool grounded_buffer, walking, sprinting, crouching, camera_delayed, footstep_buffer;
    Vector3 target_velocity, true_velocity, ground_normal;

    public bool grounded;

    // MAIN //

    void Start(){
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 1;
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
        Movement();
        SoundEffects();
        Animate();
    }

    void LateUpdate(){
        MouseLook();
    }

    // Camera //

    void MouseLook(){
        if(!camera_delayed || !CanLook)
            return;

        float x_mod = Input.GetAxis("Mouse X") * MouseSens * Time.deltaTime;
        float y_mod = -1 * Input.GetAxis("Mouse Y") * MouseSens * Time.deltaTime;
        x_mod += Input.GetAxis("Controller X") * ControllerSens * Time.deltaTime;
        y_mod += Input.GetAxis("Controller Y") * ControllerSens * Time.deltaTime;


        x_rot = Mathf.Clamp(x_rot + y_mod, -85f, 85f);
        y_rot += x_mod;
        displayed_x_rot = Mathf.Lerp(displayed_x_rot, x_rot, cam_float_speed * Time.deltaTime);
        displayed_y_rot = Mathf.Lerp(displayed_y_rot, y_rot, cam_float_speed * Time.deltaTime);
        Head.localRotation = Quaternion.Euler(displayed_x_rot, displayed_y_rot, head_tilt);
    }

    void SetCursor(){
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Movement //

    public void Movement(){
        if(!CanMove){
            CleanBoolean();
            return;
        }

        GatherBoolean();
        SetTargetVelocity();
        LerpVelocity();
        ApplyVelocity();
    }

    void CleanBoolean(){
        grounded = true;
        walking = false;
        crouching = false;
        sprinting = false;
    }

    void GatherBoolean(){
        ManageGrounded();
        walking = Mathf.Abs(target_velocity.x) > 0.5f || Mathf.Abs(target_velocity.z) > 0.5f;
        ManageCrouching();
        sprinting = !crouching && Input.GetButton("Sprint") && Input.GetAxisRaw("Vertical") > 0.5f;
    }

    void ManageGrounded(){
        SetGrounded();
        PlayGroundedSFX();
    }

    void PlayGroundedSFX(){
        if(grounded && grounded != grounded_buffer){
            PlaySFX("Footstep", SFX_Lookup);
            PlaySFX("Footstep", SFX_Lookup);
        }
        grounded_buffer = grounded;
    }

    void ManageCrouching(){
        bool new_move = (Input.GetButton("Crouch") != crouching);
        crouching = Input.GetButton("Crouch");
        if(!new_move || !grounded)
            return;
        if(crouching)
            PlaySFX("Crouch_Down", SFX_Lookup);
        else
            PlaySFX("Crouch_Up", SFX_Lookup);
    }

    void SetGrounded(){
        RaycastHit hit;
        grounded = false;
        float radius = _CharacterController.radius;
        if (Physics.SphereCast(transform.position + (Vector3.up * radius), radius, Vector3.down, out hit, 0.15f, GroundLayer)){
            grounded = true;
            if (Vector3.Angle(hit.normal, Vector3.up) < 45f)
                ground_normal = hit.normal;
            else
                ground_normal = Vector3.up;
        }
        else
            ground_normal = Vector3.up;
    }

    void SetTargetVelocity(){
        Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        input = input.normalized * movement_speed * MovementSpeedMultiplier();
        Vector3 camera_forward = Vector3.ProjectOnPlane(Cam.transform.forward, Vector3.up).normalized;
        Vector3 camera_right = Vector3.ProjectOnPlane(Cam.transform.right, Vector3.up).normalized;
        target_velocity = (camera_forward * input.z + camera_right * input.x);
    }

    void ApplyVelocity(){        
        Vector3 horizontalMovement = Vector3.ProjectOnPlane(true_velocity, ground_normal);
        _CharacterController.Move((horizontalMovement + (Vector3.up * FallSpeed())) * Time.deltaTime);
        print(FallSpeed());
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
        true_velocity = Vector3.Lerp(true_velocity, target_velocity, Time.deltaTime * acceleration);
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
        OverlayCam.fieldOfView = Mathf.Lerp(OverlayCam.fieldOfView, GetOverlayFOV(), fov_change * Time.deltaTime);
    }

    void ApplyAnimations(){
        HeadHolder.localPosition = new Vector3(0, head_height, 0);
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

    float GetOverlayFOV(){
        if(sprinting)
            return overlay_sprint_fov;
        if(walking)
            return overlay_walk_fov;
        return base_fov; 
    }

    int GetHeadBop(){
        if(sprinting)
            return 2;
        if(walking)
            return 1;
        return 0;
    }

    // Sound //

    public void SoundEffects(){
        CheckFootsteps();
    }

    void CheckFootsteps(){
        if(!walking){
            footstep_timer = 0;
            return;
        }
        if(footstep_timer > GetFootstepDelay()){
            footstep_timer = 0;
            FootstepSFX();
        }
        else{
            if(walking)
                footstep_timer += Time.deltaTime;
        }
    }

    float GetFootstepDelay(){
        if(sprinting)
            return walk_ftsp_period / sprint_multipler;
        return walk_ftsp_period;
    }

    void FootstepSFX(){
        if(!grounded)
            return;
        if(!crouching)
            PlaySFX("Footstep", SFX_Lookup);
        else
            PlaySFX("Footstep_Light", SFX_Lookup);
    }
}