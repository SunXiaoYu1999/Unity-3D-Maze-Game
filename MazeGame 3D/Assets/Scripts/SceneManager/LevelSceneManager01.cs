using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TNet;

public class LevelSceneManager01 : TNBehaviour
{
    public LevelUIManager01 m_uiManager;
    public GameObject m_mazeGMO;
    public string m_playerProfabsPath;
    public GenMaze m_Generator;
    private void _initUiManager()
    {
        if (m_uiManager == null)
        {
            m_uiManager = GameObject.FindObjectOfType<LevelUIManager01>();
            if (m_uiManager == null)
            {
                Debug.Log("LevelUiManager 没找到！！！");
            }
        }
    }
    private void _initMazeGMO()
    {
        if (m_mazeGMO == null)
        {
            m_mazeGMO = GameObject.Find(ConstVariable.mazeGameobject);
            if (m_uiManager == null)
            {
                Debug.Log("LevelSceneManager01 初始化时候，m_mazeGMO 没找到！！！");
            }
        }
    }
    private void init()
    {
        _initUiManager();
        _initMazeGMO();
    }

    void Start()
    {
        /* 初始化变量 */
        init();

        /* 查询房间玩家信息并显示 */
        ShowPlayer();
    }

    private float m_updatePlayerNameTimeGap = 1.0f;
    private float m_updatePlayerNameNextTime = 0.0f;
    private void UpdatePlayerNamePannel()
    {
        if (m_updatePlayerNameNextTime < Time.time)
        {
            m_updatePlayerNameNextTime += m_updatePlayerNameTimeGap;
            ShowPlayer();
        }
    }
    /// <summary>
    /// 在房间列表上显示玩家信息
    /// </summary>
    public void ShowPlayer()
    {
        if (!m_uiManager.m_Menu.activeSelf) return;     // 没有选择菜单界面不进行显示

        /* 清空当前面板的信息 */
        for (int i = 0; i < 8; ++i)
        {
            m_uiManager.m_RoomPlayerNames.m_PlayerNames[i].text = "";
        }

        /* 拉取当前频道的玩家 */
        Channel curChannel = TNManager.GetChannel(ConstVariable.curRoomChannel);
        TNet.List<string> curPlayerList = new TNet.List<string>();

        /* 当前玩家不是房主 */
        if(TNManager.player.id != TNManager.GetHost(ConstVariable.curRoomChannel).id)
        {
            curPlayerList.Add(TNManager.player.name);
        }

        if (curChannel != null)
        {
            foreach (var player in curChannel.players)
            {
                if(player.id != TNManager.GetHost(ConstVariable.curRoomChannel).id)
                    curPlayerList.Add(player.name);
            }
            curPlayerList.Sort((string a, string b) => { return string.Compare(a, b, true); });
        }

        m_uiManager.m_RoomPlayerNames.m_PlayerNames[0].text = "(Host)" + TNManager.GetHost(ConstVariable.curRoomChannel).name;
        
        /* 显示玩家姓名 */
        for (int i = 0; curChannel != null &&  i < curPlayerList.Count; ++i)
        {
            m_uiManager.m_RoomPlayerNames.m_PlayerNames[i + 1].text = curPlayerList.buffer[i];
        }

    }

    private int m_randomSeed;
    public void OnStartGame()
    {
        m_randomSeed = (int)Time.time;

        /* 生成一个游戏频道 */
        ConstVariable.curGamingChannel = GetUseableChannelID(GlobalConfig.playerPlayGameChannelStart, GlobalConfig.playerPlayGameChannelEnd);

        /* 发送给房间内其他，我们需要 */
        if(TNManager.isConnected && TNManager.IsInChannel(ConstVariable.curRoomChannel))
        {
            tno.Send("StartGame", Target.All, ConstVariable.curGamingChannel, m_randomSeed);
        }
        else
        {
            Debug.Log("开始游戏失败，当前不处于连接状态或者不在房间频道内");
        }

        /* 禁用开始游戏按钮,不能重复开始游戏 */
        m_uiManager.m_btnStartGame.interactable = false;

    }

    [RFC]
    public void StartGame(int gameChannelID, int randomSeed)
    {
        /* 随机生成迷宫 */
        m_Generator.Create(randomSeed);

        /* 加入游戏频道 */
        TNManager.JoinChannel(gameChannelID, false, false);

        /* 订阅 加入成功回调 */
        TNManager.onJoinChannel += OnJoinChannel;

        /* 当前进入游戏状态 */
        ConstVariable.isGaming = true;

        /* 返回菜单 */
        if(m_uiManager.m_Menu.activeSelf)
            m_uiManager.m_btnMenu.onClick.Invoke();

        /* 客户端记录当前游戏频道 */
        ConstVariable.curGamingChannel = gameChannelID; 

    }

    /* 加入频道成功后，产生角色 */
    public void OnJoinChannel(int channelID, bool success, string message)
    {
        if(success)
        {
            Transform start = m_mazeGMO.transform.Find(ConstVariable.mazeStart);
            TNManager.Instantiate(channelID, "CreateAtPosition", m_playerProfabsPath, false, start.position, start.rotation);
            TNManager.onJoinChannel -= OnJoinChannel;
        }
        else
        {
            ConstVariable.isGaming = false;
            Debug.Log("加入到游戏频道" + ConstVariable.curGamingChannel + "失败,当前所在频道为" + channelID + ", ewrror message: " + message);
        }
    }


    [RCC]
    public GameObject CreateAtPosition(GameObject prefab, Vector3 pos, Quaternion rot)
    {
        // Instantiate the prefab
        GameObject go = prefab.Instantiate();

        // Set the position and rotation based on the passed values
        Transform t = go.transform;
        t.position = pos;
        t.rotation = rot;
        return go;
    }

    public void OnExit()
    {
        /* 退出到菜单 */
        TNManager.JoinChannel(GlobalConfig.menuChannelID, "Menu", !GlobalConfig.menuLeaveDestory, GlobalConfig.PlayerLimit, null,true);

        /* 游戏状态设置 */
        ConstVariable.isGaming = false;
    }

    public void Update()
    {
        /* 定时更新房间内玩家信息 */
        UpdatePlayerNamePannel();

        /* 定时更新当前房间 channel 信息 */
        UpdateChannelList();
    }


    float nextChannelListTime;
    private void UpdateChannelList()
    {
        if(nextChannelListTime < Time.time)
        {
            nextChannelListTime += 3f;
            TNManager.GetChannelList(GetServerList);
        }
    }
    private static TNet.List<Channel.Info> serverChannelList = new TNet.List<Channel.Info>();
    private static void GetServerList(TNet.List<Channel.Info> list)
    {
        serverChannelList.Clear();
        foreach (Channel.Info info in list)
        {
            serverChannelList.Add(info);
        }
    }
    public static int GetUseableChannelID(int start, int end)
    {
        int ret = -1;
        Dictionary<int, bool> hash = new Dictionary<int, bool>();
        if (TNManager.isConnected)
        {
            foreach (Channel.Info info in serverChannelList)
            {
                hash.Add(info.id, true);
            }
            for (int i = start; i <= end; ++i)
            {
                if (!hash.ContainsKey(i))
                {
                    ret = i;
                    break;
                }
            }
            hash.Clear();
        }
        return ret;
    }


    /* 结算面板返回房间按钮逻辑 */
    public void OnReturnRoom()
    {
        m_uiManager.m_gameEnd.SetActive(false);
        ConstVariable.isGaming = false;
    }
}
