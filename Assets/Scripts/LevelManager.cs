using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;
using UnityEditor;
using System.IO;

public class LevelManager : MonoBehaviour {
    [SerializeField] private Tilemap resultMap, pieceMap, gridMap;
    [SerializeField] private int levelIndex;

    public LevelTile orangeTile, greenTile, redTile, gridTile;

    public ScriptableLevel GetLevelData() {
        var newLevel = ScriptableObject.CreateInstance<ScriptableLevel>();

        newLevel.levelIndex = levelIndex;
        newLevel.name = $"Level {levelIndex}";

        newLevel.resultTiles = GetTilesFromMap(resultMap).ToList();
        newLevel.pieceTiles = GetTilesFromMap(pieceMap).ToList();
        newLevel.gridTiles = GetTilesFromMap(gridMap).ToList();

#if UNITY_EDITOR
        ScriptableObjectUtility.SaveLeveFile(newLevel);
#endif
        IEnumerable<SaveTile> GetTilesFromMap(Tilemap map) {
            foreach (var pos in map.cellBounds.allPositionsWithin) {
                if (map.HasTile(pos)) {
                    var levelTile = map.GetTile<LevelTile>(pos);
                    yield return new SaveTile() {
                        position = pos,
                        type = levelTile.type
                    };

                }
            }
        }
        return newLevel;
    }

    public void ClearLevel() {
        var maps = FindObjectsOfType<Tilemap>();

        foreach (var tilemap in maps) {
            tilemap.ClearAllTiles();
        }
    }

    public ScriptableLevel LoadLevel(ScriptableLevel level) {
        ClearLevel();

        foreach (var savedTile in level.resultTiles) {
            switch (savedTile.type) {
                case TileType.red:
                    resultMap.SetTile(savedTile.position, redTile);
                    break;
            }
        }

        foreach (var savedTile in level.pieceTiles) {
            switch (savedTile.type) {
                case TileType.green:
                    pieceMap.SetTile(savedTile.position, greenTile);
                    break;

                case TileType.orange:
                    pieceMap.SetTile(savedTile.position, orangeTile);
                    break;
            }
        }

        foreach (var savedTile in level.gridTiles) {
            switch (savedTile.type) {
                case TileType.grid:
                    gridMap.SetTile(savedTile.position, gridTile);
                    break;
            }
        }
        return level;
    }
    public ScriptableLevel LoadLevel(int levelIndex) {
        var level = Resources.Load<ScriptableLevel>($"Levels/Level {levelIndex}");

        if (level == null) {
            Debug.LogError($"Level {levelIndex} does not exist.");
            return null;
        }

        return LoadLevel(level);
    }

    public void LoadLevel() {
        LoadLevel(this.levelIndex);
    }

    public void ConvertLevelJsonToScriptableLevel() {
        var path = Application.persistentDataPath + "/levelData.txt";

        if (File.Exists(path)) {
            using (StreamReader reader = new StreamReader(path)) {
                string data = reader.ReadToEnd();

                var levelData = ScriptableLevel.Deserialize(data);

                levelData.levelIndex = levelIndex;
                levelData.name = $"Level {levelIndex}";
#if UNITY_EDITOR
                ScriptableObjectUtility.SaveLeveFile(levelData);
#endif
            }
        }
    }

    public LevelTile TileTypeToLevelTile(TileType type) {
        if (type.Equals(TileType.green)) {
            return greenTile;
        }
        else if (type.Equals(TileType.orange)) {
            return orangeTile;
        }
        else if (type.Equals(TileType.red)) {
            return redTile;
        }
        else {
            return gridTile;
        }
    }
}

#if UNITY_EDITOR

public static class ScriptableObjectUtility {
    public static void SaveLeveFile(ScriptableLevel level) {
        AssetDatabase.CreateAsset(level, $"Assets/Resources/Levels/{level.name}.asset");

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}

#endif

