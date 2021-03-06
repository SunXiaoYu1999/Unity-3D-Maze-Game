//-------------------------------------------------
//                    TNet 3
// Copyright © 2012-2018 Tasharen Entertainment Inc
//-------------------------------------------------

using TNet;
using UnityEngine;

/// <summary>
/// Spin the object by dragging it sideways and move it up/down.
/// </summary>

public class MoveSpinObject : TNBehaviour
{
    void OnDrag(Vector2 delta)
    {
        if (tno.isMine)
        {
            Vector3 euler = transform.eulerAngles;
            euler.y -= delta.x * 0.5f;
            transform.eulerAngles = euler;

            Vector3 pos = transform.position;
            pos.y += delta.y * 0.01f;
            transform.position = pos;
        }
    }
}
