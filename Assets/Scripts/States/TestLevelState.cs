using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLevelState : StateBase {
    public LevelCreatorManager levelCreatorManager;
    public TestLevelState(GameController gameController, Gameboard gameboard, LevelManager levelManager, LevelCreatorManager levelCreatorManager) : base(gameController, gameboard, levelManager) {
        this.levelCreatorManager = levelCreatorManager;
    }

    public override void OnEnter() {
        LevelManager.LoadLevel(levelCreatorManager.savedLevel);
        Gameboard.allowMoveInput = true;
        Gameboard.cells = Gameboard.GetTilePositionsFromTilemap(Gameboard.piecesMap, Gameboard.orangeTile);
        Gameboard.cameraMovement.Cal(GameController.gameSettings.mobile);
    }

    public override void OnExit() {
        Gameboard.allowMoveInput = false;
        levelCreatorManager.SetMapSize(levelCreatorManager.currentLevelCreatorSize);
    }

    public override void OnUpdate() {
        if (Gameboard.allowMoveInput) {
            Gameboard.UpdateInput();
        }
    }
}
