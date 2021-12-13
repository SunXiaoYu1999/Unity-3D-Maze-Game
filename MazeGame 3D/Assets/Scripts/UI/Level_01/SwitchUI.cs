using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SwitchUI : MonoBehaviour
{
    public GameObject m_targetGMO;  // 被启用（禁用的对象）

    public string m_OnText;
    public string m_OffText;

    public bool m_curStatus;        // 默认状态

    public Text m_btnText;
    private void Start()
    {
        if (m_btnText == null)
        {
            Transform tmp = this.transform.Find("Text");
            if (tmp != null)
                m_btnText = tmp.GetComponent<Text>();
        }
        if (m_btnText == null)
            Debug.Log("没找到文字组件");
    }

    public void OnClick()
    {
        if(m_curStatus == true)     
        {
            m_targetGMO.SetActive(false);
            m_btnText.text = m_OffText;
            m_curStatus = false;
        }
        else
        {
            m_targetGMO.SetActive(true);
            m_btnText.text = m_OnText;
            m_curStatus = true;
        }
    }
}
