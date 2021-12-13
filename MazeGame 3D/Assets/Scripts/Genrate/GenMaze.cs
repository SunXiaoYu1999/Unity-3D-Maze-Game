using System.Collections.Generic;
using UnityEngine;

public class GenMaze : MonoBehaviour
{
    public GameObject m_mazeWall;                   /* 迷宫墙的预制件 */
    public float m_mazeWallLen = 1.0f;              /* 预制件的边长 */
    public Transform m_leftTopTransform;            /* 迷宫 (x * z) 左上坐标（所有预制件坐标与该变换组键的 y 对齐）(相对坐标) */
    public Transform m_rightBottomTransform;        /* 迷宫 (x * z) 右下坐标（相对坐标） */
    public int m_wallLength = 1;                    /* 迷宫墙的边长（预制件的边长为单位） */
    public int m_roadLength = 4;                    /* 迷宫路的边长（预制件的边长为单位） */
    public bool m_floatFixed = true;                /* 将迷宫坐标调整为整数防止浮点数问题 */
    public float m_floatRoundingOff = 0.0f;         /* float 舍入处理 */
    public bool m_showGenBoundaryBox = true;        /* 在 Scene 场景中显示生成区域 */

    /* 左上角坐标加上边长，创建迷宫时候的坐标 */
    private Vector3 m_leftTopPosition;            /* 迷宫 (x * z) 左上坐标（所有预制件坐标与该变换组键的 y 对齐）(世界坐标) */
    private Vector3 m_rightBottomPosition;        /* 迷宫 (x * z) 右下坐标（世界坐标） */
    private float m_left;
    private float m_top;
    private float m_walllen = 1.0f;
    private float m_roadlen = 1.0f;
    private int wallcountX;
    private int wallcountZ;


    private List<List<ChunkType>> mazeMap;

    private GameObject wall = null;
    private GameObject start = null;
    private GameObject end = null;

    public struct Chunk
    {
        public int x_index;
        public int z_index;
    };

    /* 生成迷宫时使用一个二维数组进行生成 */
    public enum ChunkType
    {
        Road = 0,
        Wall,
        GenWallFlag,    /* 在生成迷宫时，需要动态更新地图块的数值，在生成结束后再恢复至原始数据中 */
        GenRoadFlag,    /* 在生成迷宫时，需要动态更新地图块的数值，在生成结束后再恢复至原始数据中 */
        Null
    }

    private void _CalculateBoundary()
    {
        float left, right, top, bottom;
        left = m_leftTopPosition.x;
        right = m_rightBottomPosition.x;
        top = m_leftTopPosition.z;
        bottom = m_rightBottomPosition.z;

        float offsetX, offsetZ;
        float distanceLR = Mathf.Abs(left - right);
        float distanceTB = Mathf.Abs(top - bottom);
        m_walllen = m_wallLength * m_mazeWallLen;
        m_roadlen = m_roadLength * m_mazeWallLen;
        float len = m_walllen + m_roadlen;
        int countOfWallX = (int)((distanceLR - m_walllen) / len) * 2 + 1;
        int countOfWallZ = (int)((distanceTB - m_walllen) / len) * 2 + 1;

        offsetX = (distanceLR - ((countOfWallX / 2) * len + m_walllen)) / 2f;
        offsetZ = (distanceTB - ((countOfWallZ / 2) * len + m_walllen)) / 2f;
        wallcountX = countOfWallX;
        wallcountZ = countOfWallZ;

        if (m_floatFixed)
        {
            m_left = (float)((int)(Mathf.Min(left, right) + offsetX + m_floatRoundingOff));
            m_top = (float)((int)(Mathf.Max(top, bottom) - offsetZ - m_floatRoundingOff));
        }
        else
        {
            m_left = Mathf.Min(left, right) + offsetX + m_floatRoundingOff;
            m_top = Mathf.Max(top, bottom) - offsetZ - m_floatRoundingOff;
        }
    }

    public void Create()
    {
        if (m_mazeWall == null || m_leftTopPosition == null || m_rightBottomPosition == null)
            return;
        /* 计算前的初始化*/
        InitCreate();

        /* 清除之前存在的地图块 */
        ClearExistChunk();

        /* 在 Scene 中画出我们的生成框 */
        if (m_showGenBoundaryBox)
            DrawBoundBox();

        /* 初始化迷宫方格 */
        InitMazeChunk();

        /* 随机生成迷宫 */
        RandomGenChunk();

        /* 设定起点，终点 */
        SetStartEnd();

        /* 根据生成的迷宫，将迷宫墙对象创建出来 */
        DoCreateWall();
    }


