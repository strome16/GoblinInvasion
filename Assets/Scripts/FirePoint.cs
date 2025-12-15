using UnityEngine;

public class FirePoint : MonoBehaviour
{

    public GameObject laserPrefab;
    public Transform firePoint;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // fire laser when left mouse button is clicked
        if (Input.GetMouseButtonDown(0))
        {
            Instantiate(laserPrefab, firePoint.position, firePoint.rotation);
        }
    }
}
