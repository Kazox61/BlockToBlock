using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName ="New Tile", menuName = "2D/Tiles/Default Tile")]
public class LevelTile : Tile{
    public TileType type;
}

[System.Serializable]
public enum TileType {
    red,
    orange,
    green,
    grid
}