    public void Create(int randomSeed)
    {
        if (m_mazeWall == null || m_leftTopPosition == null || m_rightBottomPosition == null)
            return;
        /* 计算前的初始化*/
        InitCreate();

        /* 清除之前存在的地图块 */
        ClearExistChunk();

        /* 在 Scene 中画出我们的生成框 */
        if (m_showGenBoundaryBox)
            DrawBoundBox();

        /* 初始化迷宫方格 */
        InitMazeChunk();

        /* 随机生成迷宫 */
        Random.InitState(randomSeed);
        RandomGenChunk();

        /* 设定起点，终点 */
        SetStartEnd();

        /* 根据生成的迷宫，将迷宫墙对象创建出来 */
        DoCreateWall();
    }

    public void SetStartEnd()
    {

    }


    /* 删除已经存在的地图块 */
    public void ClearExistChunk()
    {
        if (wall == null || start == null || end == null)
        {
            InitCreate();
        }

        Transform[] childs = wall.GetComponentsInChildren<Transform>();
        foreach (Transform tmp in childs)
        {
            if (tmp != null && tmp.gameObject != null)
            {
                DestroyImmediate(tmp.gameObject);
            }
        }
    }


#region 随机生成迷宫
    /* 随机生成一个合法的开始点 */
    private Chunk __gen_random_satrt__()
    {
        Chunk ret = new Chunk();
        ret.x_index = Random.Range(0, (wallcountX / 2)) * 2 + 1;
        ret.z_index = Random.Range(0, (wallcountZ / 2)) * 2 + 1;
        return ret;
    }
    /* 将 mazeMap 在 Chunk 处的 ChunkType 改变为 chunkType*/
    private void __change_chunktype__(Chunk chunk, ChunkType chunkType)
    {
        if (__valid_index__(chunk))
            mazeMap[chunk.x_index][chunk.z_index] = chunkType;
    }
    /* 返回 chunk 块在 mazeMap 中的 ChunkType */
    private ChunkType __get_chunktype__(Chunk chunk)
    {
        if (__valid_index__(chunk))
            return mazeMap[chunk.x_index][chunk.z_index];
        else
            return ChunkType.Null;
    }
    /* 将 road 周围的类型为 ChunkType.Wall 的 chunk 进队，并且更新 其类型为  ChunkType.GenWallFlag*/
    private bool __valid_index__(Chunk chunk)
    {
        bool ret = true;
        if (chunk.x_index < 0 || chunk.z_index < 0 || chunk.x_index >= wallcountX || chunk.z_index >= wallcountZ)
        {
            ret = false;
        }
        return ret;
    }
    /* 从 road 周围找到所有的 wall chunk，并将其 mazpMap 位置更新为 ChunkType.WallFlag */
    private void __find_all_wall_chunk_from__(Chunk road, List<Chunk> wallList)
    {
        int[,] dir = new int[,] { { -1, 0 }, { 1, 0 }, { 0, 1 }, { 0, -1 } };

        Chunk tmp = new Chunk();
        for (int i = 0; i < 4; ++i)
        {
            tmp.x_index = road.x_index + dir[i, 0];
            tmp.z_index = road.z_index + dir[i, 1];
            if (__valid_index__(tmp) && __get_chunktype__(tmp) == ChunkType.Wall)
            {
                __change_chunktype__(tmp, ChunkType.GenWallFlag);
                wallList.Add(tmp);
            }
        }
    }
    /* 从 wall 周围找类型为 ChunkType.Road 的 chunk, 若存在返回 true 并将找的结果保存进 curRoad , 若不存在返回 false */
    private bool __find_one_road_chunk_from__(Chunk wall, List<Chunk> wallList, ref Chunk curRoad)
    {
        bool ret = false;
        int[,] dir = new int[,] { { -1, 0 }, { 1, 0 }, { 0, 1 }, { 0, -1 } };

        Chunk tmp = new Chunk();
        for (int i = 0; i < 4 && ret == false; ++i)
        {
            tmp.x_index = wall.x_index + dir[i, 0];
            tmp.z_index = wall.z_index + dir[i, 1];
            if (__valid_index__(tmp) && __get_chunktype__(tmp) == ChunkType.Road)
            {
                curRoad = tmp;
                ret = true;
            }
        }
        return ret;
    }
    /* 随机从 wallList 中选取一个 wall */
    private Chunk __random_get_element__(List<Chunk> wallList)
    {
        Chunk wall;
        if (wallList.Count <= 0)
            return new Chunk();
        else
            wall = wallList[Random.Range(0, wallList.Count - 1)];
        return wall;
    }
    /* 从 wallList 中删除一个 wall */
    private void __delete_chunk_from__(Chunk wall, List<Chunk> wallList)
    {
        wallList.Remove(wall);
    }
    /* 随机生成一个迷宫,数据存于 mazeMap，具体算法过程如下:
     * 1. 定义一个队列 wallList
     * 2. 随机在地图上的 road 处生成一个开始点
     * 3. 将地图 (mazeMap) 上开始点的 ChunkType 更新为 ChunkType.GenRoadFlag, 定义两个变量 curRoad = start, curWall = null;
     * 4. 使用 do while()循环，当队列 循环结束条件为 队列 list 为空
     *      4.1 以 curRoad 为基准，将周围所有 ChunkType 为 Wall 的 Chunk 进队，并且更新 mazeMap 在墙处的 ChunkType 为 ChunkType.GenWallFlag
     *      4.2 从当前队列 wallList 中随机选取一个 Chunk 作为 curWall
     *      4.3 以 curWall 为基准，查找周围的所有 ChunkType 为 Road 的 Chunk，若找到则更新 mazeMap 在该 Road 处的 ChunkType 为 ChunkType.GenRoadFlag，没找到 继续执行下面
     *      4.3 从队列 wallList 中移除 curWall
     */
    public void RandomGenChunk()
    {
        List<Chunk> wallList = new List<Chunk>();
        Chunk start = __gen_random_satrt__();
        __change_chunktype__(start, ChunkType.GenRoadFlag);
        Chunk curRoad = start, curWall;
        do
        {
            __find_all_wall_chunk_from__(curRoad, wallList);
            curWall = __random_get_element__(wallList);
            if (__find_one_road_chunk_from__(curWall, wallList, ref curRoad) == true)
            {
                __change_chunktype__(curRoad, ChunkType.GenRoadFlag);
                __change_chunktype__(curWall, ChunkType.Road);  /* 大打通这个墙 */
            }
            __delete_chunk_from__(curWall, wallList);
        } while (wallList.Count > 0);

        /* 恢复刚刚更改的地图数据 */
        for (int i = 0; i < wallcountX; ++i)
        {
            for (int j = 0; j < wallcountZ; ++j)
            {
                switch (mazeMap[i][j])
                {
                    case ChunkType.GenRoadFlag:
                        mazeMap[i][j] = ChunkType.Road;
                        break;
                    case ChunkType.GenWallFlag:
                        mazeMap[i][j] = ChunkType.Wall;
                        break;
                }
            }
        }
    }
#endregion



