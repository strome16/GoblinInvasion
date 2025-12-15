using UnityEngine;

public class FastEnemy : Enemy
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
        {
            speed = 6f; // Fast enemy speed
            maxHealth = 2; // Fast enemy health (weaker)
            contactDamage = 5; // Fast enemy contact damage (less damage)
        }
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }
}
