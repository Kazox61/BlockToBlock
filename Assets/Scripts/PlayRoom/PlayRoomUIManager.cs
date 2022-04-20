using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayRoomUIManager : MonoBehaviour {
    #region References
    public TMP_Text textMovesCounter;
    public TMP_Text textLevelIndex;
    public TMP_Text textPersonalRecord;
    [SerializeField] private TMP_Text textLevelCompleted, textEndscreenMoves;
    [SerializeField] private TMP_Text textInfo;
    [SerializeField] private GameObject buttonInfo, popupInfo, panelEndscren;
    public GameObject buttonBackToEditMode, panelMapGameplay;
    [SerializeField] private GameplayHandler gameplayHandler;
    [SerializeField] private SceneLoadChannelSO sceneLoadChannelSO;
    [SerializeField] private UserInfoChannelSO userInfoChannelSO;
    [HideInInspector] public int currentLevelIndex;
    #endregion


    public void Awake() {
        gameplayHandler.OnMapLevelFinishedAction += OnMapLevelFinished;
    }


    #region Button-Methods

    public void ButtonBackToEditModeClicked() {
        sceneLoadChannelSO.OnEventRaised(2, true);
    }

    public void HomeMenuButtonClicked() {
        sceneLoadChannelSO.RaiseEvent(1, true);
    }

    public void UpdateInfoButton() {
        if (gameplayHandler.currentPlayedLevel.infoText == null || gameplayHandler.currentPlayedLevel.infoText == "") {
            buttonInfo.SetActive(false);
        }
        else {
            textInfo.text = gameplayHandler.currentPlayedLevel.infoText;
        }
    }

    public void ButtonInfoClicked() {
        popupInfo.SetActive(true);
    }

    public void OnMapLevelFinished() {
        userInfoChannelSO.AddCompletedLevel(currentLevelIndex, gameplayHandler.MovesCounter);
        panelEndscren.SetActive(true);
        textLevelCompleted.text = $"Level {currentLevelIndex} COMPLETE!";
        textEndscreenMoves.text = $"Moves: {gameplayHandler.MovesCounter}";

    }

    public void ButtonNextLevelClicked() {
        currentLevelIndex += 1;
        gameplayHandler.LoadLevel(currentLevelIndex);
        UpdatePersonalRecordText();
        panelEndscren.SetActive(false);
    }

    public void ButtonSameLevelClicked() {
        gameplayHandler.ResetLevel();
        panelEndscren.SetActive(false);
    }
    #endregion

    public void UpdatePersonalRecordText() {

        var levelInfos = userInfoChannelSO.GetLevelInfos();

        if (levelInfos.TryGetValue(currentLevelIndex, out var result)) {
            textPersonalRecord.text = $"Personal Record: {result}";
        }
    }
}
