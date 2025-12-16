using UnityEditor.Build;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{

    public GameObject[] enemyPrefabs;
    public GameObject powerupPrefab;

    public BoxCollider arenaBounds;

    public int enemyCount;
    public int waveNumber = 1;

    public float spawnPotion = 1f; // Height at which to spawn objects
    public float spawnEnemy = 0f;     // height of enemy spawn
    public float edgePadding = 1.0f; // Padding from the edges of the arena

    private PlayerHealth playerHealth;

    [Header("Win Condition")]
    [SerializeField] private int maxWave = 30;          // final wave
    private bool gameEnded = false;                     // stop logic after win/lose

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // get PlayerHealth so we can stop spawning if player is dead
        GameObject player = GameObject.Find("Player");
        if (player != null)
        {
            playerHealth = player.GetComponent<PlayerHealth>();
        }

        // spawn first wave
        SpawnEnemyWave(waveNumber);

        // update UI for wave 1
        if (MainUI.Instance != null)
        {
            MainUI.Instance.UpdateWave(waveNumber);
        }

        // spawn initial powerup
        Instantiate(powerupPrefab, GeneratePotionPosition(), powerupPrefab.transform.rotation);
    }

    // Update is called once per frame
    void Update()
    {
        // if game has already ended, do nothing
        if (gameEnded) return;

        // if player is dead, do not spawn more enemies
        if (playerHealth != null && playerHealth.IsDead)
        {
            return;
        }

        enemyCount = FindObjectsByType<Enemy>(FindObjectsSortMode.None).Length;

        // counts number of enemies in the scene, if 0, spawns next wave
        if (enemyCount == 0)
        {
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
            if (powerupCount == 0)
            {
                Instantiate(powerupPrefab, GeneratePotionPosition(), powerupPrefab.transform.rotation);
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

            Instantiate(enemyPrefabs[index], GenerateEnemyPosition(), enemyPrefabs[index].transform.rotation);
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
