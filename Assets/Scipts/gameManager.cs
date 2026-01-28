using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using System.Collections;
public class gameManager : MonoBehaviour
{
    public static gameManager instance;

    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    [SerializeField] TMP_Text gameGoalCountText;

    [Header("Leveles")]
    [SerializeField] private spawner[] spawners;
    [SerializeField] private int[] enemiesPerLevel = { 10, 15, 20, 25, 30 };

    [Header("Level UI")]
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private GameObject levelBanner;
    [SerializeField] private TMP_Text levelBannerText;
    [SerializeField] private float bannerTime = 1.2f;

    private Coroutine bannerRoutine;

    private int currentLevel = 0;

    public GameObject player;
    public playerControllerNew playerScript;
    public Image playerHPBar;
    public GameObject playerDamageScreen;

    public bool isPaused;

    float timeScaleOrig;
    int gameGoalCount;


    private void Start()
    {
        StartLevel(0);
    }

    private void ShowLevelBanner(int levelNumber)
    {
        if (levelBanner == null || levelBannerText == null) return;

        if(bannerRoutine != null) 
            StopCoroutine(bannerRoutine);

        bannerRoutine = StartCoroutine(levelBannerRoutine(levelNumber));
    }

    private IEnumerator levelBannerRoutine(int levelNumber)
    {
        levelBannerText.text = "Level " + levelNumber + "!";
        levelBanner.SetActive(true);
        yield return new WaitForSeconds(bannerTime);
        levelBanner.SetActive(false);

        bannerRoutine = null;
    }

    private void StartLevel(int levelIndex)
    {
        currentLevel = levelIndex;

        int displayLevel = currentLevel + 1;

        if (levelText != null)
        {
            levelText.text = "Level: " + (displayLevel);
        }

        ShowLevelBanner(displayLevel);

        int amount = enemiesPerLevel[currentLevel];

        for (int i = 0; i < spawners.Length; i++)
        {
            if (spawners[i] != null)
            {
                spawners[i].StartLevel(amount);
            }
        }
    }

    private void NextLevelOrWin()
    {
        int next = currentLevel + 1;

        if (next >= enemiesPerLevel.Length)
        {
            statePause();
            menuActive = menuWin;
            menuActive.SetActive(true);
            return;
        }

        StartLevel(next);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        instance = this;

        timeScaleOrig = Time.timeScale;

        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<playerControllerNew>();

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if(menuActive == null)
            {
                statePause();
                menuActive = menuPause;
                menuActive.SetActive(true);
            }
            else if (menuActive == menuPause)
            {
                StateUnpaused();
            }
        } 
    }

    public void statePause()
    {
        isPaused = true;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void StateUnpaused()
    {
        isPaused = false;
        Time.timeScale = timeScaleOrig;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menuActive.SetActive(false);
        menuActive = null;
    }

    public void updateGameGoal(int amount)
    {
        gameGoalCount += amount;
        gameGoalCountText.text = gameGoalCount.ToString("F0");
        
        if(gameGoalCount<= 0)
        {
            NextLevelOrWin();
        }
    }

    public void youLose()
    {
        statePause();
        menuActive = menuLose;
        menuActive.SetActive(true);
    }
}
