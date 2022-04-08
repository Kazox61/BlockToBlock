using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCreationState : StateBase {
    private LevelCreatorManager levelCreatorManager;
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
        levelCreatorManager.SetMapSize(Size.large);
        levelCreatorManager.levelCreatorPanel.SetActive(true);
    }

    public override void OnExit() {
        
    }

    public override void OnUpdate() {
        if (Input.GetMouseButtonDown(0) && !Helpers.IsOverUI()) {
            var m = levelCreatorManager.cam.ScreenToWorldPoint(Input.mousePosition);
            var mousePos = new Vector3Int(Mathf.FloorToInt(m.x), Mathf.FloorToInt(m.y));
            levelCreatorManager.Draw(mousePos);
        }
    }
}
