using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName ="UserInfoChannelSO", menuName = "ScriptableObjects/UserInfoChannelSO")]
public class UserInfoChannelSO : ScriptableObject {

    public UnityAction<int, int> OnAddLevelCompletedEvent;

    public UnityAction OnGetLevelInfosEvent;


    public Dictionary<int, int> LevelInfos { get; set; }

    public void AddCompletedLevel(int levelIndex, int moves) {
        if (OnAddLevelCompletedEvent != null) {
            OnAddLevelCompletedEvent.Invoke(levelIndex, moves);
        }
    }

    public Dictionary<int, int> GetLevelInfos() {
        OnGetLevelInfosEvent.Invoke();
        return LevelInfos;
    }
}
