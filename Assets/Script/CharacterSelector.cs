using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class CharacterSelector : MonoBehaviour
{

    public static CharacterSelector instance;
    public CharacterData characterData;

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

    public static CharacterData GetData()
    {
        if (instance && instance.characterData)
        {
            return instance.characterData;
        }
        else
        {
            CharacterData[] characters = Resources.FindObjectsOfTypeAll<CharacterData>();
            if (characters.Length > 0)
            {
                return characters[UnityEngine.Random.Range(0, characters.Length)];
            }
        }
        return null;
    }

    public void SelectCharacter(CharacterData data)
    {
        characterData = data;
    }

    public void DestroySigleton()
    {
        instance = null;
        Destroy(gameObject);
    }
}
