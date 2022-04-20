using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ManagerAnchor", menuName = "ScriptableObjects/ManagerAnchor")]
public class ManagerAnchor : RuntimeAnchorBaseSO<ManagerData> {

}

public class ManagerData {
    public UserInfoManager userInfoManager;
    public LootLockerManager lootLockerManager;
}
