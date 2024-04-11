using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[CustomEditor(typeof(ObjectInstantiatorAbstract), true)]

public class ProceduralGeneratorEditor : Editor
{
    ObjectInstantiatorAbstract generator;

    private void Awake()
    {
        generator = (ObjectInstantiatorAbstract)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Generate Objects"))
        {
            generator.GenerateDungeon();
        }
    }
}
