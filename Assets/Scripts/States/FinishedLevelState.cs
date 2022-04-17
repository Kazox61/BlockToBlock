using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

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


        int levelIndex = Gameboard.currentLevel;
        int score = Gameboard.moves;

        var levelInfo = GameController.userInfo.GetLevelInfo(levelIndex);
           
        if (levelInfo == 0) {
            GameController.userInfo.levelInfos.Add(levelIndex, score);
            
        }
        else {
            if (levelInfo > score) {
                GameController.userInfo.levelInfos[levelIndex] = score;
            }
        }

        SaveUserInfo();
    }

    public override void OnExit() {
        GameController.panelEndScreenLevel.SetActive(false);
        timer = 0;
        hasPressedNextLevelButton = false;
        triggeredAnimation = false;
        triggeredEndScreen = false;
        isAnimationFinished = false;
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


    public void SaveUserInfo() {
        var path = Application.persistentDataPath + "/userinfo.json";
        FileStream fileStream = new FileStream(path, FileMode.Create);

        using (StreamWriter writer = new StreamWriter(fileStream)) {
            var json = JsonConvert.SerializeObject(GameController.userInfo);
            writer.Write(json);
        }
    }
}
