using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.IO;
using Newtonsoft.Json;

public class MainMenuUIManager : MonoBehaviour {
    #region References
    public GameObject panelBackground, panelProfile, panelButtons;

    public Button buttonSettings, buttonLevelSelection, buttonLevelCreator, buttonsMyLevels, buttonPublicLevels, buttonShowPopupLogin, buttonShowPopupSignup;

    public GameObject popupSignup, popupLogin, popupSettings, popupMyLevels, popupPublicLevels, popupLevelSeletion, popupLoginOrSignup;

    public TMP_Text textStars, textDiamonds;

    public TMP_InputField inputSignupEmail, inputSignupUsername, inputSignupPassword, inputLoginEmail, inputLoginPassword;

    public GameObject prefabMyLevel, contentMyLevels;

    public LootLockerManager lootLockerManager;
    [SerializeField] private LevelAnchor levelAnchorMainMenuToLevelCreator;
    [SerializeField] private LevelAnchor levelAnchorMainMenuToPlayRoom;
    #endregion
    #region ButtonClickMethods


    public void ButtonSettingsClicked() {
        popupSettings.SetActive(true);
    }

    public void ButtonLevelSelectionClicked() {
        popupLevelSeletion.SetActive(true);
    }

    public void ButtonLevelCreatorClicked() {
        SceneManager.LoadScene("LevelCreator");
    }

    public void ButtonMyLevelsClicked() {
        popupMyLevels.SetActive(true);
        LoadJsonLevelDataFromHardDrive();
    }

    public void ButtonPublishLevelsClicked() {
        popupPublicLevels.SetActive(true);
    }

    public void ButtonShowLoginClicked() {
        popupLogin.SetActive(true);
        popupLoginOrSignup.SetActive(false);
    }
    public void ButtonShowSignupClicked() {
        popupSignup.SetActive(true);
        popupLoginOrSignup.SetActive(false);
    }

    #endregion

    #region LoadLevel
    public void LoadJsonLevelDataFromHardDrive() {

        var directoryPath = $"{Application.persistentDataPath}/Levels";
        var levelPaths = Directory.GetFiles(directoryPath);
        foreach (var levelPath in levelPaths) {
            var obj = Instantiate(prefabMyLevel, contentMyLevels.transform);
            var box = obj.GetComponent<BoxMyLevel>();

            box.pathString = levelPath;


            var tt = levelPath.Split("/");

            var xc = tt[^1];
            var levelName = xc.Substring(7, xc.Length - 7 - 5);

            box.text.text = levelName;

            using (StreamReader reader = new StreamReader(levelPath)) {
                string data = reader.ReadToEnd();

                var level = JsonConvert.DeserializeObject<Level>(data);

                box.level = level.ToScriptableLevel();

                box.buttonEdit.onClick.AddListener(delegate () { EditMyLevelClicked(box); });

                box.buttonPlay.onClick.AddListener(delegate () { PlayMylevelClicked(box); });

                box.buttonUpload.onClick.AddListener(delegate () { lootLockerManager.UploadLevel(levelName, $"{Application.persistentDataPath}/LevelScreenshots/{levelName}.png", levelPath); });

                box.buttonDelete.onClick.AddListener(delegate () { DeleteMyLevelClicked(box); });
            }
        }

    }

    public void EditMyLevelClicked(BoxMyLevel boxLevel) {
        var transferData = new LevelTransferData() { levelData = boxLevel.level };
        levelAnchorMainMenuToLevelCreator.Item = transferData;
        SceneManager.LoadScene("LevelCreator");
    }

    public void PlayMylevelClicked(BoxMyLevel boxLevel) {
        var transferData = new LevelTransferData() { levelData = boxLevel.level, PlayMode = PlayMode.customizedMode };
        levelAnchorMainMenuToPlayRoom.Item = transferData;
        SceneManager.LoadScene("PlayRoom");
    }

    public void DeleteMyLevelClicked(BoxMyLevel boxLevel) {
        File.Delete(boxLevel.pathString);
        Destroy(boxLevel.gameObject);
    }
    #endregion
    public void EnterPlayMode(int levelIndex) {
        var level = Resources.Load<ScriptableLevel>($"Levels/Level {levelIndex}");

        if (level == null) {
            Debug.LogError($"Level {levelIndex} does not exist.");
        }
        var transferData = new LevelTransferData() { levelData = level, PlayMode = PlayMode.mapMode, levelIndex = levelIndex };
        levelAnchorMainMenuToPlayRoom.Item = transferData;
        SceneManager.LoadScene("PlayRoom");
    }

    public void DeleteChildObjects(GameObject parentObj) {
        foreach (Transform child in parentObj.transform) {
            Destroy(child.gameObject);
        }
    }

    public void LoadUserInfoAtStart() {
        var path = Application.persistentDataPath + "/userinfo.json";

        if (!File.Exists(path)) {
            //userInfo = new UserInfo();
            return;
        }

        using (StreamReader reader = new StreamReader(path)) {
            string data = reader.ReadToEnd();

            //userInfo = JsonConvert.DeserializeObject<UserInfo>(data);
        }
    }
}
