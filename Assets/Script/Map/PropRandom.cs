using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropRandom : MonoBehaviour
{

    public List<GameObject> propsSpawn;
    public List<GameObject> propsPrefab;

    // Start is called before the first frame update
    void Start()
    {
        SpanwProp();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpanwProp()
    {
        foreach (GameObject sp in propsSpawn)
        {
            int index = Random.Range(0, propsPrefab.Count);
            GameObject prop = Instantiate(propsPrefab[index], sp.transform.position, Quaternion.identity);
            prop.transform.parent = sp.transform;
        }
    }
}
