using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;

public class LevelCreatorManager : MonoBehaviour {
    public Camera cam;
    public GameObject levelCreatorPanel;
    public Size currentMapSize = Size.large;

    public Tilemap gridMap, resultMap, piecesMap;
    public LevelTile gridTile, orangeTile, greenTile, redTile;
    public GameObject indicatorGrid, indicatorOrange, indicatorGreen, indicatorRed, indicatorEraser;

    public LevelManager levelManager;
    public GameController gameController;

    public LevelTile currentTile;
    public GameObject activeIndicator;
    public ScriptableLevel savedLevel;

    public void Start() {
        activeIndicator = indicatorGrid;
        currentTile = gridTile;
    }
    public void SetMapSize(Size size) {
        if (size.Equals(Size.large)) {
            cam.orthographicSize = 9;
        }
        else if (size.Equals(Size.middle)) {
            cam.orthographicSize = 7;
        }
        else if (size.Equals(Size.small)) {
            cam.orthographicSize = 5;
        }
    }

    public void Dropdown(int i) {
        if (i == 0) {
            SetMapSize(Size.large);
        }
        else if (i == 1) {
            SetMapSize(Size.middle);
        }
        else if (i == 2) {
            SetMapSize(Size.small);
        }
    }

    public void SelectDrawingBlock(int index) {
        if (index == 0) {
            currentTile = gridTile;
            activeIndicator.SetActive(false);
            activeIndicator = indicatorGrid;
            activeIndicator.SetActive(true);
        }
        else if(index == 1) {
            currentTile = orangeTile;
            activeIndicator.SetActive(false);
            activeIndicator = indicatorOrange;
            activeIndicator.SetActive(true);
        }
        else if (index == 2) {
            currentTile = greenTile;
            activeIndicator.SetActive(false);
            activeIndicator = indicatorGreen;
            activeIndicator.SetActive(true);
        }
        else if (index == 3) {
            currentTile = redTile;
            activeIndicator.SetActive(false);
            activeIndicator = indicatorRed;
            activeIndicator.SetActive(true);
        }
        else if (index == 4) {
            currentTile = null;
            activeIndicator.SetActive(false);
            activeIndicator = indicatorEraser;
            activeIndicator.SetActive(true);
        }
    }

    public void Draw(Vector3Int pos) {
        if (currentTile == gridTile) {
            gridMap.SetTile(pos, currentTile);
        }
        else if (currentTile == orangeTile) {
            piecesMap.SetTile(pos, currentTile);
        }
        else if (currentTile == greenTile) {
            piecesMap.SetTile(pos, currentTile);
        }
        else if (currentTile == redTile) {
            resultMap.SetTile(pos, currentTile);
        }
        else if (currentTile == null) {
            gridMap.SetTile(pos, currentTile);
            piecesMap.SetTile(pos, currentTile);
            resultMap.SetTile(pos, currentTile);
        }
    }

    public void SaveLevel() {
        savedLevel = levelManager.GetLevelData();
    }

    public void SaveJsonLevelDataToHardDrive() {
        var path = Application.persistentDataPath + "/levelData.json";

        var leveldata = levelManager.GetLevelData();
        var level = leveldata.ToLevel();
        var json = JsonUtility.ToJson(level);

        FileStream fileStream = new FileStream(path, FileMode.Create);

        using(StreamWriter writer = new StreamWriter(fileStream)) {
            writer.Write(json);
        }

    }

    public void LoadJsonLevelDataToHardDrive() {
        var path = Application.persistentDataPath + "/levelData.json";

        if (File.Exists(path)) {
            using (StreamReader reader = new StreamReader(path)) {
                string json = reader.ReadToEnd();

                var level = JsonUtility.FromJson<Level>(json);

                var levelData = level.ToScriptableLevel();
                levelManager.LoadLevel(levelData);
            }
        }

    }

    public bool CanRunLevel() {
        List<Vector3Int> gridposes = new List<Vector3Int>();

        foreach (var pos in gridMap.cellBounds.allPositionsWithin) {
            if (gridMap.HasTile(pos)) {
                gridposes.Add(pos);
            }
        }
        int i = 0;
        foreach (var pos in resultMap.cellBounds.allPositionsWithin) {
            if (resultMap.HasTile(pos)) {
                i += 1;
                if (!gridposes.Contains(pos)) {
                    return false;

                }
            }
        }
        if (i == 0) return false;
        int x = 0;
        foreach (var pos in piecesMap.cellBounds.allPositionsWithin) {
            if (piecesMap.HasTile(pos)) {
                if (piecesMap.GetTile<LevelTile>(pos) == orangeTile) {
                    x += 1;
                }
                if (!gridposes.Contains(pos)) {
                    return false;
                }
            }
        }
        if (x == 0) return false;


        return true;
    }
}

public enum Size {
    large,
    middle,
    small,
}
