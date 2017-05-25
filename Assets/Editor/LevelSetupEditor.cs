using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelManager))]
public class LevelSetupEditor : Editor {
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        LevelManager scriptLevelManager = (LevelManager) target;
        
    }
}
