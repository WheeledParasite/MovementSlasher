using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    // Global Static Variables
    [SerializeField]
    private Transform cam;

    [SerializeField]
    private Rigidbody rb;

    [SerializeField]
    private float acceleration;
    [SerializeField]
    private Vector3 counterAccel;

    [SerializeField]
    private Vector3 speedLimit;

    [SerializeField]
    private float sensitivity;

    [SerializeField]
    private float jumpForce;
    [SerializeField]
    private float gravity;
    
    // Input Controllers
    public PlayerInputActions playerControls;
    private InputAction move;
    private InputAction jump;
    private InputAction look;
    private InputAction fire;

    // <-- Global Variables -->
    // The direction the player "wishes" to move, described as the Vector2 of planar movement (WASD, etc)
    private Vector2 wishDirection = Vector2.zero;
    private Vector2 lookInput;
    private float xRot;
    private bool slide;
    private Vector3 lastVel;
    private float distToGround;

    void Start()
    {
        distToGround = GetComponent<Collider>().bounds.extents.y;   
        slide = false;
    }

    private void Awake()
    {
        playerControls = new PlayerInputActions();
    }

    private void OnEnable()
    {
        move = playerControls.Player.Move;
        move.Enable();

        look = playerControls.Player.Look;
        look.Enable();

        jump = playerControls.Player.Jump;
        jump.Enable();
        jump.performed += Jump;
    }

    private void OnDisable()
    {
        playerControls.Disable();
        move.Disable();
        look.Disable();
        jump.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        lookInput = look.ReadValue<Vector2>();

        wishDirection = move.ReadValue<Vector2>();

        MovePlayerCamera();
    }

    // FixedUpdate is called in fixed time, once per frame. Use for physics events, keeps game the same despite FPS.
    void FixedUpdate()
    {
        MovePlayer(wishDirection);
    }

    void LateUpdate() {
      lastVel = rb.velocity;  
    }
    private void MovePlayer(Vector2 localWishDir) {
        // if (wishDir != Vector2.zero) {  
        Vector3 forward = cam.transform.forward.normalized;
        Vector3 right = cam.transform.right.normalized;
        forward.y = 0f;
        right.y = 0f;

        Vector3 wishDir = forward * localWishDir.y + right * localWishDir.x;
        //     rb.AddForce(relativeWishDir * acceleration * Time.deltaTime);
        // } else {
        //     float counter = counterAccel.x;
        //     if (!isGrounded()) {
        //         counter = counterAccel.y;
        //     } else if (slide) {
        //         counter = counterAccel.z;
        //     }
        //     rb.AddForce(-rb.velocity.normalized * counter * Time.deltaTime); 
        // }
        rb.velocity = rb.velocity * 0.9967f * Time.deltaTime;

        float currentSpeed = Vector3.Dot(rb.velocity, wishDir);
        float addSpeed = Mathf.Clamp(acceleration * Time.deltaTime, 0, speedLimit.x - currentSpeed);

        rb.velocity = rb.velocity + addSpeed * wishDir;

        if (!isGrounded()) {
            rb.AddForce(Vector3.up * gravity);
        }

    }

    private void MovePlayerCamera() {
        xRot -= lookInput.y * sensitivity;

        transform.Rotate(0f, lookInput.x * sensitivity, 0f);
        cam.transform.localRotation = Quaternion.Euler(xRot, 0f, 0f);
    }

    public void Jump(InputAction.CallbackContext context) {
        if (canJump()) {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private bool canJump()
    {
        return isGrounded();
    }

    private bool isGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f);
    }
}
