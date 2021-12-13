using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TNet;

public class MenuSettingBtn : TNBehaviour
{
    public Text m_textSetting;
    public GameObject m_pannelSetting;
    public Text m_textInput;
    public Slider m_mouseSpeed;

    private bool m_setting = false;
    void Start()
    {
        if (m_textSetting == null)
            m_textSetting = this.transform.Find("Text").GetComponent<Text>();
        if (m_pannelSetting == null)
            m_pannelSetting = this.transform.parent.Find("SettingPannel").gameObject;
        if (m_textInput == null)
            m_textInput = m_pannelSetting.transform.Find("Name").Find("Text").GetComponent<Text>();
        if (m_mouseSpeed == null)
            m_mouseSpeed = m_pannelSetting.transform.Find("MouseSpeed").GetComponent<Slider>();
    }


    public void OnSetting()
    {
        if(m_setting)
        {
            m_pannelSetting.SetActive(false);
            m_textSetting.text = "设    置";
            m_setting = false;
            /* 保存设置 */
            TNManager.playerName = m_textInput.text;
            ConstVariable.playerName = m_textInput.text;
            PlayerPrefs.SetString("PlayerName", ConstVariable.playerName);

            ConstVariable.mouseXSpeed = m_mouseSpeed.value * 4.0f;
            ConstVariable.mouseXSpeed = m_mouseSpeed.value * 4.0f;
            PlayerPrefs.SetFloat("MouseXSpeed", ConstVariable.mouseXSpeed);
            PlayerPrefs.SetFloat("MouseYSpeed", ConstVariable.mouseYSpeed);

        }
        else
        {
            ConstVariable.playerName = TNManager.playerName;
            m_pannelSetting.SetActive(true);
            m_textSetting.text = "确    定";
            m_setting = true;
            m_mouseSpeed.value = ConstVariable.mouseXSpeed / 4.0f;
            ConstVariable.playerName = TNManager.playerName;
        }

    }

    public void OnCannel()
    {
        if (m_setting)
        {
            m_pannelSetting.SetActive(false);
            m_textSetting.text = "设    置";
            m_setting = false;
            /* 保存设置 */
        }
        else
        {
            m_pannelSetting.SetActive(true);
            m_textSetting.text = "确    定";
            m_setting = true;
        }
    }
}
