using UnityEngine;

public class MazeManagement : MonoBehaviour
{
    private static MazeManagement m_instance = null;

    public static MazeManagement Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = MazeUtils.GetMazeManagementObject(true).GetComponent<MazeManagement>();
            }
            return m_instance;
        }
    }



}
