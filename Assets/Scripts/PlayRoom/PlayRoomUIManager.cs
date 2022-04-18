using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayRoomUIManager : MonoBehaviour {
    #region References
    public TMP_Text textMovesCounter;
    public TMP_Text textLevelIndex;
    [SerializeField] private TMP_Text textInfo;
    [SerializeField] private GameObject buttonInfo, popupInfo;
    [SerializeField] private GameplayHandler gameplayHandler;
    #endregion

    #region Button-Methods

    public void HomeMenuButtonClicked() {
        SceneManager.LoadScene("MainMenu");
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
    #endregion
}
