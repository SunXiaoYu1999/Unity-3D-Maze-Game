using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GenPlane))]
public class GenPlaneEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        GenPlane mazeGen = (GenPlane)target;
        if (GUILayout.Button("���ɵ���"))
        {
            mazeGen.Create();
        }
        if (GUILayout.Button("�������"))
        {
            mazeGen.Clear();
        }
    }
}
