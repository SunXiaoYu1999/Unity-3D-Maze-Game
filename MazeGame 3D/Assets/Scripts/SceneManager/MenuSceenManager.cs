using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TNet;

public class MenuSceenManager : TNBehaviour
{
    /// <summary>
    /// 房间信息更新的时间间隔
    /// </summary>
    public float m_roomUndateTimeGap = 2.0f;

    public Transform m_roomRoot = null;

    public MenuUIManager m_uiManager = null;

    private int curAttemptJoinChannel;
    private void OnEnable()
    {
        TNManager.onJoinChannel += OnJoinChannel;
    }
    private void OnDisable()
    {
        TNManager.onJoinChannel -= OnJoinChannel;
    }


    private void Start()
    {
        if(m_uiManager == null)
        {
            m_uiManager = GameObject.FindObjectOfType<MenuUIManager>();
            if (m_uiManager == null) { Debug.Log("Menu 场景 MenuUIManager 组件缺失！！！"); }
        }
        if(m_uiManager != null)
        {
            if(m_roomRoot == null)
            {
                m_roomRoot = m_uiManager.m_roomRoot;
            }
        }
    }

    /* 创建房间 */
    public void OnCreateRoom()
    {
        /* 拉取服务器房间列表 */ // 不需要，因为已经定时拉取了
//        TNManager.GetChannelList(CopyServerChannelList);

        Dictionary<int, bool> tmpHash = new Dictionary<int, bool>();

        int roomNumber = GlobalConfig.playerChannelStart-1;
        /* 产生未分配的房间号 */
        foreach (var channel in roomChannels)
        {
            tmpHash.Add(channel.id, true);
        }
        for(int i = GlobalConfig.playerChannelStart; i <= GlobalConfig.playerChannelEnd; ++i)
        {
            if(!tmpHash.ContainsKey(i))
            {
                roomNumber = i;
                break;
            }
        }
        if (roomNumber == GlobalConfig.playerChannelStart - 1)
        {
            Debug.Log("创建房间失败，房间数量已经超过上限");
            return;
        }

        ConstVariable.curRoomChannel = roomNumber;

        Room tmp = DoCreateRoom(roomNumber, true, 1, GlobalConfig.RoomPlayerLimit,"Level_01",false);
        roomList.Add(roomNumber, tmp);
        /* 加入房间 */
        EnterTheRoom(tmp);
    }

    private float targetTimeRoomUpdate = 0f;
    private void Update()
    {
        /* 定时更新房间信息 */
        if (targetTimeRoomUpdate < Time.time)
        {
            targetTimeRoomUpdate += m_roomUndateTimeGap;
            UpdateRoom();
        }

    }
   
    private Dictionary<int, Room> roomList = new Dictionary<int, Room>();
    private TNet.List<Channel.Info> roomChannels = new TNet.List<Channel.Info>();
    private void UpdateRoom()
    {

        /* 查询并更新服务器所有房间 */
        TNManager.GetChannelList(CopyServerChannelList);

        /* 查询并更新所有房间的玩家数量，以及房间玩家数量上限 */
        foreach(var room in roomList){ room.Value.alive = false;  }   // 本地房间是否存在全部更新为 false
        foreach (var channel in roomChannels) { 
            if(roomList.ContainsKey(channel.id))  // channel ID 依然存在
            {
                roomList[channel.id].m_curPlayer = channel.players;
                roomList[channel.id].m_levelName = channel.level;
                roomList[channel.id].m_maxPlayer = channel.limit;
                roomList[channel.id].m_text.text = "房间号: " + channel.id + "\n玩家: " + channel.players.ToString() + "/" + channel.limit.ToString();
                roomList[channel.id].alive = true;
            }
            else                                // 服务器存在 channel, 但是本地不存在该 channel
            {
                // 只显示玩家频道上的 频道
                if (channel.id >= GlobalConfig.playerChannelStart && channel.id <= GlobalConfig.playerChannelEnd)
                {
                    Room newRoom = DoCreateRoom(channel, true);     // 创建一个新的房间
                    roomList.Add(channel.id, newRoom);              // 加入本地房间队列

                }
            }
        }

        /* 再次遍历队列，清理不存在的 room */
        foreach (var room in roomList) 
        { 
            if(room.Value.alive == false)
            {
                if(room.Value != null)
                    Destroy(room.Value.gameObject);
                roomList.Remove(room.Key);
            }
        }
    }

    public Room DoCreateRoom(Channel.Info info, bool alive)
    {
        return DoCreateRoom(info.id, alive, info.players, info.limit, info.level, info.hasPassword);
    }

    public Room DoCreateRoom(int channelID, bool alive,int curPlayer, int maxPlayer, string levelName, bool hasPassWord)
    {
        var room = Instantiate(m_uiManager.RoomButton, m_uiManager.m_roomRoot);
        room.gameObject.SetActive(true);
        //GameObject buttonGmo = new GameObject();
        //GameObject textGmo = new GameObject();
        //buttonGmo.AddComponent<Room>();
        //textGmo.AddComponent<Text>();
        //Room tmp = buttonGmo.GetComponent<Room>();


        /* 设置属性 */
        //roomButton.m_text = textGmo.GetComponent<Text>();
        room.m_channelID = channelID;
        room.alive = alive;
        room.m_curPlayer = curPlayer;
        room.m_maxPlayer = maxPlayer;
        room.m_text.text = "房间号: " + channelID + "\n玩家: " + curPlayer.ToString() + "/" + maxPlayer.ToString();
        room.m_levelName = levelName;
        room.m_hasPossWord = hasPassWord;
     
        /* 添加房间按钮的事件绑定 */
        EnterRoomButtonOnClick onClick = new EnterRoomButtonOnClick(EnterTheRoom);
        room.onClick.AddListener(() => { 
            if(onClick != null)
            {
                onClick(room);
            }
        });


        //textGmo.transform.parent = buttonGmo.transform;

        /* 设置房间处于正确的 pannel 上显示 */
        if (m_roomRoot == null)
        {
            Debug.Log("房间跟节点为空！！！");
        }
        else
        {
           // buttonGmo.transform.parent = m_roomRoot;
        }

        return room;
    }

    public delegate void EnterRoomButtonOnClick(Room room);

    /* 进入房间逻辑 */
    public void EnterTheRoom(Room room)    
    {
        ConstVariable.curRoomChannel = room.m_channelID;
        curAttemptJoinChannel = room.m_channelID;
        TNManager.JoinChannel(room.m_channelID, room.m_levelName, false, room.m_maxPlayer, null);
    }

    private void CopyServerChannelList(TNet.List<Channel.Info> serverChannelList)
    {
        roomChannels = serverChannelList;
    }

    public void OnExit()
    {
        /* 保存游戏数据 */
        SavePlayerData();

        Application.Quit();
    }

    /// <summary>
    /// 保存用户的数据
    /// </summary>
    public void  SavePlayerData()
    {
        Debug.Log("保存游戏数据");
    }

    public void OnJoinChannel(int channelID, bool success, string message)
    {
        if(channelID == curAttemptJoinChannel && success == false)
        {
            m_uiManager.m_showPannel.text = "加入房间失败\n" + message;
        }
    }
}
