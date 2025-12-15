using UnityEngine;

public class MouseCamera : MonoBehaviour
{

    public Transform player;
    public float mouseSensitivity = 100f;
    [SerializeField] private Vector3 offset = new Vector3(0, 3, -3.5f);

    // how far camera tilts down
    [SerializeField] private float pitch = 21f;

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
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        yaw += mouseX;

        // rotate camera around player
        Quaternion rot = Quaternion.Euler(pitch, yaw, 0);

        // position camera behind and above player using offset
        transform.position = player.position + rot * offset;

        // apply rotation to camera
        transform.rotation = rot;
    }
}
