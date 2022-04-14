using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LootLocker.Requests;

public class LootLockerManager : MonoBehaviour {

    public void Login() {

        LootLockerSDKManager.StartGuestSession((response) => {
            if (response.success) {
                Debug.Log("Success");
            }
            else {
                Debug.Log("Failed");
            }
        });

    }
}
