using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TNet;

[RequireComponent(typeof(BoxCollider))]
public class MazeEnd : TNBehaviour 
{
    public LevelSceneManager01 m_sceneManager;

    private BoxCollider m_boxCollider;
    private void Start()
    {
        m_boxCollider = GetComponent<BoxCollider>();
        if (m_sceneManager == null)
            m_sceneManager = GameObject.Find("SceneManager").GetComponent<LevelSceneManager01>();
    }

    /* ����ҽ�����Ϸ�յ� */
    private void OnTriggerEnter(Collider other)
    {
        /* ��������ҷ�����Ϸ���������ҷ���Ӯ�ҵ� player.ID */
        if(TNManager.isConnected && TNManager.IsInChannel(ConstVariable.curGamingChannel))
        {
            tno.Send("OnGameOver", Target.All, TNManager.player.id, TNManager.player.name);
            Debug.Log("��ȷ������GameOver��Ϣ");
        }
    }

    [RFC]
    public void OnGameOver(int playerID, string playerName)
    {
        /* ���õ�ǰ��Ϸ״̬Ϊ������Ϸ */
        ConstVariable.isGaming = false;

        /* ɾ����ǰ��Ϸ��ɫ */
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach(var player in players)
        {
            Destroy(player);
        }

        /* ��ʾ�÷���� */
        if(TNManager.isConnected && TNManager.IsInChannel(ConstVariable.curGamingChannel) && playerID == TNManager.player.id)
        {
            GameEnd.isWin = true;
        }
        else
        {
            GameEnd.isWin = false;
        }
        ConstVariable.winner = "Winner\n" + playerName;
        m_sceneManager.m_uiManager.m_gameEnd.SetActive(true);
        

        /* �뿪��ǰ��ϷƵ�� */
        TNManager.LeaveChannel(ConstVariable.curGamingChannel);
    }

    private void OnTriggerStay(Collider other)
    {
        
    }

    private void OnTriggerExit(Collider other)
    {
        
    }
}
