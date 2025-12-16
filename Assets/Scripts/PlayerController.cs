using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 10f;
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float gravityModifier = 2;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float turnSpeed = 720f;

    private Rigidbody playerRb;
    private Vector3 moveDir;
    private Quaternion desiredRotation;

    [Header("Sprint")]
    [SerializeField] private float speedMultiplier = 1.5f;
    [SerializeField] private float accelerationSpeed = 8f;
    private float currentSpeedMultiplier = 1f;  // smoothed value

    [Header("Animations")]
    [SerializeField] private Animator animator;

    private float horizontalInput;
    private float verticalInput;

    private bool isOnGround = true;
    private bool wantsToJump;
    private bool isDead;
    [SerializeField] private int deathType = 1;
    private bool shotQueued;

    private void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        
        if (playerRb == null )
            Debug.LogError($"{nameof(PlayerController)} on {name} has no Rigidbody.");

        // set global gravity scale
        Physics.gravity = new Vector3(0, -9.81f * gravityModifier, 0);

        desiredRotation = transform.rotation;

        if (animator == null)
            animator = GetComponentInChildren<Animator>(true);

        if (animator != null)
        {
            animator.SetInteger("WeaponType_int", 1);
        }
        else
        {
            Debug.LogWarning($"{nameof(PlayerController)} on {name} has no Animator assigned or found in children.");
        }

        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    private void Update()
    {
        if (isDead) return;
        if (cameraTransform == null) return;
       
        // Aiming
        bool isAiming = Input.GetMouseButton(1);
        if (animator != null)
            animator.SetBool("IsAiming", isAiming);

        // shooting (for animation trigger)
        if (isAiming && Input.GetMouseButtonDown(0))
        {
            shotQueued = true;
        }

        if (animator != null)
        {
            animator.SetBool("Shoot_b", shotQueued);
        }
        shotQueued = false;

        // movement input
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        // direction relative to camera
        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;
        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        moveDir = (camForward * verticalInput + camRight * horizontalInput).normalized;

        // sprint multiplier
        bool sprintInput = Input.GetKey(KeyCode.LeftShift) && isOnGround && verticalInput > 0f;
        float targetMultiplier = sprintInput ? speedMultiplier : 1f;
        currentSpeedMultiplier = Mathf.Lerp(currentSpeedMultiplier, targetMultiplier, accelerationSpeed * Time.deltaTime);

        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && isOnGround)
        {
            wantsToJump = true;
            isOnGround = false;

            if (animator != null)
                animator.SetBool("Jump_b", true);
        }

        if (animator != null)
            animator.SetBool("Grounded", isOnGround);


        // rotate player to face camera direction
        bool hasMoveInput = Mathf.Abs(horizontalInput) > 0.1f || Mathf.Abs(verticalInput) > 0.1f;
        if (hasMoveInput)
        {
            Vector3 facingDir = cameraTransform.forward;
            facingDir.y = 0f; // keep it flat

            if (facingDir.sqrMagnitude > 0.001f)
                desiredRotation = Quaternion.LookRotation(facingDir);
        }

        // animator movement parameters
        if (animator != null)
        {
            animator.SetFloat("Speed_f", moveDir.magnitude);
            animator.SetBool("IsSprinting", sprintInput);
        }  
    }
      
    private void FixedUpdate()
    {
        if (isDead || playerRb == null) return;

        Vector3 currentVel = playerRb.linearVelocity;
        Vector3 targetVel = moveDir * speed * currentSpeedMultiplier;

        // move player
        playerRb.linearVelocity = new Vector3(targetVel.x, currentVel.y, targetVel.z);

        // jump (impulse)
        if (wantsToJump)
        {
            playerRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isOnGround = false;
            wantsToJump = false;
        }

        // smooth rotation
        playerRb.MoveRotation(
            Quaternion.RotateTowards(playerRb.rotation, desiredRotation, turnSpeed * Time.fixedDeltaTime));

    }

      private void OnCollisionEnter(Collision collision)
      {
        if (!collision.gameObject.CompareTag("Ground")) return;

            isOnGround = true;

            if (animator != null)
                animator.SetBool("Jump_b", false);
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

    public void Die(int type = 1)
    {
        if (isDead) return;
        isDead = true;

        // stop movement immediately
        moveDir = Vector3.zero;
        wantsToJump = false;


        if (animator != null)
        {
            // stop movement signals
            animator.SetFloat("Speed_f", 0f);
            animator.SetBool("IsSprinting", false);
            animator.SetBool("Jump_b", false);
            animator.SetBool("Shoot_b", false);
            animator.SetBool("IsAiming", false);

            // DEATH
            animator.SetInteger("DeathType_int", type);
            animator.SetBool("IsDead", true);
        }

        if (playerRb != null)
        {
            // physics stop, gravity still works
            playerRb.linearVelocity = Vector3.zero;
            playerRb.angularVelocity = Vector3.zero;
        }
    }
}
