using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    [Header("Game Settings")]
    public GameSettings gameSettings; // ScriptableObject reference
    [SerializeField] GameObject levelClearedPanel;
    [SerializeField] GameObject levelFailedPanel;


    public TextMeshProUGUI goalScoreText;

    private LinkableGrid grid;
    private PoolManager pool;
    Vector2Int dimensions;
    private int moveCount = 20;

    private int goalScore = 150;


    void Start()
    {
        LoadSettings();
        GridSetup();
        PoolSetup();
        StartCoroutine(grid.PopulateGrid());
    }

    private void LoadSettings()
    {
        dimensions = gameSettings.gridDimensions;
        moveCount = gameSettings.maxMoves;
        goalScore = gameSettings.goalScore;
        goalScoreText.text = goalScore.ToString();
        ScoreManager scoreManager = ScoreManager.Instance;
        scoreManager.SetMaxMovesAndGoalScore(moveCount, goalScore);
        scoreManager.OnGameOver += HandleGameOver;
        scoreManager.OnLevelComplete += HandleLevelComplete;
        scoreManager.LevelEndBanner += HandleShowBanner;
    }

    private void GridSetup()
    {

        grid = (LinkableGrid)LinkableGrid.Instance;

        grid.gameObject.transform.position = new Vector2(-(dimensions.x - 1) / 2f, (-(dimensions.y - 1) / 2f) - 2);


        grid.InitializeGrid(dimensions);
        grid.SetValues();

    }

    private void PoolSetup()
    {
        pool = PoolManager.Instance;

        pool.gameObject.transform.position = new Vector2(-(dimensions.x - 1) / 2f, (-(dimensions.y - 1) / 2f) - 2);

        pool.PoolAllObjects(dimensions.x * dimensions.y / 2);
    }


    private void HandleGameOver()
    {
        levelFailedPanel.SetActive(true);
        gameSettings.isGameActive = false;
        grid.gameObject.SetActive(false);
        pool.gameObject.SetActive(false);
        Debug.Log("Game Over");



        //Transform levelTransform = levelFailedPanel.transform.Find("PopupBase/PopupRibbon/LevelText");
        // TMP_Text levelText = levelTransform.GetComponent<TMP_Text>();
        // levelText.text = "Level " + ParameterData.Instance.currentLevel.level_number;


    }

    private void HandleLevelComplete()
    {
        levelClearedPanel.SetActive(true);
        gameSettings.isGameActive = false;
        grid.gameObject.SetActive(false);
        pool.gameObject.SetActive(false);
        Debug.Log("Level Complete");

        //Transform starTransform = levelClearedPanel.transform.Find("PopupBase/Star");
        //Transform levelTransform = levelClearedPanel.transform.Find("PopupBase/PopupRibbon/LevelText");

        //Star starScript = starTransform.GetComponent<Star>();
        //TMP_Text levelText = levelTransform.GetComponent<TMP_Text>();

        //starScript.PlayParticles();
        //levelText.text = "Level " + ParameterData.Instance.currentLevel.level_number;
        //int nextLevel = ParameterData.Instance.currentLevel.level_number + 1;

    }

    private void HandleShowBanner()
    {
        //MoveGameObject.MoveDownAndDisappear(levelEndBanner);
    }

    public void ContinuteButton()
    {
        SceneManager.LoadScene(0);
    }


}
