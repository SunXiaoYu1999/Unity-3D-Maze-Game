using UnityEngine;
using TNet;
using System.Collections.Generic;

public class MazeUtils : MonoBehaviour
{
    
    public static GameObject GetMazeManagementObject(bool create = true)
    {
        GameObject gamObj = null;
        gamObj = GameObject.Find(ConstVariable.mazeManagement);
        if (gamObj == null && create == true)
        {
            gamObj = new GameObject(ConstVariable.mazeManagement);
            gamObj.AddComponent<MazeManagement>();
        }
        return gamObj;
    }

    public static GameObject GetMazeFloorObject(bool create = true)
    {
        GameObject gamObj = null;
        gamObj = GameObject.Find(ConstVariable.mazePlane);
        if (gamObj == null && create == true)
        {
            gamObj = new GameObject(ConstVariable.mazePlane);
            gamObj.transform.position = new Vector3(0f, 0f, 0f);
        }
        return gamObj;
    }

    public static GameObject GetMazeObject(bool create = true)
    {
        GameObject gamObj = null;
        gamObj = GameObject.Find(ConstVariable.mazeGameobject);
        if (gamObj == null && create == true)
        {
            gamObj = new GameObject(ConstVariable.mazeGameobject);
        }
        return gamObj;
    }

    public static GameObject GetMazeWallObject(bool create = true)
    {
        GameObject gamObj = null;
        gamObj = GameObject.Find(ConstVariable.mazeWall);
        if (gamObj == null && create == true)
        {
            GameObject maze = GetMazeObject(true);
            gamObj = new GameObject(ConstVariable.mazeWall);
            gamObj.transform.parent = maze.transform;
        }
        return gamObj;
    }

    public static GameObject GetMazeStartObject(bool create = true)
    {
        GameObject gamObj = null;
        gamObj = GameObject.Find(ConstVariable.mazeStart);
        if (gamObj == null && create == true)
        {
            GameObject maze = GetMazeObject(true);
            gamObj = new GameObject(ConstVariable.mazeStart);
            gamObj.transform.parent = maze.transform;
        }
        return gamObj;
    }

    public static GameObject GetMazeEndObject(bool create = true)
    {
        GameObject gamObj = null;
        gamObj = GameObject.Find(ConstVariable.mazeEnd);
        if (gamObj == null && create == true)
        {
            GameObject maze = GetMazeObject(true);
            gamObj = new GameObject(ConstVariable.mazeEnd);
            gamObj.transform.parent = maze.transform;
        }
        return gamObj;
    }
    
    private static TNet.List<Channel.Info> serverList = new TNet.List<Channel.Info>();
    private static void GetServerList(TNet.List<Channel.Info> list)
    {
        foreach (Channel.Info info in list)
        {
            serverList.Add(info);
        }
    }
    public static int GetUseableChannelID(int start, int end)
    {
        int ret = -1;
        Dictionary<int, bool> hash = new Dictionary<int, bool>();
        if (TNManager.isConnected)
        {
            TNManager.GetChannelList(GetServerList);
            Debug.Log(serverList.Count);
            foreach(Channel.Info info in serverList)
            {
                hash.Add(info.id, true);
            }
            for(int i = start; i<= end; ++i)
            {
                if(!hash.ContainsKey(i))
                {
                    ret = i;
                    break;
                }
            }
            serverList.Clear();
            hash.Clear();
        }
        return ret;
    }
}
