using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateBase {
    
    public GameController GameController { get; private set; }
    public Gameboard Gameboard { get; private set; }
    public LevelManager LevelManager { get; private set; }

    public bool isAnimationFinished;

    public StateBase(GameController gameController, Gameboard gameboard, LevelManager levelManager) {

        GameController = gameController;
        Gameboard = gameboard;
        LevelManager = levelManager;
    }
    public virtual void OnEnter() {

    }


    public virtual void OnUpdate() {

    }

    public virtual void OnExit() {

    }

}
