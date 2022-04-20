using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SceneAnchor", menuName = "ScriptableObjects/SceneAnchor")]
public class SceneAnchor : RuntimeAnchorBaseSO<SceneData> {

}

public class SceneData {
    public MainMenuUIManager mainMenuUIManager;
    public PublicLevelsUIManager publicLevelsUIManager;
    public MessageManager messageManager;
    public GameObject contentPublicLevels;
}
