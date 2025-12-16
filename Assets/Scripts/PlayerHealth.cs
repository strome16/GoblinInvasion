using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    public bool IsDead { get; private set; }

    private void Awake()
    {
        currentHealth = maxHealth;
        IsDead = false;

        if (MainUI.Instance != null)
        {
            MainUI.Instance.UpdateHealth(currentHealth, maxHealth);
        }
    }

    public void TakeDamage(int damage)
    {
        if (IsDead) return; // don't take damage if already dead

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        if (MainUI.Instance != null)
        {
            MainUI.Instance.UpdateHealth(currentHealth, maxHealth);
        }

        Debug.Log($"Player took {damage} damage, current health: {currentHealth}");

        if (currentHealth <= 0 && !IsDead)
        {
            Die();
        }
    }

    public void Heal(int  amount)
    {
        if (IsDead) return; // don't heal if dead

        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth); // clamp to maxHealth (100)

        if (MainUI.Instance != null)
        {
            MainUI.Instance.UpdateHealth(currentHealth, maxHealth);
        }

        Debug.Log($"Player healed {amount}, current health: {currentHealth}");
    }
   private void Die()
    {
        IsDead = true;
        Debug.Log("Player has died!");

        // trigger death animation
        PlayerController controller = GetComponent<PlayerController>();
        if (controller != null)
        {
            controller.Die(1);
        }

        // disable shooting
        FirePoint fire = GetComponentInChildren<FirePoint>();
        if (fire != null)
        {
            fire.enabled = false;
        }

        //show game over UI
        if (MainUI.Instance != null)
        {
            MainUI.Instance.ShowGameOver();
        }
    }
}
