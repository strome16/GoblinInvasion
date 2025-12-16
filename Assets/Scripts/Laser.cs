using UnityEngine;

public class Laser : MonoBehaviour
{
    [Header("Laser Settings")]
    [SerializeField] private float speed = 20f; // speed of the laser projectile
    [SerializeField] private int damage = 1; // damage dealt by the laser
    [SerializeField] private float maxLife = 5f;

    private void Start()
    {
        // cleans up bullet in case it doesnt hit anything
        Destroy(gameObject, maxLife);
    }

    // Update is called once per frame
    private void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        // check if the laser hits an enemy
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            // tell enemy to take damage
            enemy.TakeDamage(damage);

            //destroy the laser projectile on hit
            Destroy(gameObject);
            return; // exit method before checking for wall
        }

        if (other.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }

}