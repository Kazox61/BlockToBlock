using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using LootLocker.Requests;
using TMPro;
using System;
using UnityEngine.Networking;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class LootLockerManager : MonoBehaviour {
    [HideInInspector] public MainMenuUIManager mainMenuUIManager;
    [HideInInspector] public PublicLevelsUIManager publicLevelsUIManager;
    [HideInInspector] public MessageManager messageManager;
    [SerializeField] private UserInfoManager userInfoManager;
    public GameObject prefabPublicLevel;
    [HideInInspector] public GameObject contentPublicLevels;
    [SerializeField] private SceneAnchor sceneAnchor;
    [SerializeField] private ManagerAnchor managerAnchor;
    [SerializeField] private SceneLoadChannelSO sceneLoadChannelSO;

    [SerializeField] private LevelAnchor levelAnchor;

    public Sprite spriteNoVote, spriteVoteLike, spriteVoteDislike;

    public void OnEnable() {
        sceneAnchor.OnEventRaised += SetData;
        managerAnchor.Item = new ManagerData() {
            lootLockerManager = this,
            userInfoManager = userInfoManager
        };
    }

    public void Awake() {
        sceneLoadChannelSO.RaiseEvent(1, false);
    }

    public void SetData(SceneData data) {
        mainMenuUIManager = data.mainMenuUIManager;
        publicLevelsUIManager = data.publicLevelsUIManager;
        messageManager = data.messageManager;
        contentPublicLevels = data.contentPublicLevels;
    }

    #region Login/Register
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

    public void Signup() {
        LootLockerSDKManager.WhiteLabelSignUp(mainMenuUIManager.inputSignupEmail.text, mainMenuUIManager.inputSignupPassword.text, (response) => {
            if (response.success) {
                LootLockerSDKManager.WhiteLabelLogin(mainMenuUIManager.inputSignupEmail.text, mainMenuUIManager.inputSignupPassword.text, true, (response) => {
                    if (response.success) {
                        Debug.Log("Logged in");
                        LootLockerSDKManager.SetPlayerName(mainMenuUIManager.inputSignupUsername.text, (response) => {
                            mainMenuUIManager.popupSignup.SetActive(false);
                        });
                    }
                });
            }
        });
    }

    public void Login() {
        LootLockerSDKManager.WhiteLabelLogin(mainMenuUIManager.inputLoginEmail.text, mainMenuUIManager.inputLoginPassword.text, true, (response) => {
            if (response.success) {
                mainMenuUIManager.popupLogin.SetActive(false);
            }
        });
    }
    #endregion

    #region Upload Data
    public void UploadLevel(string levelName, string screenshotFilePath, string levelFilePath) {
        LootLockerSDKManager.CreatingAnAssetCandidate(levelName, (response) => {
            if (response.success) {
                UploadLevelData(response.asset_candidate_id, levelName, screenshotFilePath, levelFilePath);
            }
        });
        
    }

    public void UploadLevelData(int levelID, string levelName, string screenshotFilePath, string levelFilePath) {

        LootLocker.LootLockerEnums.FilePurpose screenshotFileType = LootLocker.LootLockerEnums.FilePurpose.primary_thumbnail;

        Dictionary<string, string> votes = new Dictionary<string, string>();
        votes.Add("counterVotesLike", "0");
        votes.Add("counterVotesDislike", "0");


        LootLockerSDKManager.AddingFilesToAssetCandidates(levelID, screenshotFilePath, levelName, screenshotFileType, (screenshotResponse) => {
            if (screenshotResponse.success) {
                LootLocker.LootLockerEnums.FilePurpose textFileType = LootLocker.LootLockerEnums.FilePurpose.file;
                LootLockerSDKManager.AddingFilesToAssetCandidates(levelID, levelFilePath, levelName, textFileType, (fileResponse) => {
                    if (fileResponse.success) {
                        LootLockerSDKManager.UpdatingAnAssetCandidate(levelID, true, (finishedResponse) => {
                            GUIUtility.systemCopyBuffer = finishedResponse.asset_candidate.asset_id.ToString();
                            messageManager.SpawnMessage(8, "Copied LevelID to clipboard!");
                        }, null, votes);
                    }
                });
            }
        });
    }
    #endregion

    #region Download and Sort Assets
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

            publicLevel.id = asset.id;

            LootLockerFile[] levelImageFiles = asset.files;

            StartCoroutine(DownloadLevelImage(levelImageFiles[0].url.ToString(), publicLevel));

            publicLevel.levelURL = levelImageFiles[1].url.ToString();

            publicLevel.buttonLike.onClick.AddListener(delegate () { publicLevelsUIManager.VoteLike(publicLevel); });

            publicLevel.buttonDislike.onClick.AddListener(delegate () { publicLevelsUIManager.VoteDislike(publicLevel); });

            publicLevel.buttonPlay.onClick.AddListener(delegate () { StartCoroutine(DownloadLevelData(publicLevel)); });

            if (userInfoManager.UserInfo.votes.TryGetValue(asset.id, out var voteInfo)) {
                if (voteInfo) {
                    publicLevel.imageBGLike.sprite = spriteVoteLike;
                    publicLevel.imageBGDislike.sprite = spriteNoVote;
                    publicLevel.voteStatus = VoteStatus.like;
                }
                else {
                    publicLevel.imageBGDislike.sprite = spriteVoteDislike;
                    publicLevel.imageBGLike.sprite = spriteNoVote;
                    publicLevel.voteStatus = VoteStatus.dislike;
                }
            }
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
                foreach (var x in response.assets[0].storage) {
                    Debug.Log($"{x.key} / {x.value}");
                }
                AddLevelsToContent(response.assets.ToList());
            }
        });
        
    }

    public void UploadVotesToAsset(BoxPublicLevel box) {
        string[] ids = new string[] { box.id.ToString() };
        var counterLikes = 0;
        var counterDislikes = 0;
        LootLockerSDKManager.GetAssetsById(ids, (response) => {
            if (response.success) {
                if (response.assets.Length != 1) {
                    Debug.Log("Something went wrong!!!!");
                    return;
                }
                counterLikes = int.Parse(response.assets[0].storage.Where(x => x.key == "counterVotesLike").FirstOrDefault().value);
                counterDislikes = int.Parse(response.assets[0].storage.Where(x => x.key == "counterVotesDislike").FirstOrDefault().value);
                Debug.Log(counterLikes);
                Debug.Log(counterDislikes);
                Debug.Log(box.id);

                Dictionary<string, string> votes = new Dictionary<string, string>();
                votes.Add("counterVotesLike", (counterLikes+1).ToString());

                LootLockerSDKManager.UpdateOneOrMoreKeyValuePairForAssetInstances(box.id, votes, (response) => {

                });
            }
        });
        
        
        
    }

    #endregion

    #region Download Data
    IEnumerator DownloadLevelImage(string url, BoxPublicLevel publicLevel) {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);

        yield return www.SendWebRequest();

        Texture2D loadedImage = DownloadHandlerTexture.GetContent(www);

        publicLevel.image.sprite = Sprite.Create(loadedImage, new Rect(0, 0, loadedImage.width, loadedImage.height), Vector2.zero);

    }


    IEnumerator DownloadLevelData(BoxPublicLevel publicLevel) {
        UnityWebRequest www = UnityWebRequest.Get(publicLevel.levelURL);

        yield return www.SendWebRequest();

        var level = JsonConvert.DeserializeObject<Level>(www.downloadHandler.text);

        //yield return new WaitForSeconds(1f);

        var transferData = new LevelTransferData() { levelData = level.ToScriptableLevel(), PlayMode = PlayMode.customizedMode };
        levelAnchor.Item = transferData;

        sceneLoadChannelSO.RaiseEvent(3, true);

    }
    #endregion
}
