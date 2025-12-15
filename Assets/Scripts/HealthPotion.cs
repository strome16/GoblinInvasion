using UnityEngine;

public class HealthPotion : MonoBehaviour
{
    [SerializeField] private int healAmount = 15;
    [SerializeField] private float rotateSpeed = 90f; // degrees per second

    // Update is called once per frame
    void Update()
    {
        // rotate the potion around its Y axis
        transform.Rotate(0f, rotateSpeed * Time.deltaTime, 0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
           PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.Heal(healAmount);
            }

            // Destroy the potion after it has been collected
            Destroy(gameObject);
        }
    }
}
