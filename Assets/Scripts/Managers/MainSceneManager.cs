using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("UI Elements")]
    public Button startGameButton;

    [Header("Grid Width")]
    public TextMeshProUGUI gridWidthText;
    public Button gridWidthMinusButton;
    public Button gridWidthPlusButton;

    [Header("Grid Height")]
    public TextMeshProUGUI gridHeightText;
    public Button gridHeightMinusButton;
    public Button gridHeightPlusButton;

    [Header("Max Moves")]
    public TextMeshProUGUI maxMovesText;
    public Button maxMovesMinusButton;
    public Button maxMovesPlusButton;

    [Header("Goal Score")]
    public TextMeshProUGUI goalScoreText;
    public Button goalScoreMinusButton;
    public Button goalScorePlusButton;

    [Header("Game Settings")]
    public GameSettings gameSettings; // ScriptableObject reference

    private void Start()
    {
        Application.targetFrameRate = 60;
        // Button events
        startGameButton.onClick.AddListener(StartGame);

        gridWidthMinusButton.onClick.AddListener(() => ChangeGridWidth(-1));
        gridWidthPlusButton.onClick.AddListener(() => ChangeGridWidth(1));

        gridHeightMinusButton.onClick.AddListener(() => ChangeGridHeight(-1));
        gridHeightPlusButton.onClick.AddListener(() => ChangeGridHeight(1));

        maxMovesMinusButton.onClick.AddListener(() => ChangeMaxMoves(-1));
        maxMovesPlusButton.onClick.AddListener(() => ChangeMaxMoves(1));

        goalScoreMinusButton.onClick.AddListener(() => ChangeGoalScore(-10));
        goalScorePlusButton.onClick.AddListener(() => ChangeGoalScore(10));

        UpdateUI();
    }

    public void SetGameSettings(Vector2Int dimensions, int maxMoves, int goalScore)
    {
        gameSettings.gridDimensions = dimensions;
        gameSettings.maxMoves = maxMoves;
        gameSettings.goalScore = goalScore;
        UpdateUI();
    }

    void UpdateUI()
    {
        gridWidthText.text = gameSettings.gridDimensions.x.ToString();
        gridHeightText.text = gameSettings.gridDimensions.y.ToString();
        maxMovesText.text = gameSettings.maxMoves.ToString();
        goalScoreText.text = gameSettings.goalScore.ToString();
    }

    void ChangeGridWidth(int delta)
    {
        gameSettings.gridDimensions.x = Mathf.Max(1, gameSettings.gridDimensions.x + delta);
        UpdateUI();
    }

    void ChangeGridHeight(int delta)
    {
        gameSettings.gridDimensions.y = Mathf.Max(1, gameSettings.gridDimensions.y + delta);
        UpdateUI();
    }

    void ChangeMaxMoves(int delta)
    {
        gameSettings.maxMoves = Mathf.Max(1, gameSettings.maxMoves + delta);
        UpdateUI();
    }

    void ChangeGoalScore(int delta)
    {
        gameSettings.goalScore = Mathf.Max(0, gameSettings.goalScore + delta);
        UpdateUI();
    }

    void StartGame()
    {

        Debug.Log($"Starting game with settings: Grid {gameSettings.gridDimensions}, MaxMoves {gameSettings.maxMoves}, GoalScore {gameSettings.goalScore}");
        gameSettings.isGameActive = true;
        SceneManager.LoadScene(1);
    }

    void OpenSettings()
    {
        Debug.Log("Settings button clicked");
        // Show/hide parameter controls if needed
    }
}
