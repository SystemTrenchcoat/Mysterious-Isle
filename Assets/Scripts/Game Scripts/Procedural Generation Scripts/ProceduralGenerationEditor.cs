using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[CustomEditor(typeof(ObjectInstantiatorAbstract), true)]

public class RandomDungeonGeneratorEditor : Editor
{
    ObjectInstantiatorAbstract generator;

    private void Awake()
    {
        generator = (ObjectInstantiatorAbstract)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Generate Enemies"))
        {
            generator.GenerateDungeon();
        }
    }
}
