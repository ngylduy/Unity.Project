using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkTrigger : MonoBehaviour
{

    MapController mapController;
    public GameObject target;

    // Start is called before the first frame update
    void Start()
    {
        mapController = FindObjectOfType<MapController>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            mapController.currentChunk = target;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (mapController.currentChunk == target)
            {
                mapController.currentChunk = null;
            }
        }
    }
}
