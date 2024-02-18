using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Passive Item Table Object", menuName = "ScriptableObjects/PassiveItem")]
public class PassiveItemScripttableObject : ScriptableObject
{
    [SerializeField]
    float multiplier;

    public float Multiplier { get => multiplier; private set => multiplier = value; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
