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

    public GameObject prefabMyLevel, contentMyLevels, contentPublicLevels;
    public LootLockerManager lootLockerManager;
    public UserInfoChannelSO userInfoChannelSO;
    [SerializeField] private LevelIcon[] levelIcons;
    [SerializeField] private MessageManager messageManager;
    [SerializeField] private PublicLevelsUIManager publicLevelsUIManager;
    [SerializeField] private LevelAnchor levelAnchor;
    [SerializeField] private SceneAnchor sceneAnchor;
    [SerializeField] private ManagerAnchor managerAnchor;
    [SerializeField] private SceneLoadChannelSO sceneLoadChannelSO;
    #endregion

    #region Unity-Callbacks
    private void Awake() {
        if (managerAnchor.IsSet) {
            lootLockerManager = managerAnchor.Item.lootLockerManager;
        }
    }
    private void Start() {
        SceneData sceneData = new SceneData() {
            contentPublicLevels = contentPublicLevels,
            mainMenuUIManager = this,
            publicLevelsUIManager = publicLevelsUIManager,
            messageManager = messageManager
        };
        sceneAnchor.Item = sceneData;
    }
    #endregion
    #region ButtonClickMethods

    public void ButtonLoginClicked() {
        lootLockerManager.Login();
    }

    public void ButtonSignupClicked() {
        lootLockerManager.Signup();
    }

    public void ButtonSettingsClicked() {
        popupSettings.SetActive(true);
    }

    public void ButtonLevelSelectionClicked() {
        levelIcons[0].locker.SetActive(false);
        levelIcons[0].canPlay = true;
        var levelInfos = userInfoChannelSO.GetLevelInfos();
        for (int i = levelIcons.Length - 1; i >= 1; i--) {
            if (levelInfos.ContainsKey(i)) {
                levelIcons[i].locker.SetActive(false);
                levelIcons[i].canPlay = true;
            }
        }



        popupLevelSeletion.SetActive(true);
    }

    public void ButtonLevelCreatorClicked() {
        sceneLoadChannelSO.RaiseEvent(2, true);
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

        var directoryPath = Path.Combine(Application.persistentDataPath, "Levels");
        var levelPaths = Directory.GetDirectories(directoryPath);
        Debug.Log(directoryPath);
        foreach (var levelPath in levelPaths) {
            Debug.Log(levelPath);

            var obj = Instantiate(prefabMyLevel, contentMyLevels.transform);
            var box = obj.GetComponent<BoxMyLevel>();
            var levelName = new DirectoryInfo(levelPath).Name;
            var levelDataPath = Path.Combine(levelPath, "levelData.json");
            var levelScreenshotPath = Path.Combine(levelPath, "levelScreenshot.png");
            box.pathString = levelPath;

            box.text.text = levelName;

            byte[] imageData = File.ReadAllBytes(levelScreenshotPath);
            Texture2D texture = new Texture2D(100, 100);
            texture.LoadImage(imageData);
            var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            box.image.sprite = sprite;
            using (StreamReader reader = new StreamReader(levelDataPath)) {
                string data = reader.ReadToEnd();

                var level = JsonConvert.DeserializeObject<Level>(data);

                box.level = level.ToScriptableLevel();

                box.buttonEdit.onClick.AddListener(delegate () { EditMyLevelClicked(box); });

                box.buttonPlay.onClick.AddListener(delegate () { PlayMylevelClicked(box); });

                box.buttonUpload.onClick.AddListener(delegate () { lootLockerManager.UploadLevel(levelName, levelScreenshotPath, levelDataPath); });

                box.buttonDelete.onClick.AddListener(delegate () { DeleteMyLevelClicked(box); });
            }
        }

    }

    public void EditMyLevelClicked(BoxMyLevel boxLevel) {
        var transferData = new LevelTransferData() { levelData = boxLevel.level };
        levelAnchor.Item = transferData;
        sceneLoadChannelSO.RaiseEvent(2, true);
    }

    public void PlayMylevelClicked(BoxMyLevel boxLevel) {
        var transferData = new LevelTransferData() { levelData = boxLevel.level, PlayMode = PlayMode.customizedMode };
        levelAnchor.Item = transferData;
        sceneLoadChannelSO.RaiseEvent(3, true);
    }

    public void DeleteMyLevelClicked(BoxMyLevel boxLevel) {
        Directory.Delete(boxLevel.pathString, true);
        Destroy(boxLevel.gameObject);
    }
    #endregion
    public void EnterPlayMode(int levelIndex) {
        if (!levelIcons[levelIndex-1].canPlay) {
            return;
        }

        var level = Resources.Load<ScriptableLevel>($"Levels/Level {levelIndex}");

        if (level == null) {
            Debug.LogError($"Level {levelIndex} does not exist.");
        }
        var transferData = new LevelTransferData() { levelData = level, PlayMode = PlayMode.mapMode, levelIndex = levelIndex };
        levelAnchor.Item = transferData;
        sceneLoadChannelSO.RaiseEvent(3, true);
    }

    public void DeleteChildObjects(GameObject parentObj) {
        foreach (Transform child in parentObj.transform) {
            Destroy(child.gameObject);
        }
    }
}
