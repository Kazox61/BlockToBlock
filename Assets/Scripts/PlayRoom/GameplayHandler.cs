using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Tilemaps;
using UnityEngine.Events;
public class GameplayHandler : MonoBehaviour {
    #region References
    [SerializeField] private Tilemap resultMap, pieceMap, gridMap, teleportMap;

    [SerializeField] private LevelTile orangeTile, greenTile, redTile, gridTile, teleportTile;

    [SerializeField] private PlayRoomUIManager playRoomUIManager;
    public LevelHelper levelHelper;
    [SerializeField] private LevelAnchor levelAnchor;
    [SerializeField] private CameraMovement cameraMovement;
    public UnityAction OnMapLevelFinishedAction;
    #endregion

    #region Variables
    private List<Vector2Int> cells;
    private int currentLevelIndex = 1;
    public int MovesCounter { get; private set; }
    [HideInInspector] public ScriptableLevel currentPlayedLevel;
    private PlayMode PlayMode;
    #endregion

    #region Unity-Callbacks
    private void Awake() {
        if(levelAnchor.IsSet) {
            var Item = levelAnchor.Item;
            var scriptableLevel = Item.levelData;
            levelHelper.LoadLevel(scriptableLevel);
            currentPlayedLevel = scriptableLevel;
            PlayMode = Item.PlayMode;
            

            if (PlayMode.Equals(PlayMode.mapMode)) {
                playRoomUIManager.currentLevelIndex = Item.levelIndex;
                UpdateLevelIndex(Item.levelIndex);
                playRoomUIManager.UpdatePersonalRecordText();
            }

            if (PlayMode.Equals(PlayMode.testMode)) {
                playRoomUIManager.buttonBackToEditMode.SetActive(true);
                playRoomUIManager.panelMapGameplay.SetActive(false);
            }
            
        }

        playRoomUIManager.UpdateInfoButton();
        cameraMovement.CalculateCameraZoom();
    }

    private void Start() {
        MovesCounter = 0;
        var cellsWithInfo = GetTilesFromTilemap(pieceMap, orangeTile);
        cells = cellsWithInfo.Keys.ToList();
    }
    #endregion


    #region Piece Movement
    public void MoveUp() {
        Move(Vector2Int.up);
    }
    public void MoveDown() {
        Move(Vector2Int.down);
    }
    public void MoveLeft() {
        Move(Vector2Int.left);
    }
    public void MoveRight() {
        Move(Vector2Int.right);
    }

    public void Move(Vector2Int translation) {
        for (int i = 0; i < cells.Count; i++) {
            pieceMap.SetTile((Vector3Int)cells[i], null);
        }
        for (int i = 0; i < cells.Count; i++) {
            cells[i] = cells[i] + translation;
        }
        for (int i = 0; i < cells.Count; i++) {
            pieceMap.SetTile((Vector3Int)cells[i], orangeTile);
        }

        UpdateMoveCounter(MovesCounter + 1);

        AddNeighbors();

        RemoveCellsNotOnBoard();

        for (int i = 0; i < cells.Count; i++) {
            pieceMap.SetTile((Vector3Int)cells[i], null);
        }

        TeleportPieces();

        for (int i = 0; i < cells.Count; i++) {
            pieceMap.SetTile((Vector3Int)cells[i], orangeTile);
        }

        AddNeighbors();

        if (cells.Count == 0) {
            ResetLevel();
        }

        if (GameIsFinished()) {
            if (PlayMode.Equals(PlayMode.mapMode)) {
                OnMapLevelFinishedAction.Invoke();
            }
            else if (PlayMode.Equals(PlayMode.customizedMode)) {
                Debug.Log("CustomizedEndScreen");
            }
        }
    }

