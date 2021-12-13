using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Room : Button
{
    // Start is called before the first frame update
    public int m_channelID;     // 当前的频道 ID
    public int m_maxPlayer;     // 最大玩家数量
    public int m_curPlayer;     // 当前玩家数量
    public Text m_text;         // 按钮上的文字
    public string m_levelName;  // 房间关卡名称
    public bool m_hasPossWord;  // 房间密码
    public bool alive = false;  // 当前房间是否还存在于服务器中

    protected override void Awake()
    {
        base.Awake();
        var textTrans = transform.Find("Text");
        m_text = textTrans.gameObject.GetComponent<Text>();
    }
}
