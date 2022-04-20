using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelCreatorUIManager : MonoBehaviour {
    #region References
    [SerializeField] private GameObject indicatorGrid, indicatorOrange, indicatorGreen, indicatorRed, indicatorTeleport, indicatorEraser;
    [SerializeField] private LevelCreator levelCreator;
    [SerializeField] private GameObject panelTeleportFields;
    public TMP_InputField inputLevelName;
    [SerializeField] private SceneLoadChannelSO sceneLoadChannelSO;
    #endregion

    #region Unity-Callbacks
    private void Start() {
        levelCreator.activeIndicator = indicatorGrid;
    }
    #endregion

    #region Button-Methods

    public void HomeMenuButtonClicked() {
        sceneLoadChannelSO.RaiseEvent(1, true);
    }

    public void ChangeDrawingBlock(int index) {
        DisableTeleportFields();
        if (index == 0) {
            levelCreator.currentTile = levelCreator.gridTile;
            levelCreator.activeIndicator.SetActive(false);
            levelCreator.activeIndicator = indicatorGrid;
            levelCreator.activeIndicator.SetActive(true);
        }
        else if (index == 1) {
            levelCreator.currentTile = levelCreator.orangeTile;
            levelCreator.activeIndicator.SetActive(false);
            levelCreator.activeIndicator = indicatorOrange;
            levelCreator.activeIndicator.SetActive(true);
        }
        else if (index == 2) {
            levelCreator.currentTile = levelCreator.greenTile;
            levelCreator.activeIndicator.SetActive(false);
            levelCreator.activeIndicator = indicatorGreen;
            levelCreator.activeIndicator.SetActive(true);
        }
        else if (index == 3) {
            levelCreator.currentTile = levelCreator.redTile;
            levelCreator.activeIndicator.SetActive(false);
            levelCreator.activeIndicator = indicatorRed;
            levelCreator.activeIndicator.SetActive(true);
        }
        else if (index == 4) {
            ShowTeleportFields();
            levelCreator.currentTile = levelCreator.teleportTile;
            levelCreator.activeIndicator.SetActive(false);
            levelCreator.activeIndicator = indicatorTeleport;
            levelCreator.activeIndicator.SetActive(true);
        }
        else if (index == 5) {
            levelCreator.currentTile = null;
            levelCreator.activeIndicator.SetActive(false);
            levelCreator.activeIndicator = indicatorEraser;
            levelCreator.activeIndicator.SetActive(true);
        }
    }

    public void ShowTeleportFields() {
        panelTeleportFields.SetActive(true);
    }

    public void DisableTeleportFields() {
        panelTeleportFields.SetActive(false);
    }

    public void DropdownTeleportDirection(int i) {
        levelCreator.teleportDirection = i;
    }

    public void InputTeleportIndex(string input) {
        levelCreator.teleportIndex = int.Parse(input);
    }

    public void DropdownScreenDrawSize(int i) {
        if (i == 0) {
            levelCreator.SetMapSize(Size.large);
        }
        else if (i == 1) {
            levelCreator.SetMapSize(Size.middle);
        }
        else if (i == 2) {
            levelCreator.SetMapSize(Size.small);
        }
    }
    #endregion
}