    public void TeleportPieces() {
        Dictionary<int, Vector2Int> changes = new Dictionary<int, Vector2Int>();


        var teleportTiles = GetTilesFromTilemap(teleportMap, teleportTile);
        foreach (var cell in cells) {
            if (teleportTiles.ContainsKey(cell)) {
                teleportTiles.TryGetValue(cell, out var teleportTile);

                foreach (var v in teleportTiles.Values) {

                    if (v.teleportIndex == teleportTile.teleportIndex && v.teleportDir != teleportTile.teleportDir) {


                        var newPos = teleportTiles.FirstOrDefault(x => x.Value.teleportIndex == v.teleportIndex && x.Value.teleportDir == v.teleportDir).Key;


                        int index = cells.FindIndex(s => s.Equals(cell));
                        if (index != -1)

                            changes.Add(index, newPos);
                    }
                }
            }
        }

        foreach (var change in changes) {
            cells[change.Key] = change.Value;

        }
    }
    #endregion

    #region Help Functions

    public void AddNeighbors() {
        for (int i = cells.Count - 1; i >= 0; i--) {
            foreach (var direction in Directions.hvDirections) {
                var testCell = cells[i] + direction;
                if (pieceMap.HasTile((Vector3Int)testCell) && !cells.Contains(testCell)) {
                    pieceMap.SetTile((Vector3Int)testCell, orangeTile);
                    cells.Add(testCell);
                    AddNeighbors();
                }
            }
        }
    }

    public void RemoveCellsNotOnBoard() {
        List<Vector2Int> removedList = new List<Vector2Int>();
        foreach (var cell in cells) {
            if (!gridMap.HasTile((Vector3Int)cell)) {
                pieceMap.SetTile((Vector3Int)cell, null);
            }
            else {
                removedList.Add(cell);
            }
        }
        cells = removedList;
    }

    public Dictionary<Vector2Int, LevelTile> GetTilesFromTilemap(Tilemap map, LevelTile comparedTile) {
        BoundsInt bounds = map.cellBounds;
        TileBase[] allTiles = map.GetTilesBlock(bounds);


        Dictionary<Vector2Int, LevelTile> results = new Dictionary<Vector2Int, LevelTile>();

        foreach (var pos in bounds.allPositionsWithin) {
            if (map.HasTile(pos)) {
                var levelTile = map.GetTile<LevelTile>(pos);
                if (levelTile.type.Equals(comparedTile.type)) {
                    results.Add((Vector2Int)pos, levelTile);
                }
            }
        }
        return results;
    }

    public bool GameIsFinished() {
        BoundsInt bounds = resultMap.cellBounds;
        List<Vector2Int> resultPositions = new List<Vector2Int>();

        foreach (var pos in bounds.allPositionsWithin) {
            if (resultMap.HasTile(pos)) {
                resultPositions.Add((Vector2Int)pos);
            }
        }
        foreach (var resultPosition in resultPositions) {
            if (!cells.Contains(resultPosition)) return false;
        }
        return true;
    }
    #endregion


    public void LoadLevel(int levelIndex) {
        UpdateMoveCounter(0);
        currentPlayedLevel = levelHelper.LoadLevel(levelIndex);
        UpdateLevelIndex(levelIndex);
        var cellsWithInfo = GetTilesFromTilemap(pieceMap, orangeTile);
        cells = cellsWithInfo.Keys.ToList();

        cameraMovement.CalculateCameraZoom();
    }
    public void ResetLevel() {
        UpdateMoveCounter(0);
        levelHelper.LoadLevel(currentPlayedLevel);

        var cellsWithInfo = GetTilesFromTilemap(pieceMap, orangeTile);
        cells = cellsWithInfo.Keys.ToList();

        cameraMovement.CalculateCameraZoom();
    }

    public void UpdateMoveCounter(int moveCount) {
        MovesCounter = moveCount;
        playRoomUIManager.textMovesCounter.text = $"Moves: {MovesCounter}";
    }

    public void UpdateLevelIndex(int levelIndex) {
        currentLevelIndex = levelIndex;
        playRoomUIManager.textLevelIndex.text = $"Level: {currentLevelIndex}";
    }
}


public static class Directions {
    public static Vector2Int[] hvDirections = new Vector2Int[] { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };
}

public enum PlayMode {
    testMode,
    customizedMode,
    mapMode
}