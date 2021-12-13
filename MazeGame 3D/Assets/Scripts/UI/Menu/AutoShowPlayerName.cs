using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TNet;
using UnityEngine.UI;

public class AutoShowPlayerName : MonoBehaviour
{
    private Text m_text;

    private void Awake()
    {
        if(TNManager.isConnected)
            TNManager.playerName = ConstVariable.playerName;
    }
    void Start()
    {
        m_text = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        m_text.text = TNManager.playerName;
    }
}
