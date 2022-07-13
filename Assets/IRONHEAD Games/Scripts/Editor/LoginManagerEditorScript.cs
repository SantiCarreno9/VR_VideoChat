using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LoginManager))]
public class LoginManagerEditorScript : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        EditorGUILayout.HelpBox("This script is responsible for connecting to Photon Server.", MessageType.Info);

        LoginManager loginManager = target as LoginManager;
        if (GUILayout.Button("Connect Anonymously"))
        {
            loginManager.ConnectAnonymously();
        }

        if (GUILayout.Button("Connect"))
        {
            loginManager.ConnectToGame();
        }

    }
}