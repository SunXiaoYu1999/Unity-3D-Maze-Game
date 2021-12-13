using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TNet;
using UnityEngine.UI;

public class GameEnd : TNBehaviour
{
    public GameObject m_defeat;
    public GameObject m_win;
    public static bool isWin = false;
    public Text m_showPannel;

    protected override void Awake()
    {
        base.Awake();

        if (m_defeat == null)
            m_defeat = transform.Find("Defeat").gameObject;
        if (m_win == null)
            m_win = transform.Find("Win").gameObject;
        if (m_showPannel == null)
            m_win = transform.Find("Text").gameObject;
    }
    private void OnEnable()
    {
        if (isWin)
            m_win.SetActive(true);
        else
            m_defeat.SetActive(true);
        m_showPannel.text = ConstVariable.winner;
    }

    private void OnDisable()
    {
        m_defeat.SetActive(false);
        m_win.SetActive(false);
        isWin = false;
    }
}
