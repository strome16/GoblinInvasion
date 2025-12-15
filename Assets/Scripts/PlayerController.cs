using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody playerRb;
    public float speed = 30f;
    public float jumpForce = 8f;
    public float gravityModifier = 2;
    public Transform cameraTransform;
    public float turnSpeed = 10f;

    private float horizontalInput;
    private float verticalInput;

    private bool isOnGround = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        Physics.gravity = new Vector3(0, -9.81f * gravityModifier, 0);
    }

    // Update is called once per frame
    void Update()
    {
        // link key presses to movement direction
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // direction relative to camera
        Vector3 camForward = Camera.main.transform.forward;
        Vector3 camRight = Camera.main.transform.right;

        camForward.y = 0;
        camRight.y = 0;

        Vector3 moveDir = (camForward * vertical + camRight * horizontal).normalized;

        Vector3 currentVel = playerRb.linearVelocity;
        Vector3 targetVel = moveDir * speed;

        // move player
        playerRb.linearVelocity = new Vector3(targetVel.x, currentVel.y, targetVel.z);

        // allows player to jump when on the ground
        if (Input.GetKeyDown(KeyCode.Space) && isOnGround)
        {
            playerRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isOnGround = false;
        }

        /// rotate player to face camera direction
        Vector3 facingDir = cameraTransform.forward;
        facingDir.y = 0; // keep it flat

        if (facingDir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(facingDir);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, targetRot, turnSpeed * Time.deltaTime);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Powerup"))
        {
            Destroy(other.gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isOnGround = true;
        }
    }

}
