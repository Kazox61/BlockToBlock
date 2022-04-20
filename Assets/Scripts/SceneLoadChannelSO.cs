using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName ="SceneLoadChannelSO", menuName = "ScriptableObjects/SceneLoadChannelSO")]
public class SceneLoadChannelSO : ScriptableObject {
    public UnityAction<int, bool> OnEventRaised;

    public bool log = false;

    public void RaiseEvent(int sceneReference, bool unloadOtherScenes) {
        if (log) Debug.Log($"Invoked {name}", this);
        if (OnEventRaised != null) OnEventRaised.Invoke(sceneReference, unloadOtherScenes);
        else Debug.LogWarning($"Evemt {name} was raised, but nobody is listening.", this);
    }
}