    private void __create_road_chunk(int i, int j)
    {

    }
    private void __create_wall_chunk(int i, int j)
    {
        string wallName = "wall_" + i.ToString() + "_" + j.ToString();
        GameObject wallGMO = new GameObject(wallName);
        wallGMO.transform.parent = wall.transform;

        float len = m_roadlen + m_walllen;
        Vector3 thisOrigin = new Vector3();
        thisOrigin.x = m_left + (i / 2) * len + (i % 2) * (m_walllen);
        thisOrigin.z = m_top - (j / 2) * len - (j % 2) * (m_walllen);

        /* 世界坐标 */
        Vector3 position = new Vector3(0.0f, m_leftTopPosition.y, 0.0f);

        int x_len = (i % 2 == 0) ? (m_wallLength) : (m_roadLength);
        int z_len = (j % 2 == 0) ? (m_wallLength) : (m_roadLength);

        for (int x = 0; x < x_len; ++x)
        {
            for (int z = 0; z < z_len; ++z)
            {
                position.x = thisOrigin.x + x * m_mazeWallLen;
                position.z = thisOrigin.z - z * m_mazeWallLen;
                Instantiate(m_mazeWall, position, Quaternion.identity, wallGMO.transform);
            }
        }
    }

    /* 根据产生的 mazeMap 数据，创建地图块信息 */
    public void DoCreateWall()
    {
        if (wall == null || start == null || end == null)
        {
            InitCreate();
        }
        for (int i = 0; i < wallcountX; ++i)
        {
            for (int j = 0; j < wallcountZ; ++j)
            {
                switch (mazeMap[i][j])
                {
                    case ChunkType.Road:
                        __create_road_chunk(i, j);
                        break;
                    case ChunkType.Wall:
                        __create_wall_chunk(i, j);
                        break;
                }
            }
        }
    }


