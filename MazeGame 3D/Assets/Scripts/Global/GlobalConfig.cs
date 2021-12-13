using UnityEngine;

public class GlobalConfig 
{
    /// <summary>
    /// 服务器的 IP 地址
    /// </summary>
    public static string serverIpAddress = "119.29.12.176";

    /// <summary>
    /// 服务器开放的 UDP 端口
    /// </summary>
    public static int serverPort = 5127;

    /// <summary>
    /// 游戏大厅频道ID
    /// </summary>
    public static int menuChannelID = 21;

    /// <summary>
    /// 服务器玩家上限
    /// </summary>
    public static int PlayerLimit = 1000;

    /// <summary>
    /// 房间人数上限
    /// </summary>
    public static int RoomPlayerLimit = 8;

    /// <summary>
    /// 最后一个人离开大厅，服务器关闭大厅的频道
    /// </summary>
    public static bool menuLeaveDestory = true;

    /// <summary>
    /// 1000 以下的频道保留给程序使用，玩家开房间只使用 1000 以上的频道
    /// </summary>
    public static int playerChannelStart = 1000;
    /// <summary>
    /// 1000 以下的频道保留给程序使用，玩家开房间只使用 1000 以上的频道
    /// </summary>
    public static int playerChannelEnd = 1100;

    /// <summary>
    /// 玩家游戏的频道 开始
    /// </summary>
    public static int playerPlayGameChannelStart = 2000;
    /// <summary>
    /// 玩家游戏的频道 结束
    /// </summary>
    public static int playerPlayGameChannelEnd = 2100;

}
