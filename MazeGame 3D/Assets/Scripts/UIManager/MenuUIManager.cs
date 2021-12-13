using UnityEngine;
using UnityEngine.UI;
public class MenuUIManager : MonoBehaviour
{
    /// <summary>
    /// 房间的根节点
    /// </summary>
    public Transform m_roomRoot = null;
    public Room RoomButton;

    public Text m_showPannel;

    // Start is called before the first frame update
    void Start()
    {
        if(m_roomRoot == null)
        {
            m_roomRoot = GameObject.Find("Room").GetComponent<Transform>();
            if (m_roomRoot == null) { Debug.Log("在场景中没有找到 Room Pannel 的根节点"); }
        }
    }
}
