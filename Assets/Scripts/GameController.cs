using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using Newtonsoft.Json;

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
    public UserInfo userInfo;

    public GameObject gridDrawer, panelTeleportInputs;

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
        LoadUserInfoAtStart();
    }


    public void Update() {
        StateMachine.CurrentState.OnUpdate();

        if (Input.GetKeyDown(KeyCode.M)) {
            ScreenCapture.CaptureScreenshot("LevelCreator.png", 2);
        }
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
        panelLevelCreator.SetActive(true);
        panelControls.SetActive(true);
        panelTestLevel.SetActive(false);
        panelGameUI.SetActive(false);
    }

    public void ExitGame() {
        Application.Quit();
    }
    #endregion

    public void ShowTeleportFields() {
        panelTeleportInputs.SetActive(true);
    }

    public void DisableTeleportFields() {
        panelTeleportInputs.SetActive(false);
    }

    public void DeleteChildObjects(GameObject parentObj) {
        foreach (Transform child in parentObj.transform) {
            Destroy(child.gameObject);
        }
    }

    public void LoadLevelFromMyLevels(BoxMyLevel boxLevel) {

        levelManager.LoadLevel(boxLevel.level);
        panelStartScreen.SetActive(false);
        levelCreatorManager.inputLevelName.text = boxLevel.text.text;


        StateMachine.TryEnterState(LevelCreationState);
    }

    public void DeleteLevelFromMyLevels(BoxMyLevel boxLevel) {
        File.Delete(boxLevel.pathString);
        Destroy(boxLevel.gameObject);
    }

    public void LoadUserInfoAtStart() {
        var path = Application.persistentDataPath + "/userinfo.json";

        if (!File.Exists(path)) {
            userInfo = new UserInfo();
            return;
        }

        using (StreamReader reader = new StreamReader(path)) {
            string data = reader.ReadToEnd();

            userInfo = JsonConvert.DeserializeObject<UserInfo>(data);
        }
    }
}
