using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //public DebugMe debugMe;

    public float speed = 15.5f;
    public float turnSpeed = 150f;
    public float jumpForce;
    public float gravityModifier;
    public float maxHeadRotation = 45f;
    public float minHeadRotation = 315f;
    public GameObject playerHead;

    private float horizontalInput;
    private float forwardInput;
    private float mouseInputX;
    private float mouseInputY;
    private Quaternion headRotationOld;
    private Vector3 resetPosition = new Vector3(0, 2, 0);
    private Vector3 resetRotation = new Vector3(0, -180, 0);

    public float jumpHeight = 5; // 0.5f;
    public float jumpSpeed = 10; // 1;
    private float jumpControl;
    private float jumpCount;
    private bool isJumping = false;
    private bool isFalling = false;
    private Rigidbody playerRB;
    // public bool isOnGround = true;

    void Start()
    {
        playerRB = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // Player Movement
        horizontalInput = Input.GetAxis("Horizontal");
        forwardInput = Input.GetAxis("Vertical");

        if (!isJumping)
        {
            if (horizontalInput != 0)
            {
                transform.Translate(Vector3.right * horizontalInput * Time.deltaTime * speed);
            }
            if (forwardInput != 0)
            {
                transform.Translate(Vector3.forward * forwardInput * Time.deltaTime * speed);
            }
        }

        // Player Rotation + Head Look up/down
        mouseInputX = Input.GetAxis("Mouse X");
        mouseInputY = Input.GetAxis("Mouse Y");
        Util.DebugMe("mouseInputX", mouseInputX.ToString());

        // if (mouseInputX != 0) playerRB.AddTorque(Vector3.up * mouseInputX * turnSpeed, ForceMode.VelocityChange);
        if (mouseInputX != 0) transform.Rotate(Vector3.up * mouseInputX * turnSpeed * Time.deltaTime);
        // Util.DebugMe("mouseInputX", mouseInputX.ToString());

        if (mouseInputY != 0 && playerHead.transform.rotation.x != maxHeadRotation && playerHead.transform.rotation.x != minHeadRotation)
        {
            headRotationOld = playerHead.transform.rotation;
            // playerRB.AddTorque(Vector3.right * mouseInputY * turnSpeed, ForceMode.VelocityChange);
            playerHead.transform.Rotate(Vector3.right * mouseInputY * turnSpeed * Time.deltaTime);
            // debugMe.Show("playerHead.transform.rotation.eulerAngles.x", playerHead.transform.rotation.eulerAngles.x.ToString());

            if (playerHead.transform.rotation.eulerAngles.x > maxHeadRotation && playerHead.transform.rotation.eulerAngles.x < minHeadRotation)
            {
                playerHead.transform.rotation = headRotationOld;
            }
        }

        Util.DebugMe("isJumping", isJumping.ToString());
        if (!isJumping)
        {
            // Jumping
            if (Input.GetButtonDown("Jump"))
            {
                // jumpCount = jumpHeight;
                playerRB.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                playerRB.AddForce(Vector3.forward * forwardInput * jumpForce, ForceMode.Impulse);
                isJumping = true;
                isFalling = false;
                jumpControl = transform.position.y;
            }
        } else // Controls if is falling and if touch ground
        {
            if (!isFalling)
            {
                if (transform.position.y == jumpControl) // Si es misma altura anterior asumimos que empieza a caer
                {
                    isFalling = true;
                }
                else
                {
                    jumpControl = transform.position.y;
                }
            } else
            {
                if (transform.position.y == jumpControl) // Si es misma altura anterior asumimos que dejó de caer
                {
                    isJumping = false;
                    isFalling = false;
                }
                else
                {
                    jumpControl = transform.position.y;
                }
            }
        }

        // Check if falls in pit
        if (transform.position.y <= 1.5f)
        {
            transform.position = resetPosition;
            transform.rotation = Quaternion.Euler(resetRotation);
        }
    }
}
