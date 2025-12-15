using UnityEngine;

public class SpawnManager : MonoBehaviour
{

    public GameObject[] enemyPrefabs;
    public GameObject powerupPrefab;

    public BoxCollider arenaBounds;

    public int enemyCount;
    public int waveNumber = 1;

    public float spawnY = 0.25f; // Height at which to spawn objects
    public float edgePadding = 1.0f; // Padding from the edges of the arena

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpawnEnemyWave(waveNumber);
        Instantiate(powerupPrefab, GenerateSpawnPosition(), powerupPrefab.transform.rotation);
    }

    // Update is called once per frame
    void Update()
    {
        enemyCount = FindObjectsByType<Enemy>(FindObjectsSortMode.None).Length;

        // counts number of enemies in the scene, if 0, spawns next wave
        if (enemyCount == 0)
        {
            // increase wave number
            waveNumber++;
            SpawnEnemyWave(waveNumber);

            // spawn a powerup if there are none present
            int powerupCount = GameObject.FindGameObjectsWithTag("Powerup").Length;
            if (powerupCount == 0)
            {
                Instantiate(powerupPrefab, GenerateSpawnPosition(), powerupPrefab.transform.rotation);
            }
        }
    }

    void SpawnEnemyWave(int enemiesToSpawn)
    {
        // spawns a number of enemies equal to the wave number
        for (int i = 0; i < enemiesToSpawn; i++)
        {
            // select a random enemy prefab to spawn
            int index = Random.Range(0, enemyPrefabs.Length);
            GameObject enemyToSpawn = enemyPrefabs[index];

            Instantiate(enemyPrefabs[index], GenerateSpawnPosition(), enemyPrefabs[index].transform.rotation);
        }
    }

    private Vector3 GenerateSpawnPosition()
    {
        // use areaBounds to get the size of the arena
        Bounds b = arenaBounds.bounds;

        float x = Random.Range(b.min.x + edgePadding, b.max.x - edgePadding);
        float z = Random.Range(b.min.z + edgePadding, b.max.z - edgePadding);

        return new Vector3(x, spawnY, z);
    }

}
