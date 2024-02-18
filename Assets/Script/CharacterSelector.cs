using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class CharacterSelector : MonoBehaviour
{

    public static CharacterSelector instance;
    public CharacterScriptableObject characterData;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.LogWarning("CharacterSelector instance already exists, destroying " + this + " one");
            Destroy(gameObject);
        }
    }

    public static CharacterScriptableObject GetData()
    {
        return instance.characterData;
    }

    public void SelectCharacter(CharacterScriptableObject data)
    {
        characterData = data;
    }

    public void DestroySigleton()
    {
        instance = null;
        Destroy(gameObject);
    }
}
