using UnityEngine;

public class FirePoint : MonoBehaviour
{

    public GameObject laserPrefab;
    public Transform firePoint;

    private PlayerHealth playerHealth;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // find PlayerHealth component on thie object or any parent
        playerHealth = GetComponentInParent<PlayerHealth>();
    }

    // Update is called once per frame
    void Update()
    {
        // if no reference/player is dead, do nothing
        if (playerHealth == null || playerHealth.IsDead)
        {
            return;
        }

        // fire laser when left mouse button is clicked
        if (Input.GetMouseButtonDown(0))
        {
            Instantiate(laserPrefab, firePoint.position, firePoint.rotation);
        }
    }
}
