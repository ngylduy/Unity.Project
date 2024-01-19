using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{

    public List<GameObject> terrainChunks;
    public GameObject player;
    public float checkRadius;
    Vector3 noTerrainPositon;
    public LayerMask terrainMask;
    PlayerMoverment playerMoverment;

    public GameObject currentChunk;


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
        playerMoverment = FindObjectOfType<PlayerMoverment>();
    }

    // Update is called once per frame
    void Update()
    {
        chunkChecker();
        ChuckOptimer();
    }

    void chunkChecker()
    {
        if (!currentChunk)
        {
            return;
        }
        if (playerMoverment.moveDir.x > 0 && playerMoverment.moveDir.y == 0) //Right
        {
            if (!Physics2D.OverlapCircle(currentChunk.transform.Find("Right").position, checkRadius, terrainMask))
            {
                noTerrainPositon = currentChunk.transform.Find("Right").position;
                SpanwChunk();
            }
        }
        else if (playerMoverment.moveDir.x < 0 && playerMoverment.moveDir.y == 0) //Left
        {
            if (!Physics2D.OverlapCircle(currentChunk.transform.Find("Left").position, checkRadius, terrainMask))
            {
                noTerrainPositon = currentChunk.transform.Find("Left").position;
                SpanwChunk();
            }
        }
        else if (playerMoverment.moveDir.x == 0 && playerMoverment.moveDir.y > 0) //Up
        {
            if (!Physics2D.OverlapCircle(currentChunk.transform.Find("Up").position, checkRadius, terrainMask))
            {
                noTerrainPositon = currentChunk.transform.Find("Up").position;
                SpanwChunk();
            }
        }
        else if (playerMoverment.moveDir.x == 0 && playerMoverment.moveDir.y < 0) //Down
        {
            if (!Physics2D.OverlapCircle(currentChunk.transform.Find("Down").position, checkRadius, terrainMask))
            {
                noTerrainPositon = currentChunk.transform.Find("Down").position;
                SpanwChunk();
            }
        }
        else if (playerMoverment.moveDir.x > 0 && playerMoverment.moveDir.y > 0) //Right Up
        {
            if (!Physics2D.OverlapCircle(currentChunk.transform.Find("Top Right").position, checkRadius, terrainMask))
            {
                noTerrainPositon = currentChunk.transform.Find("Top Right").position;
                SpanwChunk();
            }
        }
        else if (playerMoverment.moveDir.x > 0 && playerMoverment.moveDir.y < 0) //Right Down
        {
            if (!Physics2D.OverlapCircle(currentChunk.transform.Find("Down Right").position, checkRadius, terrainMask))
            {
                noTerrainPositon = currentChunk.transform.Find("Down Right").position;
                SpanwChunk();
            }
        }
        else if (playerMoverment.moveDir.x < 0 && playerMoverment.moveDir.y > 0) //Left Up
        {
            if (!Physics2D.OverlapCircle(currentChunk.transform.Find("Top Left").position, checkRadius, terrainMask))
            {
                noTerrainPositon = currentChunk.transform.Find("Top Left").position;
                SpanwChunk();
            }
        }
        else if (playerMoverment.moveDir.x < 0 && playerMoverment.moveDir.y < 0) //Left Down
        {
            if (!Physics2D.OverlapCircle(currentChunk.transform.Find("Down Left").position, checkRadius, terrainMask))
            {
                noTerrainPositon = currentChunk.transform.Find("Down Left").position;
                SpanwChunk();
            }
        }
    }

    void SpanwChunk()
    {
        int rand = Random.Range(0, terrainChunks.Count);
        latesChunk = Instantiate(terrainChunks[rand], noTerrainPositon, Quaternion.identity);
        spawnedChunks.Add(latesChunk);
    }

    void ChuckOptimer()
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
