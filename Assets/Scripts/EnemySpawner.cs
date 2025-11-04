using UnityEngine;
using System.Collections; 

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    public GameObject normalEnemyPrefab;
    public GameObject timeAddPrefab;
    public GameObject timeSubtractPrefab;

    [Header("Wave Settings")]
    [Tooltip("Time BETWEEN different waves")]
    public float waveDelay = 3f; 
    [Tooltip("Time BETWEEN each enemy in a single wave")]
    public float spawnInWaveDelay = 0.2f; 

    [Header("Special Enemy Settings")]
    [Tooltip("Time BETWEEN special +/- enemy spawns")]
    public float specialEnemySpawnDelay = 10f;
    
    [Header("Zone Settings")]
    [Tooltip("How far from the top/bottom edge to spawn")]
    public float spawnPadding = 2.0f;

    
    private Camera mainCamera;
    private float minX, maxX, minY, maxY;
    private bool wavesStarted = false;
    
    private Vector2 playerTargetArea; 

    void Start()
    {
        mainCamera = Camera.main;
        Vector2 minBounds = mainCamera.ViewportToWorldPoint(new Vector2(0, 0));
        Vector2 maxBounds = mainCamera.ViewportToWorldPoint(new Vector2(1, 1));
        
        minX = minBounds.x;
        maxX = maxBounds.x;
        minY = minBounds.y;
        maxY = maxBounds.y;
        
        playerTargetArea = new Vector2(0, minY + spawnPadding);
    }

    void Update()
    {
        if (GameManager.Instance.isGameActive && !wavesStarted)
        {
            wavesStarted = true;
            StartCoroutine(SpawnWaves()); 
            
            if (DataManager.Instance.selectedMode == GameMode.Timed)
            {
                StartCoroutine(SpawnSpecialEnemies());
            }
        }
    }

    IEnumerator SpawnWaves()
    {
        yield return new WaitForSeconds(1f); 
        
        while (GameManager.Instance.isGameActive)
        {
            int waveType = Random.Range(0, 3); 
            switch (waveType)
            {
                case 0:
                    yield return StartCoroutine(SpawnWave_SideStreams());
                    break;
                case 1:
                    yield return StartCoroutine(SpawnWave_V_Formation());
                    break;
                case 2:
                    yield return StartCoroutine(SpawnWave_TopLine());
                    break;
            }
            yield return new WaitForSeconds(waveDelay);
            if (DataManager.Instance.selectedMode == GameMode.Survival)
            {
                waveDelay *= 0.90f; // Made this faster as requested
            }
        }
    }

    IEnumerator SpawnSpecialEnemies()
    {
        while (GameManager.Instance.isGameActive)
        {
            yield return new WaitForSeconds(specialEnemySpawnDelay);
            
            float x = (Random.value < 0.5f) ? minX - 1f : maxX + 1f; 
            float y = Random.Range(minY + spawnPadding, maxY - spawnPadding); 
            
            GameObject prefabToSpawn = (Random.value < 0.5f) ? timeAddPrefab : timeSubtractPrefab;
            
            SpawnEnemy(prefabToSpawn, new Vector2(x, y));
        }
    }

    IEnumerator SpawnWave_SideStreams()
    {
        float y = maxY - spawnPadding; 
        for (int i = 0; i < 6; i++)
        {
            SpawnEnemy(normalEnemyPrefab, new Vector2(minX - 1f, y)); 
            SpawnEnemy(normalEnemyPrefab, new Vector2(maxX + 1f, y));
            y -= 1.5f; 
            yield return new WaitForSeconds(spawnInWaveDelay * 2f); 
        }
    }

    IEnumerator SpawnWave_V_Formation()
    {
        float y = maxY + 1f; 
        float xSpacing_1 = 1.5f; 
        float xSpacing_2 = 3.0f; 
        
        SpawnEnemy(normalEnemyPrefab, new Vector2(0, y));
        yield return new WaitForSeconds(spawnInWaveDelay);
        
        SpawnEnemy(normalEnemyPrefab, new Vector2(-xSpacing_1, y));
        SpawnEnemy(normalEnemyPrefab, new Vector2(xSpacing_1, y));
        yield return new WaitForSeconds(spawnInWaveDelay);

        SpawnEnemy(normalEnemyPrefab, new Vector2(-xSpacing_2, y));
        SpawnEnemy(normalEnemyPrefab, new Vector2(xSpacing_2, y));
    }
    
    IEnumerator SpawnWave_TopLine()
    {
        float x = minX + 1.5f; 
        for (int i = 0; i < 5; i++)
        {
            SpawnEnemy(normalEnemyPrefab, new Vector2(x, maxY + 1f));
            x += 1.5f; 
            yield return new WaitForSeconds(spawnInWaveDelay);
        }
    }
    
    void SpawnEnemy(GameObject prefabToSpawn, Vector2 spawnPosition)
    {
        GameObject enemyObj = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
        
        Enemy enemyScript = enemyObj.GetComponent<Enemy>();
        if (enemyScript != null)
        {
            Vector2 moveDirection = (playerTargetArea - spawnPosition).normalized;
            enemyScript.moveDirection = moveDirection;

            // Check if the slow power-up is active
            if (GameManager.Instance.isEnemySlowActive)
            {
                enemyScript.SetSlowed(true);
            }
        }
    }
}