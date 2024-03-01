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
    public Text currentHealthText;
    public Text currentMoveSpeedText;
    public Text currentRecoveryText;
    public Text currentProjectileSpeedText;
    public Text currentMightText;
    public Text currentMagnetText;

    [Header("Result screen display")]
    public Image chosenCharacterImage;
    public Text chosenCharacterName;
    public Text levelReached;

    public Text timeSurvived;

    public List<Image> chosenWeaponsImages = new List<Image>(6);
    public List<Image> chosenPassiveItemImages = new List<Image>(6);

    [Header("Stopwatch")]
    public float timeLimit; //Time limit in seconds
    float stopwatchTIme; //Time passed since the game started
    public Text stopwatchText;

    //Check if game is over
    public bool isGameOver = false;

    //Check if player is choosing an upgrade
    public bool choosingUpgrade;


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

        WaitForEndOfFrame w = new WaitForEndOfFrame();
        float t = 0;
        float yOffset = 0;
        while (t < duration)
        {
            yield return w;
            t += Time.deltaTime;

            tmpPro.color = new Color(tmpPro.color.r, tmpPro.color.g, tmpPro.color.b, 1 - (t / duration));
            yOffset += speed * Time.deltaTime;
            rect.position = referenceCamera.WorldToScreenPoint(target.position + new Vector3(0, yOffset));
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
        } else if (currentGameState == GameState.LevelUp)
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

    public void SetCharacterStats(CharacterScriptableObject character)
    {
        chosenCharacterImage.sprite = character.Icon;
        chosenCharacterName.text = character.Name;
    }

    public void LevelReached(int level)
    {
        levelReached.text = level.ToString();
    }

    public void WeaponAndPassiveItem(List<Image> weaponData, List<Image> passiveItemData)
    {
        if (weaponData.Count != chosenWeaponsImages.Count
        || passiveItemData.Count != chosenPassiveItemImages.Count)
        {
            Debug.LogWarning("The number of weapons and the number of images are different");
            return;
        }

        //Assign weaponData data to chosenWeaponsImages
        for (int i = 0; i < chosenWeaponsImages.Count; i++)
        {
            //Check that sprite is not null in weaponData
            if (weaponData[i].sprite)
            {
                chosenWeaponsImages[i].enabled = true;
                chosenWeaponsImages[i].sprite = weaponData[i].sprite;
            }
            else
            {
                chosenWeaponsImages[i].enabled = false;
            }
        }

        for (int i = 0; i < chosenPassiveItemImages.Count; i++)
        {
            //Check that sprite is not null in passiveItemData
            if (passiveItemData[i].sprite)
            {
                chosenPassiveItemImages[i].enabled = true;
                chosenPassiveItemImages[i].sprite = passiveItemData[i].sprite;
            }
            else
            {
                chosenPassiveItemImages[i].enabled = false;
            }
        }

    }

    void UpdateStopwatch()
    {
        stopwatchTIme += Time.deltaTime;

        UpdateStopwatchDisplay();

        if (stopwatchTIme >= timeLimit)
        {
            stopwatchTIme = timeLimit;
            GameOver();
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
    }

    public void EndLevelUp()
    {
        choosingUpgrade = false;
        Time.timeScale = 1f;
        levelUpScreen.SetActive(true);
        ChangeState(GameState.Playing);
    }
}
