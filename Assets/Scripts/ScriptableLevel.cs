using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptableLevel : ScriptableObject {
    public int levelIndex;
    public List<SaveTile> resultTiles;
    public List<SaveTile> pieceTiles;
    public List<SaveTile> gridTile;

    public string infoText;

    public Level ToLevel() {
        var level = new Level() {
            levelIndex = this.levelIndex,
            resultTiles = this.resultTiles,
            pieceTiles = this.pieceTiles,
            gridTile = this.gridTile,
            infoText = this.infoText

        };
        return level;
    }
}


[System.Serializable]
public class SaveTile {
    public Vector3Int position;
    public LevelTile tile;
}

public class Level {
    public int levelIndex;
    public List<SaveTile> resultTiles;
    public List<SaveTile> pieceTiles;
    public List<SaveTile> gridTile;

    public string infoText;


    public ScriptableLevel ToScriptableLevel() {
        var scriptableLevel = ScriptableObject.CreateInstance<ScriptableLevel>();
        scriptableLevel.levelIndex = this.levelIndex;
        scriptableLevel.resultTiles = this.resultTiles;
        scriptableLevel.pieceTiles = this.pieceTiles;
        scriptableLevel.gridTile = this.gridTile;
        scriptableLevel.infoText = this.infoText;
        return scriptableLevel;
    }
}