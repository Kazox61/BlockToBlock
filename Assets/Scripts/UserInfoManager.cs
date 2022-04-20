using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using System.Linq;

public class UserInfoManager : MonoBehaviour {

    [SerializeField] private UserInfoChannelSO userInfoChannelSO;
    public UserInfo UserInfo { get; private set; }
    private string path = "";

    private float timer = 0;

    private void Awake() {
        userInfoChannelSO.OnAddLevelCompletedEvent += AddCompletedLevel;
        userInfoChannelSO.OnGetLevelInfosEvent += AddLevelInfos;
    }

    private void Start() {
        path = Path.Combine(Application.persistentDataPath, "userinfo.json");
        LoadUserInfo();
    }

    public void Update() {
        timer += Time.deltaTime;
        if (timer > 10) {
            timer -= 10;
            SaveUserInfo();
        }
    }

    private void AddCompletedLevel(int levelIndex, int moves) {
        if (UserInfo.levelInfos.TryGetValue(levelIndex, out var oldRecord)) {
            if (oldRecord > moves) {
                UserInfo.levelInfos[levelIndex] = moves;
                return;
            }

        }
        else {
            UserInfo.levelInfos.Add(levelIndex, moves);
        }
    }

    private void AddLevelInfos() {
        userInfoChannelSO.LevelInfos = UserInfo.levelInfos;
    }







    public void LoadUserInfo() {

        if (!File.Exists(path)) {
            UserInfo = new UserInfo();
            return;
        }

        using (StreamReader reader = new StreamReader(path)) {
            string data = reader.ReadToEnd();

            UserInfo = JsonConvert.DeserializeObject<UserInfo>(data);
        }
    }

    public void SaveUserInfo() {
        if (UserInfo == null) {
            LoadUserInfo();
            return;
        }

        FileStream fileStream = new FileStream(path, FileMode.Create);

        using (StreamWriter writer = new StreamWriter(fileStream)) {
            var json = JsonConvert.SerializeObject(UserInfo);
            writer.Write(json);
        }
    }
}
