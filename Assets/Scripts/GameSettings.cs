using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "ScriptableObjects/GameSettings")]
public class GameSettings : ScriptableObject {
    public bool mobile = true;


    public void SetMobile(bool result) {
        mobile = result;
    }
}
