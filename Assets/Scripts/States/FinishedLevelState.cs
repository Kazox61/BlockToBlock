using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishedLevelState : StateBase {


    private float timeTillRemoveUI = 1;

    public bool hasPressedNextLevelButton;
    private bool triggeredAnimation;
    private bool triggeredEndScreen;

    public float timer;

    public FinishedLevelState(GameController gameController, Gameboard gameboard, LevelManager levelManager) : base(gameController, gameboard, levelManager) {
    }

    public override void OnEnter() {
        timer = 0;
        hasPressedNextLevelButton = false;
        triggeredAnimation = false;
        triggeredEndScreen = false;
        isAnimationFinished = false;
    }

    public override void OnExit() {
        GameController.endScreen.SetActive(false);
    }

    public override void OnUpdate() {
        if (isAnimationFinished) {
            GameController.StateMachine.TryEnterState(GameController.IngameState);
        }

        if (!hasPressedNextLevelButton) return;
        timer += Time.deltaTime;

        if (!triggeredAnimation) {
            GameController.anim.SetTrigger("Fade2");
            triggeredAnimation = true;
        }

        if (timer >= timeTillRemoveUI && !triggeredEndScreen) {
            triggeredEndScreen = true;
            Gameboard.LoadNextLevel();
        }

        
    }
}
