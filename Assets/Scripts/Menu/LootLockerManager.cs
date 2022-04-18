using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using LootLocker.Requests;
using TMPro;
using System;
using UnityEngine.Networking;
using System.Threading.Tasks;

public class LootLockerManager : MonoBehaviour {

    [SerializeField] private MainMenuUIManager mainMenuUIManager;

    public GameObject prefabPublicLevel, contentPublicLevels;

    private void Start() {
        CheckSession();

    }

    private void CheckSession() {
        LootLockerSDKManager.CheckWhiteLabelSession(response => {
            if (response) LootLockerSDKManager.StartWhiteLabelSession((response) => {
                if (response.success) {
                    Debug.Log("Logged in");
                }
            });
            else mainMenuUIManager.popupLoginOrSignup.SetActive(true);
        });
    }

    public void SignupClicked() {
        LootLockerSDKManager.WhiteLabelSignUp(mainMenuUIManager.inputSignupEmail.text, mainMenuUIManager.inputSignupPassword.text, (response) => {
            if (response.success) {
                LootLockerSDKManager.WhiteLabelLogin(mainMenuUIManager.inputSignupEmail.text, mainMenuUIManager.inputSignupPassword.text, true, (response) => {
                    if (response.success) {
                        Debug.Log("Logged in");
                        LootLockerSDKManager.SetPlayerName(mainMenuUIManager.inputSignupUsername.text, (response) => {
                            if (response.success) Debug.Log("Username changed");
                        });
                    }
                });
            }
        });
    }

    public void LoginClicked() {
        LootLockerSDKManager.WhiteLabelLogin(mainMenuUIManager.inputLoginEmail.text, mainMenuUIManager.inputLoginPassword.text, true, (response) => {
            if (response.success) {
                Debug.Log("Logged in");
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

    

    public async Task<List<LootLockerCommonAsset>> GetAllAssets() {
        var assets = new List<LootLockerCommonAsset>();
        LootLockerSDKManager.GetAssetListWithCount(10000, (response) => {
            if (response.success) {
                for (int i = 0; i < response.assets.Length; i++) {
                    assets.Add(response.assets[i]);
                }
            }
            
        }, null, true);

        LootLockerSDKManager.ResetAssetCalls();
        await Task.Delay(1000);
        return assets;
    } 

    public void AddLevelsToContent(List<LootLockerCommonAsset> assets) {
        foreach (var asset in assets) {
            var item = Instantiate(prefabPublicLevel, contentPublicLevels.transform);

            var publicLevel = item.GetComponent<BoxPublicLevel>();

            publicLevel.text.text = asset.name;

            LootLockerFile[] levelImageFiles = asset.files;

            StartCoroutine(DownloadLevelImage(levelImageFiles[0].url.ToString(), publicLevel));

            publicLevel.levelURL = levelImageFiles[1].url.ToString();

            publicLevel.buttonPlay.onClick.AddListener(delegate () { StartCoroutine(DownloadLevelData(publicLevel)); });
        }
    }
    public async void SearchLevelByLevelname(string levelname) {
        var assets = await GetAllAssets();

        var filteredAssets = assets.Where(e => e.name == levelname).ToList();

        AddLevelsToContent(filteredAssets);
    }

    public async void ShowGlobalLevelsSortedByLikes() {

        var assets = await GetAllAssets();
        AddLevelsToContent(assets);
    }

    public void SearchLevelByID(string ID) {
        string[] ids = new string[] { ID };
        LootLockerSDKManager.GetAssetsById(ids, (response) => {
            if (response.success) {
                if (response.assets.Length != 1) {
                    //Nothing found
                    return;
                }

                var item = Instantiate(prefabPublicLevel, contentPublicLevels.transform);

                var publicLevel = item.GetComponent<BoxPublicLevel>();

                publicLevel.text.text = response.assets[0].name;

                LootLockerFile[] levelImageFiles = response.assets[0].files;

                StartCoroutine(DownloadLevelImage(levelImageFiles[0].url.ToString(), publicLevel));

                publicLevel.levelURL = levelImageFiles[1].url.ToString();

                publicLevel.buttonPlay.onClick.AddListener(delegate () { StartCoroutine(DownloadLevelData(publicLevel)); });
            }
        });
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

    }
}
