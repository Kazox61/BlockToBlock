using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngameState : StateBase {

    private float timer = 0;

    private float timeTillAnimationStarts = 2;
    private float timeTillRemoveGameUI = 3;

    private bool triggeredAnimation;
    private bool triggeredEndScreen;

    public IngameState(GameController gameController, Gameboard gameboard, LevelManager levelManager) : base(gameController, gameboard, levelManager) {
    }

    public override void OnEnter() {
        Gameboard.allowMoveInput = true;
        isAnimationFinished = false;
        triggeredAnimation = false;
        triggeredEndScreen = false;
        timer = 0;

    }

    public override void OnExit() {
        Gameboard.allowMoveInput = false;

    }

    public override void OnUpdate() {

        if (Gameboard.IsGameFinished() && timer == 0) {
            Gameboard.allowMoveInput = false;
            timer += Time.deltaTime;
        }
        else if (timer != 0) {
            timer += Time.deltaTime;
        }

        if (timer >= timeTillAnimationStarts && !triggeredAnimation) {
            GameController.anim.SetTrigger("Fade");
            triggeredAnimation = true;
        }

        if (timer >= timeTillRemoveGameUI && !triggeredEndScreen) {
            GameController.ShowEndScreen();
            triggeredEndScreen = true;
        }

        if (isAnimationFinished) {
            GameController.StateMachine.TryEnterState(GameController.FinishedLevelState);
        }


        if (Gameboard.allowMoveInput) {
            Gameboard.UpdateInput();
        }
    }
}