    /* 初始化 mazeMap  */
    public void InitMazeChunk()
    {
        /* 迷宫最大只能生成 2000 * 2000 的 */
        if (wallcountX > 2001)
        {
            wallcountX = 2001;
        }
        if (wallcountZ > 2001)
        {
            wallcountZ = 2001;
        }

        /* 初始化map */
        mazeMap = new List<List<ChunkType>>();
        for (int i = 0; i < wallcountX; ++i)
        {
            List<ChunkType> tmp = new List<ChunkType>();
            for (int j = 0; j < wallcountZ; ++j)
            {
                if (i % 2 == 1 && j % 2 == 1)
                {
                    tmp.Add(ChunkType.Road);
                }
                else
                {
                    tmp.Add(ChunkType.Wall);
                }
            }
            mazeMap.Add(tmp);
        }
    }


    /* 画出待生成区域的边界 */
    public void DrawBoundBox()
    {
#if UNITY_EDITOR
        List<Vector3> position = new List<Vector3>();
        int xChunkCount = (wallcountX / 2) * (m_wallLength + m_roadLength) + m_wallLength;
        int zChunkCount = (wallcountZ / 2) * (m_wallLength + m_roadLength) + m_wallLength;
        position.Add(new Vector3(m_left, m_leftTopPosition.y, m_top));
        position.Add(new Vector3(m_left + xChunkCount * m_mazeWallLen, m_leftTopPosition.y, m_top));
        position.Add(new Vector3(m_left, m_leftTopPosition.y, m_top - zChunkCount * m_mazeWallLen));
        position.Add(new Vector3(m_left + xChunkCount * m_mazeWallLen, m_leftTopPosition.y, m_top - zChunkCount * m_mazeWallLen));
        position.Add(new Vector3(m_left, m_leftTopPosition.y + 2.0f, m_top));
        position.Add(new Vector3(m_left + xChunkCount * m_mazeWallLen, m_leftTopPosition.y + 2.0f, m_top));
        position.Add(new Vector3(m_left, m_leftTopPosition.y + 2.0f, m_top - zChunkCount * m_mazeWallLen));
        position.Add(new Vector3(m_left + xChunkCount * m_mazeWallLen, m_leftTopPosition.y + 2.0f, m_top - zChunkCount * m_mazeWallLen));

        Debug.DrawLine(position[0], position[1], Color.white);
        Debug.DrawLine(position[0], position[2], Color.white);
        Debug.DrawLine(position[3], position[1], Color.white);
        Debug.DrawLine(position[3], position[2], Color.white);
        Debug.DrawLine(position[0], position[4], Color.white);
        Debug.DrawLine(position[1], position[5], Color.white);
        Debug.DrawLine(position[2], position[6], Color.white);
        Debug.DrawLine(position[3], position[7], Color.white);
        Debug.DrawLine(position[4], position[5], Color.white);
        Debug.DrawLine(position[4], position[6], Color.white);
        Debug.DrawLine(position[7], position[5], Color.white);
        Debug.DrawLine(position[7], position[6], Color.white);
#endif
    }

    /* 将坐标转换成世界坐标 */
    private void __coordinate_transformation()
    {
        m_leftTopPosition = m_leftTopTransform.position;
        m_rightBottomPosition = m_rightBottomTransform.position;
    }
    public void InitCreate()
    {
        /* 坐标转换到世界坐标 */
        __coordinate_transformation();

        /* 计算边界 */
        _CalculateBoundary();

        /* 找到墙，起点、终点的引用 */
        wall = MazeUtils.GetMazeWallObject();
        start = MazeUtils.GetMazeStartObject();
        end = MazeUtils.GetMazeEndObject();
    }


    private void Start()
    {
        ///* 该组件只在编辑迷宫地图时候使用，游戏开始时应当销毁 */
        //if (Application.isPlaying)
        //{
        //    Destroy(this.gameObject);
        //}
    }
}
