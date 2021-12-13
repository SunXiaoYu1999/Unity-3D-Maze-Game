using TNet;
using UnityEngine;

public class myTnetServer : TNEventReceiver
{
    public static myTnetServer instance = null;         /* 只有一个服务器实例 */

    public int m_port = 2222;
    public int m_tcpPort = 2223;
    public string m_ipAddress = "127.0.0.1";

    /* 网络连接回调函数 */
    protected override void OnConnect(bool success, string msg)
    {
        base.OnConnect(success, msg);
        if (success)
        {
            Debug.Log("连接成功!!!");
        }
    }

    /* 网络连接失败 */
    protected override void OnError(string msg)
    {
        base.OnError(msg);
        Debug.Log(msg);
    }


    protected override void OnPlayerJoin(int channelID, Player p)
    {
        base.OnPlayerJoin(channelID, p);
        Debug.Log($"新玩家 [{p.name}] 加入 [{channelID}] 频道");
    }

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //           TNManager.onPlayerJoin = OnPlayerJoin; // 多此一举？
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        if (TNServerInstance.Start(m_tcpPort, m_port, "SeverData.dat", false))
        {
            TNManager.Connect();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
