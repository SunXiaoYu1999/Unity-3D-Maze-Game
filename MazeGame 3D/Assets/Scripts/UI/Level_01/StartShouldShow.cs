using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TNet;
using UnityEngine.UI;
public class StartShouldShow : MonoBehaviour
{
    private float detectTimeGap = 1.0f;
    private float nextTime = 0f;
    public int curRoomChannel = ConstVariable.curRoomChannel;
    void Update()
    {
        

        if (nextTime < Time.time)
        {
            nextTime += detectTimeGap;

            if (curRoomChannel == 0)
            {
                curRoomChannel = ConstVariable.curRoomChannel;
            }

            Player curPlayer = TNManager.player;
            Player hostPlayer = TNManager.GetHost(curRoomChannel);
            if ( (curPlayer != null && hostPlayer != null && curPlayer != hostPlayer ) || ConstVariable.isGaming)   // ��ǰ��Ҳ��Ƿ��� ���ߵ�ǰ����Ϸ״̬
            {
                GetComponent<Button>().interactable = false;
            }
            else
            {
                GetComponent<Button>().interactable = true;
            }
        }
    }
}
