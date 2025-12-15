using UnityEngine;

public class Laser : MonoBehaviour
{

    public float speed = 20f; // speed of the laser projectile
    public int damage = 1; // damage dealt by the laser

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
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

        if (other.gameObject.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }

}