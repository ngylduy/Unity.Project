using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    //Define the different states of the game
    public enum GameState
    {
        Playing,
        Paused,
        GameOver,
        LevelUp
    }
    //Count the number of enemies defeated
    public int enemiesDefeated = 0;
    //Store the current state of the game
    public GameState currentGameState;
    //Store the previous state of the game
    public GameState previousGameState;

    [Header("Damage Text Settings")]
    public Canvas damageTextCanvas;
    public float textFontSize = 15;
    public TMP_FontAsset textFont;
    public Camera referenceCamera;

    [Header("Screens")]
    public GameObject pauseMenu;
    public GameObject resultsScreen;
    public GameObject levelUpScreen;

    [Header("Current stats text")]
    public TMP_Text currentHealthText;
    public TMP_Text currentMoveSpeedText;
    public TMP_Text currentRecoveryText;
    public TMP_Text currentProjectileSpeedText;
    public TMP_Text currentMightText;
    public TMP_Text currentMagnetText;

    [Header("Result screen display")]
    public Image chosenCharacterImage;
    public TMP_Text chosenCharacterName;
    public TMP_Text levelReached;
    public TMP_Text timeSurvived;
    public List<Image> chosenWeaponsImages = new List<Image>(6);
    public List<Image> chosenPassiveItemImages = new List<Image>(6);
    public List<TMP_Text> chosenWeaponsText = new List<TMP_Text>(6);
    public List<TMP_Text> chosenPassiveItemText = new List<TMP_Text>(6);


    [Header("Stopwatch")]
    public float timeLimit; //Time limit in seconds
    float stopwatchTIme; //Time passed since the game started
    public TMP_Text stopwatchText;

    [Header("Enemy Defeated")]
    public TMP_Text enemyDefeatedText;

    //Check if game is over
    public bool isGameOver = false;

    //Check if player is choosing an upgrade
    public bool choosingUpgrade;

    //Reference to the player object
    public GameObject playerObject;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning("There is more than one GameManager and " + this + "Deleted");
            Destroy(gameObject);
        }
        DisableScreen();
    }

    private void Update()
    {
        //Define the behaviour of the game based on the current state
        switch (currentGameState)
        {
            case GameState.Playing:
                //If the game is playing, update the game
                CheckForPauseAndResume();
                UpdateStopwatch();
                break;
            case GameState.Paused:
                //If the game is paused, display the pause menu
                CheckForPauseAndResume();
                break;
            case GameState.GameOver:
                //If the game is over, display the game over screen
                if (!isGameOver)
                {
                    isGameOver = true;
                    Time.timeScale = 0f; //Stop the game
                    Debug.Log("Game Over");
                    ShowResults();
                }
                break;
            case GameState.LevelUp:
                //If the game is over, display the game over screen
                if (!choosingUpgrade)
                {
                    choosingUpgrade = true;
                    Time.timeScale = 0f; //Stop the game
                    Debug.Log("Level Up");
                    levelUpScreen.SetActive(true);
                }
                break;
            default:
                Debug.LogWarning("Invalid game state");
                break;
        }
    }

    public void EnemyDefeated()
    {
        // Increment the counter when an enemy is defeated
        enemiesDefeated++;
    }

    IEnumerator GenerateFloatingTextCoroutine(string text, Transform target, float duration = 1f, float speed = 50f)
    {
        GameObject textObj = new GameObject("Floating Text");
        RectTransform rect = textObj.AddComponent<RectTransform>();
        TextMeshProUGUI tmpPro = textObj.AddComponent<TextMeshProUGUI>();
        tmpPro.text = text;
        tmpPro.horizontalAlignment = HorizontalAlignmentOptions.Center;
        tmpPro.verticalAlignment = VerticalAlignmentOptions.Middle;
        tmpPro.fontSize = textFontSize;
        if (textFont) tmpPro.font = textFont;
        rect.position = referenceCamera.WorldToScreenPoint(target.position);

        Destroy(textObj, duration);

        textObj.transform.SetParent(instance.damageTextCanvas.transform);
        textObj.transform.SetSiblingIndex(0);

        WaitForEndOfFrame w = new WaitForEndOfFrame();
        float t = 0;
        float yOffset = 0;
        Vector3 lastKnownPosition = target.position;
        while (t < duration)
        {
            if (!rect)
            {
                break;
            }

            //Fade the text to the right alpha value
            tmpPro.color = new Color(tmpPro.color.r, tmpPro.color.g, tmpPro.color.b, 1 - (t / duration));
            
            if (target)
            {
                lastKnownPosition = target.position;
            }

            //Pan the text upwards
            yOffset += speed * Time.deltaTime;
            rect.position = referenceCamera.WorldToScreenPoint(lastKnownPosition + new Vector3(0, yOffset));

            //Wait for a frame and update the time
            yield return w;
            t += Time.deltaTime;
        }
    }

    public static void GenerateFloatingText(string text, Transform target, float duration = 1f, float speed = 1f)
    {
        //If the canvas is not set, return dont generate the text
        if (!instance.damageTextCanvas) return;

        //Find a reference camera if it is not set
        if (!instance.referenceCamera) instance.referenceCamera = Camera.main;

        instance.StartCoroutine(instance.GenerateFloatingTextCoroutine(
            text, target, duration, speed
            ));
    }

    //Define the medthod to change the game state
    public void ChangeState(GameState newGameState)
    {
        currentGameState = newGameState;
    }

    public void PauseGame()
    {
        if (currentGameState != GameState.Paused)
        {
            previousGameState = currentGameState;
            ChangeState(GameState.Paused);
            Time.timeScale = 0f;
            Debug.Log("Game Paused");
            //Display the pause menu
            pauseMenu.SetActive(true);
        }
    }

    public void ResumeGame()
    {
        if (currentGameState == GameState.Paused)
        {
            ChangeState(previousGameState);
            Time.timeScale = 1f;
            Debug.Log("Game Resumed");
            //Hide the pause menu
            pauseMenu.SetActive(false);
        }
        else if (currentGameState == GameState.LevelUp)
        {
            ChangeState(previousGameState);
            Time.timeScale = 1f;
            Debug.Log("Game Resumed");
            levelUpScreen.SetActive(false);
        }
    }


    public void CheckForPauseAndResume()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentGameState == GameState.Paused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    void DisableScreen()
    {
        pauseMenu.SetActive(false);
        resultsScreen.SetActive(false);
        levelUpScreen.SetActive(false);
    }

    public void GameOver()
    {
        timeSurvived.text = stopwatchText.text;
        ChangeState(GameState.GameOver);
    }

    void ShowResults()
    {
        resultsScreen.SetActive(true);
    }

    public void SetCharacterStats(CharacterData character)
    {
        chosenCharacterImage.sprite = character.Icon;
        chosenCharacterName.text = character.Name;
    }

    public void LevelReached(int level)
    {
        levelReached.text = level.ToString();
    }

    public void EDefeated()
    {
        enemyDefeatedText.text = enemiesDefeated.ToString();
    }

    public void WeaponAndPassiveItem(List<PlayerInventory.Slot> weaponData, List<PlayerInventory.Slot> passiveItemData)
    {
        if (weaponData.Count != chosenWeaponsImages.Count
        || passiveItemData.Count != chosenPassiveItemImages.Count
        || weaponData.Count != chosenWeaponsText.Count
        || passiveItemData.Count != chosenPassiveItemText.Count)
        {
            Debug.LogWarning("The number of weapons and the number of images are different");
            return;
        }

        //Assign weaponData data to chosenWeaponsImages
        for (int i = 0; i < chosenWeaponsImages.Count; i++)
        {
            //Check that sprite is not null in weaponData
            if (weaponData[i].image.sprite)
            {
                chosenWeaponsImages[i].enabled = true;
                chosenWeaponsImages[i].sprite = weaponData[i].image.sprite;
                chosenWeaponsText[i].text = "Level " + weaponData[i].item.currentLevel.ToString();
                chosenWeaponsText[i].enabled = true;
            }
            else
            {
                chosenWeaponsImages[i].enabled = false;
                chosenWeaponsText[i].enabled = false;
            }
        }

        for (int i = 0; i < chosenPassiveItemImages.Count; i++)
        {
            //Check that sprite is not null in passiveItemData
            if (passiveItemData[i].image.sprite)
            {
                chosenPassiveItemImages[i].enabled = true;
                chosenPassiveItemImages[i].sprite = passiveItemData[i].image.sprite;
                chosenPassiveItemText[i].text = "Level " + passiveItemData[i].item.currentLevel.ToString();
                chosenPassiveItemText[i].enabled = true;
            }
            else
            {
                chosenPassiveItemImages[i].enabled = false;
                chosenPassiveItemText[i].enabled = false;
            }
        }

    }

    void UpdateStopwatch()
    {
        stopwatchTIme += Time.deltaTime;

        UpdateStopwatchDisplay();

        if (stopwatchTIme >= timeLimit)
        {
            playerObject.SendMessage("Die");
        }
    }

    void UpdateStopwatchDisplay()
    {

        //Calculate the minutes and seconds
        int minutes = Mathf.FloorToInt(stopwatchTIme / 60f);
        int seconds = Mathf.FloorToInt(stopwatchTIme % 60);

        stopwatchText.text = string.Format("{0:0}:{1:00}", minutes, seconds);
    }

    public void StartLevelUp()
    {
        ChangeState(GameState.LevelUp);
        playerObject.SendMessage("RemoveAndApplyUpgrade");
    }

    public void EndLevelUp()
    {
        choosingUpgrade = false;
        Time.timeScale = 1f; //Resume the game
        levelUpScreen.SetActive(false);
        ChangeState(GameState.Playing);
    }
}
