using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadListener : MonoBehaviour {
    
    public SceneLoadChannelSO loadSingleChannel;
    public List<int> loadedScenes;

    public void OnEnable() {
        loadSingleChannel.OnEventRaised += LoadSingleRequest;
    }

    private void LoadSingleRequest(int scene, bool unloadOtherScenes) {
        if (unloadOtherScenes) {
            foreach (var loadedScene in loadedScenes) {
                SceneManager.UnloadSceneAsync(loadedScene, UnloadSceneOptions.None);
            }
        }
        loadedScenes.Clear();
        loadedScenes.Add(scene);
        SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
    }
    
}
