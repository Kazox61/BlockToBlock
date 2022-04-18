using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class BoxMyLevel : MonoBehaviour {

    [HideInInspector] public string pathString;
    public Image image;
    [HideInInspector] public ScriptableLevel level;
    public TMP_Text text;
    public Button buttonEdit;
    public Button buttonPlay;
    public Button buttonUpload;
    public Button buttonDelete;
}
