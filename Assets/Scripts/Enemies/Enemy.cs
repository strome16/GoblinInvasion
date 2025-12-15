using UnityEngine;

public class Enemy : MonoBehaviour
{

    [Header("Movement")]
    public float speed = 3f;
    private Rigidbody enemyRb;
    private GameObject player;
    private float pushForce = 1;

    [Header("Health")]
    [SerializeField] protected int maxHealth = 3;
    private int currentHealth;

    [Header("Combat")]
    [SerializeField] protected int contactDamage = 10; // deal 10 damage upon contact wtih player
    [SerializeField] private float damageInterval = 1f; // seconds between hits
    private float lastDamageTime = Mathf.NegativeInfinity;
    private PlayerHealth playerHealth;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        enemyRb = GetComponent<Rigidbody>();
        player = GameObject.Find("Player");

        if (player != null)
        {
            playerHealth = player.GetComponent<PlayerHealth>();
        }

        // start with full health
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (player == null) return;

        // if player is dead, stop moving
        if (playerHealth != null && playerHealth.IsDead)
        {
            enemyRb.linearVelocity = Vector3.zero;
            return;
        }

        // movement towards player
        Vector3 lookDirection = (player.transform.position - transform.position).normalized;
        enemyRb.linearVelocity = lookDirection * speed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // if player is dead, do not damage
            if (playerHealth != null && playerHealth.IsDead) return;
   
                TryDamagePlayer(collision.gameObject); //first hit as soon as enemy touches player
        }
    }

    void OnCollisionStay(Collision collision)
    {
        // keep enemies away from each other
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Vector3 pushDir = (transform.position - collision.transform.position).normalized;
            enemyRb.AddForce(pushDir * pushForce, ForceMode.VelocityChange);
        }

        // continuous damage to the player if touching
        if (collision.gameObject.CompareTag("Player"))
        {
            // don't damage if player is dead
            if (playerHealth != null && playerHealth.IsDead) return;

            TryDamagePlayer(collision.gameObject);
        }
    }

    private void TryDamagePlayer(GameObject playerObject)
    {
        // only damage if enough time has passed since last hit
        if (Time.time >= lastDamageTime + damageInterval)
        {
            PlayerHealth playerHealth = playerObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(contactDamage);
                lastDamageTime = Time.time;
            }
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }

}
