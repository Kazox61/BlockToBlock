using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class UserInfo {
    public bool firstlogin;
    public int gold;
    public int totalstars;
    public Dictionary<int, int> levelInfos;

    public UserInfo() {
        levelInfos = new Dictionary<int, int>();
    }

    public int GetLevelInfo(int levelIndex) {
        var info = levelInfos.TryGetValue(levelIndex, out var value);
        return value;
    }
}