using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MessageBox : MonoBehaviour {
    public TMP_Text textError;

    private float timer;
    public float timeTillRemove = 10;
    private void Update() {
        timer += Time.deltaTime;

        if (timer > timeTillRemove) {
            Destroy(gameObject);
        }
    }
}
