using UnityEngine;
using UnityEngine.Rendering;

public class MouseCamera : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform player;

    [Header("Rotation")]
    [SerializeField] private float mouseSensitivity = 100f;

    [Header("Camera Positioning")]
    [SerializeField] private Vector3 offset = new Vector3(0, 3, -3.5f);     // offset from player position
    [SerializeField] private float pitch = 21f;                             // how far camera tilts down

    [Header("Collision")]
    [SerializeField] private LayerMask collisionLayers;     // which layers count as "walls"
    [SerializeField] private float collisionRadius = 0.3f;  // radius of sphere for collision detection
    [SerializeField] private float collisionBuffer = 0.1f;  // how far from the wall to stay
    [SerializeField] private float minDistance = 0.5f;      // don't move camera inside player

    float yaw;
    private Quaternion currentRotation;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        // auto-find player if not-assigned
        if (player == null)
        {
            GameObject playerObj = GameObject.Find("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
            else
            {
                Debug.LogWarning($"{nameof(MouseCamera)} on {name} has no player assigned and could not find one named 'Player'.");
            }
        }

        // start yaw from whatever Y rotation the camera has in the editor
        yaw = transform.eulerAngles.y;
        currentRotation = transform.rotation;
    }

    private void Update()
    {
        // mouse rotation
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        yaw += mouseX;

        // rotate camera around player
        currentRotation = Quaternion.Euler(pitch, yaw, 0);
        transform.rotation = currentRotation;
    }

    private void LateUpdate()
    {
        if (player == null) return;

        // where the camera wants to be (above and behind player)
        Vector3 desiredOffset = currentRotation * offset;
        Vector3 origin = player.position;
        Vector3 direction = desiredOffset.normalized;
        float targetDistance = desiredOffset.magnitude;

        Vector3 desiredPos = origin + desiredOffset;

        //cast a sphere from player to desired camera position
        if (Physics.SphereCast(origin, collisionRadius, direction, 
            out RaycastHit hit, targetDistance, collisionLayers, 
            QueryTriggerInteraction.Ignore))
        {
            // move camera to just in front of the wall
            float adjustedDistance = Mathf.Max(hit.distance - collisionBuffer, minDistance);
            transform.position = origin + direction * adjustedDistance;
        }
        else
        {
            transform.position = desiredPos;    
        }
          
    }
}
