using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartUIManager : MonoBehaviour
{
    public Text m_showPanel;
    public Image m_sceenMask;

    private void Start()
    {
        if(m_showPanel == null)
        {
            Debug.Log("û����ȷ���� ��ʾ������� ����");
        }
    }

}
