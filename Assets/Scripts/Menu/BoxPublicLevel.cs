using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BoxPublicLevel : MonoBehaviour {
    public Image image;
    public TMP_Text text;
    public Button buttonLike, buttonDislike;

    public Image imageBGLike;
    public Image imageBGDislike;
    public Button buttonPlay;
    [HideInInspector] public int id;
    [HideInInspector] public ScriptableLevel level;
    [HideInInspector] public string levelURL;
    [HideInInspector] public VoteStatus voteStatus = VoteStatus.none;
}

public enum VoteStatus {
    none,
    like,
    dislike
}