using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random=UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    [Serializable]
    public class Wave
    {
        public string waveName;
        public List<enemyGroup> enemyGroups; //List of enemy groups in this wave
        public int waveQuota; //Total number of enemies in this wave
        public float spawnInterval; //Time between each spawn
        public int spawnCount; //Number of enemies spawned in this wave
    }

    [Serializable]
    public class enemyGroup
    {
        public string enemyName;
        public int enemyCount; //Number of enemies in this group
        public int spawnCount; //The number of enemies spawned in this group
        public GameObject enemyPrefab;
    }

    public List<Wave> waves; //List of waves
    public int currentWave; //The index of the current wave number

    [Header("Spawner Attributes")]
    float spawnTimer; //Timer for spawning enemies
    public int enemyAlive; //Number of enemies alive
    public int maxEnemyAllowed; //Maximum number of enemies allowed
    public bool maxEnemyReached = false; //Check if maximum number of enemies is reached
    public float waveInterval; //Time between each wave

    [Header("Spawn Position")]
    public List<Transform> spawnPositions; //List of spawn positions

    Transform player;



    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerStats>().transform;
        caculateWaveQuota();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentWave < waves.Count && waves[currentWave].spawnCount == 0)
        {
            StartCoroutine(BeginNextWave());
        }

        spawnTimer += Time.deltaTime;

        if (spawnTimer >= waves[currentWave].spawnInterval)
        {
            spawnTimer = 0f;
            spawnEnemy();
        }
    }

    IEnumerator BeginNextWave()
    {
        yield return new WaitForSeconds(waveInterval);
        if (currentWave < waves.Count)
        {
            currentWave++;
            caculateWaveQuota();
        }
    }

    void caculateWaveQuota()
    {
        int currentWaveQuota = 0;
        foreach (Wave wave in waves)
        {
            foreach (var enemyGroup in waves[currentWaveQuota].enemyGroups)
            {
                currentWaveQuota += enemyGroup.enemyCount;
            }
            waves[currentWave].waveQuota = currentWaveQuota;
        }
    }

    /// <summary>
    /// This method can stop spawning enemies if the maximum number of enemies is reached
    /// The method will only spawn enemies if the current wave's spawn count is less than the wave's quota
    /// </summary>
    void spawnEnemy()
    {
        if (waves[currentWave].spawnCount < waves[currentWave].waveQuota && !maxEnemyReached)
        {
            foreach (var enemyGroup in waves[currentWave].enemyGroups)
            {
                if (enemyGroup.spawnCount < enemyGroup.enemyCount)
                {
                    //Limit the number of enemies that can be spawned at once
                    if (enemyAlive >= maxEnemyAllowed)
                    {
                        maxEnemyReached = true;
                        return;
                    }
                    
                    //Spawn enemy at random position close to the player
                    Instantiate(enemyGroup.enemyPrefab, player.position + spawnPositions[Random.Range(0, spawnPositions.Count)].position, Quaternion.identity);

                    enemyGroup.spawnCount++;
                    waves[currentWave].spawnCount++;
                    enemyAlive++;
                }
            }
        }
        if (enemyAlive < maxEnemyAllowed)
        {
            maxEnemyReached = false;
        }
    }

    //Called when an enemy is killed
    public void enemyKilled()
    {
        enemyAlive--;
    }
}
