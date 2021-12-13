using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GenPlane))]
public class GenPlaneEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        GenPlane mazeGen = (GenPlane)target;
        if (GUILayout.Button("生成地面"))
        {
            mazeGen.Create();
        }
        if (GUILayout.Button("清除地面"))
        {
            mazeGen.Clear();
        }
    }
}
