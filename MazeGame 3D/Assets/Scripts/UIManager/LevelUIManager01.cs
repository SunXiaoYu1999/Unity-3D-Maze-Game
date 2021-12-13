using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelUIManager01 : MonoBehaviour
{
    [Tooltip("游戏生成迷宫后消除屏幕遮罩，这个是过渡时间")]
    public float m_maskTrisitionTime = 1.0f;
    [Tooltip("场景开始时的遮罩")]
    public GameObject m_mask;
    public bool m_maskStart = false;    // 开始执行遮罩消除动画
    [Tooltip("显示房间人名信息的面板")]
    public ShowPanel m_RoomPlayerNames;
    [Tooltip("开始游戏的按钮")]
    public Button m_btnStartGame;
    [Tooltip("退出房间的按钮")]
    public Button m_btnExitRoom;
    [Tooltip("菜单组件")]
    public GameObject m_Menu;
    [Tooltip("菜单按钮")]
    public Button m_btnMenu;
    [Tooltip("游戏结束")]
    public GameObject m_gameEnd;

    /* 初始化 mask 引用，若为空则查找 */
    private void _initMask()
    {
        if (m_mask == null)
        {
            Transform tmpTFM = Find(this.transform, "Mask");
            if (tmpTFM == null)
                m_mask = null;
            else
                m_mask = tmpTFM.gameObject;
            if (m_mask == null)
            {
                Debug.Log("没有找到UI组件中的 Mask 组件");
            }
        }
    }
    private void _initPlayerNameRef()
    {
        if (m_RoomPlayerNames == null)
        {
            Transform tmpTFM = Find(this.transform, "ShowName");
            if (tmpTFM == null)
                m_RoomPlayerNames = null;
            else
                m_RoomPlayerNames = tmpTFM.gameObject.GetComponent<ShowPanel>();
            if (m_RoomPlayerNames == null)
            {
                Debug.Log("没有找到显示联机玩家名字的面板");
            }
        }
    }
    private void _initButtonStart()
    {
        if (m_btnStartGame == null)
        {
            Transform tmpTFM = Find(this.transform, "BtnStartGame");
            if (tmpTFM == null)
                m_btnStartGame = null;
            else
                m_btnStartGame = tmpTFM.gameObject.GetComponent<Button>();
            if (m_btnStartGame == null)
            {
                Debug.Log("没有找到开始游戏按钮");
            }
        }
    }
    private void _initButtonExit()
    {
        if (m_btnExitRoom == null)
        {
            Transform tmpTFM = Find(this.transform, "BtnExit");
            if (tmpTFM == null)
                m_btnExitRoom = null;
            else
                m_btnExitRoom = tmpTFM.gameObject.GetComponent<Button>();
            if (m_btnExitRoom == null)
            {
                Debug.Log("没有找到退出房间按钮");
            }
        }
    }
    private void _initMenu()
    {
        if (m_Menu == null)
        {
            Transform tmpTFM = Find(this.transform, "Menu");
            if (tmpTFM == null)
                m_Menu = null;
            else
                m_Menu = tmpTFM.gameObject;
            if (m_Menu == null)
            {
                Debug.Log("没有找到 Menu 菜单（LevelUiManager01.cs --> LevelUiManager01._initMenu()）");
            }
        }
    }
    private void _initBtnMenu()
    {
        if (m_btnMenu == null)
        {
            Transform tmpTFM = Find(this.transform, "BtnMenu");
            if (tmpTFM == null)
                m_btnMenu = null;
            else
                m_btnMenu = tmpTFM.gameObject.GetComponent<Button>();
            if (m_btnMenu == null)
            {
                Debug.Log("没有找到 Menu 菜单（LevelUiManager01.cs --> LevelUiManager01._initMenu()）");
            }
        }
    }
    private void _initGameEnd()
    {
        if (m_gameEnd == null)
        {
            Transform tmpTFM = Find(this.transform, "GameEnd");
            if (tmpTFM == null)
                m_gameEnd = null;
            else
                m_gameEnd = tmpTFM.gameObject;
            if (m_gameEnd == null)
            {
                Debug.Log("没有找到 GameEnd LevelUiManager01.cs --> LevelUiManager01._initGameEnd()）");
            }
        }
    }

    private void Start()
    {
        _initMask();
        _initPlayerNameRef();
        _initButtonStart();
        _initButtonExit();
        _initMenu();
        _initBtnMenu();
        _initGameEnd();

    }

    public void Update()
    {
        /* 遮罩消失动画*/
        if (m_mask != null && m_mask.activeSelf == true) { MaskTrisitionAnimation(); }
            
    }

    /* 屏幕过渡 */
    private float curTime = 0.0f;
    private void MaskTrisitionAnimation()
    {
        if(m_maskStart)
        {
            Color color = m_mask.GetComponent<Image>().color;
            float t = (curTime / m_maskTrisitionTime) > 1.0f ? (1.0f) : curTime / m_maskTrisitionTime;
            color.a = Mathf.Lerp(1f, 0f, t);
            curTime += Time.deltaTime;
            if(t == 1.0f)
            {
                m_mask.SetActive(false);
            }
        }
    }


    /* 在 root 下查找 名为 name 的 GameObject,返回其 Transform 组件 */
    static public Transform Find(Transform root, string name)
    {
        Transform ret = null;
        ret = root.Find(name);
        if (ret != null)
            return ret;
        for(int i = 0; i < root.childCount; ++i)
        {
            ret = Find(root.GetChild(0), name);
            if (ret != null)
                return ret;
        }
        return ret;
    }
}