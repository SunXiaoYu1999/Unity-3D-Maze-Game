using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GenMaze))]

public class GenMazeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        GenMaze mazeGen = (GenMaze)target;
        if (GUILayout.Button("生成迷宫"))
        {
            mazeGen.Create();
        }
        if (mazeGen.m_showGenBoundaryBox == true)
        {
            mazeGen.InitCreate();
            mazeGen.DrawBoundBox();
        }
        if (GUILayout.Button("清除迷宫"))
        {
            mazeGen.ClearExistChunk();
        }

    }


}
