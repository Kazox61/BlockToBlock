using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelAnchor", menuName = "ScriptableObjects/LevelAnchor")]
public class LevelAnchor : RuntimeAnchorBaseSO<LevelTransferData> { 

}

public class LevelTransferData {
    public ScriptableLevel levelData;
    public PlayMode PlayMode;
    public int personalHighscore;
    public int levelIndex;
}
