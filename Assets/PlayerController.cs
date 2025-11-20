using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour{

    [Header(" - Main - ")]
    [SerializeField] CharacterController _CharacterController;
    [SerializeField] Transform Head;
    [SerializeField] Animator CameraAnim;

    [Header(" - Constant Modifiers - ")] // make these hard-coded constants in future
    public float MouseSens = 1f;
    public float gravity = 10f;
    public float fall_speed = 10f;
    public float movement_speed = 5f;
    public float sprint_multipler = 1.75f;
    public float acceleration = 10f;
    public float camera_swivel_amplitude = 30f;
    public float camera_swivel_speed = 90f;

    const int frame_delay = 5;
    const float floor_force = 2f;

    float x_rot, y_rot;
    bool grounded, walking, ready;
    Vector3 target_velocity, true_velocity;

    // Setup //

    void Start(){
        StartCoroutine(GetReady());
    }

    IEnumerator GetReady(){
        for(int i = 0; i < frame_delay; i++)
            yield return new WaitForEndOfFrame();
        ready = true;
    }

    // Update //

    void Update(){
        MouseLook();
        Movement();
        Animate();
    }

    // Camera //

    void MouseLook(){
        SetCursor();
        if(!ready)
            return;

        float x_mod = Input.GetAxis("Mouse X") * MouseSens * Time.deltaTime;
        float y_mod = -1 * Input.GetAxis("Mouse Y") * MouseSens * Time.deltaTime;
        x_rot = Mathf.Clamp(x_rot + y_mod, -90f, 90f);
        y_rot += x_mod;
        Head.localRotation = Quaternion.Euler(x_rot, y_rot, 0f);
    }

    void SetCursor(){
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Movement //

    public void Movement(){
        grounded = _CharacterController.isGrounded;
        SetTargetVelocity();
        LerpVelocity();
        ApplyVelocity();
    }

    void SetTargetVelocity(){
        float mult = 1;
        if(Input.GetKey(KeyCode.LeftShift))
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
        bool moving = (Mathf.Abs(true_velocity.x) > 0.1) || (Mathf.Abs(true_velocity.z) > 0.1);
        CameraAnim.SetBool("moving", moving);
    }
}
