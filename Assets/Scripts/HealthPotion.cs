using UnityEngine;

[RequireComponent(typeof(Collider))]
public class HealthPotion : MonoBehaviour
{
    [SerializeField] private int healAmount = 15;
    [SerializeField] private float rotateSpeed = 90f; // degrees per second

    private void Update()
    {
        // rotate the potion around its Y axis
        transform.Rotate(0f, rotateSpeed * Time.deltaTime, 0f, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (other.TryGetComponent<PlayerHealth>(out var playerHealth))
        {
            playerHealth.Heal(healAmount);
        }

            // Destroy the potion after it has been collected
            Destroy(gameObject);
    }
}
