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


    /* ���͸�������������ң������µ�Ƶ�������Ҵ���һ���µ����� */
    public void OnBtnCreate()
    {
        if(!TNManager.isConnected)
        {
            m_show.text += "ִ�� OnBtnCreate() ʱ���������û������\n";
        }
        if(TNManager.IsInChannel(m_roomChannel) && TNManager.IsInChannel(tno.channelID))
            tno.Send("JoinAndCreate", Target.All, m_gameChannel, m_createPos.position, m_createPos.rotation);
    }

    [RFC]
    public void JoinAndCreate(int channelID, Vector3 position, Quaternion rotation)
    {
        if(!TNManager.isConnected)
        {
            m_show.text += "ִ�� JoinAndCreate RFC ʱ������������״̬\n";
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
    //            m_show.text += "��ǰ����Ƶ�� " + m_gameChannel + " ��, ����ʧ��\n"; 
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
        if (success && channnelID == m_gameChannel)    // ��ȷ�ļ��뵽��ǰƵ����
        {
            TNManager.Instantiate(channnelID, "CreateAt", "Car", false, m_createPos.position, m_createPos.rotation);

            TNManager.onJoinChannel -= OnJoinChannel;
        }
        else
        {
            m_show.text += "���뵽��ϷƵ�� " + m_gameChannel + " ����error message :" + message;
        }
    }
    public void OnExit()
    {
        Application.Quit();
    }
}
