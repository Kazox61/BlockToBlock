using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using LootLocker.Requests;
using TMPro;
using System;
using UnityEngine.Networking;

public class LootLockerManager : MonoBehaviour {

    [SerializeField] private TMP_InputField inputEmail, inputUsername, inputPassword;

    [SerializeField] private GameObject authScreen;
    public Gameboard gameboard;

    public GameObject contentPublicLevels;
    public GameObject prefabPublicLevel;

    private void Start() {
        CheckSession();

    }

    private void CheckSession() {
        LootLockerSDKManager.CheckWhiteLabelSession(response => {
            if (response) LootLockerSDKManager.StartWhiteLabelSession((response) => {
                if (response.success) {
                    Debug.Log("Started White Label Login");
                }
            });
            else authScreen.SetActive(true);
        });
    }

    public void RegisterClicked() {
        LootLockerSDKManager.WhiteLabelSignUp(inputEmail.text, inputPassword.text, (response) => {
            if (response.success) Debug.Log("Failed to Register");
            else {
                authScreen.SetActive(false);
                LoginClicked();
            }
        });
    }

    private void LoginClicked() {
        LootLockerSDKManager.WhiteLabelLogin(inputEmail.text, inputPassword.text, true, (response) => {
            if (response.success) return;
            else {
                LootLockerSDKManager.SetPlayerName(inputUsername.text, (response) => {
                    if (response.success) Debug.Log("Failed Username");
                });
            }
        });
    }

    public void UploadLevel(string levelName, string screenshotFilePath, string levelFilePath) {
        LootLockerSDKManager.CreatingAnAssetCandidate(levelName, (response) => {
            if (response.success) {
                UploadLevelData(response.asset_candidate_id, levelName, screenshotFilePath, levelFilePath);
            }
            else {

            }
        });
    }

    public void UploadLevelData(int levelID, string levelName, string screenshotFilePath, string levelFilePath) {

        LootLocker.LootLockerEnums.FilePurpose screenshotFileType = LootLocker.LootLockerEnums.FilePurpose.primary_thumbnail;

        LootLockerSDKManager.AddingFilesToAssetCandidates(levelID, screenshotFilePath, levelName, screenshotFileType, (screenshotResponse) => {
            if (screenshotResponse.success) {
                LootLocker.LootLockerEnums.FilePurpose textFileType = LootLocker.LootLockerEnums.FilePurpose.file;
                LootLockerSDKManager.AddingFilesToAssetCandidates(levelID, levelFilePath, levelName, textFileType, (fileResponse) => {
                    
                    if (fileResponse.success) {
                        LootLockerSDKManager.UpdatingAnAssetCandidate(levelID, true, (updatedResponse) => {

                        });
                    }
                });
            }
        });
    }

    public void DownloadLevelData() {
        LootLockerSDKManager.GetAssetListWithCount(10, (response) => {
            if (response.success) {
                Debug.Log(response.assets.Length);
            }
            for (int i = 0; i < response.assets.Length; i++) {

                var item = Instantiate(prefabPublicLevel, contentPublicLevels.transform);

                var publicLevel = item.GetComponent<BoxPublicLevel>();

                publicLevel.text.text = response.assets[i].name;

                LootLockerFile[] levelImageFiles = response.assets[i].files;

                StartCoroutine(DownloadLevelImage(levelImageFiles[0].url.ToString(), publicLevel));

                publicLevel.levelURL = levelImageFiles[1].url.ToString();

                publicLevel.buttonPlay.onClick.AddListener(delegate () { StartCoroutine(DownloadLevelData(publicLevel)); });
            }
        }, null, true);

        LootLockerSDKManager.ResetAssetCalls();
    }



    IEnumerator DownloadLevelImage(string url, BoxPublicLevel publicLevel) {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);

        yield return www.SendWebRequest();

        Texture2D loadedImage = DownloadHandlerTexture.GetContent(www);

        publicLevel.image.sprite = Sprite.Create(loadedImage, new Rect(0, 0, loadedImage.width, loadedImage.height), Vector2.zero);

    }


    IEnumerator DownloadLevelData(BoxPublicLevel publicLevel) {
        UnityWebRequest www = UnityWebRequest.Get(publicLevel.levelURL);

        yield return www.SendWebRequest();

        var level = JsonUtility.FromJson<Level>(www.downloadHandler.text);

        yield return new WaitForSeconds(1f);
        gameboard.levelManager.LoadLevel(level.ToScriptableLevel());

    }
}
