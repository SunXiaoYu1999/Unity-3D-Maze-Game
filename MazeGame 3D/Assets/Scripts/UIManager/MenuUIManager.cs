using UnityEngine;
using UnityEngine.UI;
public class MenuUIManager : MonoBehaviour
{
    /// <summary>
    /// ����ĸ��ڵ�
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
            if (m_roomRoot == null) { Debug.Log("�ڳ�����û���ҵ� Room Pannel �ĸ��ڵ�"); }
        }
    }
}
