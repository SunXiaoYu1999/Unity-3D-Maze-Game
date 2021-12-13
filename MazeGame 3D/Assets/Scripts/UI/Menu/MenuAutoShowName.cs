using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuAutoShowName : MonoBehaviour
{
    public void OnEnable()
    {
        GetComponent<UnityEngine.UI.Text>().text = ConstVariable.playerName;
    }

    public void wake()
    {
        GetComponent<UnityEngine.UI.Text>().text = ConstVariable.playerName;
    }

    public void Start()
    {
        GetComponent<UnityEngine.UI.Text>().text = ConstVariable.playerName;
    }
}
