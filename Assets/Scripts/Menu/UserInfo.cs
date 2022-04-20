using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class UserInfo {
    public bool firstlogin;
    public int gold;
    public int totalstars;
    public Dictionary<int, int> levelInfos;
    public Dictionary<int, bool> votes;

    public UserInfo() {
        levelInfos = new Dictionary<int, int>();
        votes = new Dictionary<int, bool>();
    }

    public int GetLevelInfo(int levelIndex) {
        var info = levelInfos.TryGetValue(levelIndex, out var value);
        return value;
    }

    public bool HasCompletedLevel(int levelIndex) {
        if (levelInfos.TryGetValue(levelIndex, out var result)) {
            return true;
        }
        return false;
    }
}