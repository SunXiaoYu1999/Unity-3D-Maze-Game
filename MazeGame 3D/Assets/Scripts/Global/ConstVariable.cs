public class ConstVariable
{
    public static string mazeManagement = "Maze Management";

    /* 在场景中要保存一个 Maze 游戏对象，保存 Maze 的所有地图*/
    public static string mazeGameobject = "Maze";

    /* maze gameobject 的所有墙体放在这个层下 */
    public static string mazeWall = "Wall";

    public static string mazeStart = "Start";

    public static string mazeEnd = "End";

    public static string mazePlane = "Floor";

    /* 当前玩家加入的房间频道 */
    public static int curRoomChannel = 0;

    /* 当前玩家加入的房间，开始游戏时进入的频道 */
    public static int curGamingChannel = 0;

    /* 玩家的用户名 */
    public static string playerName;

    /* 当前是否在游戏状态 */
    public static bool isGaming = false;

    /* 鼠标速度 */
    public static float mouseXSpeed;
    public static float mouseYSpeed;

    public static string winner;

}
