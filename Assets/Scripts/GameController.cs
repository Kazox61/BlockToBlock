using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
    #region References
    [Header("Panels")]
    public GameObject panelStartScreen;
    public GameObject panelLevelSeletion, panelLevels, panelSettings;
    public GameObject panelGameUI, panelControls, panelEndScreenLevel, panelTopBar, panelInfo;
    public GameObject panelFading, panelLevelCreator, panelToolbar, panelSelectableBlocks, panelTestLevel;

    [Header("Others")]
    public GameSettings gameSettings;
    public LevelCreatorManager levelCreatorManager;
    [SerializeField] private Gameboard gameboard;
    [SerializeField] private LevelManager levelManager;
    public Animator anim;

    public GameObject gridDrawer;

    #endregion


    public StateMachine StateMachine { get; private set; }
    public IngameState IngameState { get; private set; }
    public FinishedLevelState FinishedLevelState { get; private set; }
    public MenuState MenuState { get; private set; }
    public LevelCreationState LevelCreationState { get; private set; }
    public TestLevelState TestLevelState { get; private set; }


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

    #region methods
    public void ShowPanelEndScreen() {
        panelEndScreenLevel.SetActive(true);
        panelTopBar.SetActive(false);
        panelControls.SetActive(false);
    }

    public void ShowPanelGameUI() {
        panelEndScreenLevel.SetActive(false);
        panelTopBar.SetActive(true);
        if (gameSettings.mobile) {
            panelControls.SetActive(true);
        }
    }


    public void ShowPanelStartScreen() {
        if (IngameState.timer > 0 || FinishedLevelState.timer > 0) return;
        panelStartScreen.SetActive(true);
        panelGameUI.SetActive(false);
        panelTopBar.SetActive(false);
        gridDrawer.SetActive(false);
        StateMachine.TryEnterState(MenuState);
    }

    public void NextLevelButtonPressed() {
        FinishedLevelState.hasPressedNextLevelButton = true;
    }

    public void TogglePanelInfo() {
        if (panelInfo.activeInHierarchy) {
            panelInfo.SetActive(false);
        }
        else {
            panelInfo.SetActive(true);
        }
    }

    public void ChangeToTestState() {
        if (!levelCreatorManager.CanRunLevel()) return;
        levelCreatorManager.SaveLevel();
        StateMachine.TryEnterState(TestLevelState);
        panelLevelCreator.SetActive(false);
        panelGameUI.SetActive(true);
        panelControls.SetActive(true);
        panelTestLevel.SetActive(true);
        gridDrawer.SetActive(false);
    }

    public void ChangeToCreationState() {
        StateMachine.TryEnterState(LevelCreationState);
        gridDrawer.SetActive(true);
        panelLevelCreator.SetActive(true);
        panelControls.SetActive(true);
        panelTestLevel.SetActive(false);
        panelGameUI.SetActive(false);
    }

    public void ExitGame() {
        Application.Quit();
    }
    #endregion
}
