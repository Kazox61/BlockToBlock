using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCreationState : StateBase {
    private LevelCreatorManager levelCreatorManager;

    private Vector3Int firstMousePos;
    public LevelCreationState(GameController gameController, Gameboard gameboard, LevelManager levelManager, LevelCreatorManager levelCreatorManager) : base(gameController, gameboard, levelManager) {
        this.levelCreatorManager = levelCreatorManager;
    }

    public override void OnEnter() {
        if (levelCreatorManager.savedLevel == null) {
            LevelManager.ClearLevel();
        }
        else {
            LevelManager.LoadLevel(levelCreatorManager.savedLevel);
        }
        levelCreatorManager.levelCreatorPanel.SetActive(true);
    }

    public override void OnExit() {
        
    }

    public override void OnUpdate() {

        if (Input.GetMouseButtonDown(0) && !Helpers.IsOverUI()) {
            firstMousePos = MousePositionToMapPosition(Input.mousePosition);
        }
        else if(Input.GetMouseButtonUp(0) && !Helpers.IsOverUI()) {
            var mousePosition = MousePositionToMapPosition(Input.mousePosition);
            var maxX = Mathf.Max(firstMousePos.x, mousePosition.x);
            var minX = Mathf.Min(firstMousePos.x, mousePosition.x);
            var maxY = Mathf.Max(firstMousePos.y, mousePosition.y);
            var minY = Mathf.Min(firstMousePos.y, mousePosition.y);

            for (int x = minX; x <= maxX; x++) {
                for (int y = minY; y <= maxY; y++) {
                    levelCreatorManager.Draw(new Vector3Int(x, y));
                }
            }
        }
    }

    public Vector3Int MousePositionToMapPosition(Vector3 mousepos) {
        var screenpos = levelCreatorManager.cam.ScreenToWorldPoint(mousepos);
        var mouseMapPosition = new Vector3Int(Mathf.FloorToInt(screenpos.x), Mathf.FloorToInt(screenpos.y));
        return mouseMapPosition;
    }
}
