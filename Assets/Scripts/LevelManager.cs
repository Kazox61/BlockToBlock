using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;
using UnityEditor;
using TMPro;

public class LevelManager : MonoBehaviour {
    [SerializeField] private Tilemap resultMap, pieceMap, gridMap;
    [SerializeField] private int levelIndex;


    public ScriptableLevel GetLevelData() {
        var newLevel = ScriptableObject.CreateInstance<ScriptableLevel>();

        newLevel.levelIndex = levelIndex;
        newLevel.name = $"Level {levelIndex}";

        newLevel.resultTiles = GetTilesFromMap(resultMap).ToList();
        newLevel.pieceTiles = GetTilesFromMap(pieceMap).ToList();
        newLevel.gridTile = GetTilesFromMap(gridMap).ToList();

#if UNITY_EDITOR
        ScriptableObjectUtility.SaveLeveFile(newLevel);
#endif
        IEnumerable<SaveTile> GetTilesFromMap(Tilemap map) {
            foreach (var pos in map.cellBounds.allPositionsWithin) {
                if (map.HasTile(pos)) {
                    var levelTile = map.GetTile<LevelTile>(pos);
                    yield return new SaveTile() {
                        position = pos,
                        tile = levelTile
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
            switch (savedTile.tile.type) {
                case TileType.red:
                    resultMap.SetTile(savedTile.position, savedTile.tile);
                    break;
            }
        }

        foreach (var savedTile in level.pieceTiles) {
            switch (savedTile.tile.type) {
                case TileType.green:
                    pieceMap.SetTile(savedTile.position, savedTile.tile);
                    break;

                case TileType.orange:
                    pieceMap.SetTile(savedTile.position, savedTile.tile);
                    break;
            }
        }

        foreach (var savedTile in level.gridTile) {
            switch (savedTile.tile.type) {
                case TileType.grid:
                    gridMap.SetTile(savedTile.position, savedTile.tile);
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

