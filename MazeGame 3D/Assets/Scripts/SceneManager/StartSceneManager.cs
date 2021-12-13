using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TNet;

public class StartSceneManager : TNBehaviour
{
    public StartUIManager m_uiManager;
    public float m_transitionAnimationtime = 3.0f;  // 过渡动画的时长
    public bool m_startCloseTrasition = false;
    public int m_connected = 0;
    // Start is called before the first frame update
    void Start()
    {
        if (m_uiManager == null)
        {
            m_uiManager = GameObject.FindObjectOfType<StartUIManager>();
            if (m_uiManager == null){Debug.Log("没能在场景 GameStart 中找到 StartUiManager 组件");}

        }
    }

    protected override void Awake()
    {
        base.Awake();

        /* 初始游戏数据加载 */
        LoadGameOriginData();
    }

    public void LoadGameOriginData()
    {
        ConstVariable.playerName = PlayerPrefs.GetString("PlayerName", "Tony");
        ConstVariable.mouseXSpeed = PlayerPrefs.GetFloat("MouseXSpeed", 200.0f);
        ConstVariable.mouseYSpeed = PlayerPrefs.GetFloat("MouseYSpeed", 200.0f);
    }

    // Update is called once per frame
    void Update()
    {
        if(m_startCloseTrasition == true)
        {
            CloseTransitionAnimation();
        }

        if (m_connected == 1)  // 连接到服务器了
        {
            m_uiManager.m_showPanel.text = "连接成功!";
        }
        else if(m_connected == -1)
        {
            m_uiManager.m_showPanel.text = "连接失败!";
        }
    }

    private Color tmp = new Color();
    private float time = 0;
    public void CloseTransitionAnimation()
    {
        tmp = m_uiManager.m_sceenMask.color;
        time += Time.deltaTime;
        if (time > m_transitionAnimationtime) time = m_transitionAnimationtime;
        tmp.a = Mathf.Lerp(0, 1.0f, time / m_transitionAnimationtime);
        m_uiManager.m_sceenMask.color = tmp;
    }

    public void OnConnectError(string msg)
    {
        m_uiManager.m_showPanel.text += msg;
    }

}
