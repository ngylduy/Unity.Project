using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{

    public List<GameObject> terrainChunks;
    public GameObject player;
    public float checkRadius;
    public LayerMask terrainMask;
    public GameObject currentChunk;

    Vector3 playerLastPosition;


    [Header("Optimiztion")]
    public List<GameObject> spawnedChunks;
    GameObject latesChunk;
    public float maxOpDist;
    float opDist;
    float optimazationTimer;
    public float optimazationTimeDur;

    // Start is called before the first frame update
    void Start()
    {
        playerLastPosition = player.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        chunkChecker();
        ChuckOptimizer();
    }

    void chunkChecker()
    {
        if (!currentChunk)
        {
            return;
        }
        Vector3 moverDir = player.transform.position - playerLastPosition;
        playerLastPosition = player.transform.position;

        string directionName = GerDirectionName(moverDir);

        CheckAndSpawnChunk(directionName);

        //Check addtional chunks if moving diagonaly
        if (directionName.Contains("Up"))
        {
            CheckAndSpawnChunk("Up");
        }
        if (directionName.Contains("Down"))
        {
            CheckAndSpawnChunk("Down");
        }
        if (directionName.Contains("Right"))
        {
            CheckAndSpawnChunk("Right");
        }
        if (directionName.Contains("Left"))
        {
            CheckAndSpawnChunk("Left");
        }
    }

    void CheckAndSpawnChunk(string direction){
        if (!Physics2D.OverlapCircle(currentChunk.transform.Find(direction).position, checkRadius, terrainMask))
        {
            SpanwChunk(currentChunk.transform.Find(direction).position);
        }
    }

    string GerDirectionName(Vector3 direction)
    {
        direction = direction.normalized;

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            //Moving horizontaly more than verticaly
            if (direction.y > 0.5f)
            {
                //Also moving upward
                return direction.x > 0 ? "Right Up" : "Left Up";
            }
            else if (direction.y < -0.5f)
            {
                //Also moving downward
                return direction.x > 0 ? "Right Down" : "Left Down";
            }
            else
            {
                //Moving straight horizontaly
                return direction.x > 0 ? "Right" : "Left";
            }
        }
        else
        {
            //Moving verticaly more than horizontaly
            if (direction.x > 0.5f)
            {
                //Also moving right
                return direction.y > 0 ? "Right Up" : "Right Down";
            }
            else if (direction.x < -0.5f)
            {
                //Also moving left
                return direction.y > 0 ? "Left Up" : "Left Down";
            }
            else
            {
                //Moving straight verticaly
                return direction.y > 0 ? "Up" : "Down";
            }
        }
    }

    void SpanwChunk(Vector3 spawnPosition)
    {
        int rand = Random.Range(0, terrainChunks.Count);
        latesChunk = Instantiate(terrainChunks[rand], spawnPosition, Quaternion.identity);
        spawnedChunks.Add(latesChunk);
    }

    void ChuckOptimizer()
    {
        optimazationTimer -= Time.deltaTime;

        if (optimazationTimer <= 0f)
        {
            optimazationTimer = optimazationTimeDur;
        }
        else { return; }

        foreach (GameObject chunk in spawnedChunks)
        {
            opDist = Vector3.Distance(player.transform.position, chunk.transform.position);
            if (opDist > maxOpDist)
            {
                chunk.SetActive(false);
            }
            else
            {
                chunk.SetActive(true);
            }
        }
    }
}
