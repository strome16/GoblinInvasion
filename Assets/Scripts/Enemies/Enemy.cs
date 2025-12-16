using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Enemy : MonoBehaviour
{

    [Header("Movement")]
    [SerializeField] protected float speed = 3f;
    [SerializeField] private float pushForce = 1;

    private Rigidbody enemyRb;
    private GameObject player;
    protected Transform playerTransform;

    [Header("Health")]
    [SerializeField] protected int maxHealth = 3;
    private int currentHealth;

    [Header("Combat")]
    [SerializeField] protected int contactDamage = 10;      // deal 10 damage upon contact wtih player
    [SerializeField] private float damageInterval = 1f;     // seconds between hits

    private float lastDamageTime = Mathf.NegativeInfinity;
    private PlayerHealth playerHealth;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        enemyRb = GetComponent<Rigidbody>();

        GameObject playerObj = GameObject.Find("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
            playerHealth = playerObj.GetComponent<PlayerHealth>();
        }
        else
        {
            Debug.LogWarning($"{nameof(Enemy)} on {name} could not find a GameObject named 'Player'.");
        }

        // allow subclasses to adjust stats before setting currentHealth
        InitializeStats();

        // start with full health
        currentHealth = maxHealth;
    }

    /// <summary>
    /// Subclasses can override this to change speed / maxHealth / contactDamage
    /// Called once from Start(), before currentHealth is set
    /// </summary>
    protected virtual void InitializeStats()
    {
        // base enemy uses inspector values
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (playerTransform == null) return;

        // if player is dead, stop moving
        if (playerHealth != null && playerHealth.IsDead)
        {
            enemyRb.linearVelocity = Vector3.zero;
            return;
        }

        // direction to player
        Vector3 dir = playerTransform.position - transform.position;
        dir.y = 0f;


        if (dir.sqrMagnitude > 0.001f)
        {
            // rotate to face player
            Quaternion targetRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRot,
                10f * Time.deltaTime);

            // move towards player
            Vector3 horizontalVel = dir.normalized * speed;
            enemyRb.linearVelocity = new Vector3(
                horizontalVel.x,
                enemyRb.linearVelocity.y, // keeps gravity
                horizontalVel.z);
        }
        else
        {
            // stop horizontal motion, keep vertical
            enemyRb.linearVelocity = Vector3.zero;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
            return;

        // if player is dead, do not damage
            if (playerHealth != null && playerHealth.IsDead) return;

        //first hit as soon as enemy touches player
        TryDamagePlayer(collision.gameObject); 
    }

    private void OnCollisionStay(Collision collision)
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
            if (playerHealth != null && playerHealth.IsDead) return;

            TryDamagePlayer(collision.gameObject);
        }
    }

    private void TryDamagePlayer(GameObject playerObject)
    {
        // only damage if enough time has passed since last hit
        if (Time.time < lastDamageTime + damageInterval)
            return;

        if (playerObject.TryGetComponent<PlayerHealth>(out var targetHealth))
        {
            targetHealth.TakeDamage(contactDamage);
            lastDamageTime = Time.time;
        }
    }

    public virtual void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }

}
