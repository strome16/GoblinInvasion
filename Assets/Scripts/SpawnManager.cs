using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private GameObject powerupPrefab;

    [Header("Arena")]
    [SerializeField] private BoxCollider arenaBounds;
    [SerializeField] private float spawnPotion = 1f; // Height at which to spawn objects
    [SerializeField] private float spawnEnemy = 0f;     // height of enemy spawn
    [SerializeField] private float edgePadding = 1.0f; // Padding from the edges of the arena

    [Header("Waves")]

    [SerializeField] private int waveNumber = 1;
    [SerializeField] private int maxWave = 30;

    private int enemyCount;
    private PlayerHealth playerHealth;
    private Transform playerTransform;

    private bool gameEnded = false;                     // stop logic after win/lose

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        // get PlayerHealth so we can stop spawning if player is dead
        GameObject player = GameObject.Find("Player");
        if (player != null)
        {
            playerHealth = player.GetComponent<PlayerHealth>();
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogWarning("SpawnManager could not find a GameObject named 'Player'.");
        }

        // spawn first wave
        SpawnEnemyWave(waveNumber);

        // update UI for wave 1
        if (MainUI.Instance != null)
        {
            MainUI.Instance.UpdateWave(waveNumber);
        }

        // spawn initial powerup
        if (powerupPrefab != null)
            Instantiate(powerupPrefab, GeneratePotionPosition(), powerupPrefab.transform.rotation);
    }

    // Update is called once per frame
    private void Update()
    {
        // if game has already ended, do nothing
        if (gameEnded) return;

        // if player is dead, do not spawn more enemies
        if (playerHealth != null && playerHealth.IsDead)
        {
            return;
        }

        // counts number of enemies in the scene, if 0, spawns next wave
        enemyCount = FindObjectsByType<Enemy>(FindObjectsSortMode.None).Length;
        if (enemyCount != 0) return;

        // if final wave defeated, trigger win instead of next wave
        if (waveNumber >= maxWave)
        {
            gameEnded = true;

            if (MainUI.Instance != null)
            {
                MainUI.Instance.ShowWinPanel();
            }
            return; // don't spawn more enemies or health potions
        }

        // increase wave number
        waveNumber++;
        SpawnEnemyWave(waveNumber);

        // update UI for new wave
        if (MainUI.Instance != null)
        {
            MainUI.Instance.UpdateWave(waveNumber);
        }

        // spawn a powerup if there are none present
        int powerupCount = GameObject.FindGameObjectsWithTag("Powerup").Length;
        if (powerupCount == 0 && powerupPrefab != null)
        {
            Instantiate(powerupPrefab, GeneratePotionPosition(), powerupPrefab.transform.rotation);
        }
    }


    private void SpawnEnemyWave(int enemiesToSpawn)
    {
        if (enemyPrefabs == null || enemyPrefabs.Length == 0)
        {
            Debug.LogWarning("SpawnManager has no enemyPrefabs assigned.");
            return;
        }

        // spawns a number of enemies equal to the wave number
        for (int i = 0; i < enemiesToSpawn; i++)
        {
            // select a random enemy prefab to spawn
            int index = Random.Range(0, enemyPrefabs.Length);
            GameObject enemyToSpawn = enemyPrefabs[index];

            Vector3 spawnPos = GenerateEnemyPosition();
            Quaternion spawnRot = enemyToSpawn.transform.rotation;

            // if player location known, face towards them
            if (playerTransform != null)
            {
                Vector3 toPlayer = playerTransform.position - spawnPos;
                toPlayer.y = 0f; // keeps upright

                if (toPlayer.sqrMagnitude > 0.0001f)
                {
                    spawnRot = Quaternion.LookRotation(toPlayer);
                }
            }

            Instantiate(enemyToSpawn, spawnPos, spawnRot);
        }
    }

    private Vector3 GeneratePotionPosition()
    {
        // use areaBounds to get the size of the arena
        Bounds b = arenaBounds.bounds;

        float x = Random.Range(b.min.x + edgePadding, b.max.x - edgePadding);
        float z = Random.Range(b.min.z + edgePadding, b.max.z - edgePadding);

        return new Vector3(x, spawnPotion, z);
    }

    private Vector3 GenerateEnemyPosition()
    {
        // use areaBounds to get the size of the arena
        Bounds b = arenaBounds.bounds;

        float x = Random.Range(b.min.x + edgePadding, b.max.x - edgePadding);
        float z = Random.Range(b.min.z + edgePadding, b.max.z - edgePadding);

        return new Vector3(x, spawnEnemy, z);
    }
}
