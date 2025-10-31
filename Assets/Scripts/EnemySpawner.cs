using UnityEngine;
using System.Collections; // We must add this for Coroutines!

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    public GameObject normalEnemyPrefab;
    public GameObject timeAddPrefab;
    public GameObject timeSubtractPrefab;

    [Header("Spawn Settings")]
    [Tooltip("Time BETWEEN different waves")]
    public float waveDelay = 4f; 
    
    [Tooltip("Time BETWEEN each enemy in a single wave")]
    public float spawnInWaveDelay = 0.2f; 
    
    [Tooltip("Chance to spawn a +/- time enemy in Timed Mode")]
    public float timeEnemyChance = 0.2f; 

    
    private Camera mainCamera;
    private float minX, maxX, spawnY;
    private bool wavesStarted = false;

    void Start()
    {
        mainCamera = Camera.main;
        Vector2 minBounds = mainCamera.ViewportToWorldPoint(new Vector2(0, 0));
        Vector2 maxBounds = mainCamera.ViewportToWorldPoint(new Vector2(1, 1));
        minX = minBounds.x;
        maxX = maxBounds.x;
        spawnY = transform.position.y; // Get the spawner's Y position
    }

    void Update()
    {
        // Wait for the GameManager to set isGameActive to true
        if (GameManager.Instance.isGameActive && !wavesStarted)
        {
            wavesStarted = true;
            StartCoroutine(SpawnWaves());
        }
    }

    IEnumerator SpawnWaves()
    {
        while (GameManager.Instance.isGameActive)
        {
            int waveType = Random.Range(0, 3); 
            switch (waveType)
            {
                case 0:
                    yield return StartCoroutine(SpawnWave_W());
                    break;
                case 1:
                    yield return StartCoroutine(SpawnWave_DoubleRow());
                    break;
                case 2:
                    yield return StartCoroutine(SpawnWave_Triangle());
                    break;
            }
            yield return new WaitForSeconds(waveDelay);
            if (DataManager.Instance.selectedMode == GameMode.Survival)
            {
                waveDelay *= 0.98f; 
            }
        }
    }

    // --- FORMATION DEFINITIONS ---

    // Formation 1: "W" shape
    IEnumerator SpawnWave_W()
    {
        float w_width = (maxX - minX) * 0.8f; 
        float w_height = 4f; // Increased height for more space
        float center = 0;
        
        Vector2 p1 = new Vector2(center - w_width / 2, spawnY - w_height); 
        Vector2 p2 = new Vector2(center - w_width / 4, spawnY);          
        Vector2 p3 = new Vector2(center, spawnY - w_height);              
        Vector2 p4 = new Vector2(center + w_width / 4, spawnY);          
        Vector2 p5 = new Vector2(center + w_width / 2, spawnY - w_height); 

        // Use a slightly longer delay for this complex wave
        float delay = spawnInWaveDelay * 1.2f; 

        // Spawn 4 enemies for each "leg" of the W
        for (int i = 0; i < 4; i++) {
            float t = (float)i / 3; 
            SpawnEnemy(Vector2.Lerp(p1, p2, t));
            yield return new WaitForSeconds(delay);
        }
        for (int i = 0; i < 4; i++) {
            float t = (float)i / 3;
            SpawnEnemy(Vector2.Lerp(p2, p3, t));
            yield return new WaitForSeconds(delay);
        }
        for (int i = 0; i < 4; i++) {
            float t = (float)i / 3;
            SpawnEnemy(Vector2.Lerp(p3, p4, t));
            yield return new WaitForSeconds(delay);
        }
        for (int i = 0; i < 4; i++) {
            float t = (float)i / 3;
            SpawnEnemy(Vector2.Lerp(p4, p5, t));
            yield return new WaitForSeconds(delay);
        }
    }

    // Formation 2: 10 enemies in two rows of 5
    IEnumerator SpawnWave_DoubleRow()
    {
        // I've increased the rowSpacing from 1.5 to 2.0
        float rowSpacing = 2.0f; 
        // I've increased the range for startX to prevent clumping on the edge
        float startX = Random.Range(minX + 2f, maxX - (rowSpacing * 5f)); 
        
        // Top Row
        for (int i = 0; i < 5; i++)
        {
            SpawnEnemy(new Vector2(startX + i * rowSpacing, spawnY));
            yield return new WaitForSeconds(spawnInWaveDelay); 
        }
        
        yield return new WaitForSeconds(0.5f);
        
        // Bottom Row
        for (int i = 0; i < 5; i++)
        {
            SpawnEnemy(new Vector2(startX + i * rowSpacing, spawnY - rowSpacing));
            yield return new WaitForSeconds(spawnInWaveDelay); 
        }
    }
    
    // Formation 3: Triangle with special enemy
    IEnumerator SpawnWave_Triangle()
    {
        float centerX = 0; 
        float y = spawnY;
        // I've increased the spacing variables
        float rowSpacing = 2.0f; 
        float xSpacing = 1.8f;   

        // Row 1 (Top)
        SpawnEnemy(new Vector2(centerX, y));
        yield return new WaitForSeconds(spawnInWaveDelay * 2);
        
        // Row 2
        y -= rowSpacing;
        SpawnEnemy(new Vector2(centerX - xSpacing, y));
        yield return new WaitForSeconds(spawnInWaveDelay);
        SpawnEnemy(new Vector2(centerX + xSpacing, y));
        yield return new WaitForSeconds(spawnInWaveDelay * 2);

        // Row 3
        y -= rowSpacing;
        SpawnEnemy(new Vector2(centerX - (xSpacing * 2), y));
        yield return new WaitForSeconds(spawnInWaveDelay);
        SpawnEnemy(new Vector2(centerX + (xSpacing * 2), y));
        yield return new WaitForSeconds(spawnInWaveDelay * 2);

        // Row 4 (Bottom, with special enemy)
        y -= rowSpacing;
        SpawnEnemy(new Vector2(centerX - (xSpacing * 3), y));
        yield return new WaitForSeconds(spawnInWaveDelay);
        SpawnEnemy(new Vector2(centerX + (xSpacing * 3), y));
        yield return new WaitForSeconds(spawnInWaveDelay);
        
        // Force-spawn a special enemy in the middle
        SpawnEnemy(new Vector2(centerX, y), true);
    }
    
    // --- Helper function to create the enemy ---
    void SpawnEnemy(Vector2 spawnPosition, bool forceSpecial = false)
    {
        GameObject prefabToSpawn = normalEnemyPrefab;

        bool spawnSpecial = forceSpecial;
        if (!forceSpecial && DataManager.Instance.selectedMode == GameMode.Timed)
        {
            if (Random.value < timeEnemyChance) 
            {
                spawnSpecial = true;
            }
        }
        
        if (spawnSpecial)
        {
            prefabToSpawn = (Random.value < 0.5f) ? timeAddPrefab : timeSubtractPrefab;
        }
        
        Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
    }
}