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

    /* 有玩家进入游戏终点 */
    private void OnTriggerEnter(Collider other)
    {
        /* 向所有玩家发送游戏结束，并且发送赢家的 player.ID */
        if(TNManager.isConnected && TNManager.IsInChannel(ConstVariable.curGamingChannel))
        {
            tno.Send("OnGameOver", Target.All, TNManager.player.id, TNManager.player.name);
            Debug.Log("正确发送了GameOver消息");
        }
    }

    [RFC]
    public void OnGameOver(int playerID, string playerName)
    {
        /* 设置当前游戏状态为不在游戏 */
        ConstVariable.isGaming = false;

        /* 删除当前游戏角色 */
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach(var player in players)
        {
            Destroy(player);
        }

        /* 显示得分面板 */
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
        

        /* 离开当前游戏频道 */
        TNManager.LeaveChannel(ConstVariable.curGamingChannel);
    }

    private void OnTriggerStay(Collider other)
    {
        
    }

    private void OnTriggerExit(Collider other)
    {
        
    }
}
