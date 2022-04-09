using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;

public class LevelCreatorManager : MonoBehaviour {
    public Camera cam;
    public GameObject levelCreatorPanel;
    public Size currentMapSize = Size.large;

    public Tilemap gridMap, resultMap, piecesMap, teleportMap;
    public LevelTile gridTile, orangeTile, greenTile, redTile, teleportTile;
    public GameObject indicatorGrid, indicatorOrange, indicatorGreen, indicatorRed, indicatorTeleport, indicatorEraser;

    public LevelManager levelManager;
    public GameController gameController;

    public LevelTile currentTile;
    public GameObject activeIndicator;
    public ScriptableLevel savedLevel;
    public Size currentLevelCreatorSize;
    public int teleportDirection = 0;
    public int teleportIndex = 0;

    public void Start() {
        activeIndicator = indicatorGrid;
        currentTile = gridTile;
        SetMapSize(Size.large);
    }
    public void SetMapSize(Size size) {
        currentLevelCreatorSize = size;
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

    public void DropDownSize(int i) {
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

    public void DropDownDirection(int i) {
        teleportDirection = i;
    }

    public void InputIndexTeleportTile(string input) {
        teleportIndex = int.Parse(input);
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
            currentTile = teleportTile;
            activeIndicator.SetActive(false);
            activeIndicator = indicatorTeleport;
            activeIndicator.SetActive(true);
        }
        else if (index == 5) {
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
        else if (currentTile == teleportTile) {
            var tile = ScriptableObject.CreateInstance<LevelTile>();
            tile.sprite = teleportTile.sprite;
            tile.type = teleportTile.type;
            tile.teleportDir = teleportDirection;
            tile.teleportIndex = teleportIndex;
            teleportMap.SetTile(pos, tile);
        }
        else if (currentTile == null) {
            gridMap.SetTile(pos, currentTile);
            piecesMap.SetTile(pos, currentTile);
            resultMap.SetTile(pos, currentTile);
            teleportMap.SetTile(pos, currentTile);
        }
    }

    public void SaveLevel() {
        savedLevel = levelManager.GetLevelData();
    }

    public void SaveJsonLevelDataToHardDrive() {
        var path = Application.persistentDataPath + "/levelData.txt";

        var leveldata = levelManager.GetLevelData();

        FileStream fileStream = new FileStream(path, FileMode.Create);

        using(StreamWriter writer = new StreamWriter(fileStream)) {
            writer.Write(leveldata.Serialize());
        }

    }

    public void LoadJsonLevelDataFromHardDrive() {
        var path = Application.persistentDataPath + "/levelData.txt";

        if (File.Exists(path)) {
            using (StreamReader reader = new StreamReader(path)) {
                string data = reader.ReadToEnd();

                var levelData = ScriptableLevel.Deserialize(data);

                levelData.levelIndex = 999;
                levelData.name = $"Level {999}";

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
        foreach (var pos in teleportMap.cellBounds.allPositionsWithin) {
            if (teleportMap.HasTile(pos)) {
                if (piecesMap.GetTile<LevelTile>(pos) == teleportTile) {
                }
                if (!gridposes.Contains(pos)) {
                    return false;
                }
            }
        }

        return true;
    }
}

public enum Size {
    large,
    middle,
    small,
}
