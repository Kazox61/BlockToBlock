using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Events;
using TMPro;
using System.Threading.Tasks;

public class Gameboard : MonoBehaviour {
    [Header("References")]
    public Tilemap gridMap;
    public Tilemap resultMap;
    public Tilemap piecesMap;
    public LevelManager levelManager;
    public Animator anim;
    public TMP_Text indexText;
    public GameObject endScreen;
    public GameObject topBar;
    public GameObject infoButton;
    public TMP_Text infoPanelText;
    public LevelTile orangeTile;
    public LevelTile greenTile;
    public TMP_Text movesText;
    public GameController gameController;
    public CameraMovement cameraMovement;


    [Header("Variables")]
    public List<Vector2Int> cells;
    public int currentLevel = 1;
    public ScriptableLevel currentLevelData;
    public int moves = 0;
    public bool allowMoveInput = true;

    public void Start() {
    }

    public void ResetLevelStats() {
        if (gameController.IngameState.timer > 0) return;


        cells.Clear();

        currentLevelData = levelManager.LoadLevel(currentLevel);
        moves = 0;
        movesText.text = "Moves: 0";
        cells = GetTilePositionsFromTilemap(piecesMap, orangeTile);
    }

    public void LoadNextLevel() {
        LoadLevel(currentLevel + 1);
    }

    public void PlaySameLevelAgain() {
        if (gameController.FinishedLevelState.timer > 0) return;
        gameController.StateMachine.TryEnterState(gameController.IngameState);
        ResetLevelStats();
        gameController.ShowPanelGameUI();
    }

    public void LoadLevel(int levelIndex) {
        if (gameController.StateMachine.CurrentState == gameController.MenuState) {
            gameController.StateMachine.TryEnterState(gameController.IngameState);
        }
        gameController.panelGameUI.SetActive(true);
        gameController.panelStartScreen.SetActive(false);
        gameController.panelLevelSeletion.SetActive(false);
        gameController.ShowPanelGameUI();

        currentLevel = levelIndex;
        indexText.text = $"Level {currentLevel}";

        ResetLevelStats();

        if (currentLevelData.infoText.Length > 0) {
            infoButton.SetActive(true);
            infoPanelText.text = currentLevelData.infoText;
        }
        else {
            infoButton.SetActive(false);
            infoPanelText.text = "";
        }

        cameraMovement.Cal(gameController.gameSettings.mobile);
    }

    public List<Vector2Int> RemoveCellsNotOnBoard(List<Vector2Int> cells) {
        List<Vector2Int> removedList = new List<Vector2Int>();
        foreach (var cell in cells) {
            if (!gridMap.HasTile((Vector3Int)cell)) {
                piecesMap.SetTile((Vector3Int)cell, null);
            }
            else {
                removedList.Add(cell);
            }
        }
        return removedList;
    }
    public bool IsGameFinished() {
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

    public List<Vector2Int> GetTilePositionsFromTilemap(Tilemap map, LevelTile comparedTile) {
        BoundsInt bounds = map.cellBounds;
        TileBase[] allTiles = map.GetTilesBlock(bounds);


        List<Vector2Int> resultPositions = new List<Vector2Int>();

        foreach (var pos in bounds.allPositionsWithin) {
            if (map.HasTile(pos)) {
                var levelTile = map.GetTile<LevelTile>(pos);
                if (levelTile.type.Equals(TileType.orange)) {
                    resultPositions.Add((Vector2Int)pos);
                }
            }
        }
        return resultPositions;

    }

    public void UpdateInput() {

        if (Input.GetKeyDown(KeyCode.A)) {
            Move(Vector2Int.left);
        }
        if (Input.GetKeyDown(KeyCode.W)) {
            Move(Vector2Int.up);
        }
        if (Input.GetKeyDown(KeyCode.D)) {
            Move(Vector2Int.right);
        }
        if (Input.GetKeyDown(KeyCode.S)) {
            Move(Vector2Int.down);
        }
        if (Input.GetKeyDown(KeyCode.R)) {
            ResetLevelStats();
        }

    }

    public void MoveUp() {
        if (allowMoveInput) {
            Move(Vector2Int.up);
        }
    }
    public void MoveDown() {
        if (allowMoveInput) {
            Move(Vector2Int.down);
        }
    }
    public void MoveLeft() {
        if (allowMoveInput) {
            Move(Vector2Int.left);
        }
    }
    public void MoveRight() {
        if (allowMoveInput) {
            Move(Vector2Int.right);
        }
    }

    public void Clear() {
        for (int i = 0; i < cells.Count; i++) {
            piecesMap.SetTile((Vector3Int)cells[i], null);
        }
    }

    public void Move(Vector2Int translation) {
        for (int i = 0; i < cells.Count; i++) {
            piecesMap.SetTile((Vector3Int)cells[i], null);
        }
        for (int i = 0; i < cells.Count; i++) {
            cells[i] = cells[i] + translation;
        }
        for (int i = 0; i < cells.Count; i++) {
            piecesMap.SetTile((Vector3Int)cells[i], orangeTile);
        }

        moves += 1;
        movesText.text = $"Moves: {moves}";

        AddNeighbors();
        cells = RemoveCellsNotOnBoard(cells);

        if (cells.Count == 0 && gameController.StateMachine.CurrentState == gameController.IngameState) {
            ResetLevelStats();
        }
        else if(cells.Count == 0) {
            gameController.StateMachine.TryEnterState(gameController.TestLevelState);
        }
    }

    public void AddNeighbors() {
        for (int i = cells.Count - 1; i >= 0; i--) {
            foreach (var direction in Directions.hvDirections) {
                var testCell = cells[i] + direction;
                if (piecesMap.HasTile((Vector3Int)testCell) && !cells.Contains(testCell)) {
                    piecesMap.SetTile((Vector3Int)testCell, orangeTile);
                    cells.Add(testCell);
                    AddNeighbors();
                }
            }
        }
    }


    public void IsNextTo() {
        foreach (var cell in cells) {
            foreach (var direction in Directions.hvDirections) {
                var testCell = cell + direction;
                if (piecesMap.HasTile((Vector3Int)testCell)) {
                    var leveltile = piecesMap.GetTile<LevelTile>((Vector3Int)testCell);
                    if (leveltile.type.Equals(TileType.green)) {
                        piecesMap.SetTile((Vector3Int)testCell, orangeTile);
                    }
                }
            }
        }
    }
}


public static class Directions {
    public static Vector2Int[] hvDirections = new Vector2Int[] { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };
}
