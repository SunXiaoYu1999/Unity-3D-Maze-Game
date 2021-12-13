using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TNet;
using UnityEngine.UI;


public class LearnSceneManager : TNBehaviour
{
    public Text m_show;
    public Transform m_createPos;

    private bool m_create = false;

    public int m_gameChannel = 25;
    public int m_roomChannel = 65;


    /* 发送给房间内其它玩家，加入新的频道，并且创建一个新的物体 */
    public void OnBtnCreate()
    {
        if(!TNManager.isConnected)
        {
            m_show.text += "执行 OnBtnCreate() 时，与服务器没有连接\n";
        }
        if(TNManager.IsInChannel(m_roomChannel) && TNManager.IsInChannel(tno.channelID))
            tno.Send("JoinAndCreate", Target.All, m_gameChannel, m_createPos.position, m_createPos.rotation);
    }

    [RFC]
    public void JoinAndCreate(int channelID, Vector3 position, Quaternion rotation)
    {
        if(!TNManager.isConnected)
        {
            m_show.text += "执行 JoinAndCreate RFC 时，不处于连接状态\n";
            return;
        }
        TNManager.JoinChannel(channelID, false, false);
        //        tno.channelID = channelID;

        TNManager.onJoinChannel += OnJoinChannel;

        //m_create = true;
    }

    //private void Update()
    //{
    //    if(m_create)
    //    {
    //        if (TNManager.isConnected && !TNManager.IsInChannel(m_gameChannel))
    //        {
    //            m_show.text += "当前不在频道 " + m_gameChannel + " 中, 创建失败\n"; 
    //            return;
    //        }

    //        TNManager.Instantiate(m_gameChannel, "CreateAt", "Car", false, m_createPos.position, m_createPos.rotation);

    //        m_create = false;
    //    }
    //}

    [RCC]
    public static GameObject CreateAt(GameObject profabs, Vector3 position, Quaternion rotation)
    {
        GameObject go = profabs.Instantiate();
        go.transform.position = position;
        go.transform.rotation = rotation;
        return go;
    }

    public void OnJoinChannel(int channnelID, bool success, string message)
    {
        if (success && channnelID == m_gameChannel)    // 正确的加入到当前频道了
        {
            TNManager.Instantiate(channnelID, "CreateAt", "Car", false, m_createPos.position, m_createPos.rotation);

            TNManager.onJoinChannel -= OnJoinChannel;
        }
        else
        {
            m_show.text += "加入到游戏频道 " + m_gameChannel + " 出错，error message :" + message;
        }
    }
    public void OnExit()
    {
        Application.Quit();
    }
}
