using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Vector3 playerMovementInput;
    private Vector2 playerMouseInput;
    private float xRot;

    [SerializeField]
    private Transform cam;
    [SerializeField]
    private Rigidbody playerRB;
    [SerializeField]
    private float speed;
    [SerializeField]
    private float sensitivity;
    [SerializeField]
    private float jumpForce;
    // Start is called before the first frame update
    void Start()
    {
        playerRB = GetComponent<Rigidbody>();

    }

    // Update is called once per frame
    void Update()
    {
        playerMovementInput = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        playerMouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        MovePlayer();
        MovePlayerCamera();
    }
    private void MovePlayer() {
        Vector3 moveVector = transform.TransformDirection(playerMovementInput) * speed;
        playerRB.velocity = new Vector3(moveVector.x, moveVector.y, moveVector.z);

        if(Input.GetKeyDown(KeyCode.Space)) {
            playerRB.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void MovePlayerCamera() {
        xRot -= playerMouseInput.y * sensitivity;

        transform.Rotate(0f, playerMouseInput.x * sensitivity, 0f);
        cam.transform.localRotation = Quaternion.Euler(xRot, 0f, 0f);
    }
}
