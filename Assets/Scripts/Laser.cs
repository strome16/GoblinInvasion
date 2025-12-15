using UnityEngine;

public class Laser : MonoBehaviour
{

    public float speed = 20f; // speed of the laser projectile

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Destroy(other.gameObject); // destroy enemy on collision
            Destroy(gameObject); // destroy projectile
        }

        if (other.gameObject.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {

        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

}