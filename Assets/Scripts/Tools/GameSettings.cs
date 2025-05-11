using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Game Data/Game Settings")]
public class GameSettings : ScriptableObject
{
    public Vector2Int gridDimensions;
    public int maxMoves;
    public int goalScore;
    public bool isGameActive;
}