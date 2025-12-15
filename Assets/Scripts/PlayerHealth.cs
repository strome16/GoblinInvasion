using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;
    // anyone can read, but only script can change it
    public bool IsDead { get; private set; }

    private void Awake()
    {
        currentHealth = maxHealth;
        IsDead = false;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        Debug.Log($"Player took {damage} damage, current health: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int  amount)
    {
        if (IsDead) return; // don't heal if dead

        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth); // clamp to maxHealth (100)

        Debug.Log($"Player healed {amount}, current health: {currentHealth}");
    }
   private void Die()
    {
        IsDead = true;
        Debug.Log("Player has died!");
        
        // diable movement
        PlayerController controller = GetComponent<PlayerController>();
        if (controller != null)
        {
            controller.enabled = false;
        }

        // disable shooting
        FirePoint fire = GetComponent<FirePoint>();
        if (fire != null)
        {
            fire.enabled = false;
        }
    }
}
