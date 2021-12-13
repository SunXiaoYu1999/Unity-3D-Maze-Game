using TNet;
using UnityEngine;

public class ConnectToServer : TNBehaviour
{
    public StartSceneManager m_sceneManager;  

    private string  m_serverIpAddress;      // 服务器 IP 地址
    private int     m_serverPort;           // 服务器端口号
    private int     m_menuChannel;          // menu 的频道 ID

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
        Debug.Log("Awake！！");
    }

    /* 连接到服务器 */
    private void Start()
    {
        m_serverIpAddress = GlobalConfig.serverIpAddress;
        m_serverPort = GlobalConfig.serverPort;
        m_menuChannel = GlobalConfig.menuChannelID;
        if (m_sceneManager == null)
        {
            m_sceneManager = GameObject.FindObjectOfType<StartSceneManager>();
            if (m_sceneManager == null){Debug.Log("在场景中没有找到 StartScenenManager !!!");}
        }

        /* 连接到服务器 */
        TNManager.Connect(m_serverIpAddress, m_serverPort);
//        TNManager.Connect("119.29.12.176", 5127);
    }

    #region 回调函数
    public void OnError(string errString)
    {
        m_sceneManager.OnConnectError(errString);
    }

    public void OnConnect(bool success, string message)
    {
        if (success)
        {
            m_sceneManager.m_startCloseTrasition = true;
            m_sceneManager.m_connected = 1;

            m_sceneManager.LoadGameOriginData();
            TNManager.playerName = ConstVariable.playerName;

            /*进入大厅(大厅的 channel 固定为 21, 且人数默认上限 65535)，显示菜单 */
            Invoke("JoinChannel", m_sceneManager.m_transitionAnimationtime);
        }
        else
        {
            m_sceneManager.m_connected = -1;
        }
    }


    public void OnDisconnect()
    {

    }

    public void OnJoinChannel(int channelID, bool success, string message)
    {
        Debug.Log(" menu 频道加入成功");
    }

    public void OnLeaveChannel(int channelID)
    {

    }
    #endregion

    private void JoinChannel()
    {
        TNManager.JoinChannel(GlobalConfig.menuChannelID, "Menu", !GlobalConfig.menuLeaveDestory, GlobalConfig.PlayerLimit, null);
    }

    /* 注册网络消息回调函数 */
    protected void OnEnable()
    {
        TNManager.onError += OnError;
        TNManager.onConnect += OnConnect;
        TNManager.onDisconnect += OnDisconnect;
        TNManager.onJoinChannel += OnJoinChannel;
        TNManager.onLeaveChannel += OnLeaveChannel;
    }

    /* 注销网络消息回调函数 */
    protected void OnDisable()
    {
        TNManager.onError -= OnError;
        TNManager.onConnect -= OnConnect;
        TNManager.onDisconnect -= OnDisconnect;
        TNManager.onJoinChannel -= OnJoinChannel;
        TNManager.onLeaveChannel -= OnLeaveChannel;
    }

}
