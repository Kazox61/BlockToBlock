using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInfo {
    public bool firstlogin;
    public int gold;
    public int totalstars;
    public List<LevelInfo> levelInfos;
}

public class LevelInfo {
    public int levelIndex;
    public bool completed;
    public int minimumMoves;
}
