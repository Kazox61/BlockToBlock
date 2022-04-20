using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageManager : MonoBehaviour {

    [SerializeField] private GameObject messageBoxPrefab;

    public void SpawnMessage(float duration, string text) {
        var messageGO = Instantiate(messageBoxPrefab, transform);

        var messageBox = messageGO.GetComponent<MessageBox>();
        messageBox.timeTillRemove = duration;
        messageBox.textError.text = text;
    }


}
