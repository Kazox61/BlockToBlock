using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ScriptableLevel : ScriptableObject {
    public int levelIndex;
    public List<SaveTile> resultTiles = new List<SaveTile>();
    public List<SaveTile> pieceTiles = new List<SaveTile>();
    public List<SaveTile> gridTiles = new List<SaveTile>();
    public List<SaveTile> teleportTiles = new List<SaveTile>();

    public string infoText;

    public string Serialize() {
        var builder = new StringBuilder();
        builder.Append("r[");
        foreach (var resultTile in resultTiles) {
            builder.Append($"{(int)resultTile.type}({resultTile.position.x}, {resultTile.position.y})");
        }
        builder.Append("]");

        builder.Append("p[");
        foreach (var piecesTile in pieceTiles) {
            builder.Append($"{(int)piecesTile.type}({piecesTile.position.x}, {piecesTile.position.y})");
        }
        builder.Append("]");

        builder.Append("g[");
        foreach (var gridTile in gridTiles) {
            builder.Append($"{(int)gridTile.type}({gridTile.position.x}, {gridTile.position.y})");
        }
        builder.Append("]");
        builder.Append("t[");
        foreach (var teleportTile in teleportTiles) {
            builder.Append($"{(int)teleportTile.type}({teleportTile.position.x}, {teleportTile.position.y})");
        }
        builder.Append("]");

        return builder.ToString();
    }

    public static ScriptableLevel Deserialize(string data) {
        var newLevel = ScriptableObject.CreateInstance<ScriptableLevel>();
        newLevel.name = "levelData";
        var maps = data.Split(']');
        var resultPositions = maps[0].Remove(0, 2).Split(")");
        foreach (var resultPosition in resultPositions) {
            if (resultPosition == "") {
                continue;
            }
            var d = resultPosition.Split('(');
            Int32.TryParse(d[0], out int result);
            var type = (TileType)result;
            var xy = d[1].Split(',');

            Vector3Int pos = new Vector3Int(Int32.Parse(xy[0]), Int32.Parse(xy[1]));

            var saveTile = new SaveTile() {
                position = pos,
                type = type,
            };
            newLevel.resultTiles.Add(saveTile);
        }

        var piecesPositions = maps[1].Remove(0, 2).Split(")");
        foreach (var piecesPosition in piecesPositions) {
            if (piecesPosition == "") {
                continue;
            }
            var d = piecesPosition.Split('(');
            Int32.TryParse(d[0], out int result);
            var type = (TileType)result;

            var xy = d[1].Split(',');

            Vector3Int pos = new Vector3Int(Int32.Parse(xy[0]), Int32.Parse(xy[1]));

            var saveTile = new SaveTile() {
                position = pos,
                type = type,
            };
            newLevel.pieceTiles.Add(saveTile);
        }

        var gridPositions = maps[2].Remove(0, 2).Split(")");
        foreach (var gridPosition in gridPositions) {
            if (gridPosition == "") {
                continue;
            }
            var d = gridPosition.Split('(');
            Int32.TryParse(d[0], out int result);
            var type = (TileType)result;

            var xy = d[1].Split(',');
            Vector3Int pos = new Vector3Int(Int32.Parse(xy[0]), Int32.Parse(xy[1]));
            var saveTile = new SaveTile() {
                position = pos,
                type = type
            };
            newLevel.gridTiles.Add(saveTile);
        }

        var teleportPositions = maps[3].Remove(0, 2).Split(")");
        foreach (var teleportPosition in teleportPositions) {
            if (teleportPosition == "") {
                continue;
            }
            var d = teleportPosition.Split('(');
            Int32.TryParse(d[0], out int result);
            var type = (TileType)result;

            var xy = d[1].Split(',');
            Vector3Int pos = new Vector3Int(Int32.Parse(xy[0]), Int32.Parse(xy[1]));
            var saveTile = new SaveTile() {
                position = pos,
                type = type
            };
            newLevel.teleportTiles.Add(saveTile);
        }
        return newLevel;
    }
}


[System.Serializable]
public class SaveTile {
    public Vector3Int position;
    public TileType type;
    public int teleportIndex;
    public int teleportDir;
}