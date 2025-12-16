using UnityEngine;

public class FirePoint : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject laserPrefab;
    [SerializeField] private Transform firePoint;

    private PlayerHealth playerHealth;
    private Animator animator;

    private void Start()
    {
        // find PlayerHealth component on thie object or any parent
        playerHealth = GetComponentInParent<PlayerHealth>();
        animator = GetComponentInParent<Animator>();

        if (laserPrefab == null)
            Debug.LogWarning($"{nameof(FirePoint)} on {name} has no laserPrefab assigned.");

        if (firePoint == null)
            Debug.LogWarning($"{nameof(FirePoint)} on {name} has no firePoint assigned.");
    }

    // Update is called once per frame
    private void Update()
    {
        // if no reference / player is dead, do nothing
        if (playerHealth == null || playerHealth.IsDead || animator == null)
            return;
        

        // fire laser when left mouse button is clicked AND aiming
        if (Input.GetMouseButtonDown(0) && animator != null && animator.GetBool("IsAiming"))
        {
            FireFlat();
        }
    }

    public void FireFlat()
    {
        if (laserPrefab == null || firePoint == null)
            return;

        // take firePoint forward and flatten vertical
        Vector3 dir = firePoint.forward;
        dir.y = 0f;

        // safety fallback
        if (dir.sqrMagnitude < 0.001f)
            dir = transform.forward;

        Quaternion rot = Quaternion.LookRotation(dir);
        Instantiate(laserPrefab, firePoint.position, rot);
    }
}
