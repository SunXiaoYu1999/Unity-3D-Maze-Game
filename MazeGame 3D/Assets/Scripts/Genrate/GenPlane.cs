using UnityEngine;

public class GenPlane : MonoBehaviour
{
    public GameObject m_plane;  /* 地面地砖预制件 */
    public float m_planeLength = 10.0f;
    public Transform m_center;  /* 地面创建中心点坐标（x-z平面） */
    public int m_xLength = 100; /* 地砖块数 */
    public int m_zLength = 100; /* 地砖块数 */

    private GameObject plane = null;
    private Vector3 m_leftTop;

    public void InitDate()
    {
        plane = MazeUtils.GetMazeFloorObject(true);
        m_leftTop.y = m_center.position.y;
        m_leftTop.x = m_center.position.x - (m_xLength / 2) * m_planeLength;
        m_leftTop.z = m_center.position.z + (m_zLength / 2) * m_planeLength;
    }
    public void Create()
    {
        InitDate();

        if (m_plane == null)
        {
            Debug.Log("没有地图预制件");
            return;
        }
        Vector3 position = new Vector3(0.0f, m_leftTop.y, 0.0f);
        for (int x = 0; x < m_xLength; ++x)
        {
            for (int z = 0; z < m_zLength; ++z)
            {
                position.x = m_leftTop.x + x * m_planeLength;
                position.z = m_leftTop.z - z * m_planeLength;
                Instantiate(m_plane, position, Quaternion.identity, plane.transform);
            }
        }
    }

    public void Clear()
    {
        if (plane == null)
            InitDate();
        Transform[] childs = plane.GetComponentsInChildren<Transform>();
        foreach (var transFM in childs)
        {
            DestroyImmediate(transFM.gameObject);
        }
    }
}
