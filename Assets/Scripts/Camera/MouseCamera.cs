using UnityEngine;

public class MouseCamera : MonoBehaviour
{

    public Transform player;
    public float mouseSensitivity = 100f;

    [Header("Camera Positioning")]
    // offset from player position
    [SerializeField] private Vector3 offset = new Vector3(0, 3, -3.5f);
    // how far camera tilts down
    [SerializeField] private float pitch = 21f;

    [Header("Collision")]
    [SerializeField] private LayerMask collisionLayers;  // which layers count as "walls"
    [SerializeField] private float collisionRadius = 0.3f;  // radius of sphere for collision detection
    [SerializeField] private float collisionBuffer = 0.1f;  // how far from the wall to stay
    [SerializeField] private float minDistance = 0.5f;  // don't move camera inside player

    float yaw;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        // start yaw from whatever Y rotation the camera has in the editor
        yaw = transform.eulerAngles.y;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // mouse rotation
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        yaw += mouseX;

        // rotate camera around player
        Quaternion rot = Quaternion.Euler(pitch, yaw, 0);

        // where the camera wants to be (above and behind player)
        Vector3 desiredOffset = rot * offset;
        Vector3 desiredPos = player.position + desiredOffset;

        // ---- Collision Detection ----
        Vector3 origin = player.position;
        Vector3 direction = desiredOffset.normalized;
        float targetDistance = desiredOffset.magnitude;

        //cast a sphere from player to desired camera position
        if (Physics.SphereCast(origin, collisionRadius, direction, out RaycastHit hit, targetDistance, collisionLayers, QueryTriggerInteraction.Ignore))
        {
            // move camera to just in front of the wall
            float adjustedDistance = hit.distance - collisionBuffer;

            // Don't let the camera get too close to the player
            if (adjustedDistance < minDistance)
            {
                adjustedDistance = minDistance;
            }

            transform.position = origin + direction * adjustedDistance;
        }

        else
        {
            // no wall, use normal desired position
            transform.position = desiredPos;
        }

        // apply rotation to camera
        transform.rotation = rot;
    }
}
