using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEditor;

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
            //Ramdomly pick a character if we are playing from the Editor
            #if UNITY_EDITOR
            string[] allAssetPath = AssetDatabase.GetAllAssetPaths();
            List<CharacterData> characters = new List<CharacterData>();
            foreach (string assetPath in allAssetPath)
            {
                if (assetPath.EndsWith(".asset"))
                {
                    CharacterData characterData = AssetDatabase.LoadAssetAtPath<CharacterData>(assetPath);
                    if (characterData != null)
                    {
                        characters.Add(characterData);
                    }
                }
            }

            if (characters.Count > 0)
            {
                int randomIndex = Random.Range(0, characters.Count);
                return characters[randomIndex];
            }
            #endif
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