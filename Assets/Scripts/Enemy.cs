using UnityEngine;

public class Enemy : MonoBehaviour
{

    public float speed = 5f;
    private Rigidbody enemyRb;
    private GameObject player;
    public float pushForce;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemyRb = GetComponent<Rigidbody>();
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 lookDirection = (player.transform.position - transform.position).normalized;
        enemyRb.linearVelocity = lookDirection * speed;
    }

    void OnCollisionStay(Collision other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Vector3 pushDir = (transform.position - other.transform.position).normalized;
            enemyRb.AddForce(pushDir * pushForce, ForceMode.VelocityChange);
        }

    }
}
