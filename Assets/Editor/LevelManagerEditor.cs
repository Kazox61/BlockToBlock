using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelHelper))]
public class LevelManagerEditor : Editor {

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        var script = (LevelHelper)target;

        if (GUILayout.Button("Save Map")) {
            script.GetLevelData();
        }

        if (GUILayout.Button("Clear Map")) {
            script.ClearLevel();
        }
        
        if (GUILayout.Button("Load Map")) {
            script.LoadLevel();
        }
        if (GUILayout.Button("Convert")) {
            script.ConvertLevelJsonToScriptableLevel();
        }
    }
}
