using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;
using TMPro;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;

public class LevelCreator : MonoBehaviour {
    #region references
    [SerializeField] private Camera cam;
    [SerializeField] private LevelAnchor levelAnchor;
    [SerializeField] private LevelCreatorUIManager levelCreatorUIManager;
    [SerializeField] private LevelHelper levelHelper;

    [SerializeField] private Tilemap gridMap, resultMap, piecesMap, teleportMap;
    public LevelTile gridTile, orangeTile, greenTile, redTile, teleportTile;
    [SerializeField] private SceneLoadChannelSO sceneLoadChannelSO;
    [SerializeField] private GameObject containerErrorMessage, prefabPopupErrorMessage;
    #endregion

    #region variables
    private float touchTimer = 0;
    private Vector3Int firstTouchPosition;
    [HideInInspector] public LevelTile currentTile;
    [HideInInspector] public GameObject activeIndicator;
    [HideInInspector] public int teleportDirection = 0;
    [HideInInspector] public int teleportIndex = 0;
    #endregion

    #region Unity-Callbacks
    private void Awake() {
        if (!levelAnchor.IsSet) {
            levelHelper.ClearLevel();
            return;
        }

        var scriptableLevel = levelAnchor.Item;
        levelHelper.LoadLevel(scriptableLevel.levelData);
    }

    private void Start() {
        currentTile = gridTile;
        SetMapSize(Size.large);
    }
    private void Update() {
        CheckTouchPlacement();
    }
    #endregion

    #region Drawing On Grid
    private void CheckTouchPlacement() {
        touchTimer += Time.deltaTime;
        if (Input.GetMouseButtonDown(0) && !Helpers.IsOverUI()) {
            firstTouchPosition = MousePositionToMapPosition(Input.mousePosition);
        }
        else if (Input.GetMouseButtonUp(0) && !Helpers.IsOverUI() && touchTimer > 0.75f) {
            var mousePosition = MousePositionToMapPosition(Input.mousePosition);
            var maxX = Mathf.Max(firstTouchPosition.x, mousePosition.x);
            var minX = Mathf.Min(firstTouchPosition.x, mousePosition.x);
            var maxY = Mathf.Max(firstTouchPosition.y, mousePosition.y);
            var minY = Mathf.Min(firstTouchPosition.y, mousePosition.y);

            for (int x = minX; x <= maxX; x++) {
                for (int y = minY; y <= maxY; y++) {
                    Draw(new Vector3Int(x, y));
                }
            }
        }
    }

    private Vector3Int MousePositionToMapPosition(Vector3 mousepos) {
        var screenpos = cam.ScreenToWorldPoint(mousepos);
        var mouseMapPosition = new Vector3Int(Mathf.FloorToInt(screenpos.x), Mathf.FloorToInt(screenpos.y));
        return mouseMapPosition;
    }

    private void Draw(Vector3Int pos) {
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
    #endregion
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

    #region Save Level to HardDrive
    public bool IsPlayable() {

        List<Vector3Int> gridposes = new List<Vector3Int>();
        var result = true;
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
                    InstantiateErrorMessage("All Result Blocks need to be on a Grid Block!");
                    result = false;
                }
            }
        }
        if (i == 0) {
            InstantiateErrorMessage("There needs to be at least one Result Block!");
            result = false;
        }

        int x = 0;
        foreach (var pos in piecesMap.cellBounds.allPositionsWithin) {
            if (piecesMap.HasTile(pos)) {
                if (piecesMap.GetTile<LevelTile>(pos) == orangeTile) {
                    x += 1;
                }
                if (!gridposes.Contains(pos)) {
                    InstantiateErrorMessage("All Moveable or Collectable Blocks need to be on a Grid Block!");
                    result = false;
                }
            }
        }
        if (x == 0) {
            InstantiateErrorMessage("There needs to be at least one Moveable Block!");
            result = false;
        }

        foreach (var pos in teleportMap.cellBounds.allPositionsWithin) {
            if (teleportMap.HasTile(pos)) {
                if (piecesMap.GetTile<LevelTile>(pos) == teleportTile) {
                }
                if (!gridposes.Contains(pos)) {
                    InstantiateErrorMessage("All Teleporters need to be on a Grid Block!");
                    result = false;
                }
            }
        }

        return result;
    }

    public void InstantiateErrorMessage(string message) {
        var errorBox = Instantiate(prefabPopupErrorMessage, containerErrorMessage.transform);
        var errorScript = errorBox.GetComponent<PopupErrorMessage>();
        errorScript.textError.text = message;
    }

    public void SaveJsonLevelDataToHardDrive() {
        var directoryPath = Path.Combine(Application.persistentDataPath, "Levels");
        if (!Directory.Exists(directoryPath)) {
            Directory.CreateDirectory(directoryPath);
        }
        var levelName = levelCreatorUIManager.inputLevelName.text;

        var levelPath = Path.Combine(directoryPath, levelName);
        if (!Directory.Exists(levelPath)) {
            Directory.CreateDirectory(levelPath);
        }
        var leveldata = levelHelper.GetLevelData();

        FileStream fileStream = new FileStream(Path.Combine(levelPath, "levelData.json"), FileMode.Create);

        using (StreamWriter writer = new StreamWriter(fileStream)) {
            var json = JsonConvert.SerializeObject(leveldata.ToLevel());
            writer.Write(json);
        }

        TakeScreenshot(Path.Combine(levelPath, "levelScreenshot.png"));

    }

    private void TakeScreenshot(string path) {
        cam.GetComponent<CameraMovement>().takeScreenshotOnNextFrame = true;
        cam.GetComponent<CameraMovement>().path = path;
    }
    #endregion

    #region Test Level
    public void TestLevel() {

        if (!IsPlayable()) {


            return;
        }


        var scriptableLevel = levelHelper.GetLevelData();

        var transferData = new LevelTransferData() { levelData = scriptableLevel, PlayMode = PlayMode.testMode };

        levelAnchor.Item = transferData;

        sceneLoadChannelSO.OnEventRaised(3, true);
    }
    #endregion
}


public enum Size {
    large,
    middle,
    small,
}
