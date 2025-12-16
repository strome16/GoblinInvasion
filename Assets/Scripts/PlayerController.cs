using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    private Rigidbody playerRb;
    public float speed = 30f;
    public float jumpForce = 8f;
    public float gravityModifier = 2;
    public Transform cameraTransform;
    public float turnSpeed = 10f;
    private Vector3 moveDir;

    [Header("Sprint")]
    public float speedMultiplier = 1.5f;
    public float accelerationSpeed = 8f;
    private float currentSpeedMultiplier = 1f;  // smoothed value

    [Header("Input")]
    private float horizontalInput;
    private float verticalInput;

    private bool isOnGround = true;
    private bool wantsToJump;

    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        Physics.gravity = new Vector3(0, -9.81f * gravityModifier, 0);
    }

    void Update()
    {
        // input
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        // direction relative to camera
        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;
        camForward.y = 0;
        camRight.y = 0;
        moveDir = (camForward * verticalInput + camRight * horizontalInput).normalized;

        // sprint multiplier (reset every frame)
        float targetMultiplier = 1f;
        if (Input.GetKey(KeyCode.LeftShift) && isOnGround && verticalInput > 0f)
            targetMultiplier = speedMultiplier;
     
        currentSpeedMultiplier = Mathf.Lerp(currentSpeedMultiplier, targetMultiplier, accelerationSpeed * Time.deltaTime);

        // allows player to jump when on the ground
        if (Input.GetKeyDown(KeyCode.Space) && isOnGround)
            wantsToJump = true;

        // rotate player to face camera direction
        Vector3 facingDir = cameraTransform.forward;
        facingDir.y = 0; // keep it flat

        if (facingDir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(facingDir);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, turnSpeed * Time.deltaTime);
        }

    }

    private void FixedUpdate()
    {
        Vector3 currentVel = playerRb.linearVelocity;
        Vector3 targetVel = moveDir * speed * currentSpeedMultiplier;

        // move player
        playerRb.linearVelocity = new Vector3(targetVel.x, currentVel.y, targetVel.z);

        if (wantsToJump)
        {
            playerRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isOnGround = false;
            wantsToJump = false;
        }

    }

      private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isOnGround = true;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isOnGround = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isOnGround = false;
    }


}
