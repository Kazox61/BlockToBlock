using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour { 
    public StateMachine StateMachine { get; private set; }
    public IngameState IngameState { get; private set; }
    public FinishedLevelState FinishedLevelState { get; private set; }
    public MenuState MenuState { get; private set; }
    public LevelCreationState LevelCreationState { get; private set; }
    public TestLevelState TestLevelState { get; private set; }

    public GameObject gridDrawer;
    public GameObject testLevelPanel;
    public GameObject levelCreatorPanel;
    public GameSettings gameSettings;
    public GameObject endScreen;
    public GameObject topBar;
    public GameObject controls;
    public GameObject gameplay;
    public GameObject startscreen;
    public GameObject levelselection;
    public GameObject infoPanel;
    public LevelCreatorManager levelCreatorManager;

    public Animator anim;
    [SerializeField] private Gameboard gameboard;
    [SerializeField] private LevelManager levelManager;

    public void Awake() {
        StateMachine = new StateMachine();

        IngameState = new IngameState(this, gameboard, levelManager);
        FinishedLevelState = new FinishedLevelState(this, gameboard, levelManager);
        MenuState = new MenuState(this, gameboard, levelManager);
        LevelCreationState = new LevelCreationState(this, gameboard, levelManager, levelCreatorManager);
        TestLevelState = new TestLevelState(this, gameboard, levelManager, levelCreatorManager);

        
    }

    public void Start() {
        StateMachine.TryEnterState(MenuState);
    }


    public void Update() {
        StateMachine.CurrentState.OnUpdate();
    }


    public void ShowEndScreen() {
        endScreen.SetActive(true);
        topBar.SetActive(false);
        controls.SetActive(false);
       
    }

    public void ShowGameUI() {
        endScreen.SetActive(false);
        topBar.SetActive(true);
        if (gameSettings.mobile) {
            controls.SetActive(true);
        }
    }


    public void NextLevelButtonPressed() {
        FinishedLevelState.hasPressedNextLevelButton = true;
    }

    public void ActivateHomeMenuScreen() {
        startscreen.SetActive(true);
        gameplay.SetActive(false);
        gridDrawer.SetActive(false);
        StateMachine.TryEnterState(MenuState);
    }

    public void ToggleInfoPanel() {
        if (infoPanel.active) {
            infoPanel.SetActive(false);
        }
        else {
            infoPanel.SetActive(true);
        }
    }

    public void ChangeToTestState() {
        if (!levelCreatorManager.CanRunLevel()) return;
        levelCreatorManager.SaveLevel();
        StateMachine.TryEnterState(TestLevelState);
        levelCreatorPanel.SetActive(false);
        gameplay.SetActive(true);
        controls.SetActive(true);
        testLevelPanel.SetActive(true);
        gridDrawer.SetActive(false);
    }

    public void ChangeToCreationState() {
        StateMachine.TryEnterState(LevelCreationState);
        gridDrawer.SetActive(true);
        levelCreatorPanel.SetActive(true);
        controls.SetActive(true);
        testLevelPanel.SetActive(false);
        gameplay.SetActive(false);
    }

    public void ExitGame() {
        Application.Quit();
    }
}
