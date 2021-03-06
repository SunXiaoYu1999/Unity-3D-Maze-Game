//-------------------------------------------------
//                    TNet 3
// Copyright © 2012-2018 Tasharen Entertainment Inc
//-------------------------------------------------

using TNet;
using UnityEngine;

/// <summary>
/// This simple script shows how to change the color of an object on all connected clients.
/// You can see it used in Example 1.
/// </summary>

[RequireComponent(typeof(TNObject))]
[RequireComponent(typeof(Renderer))]
public class ColoredObject : TNBehaviour
{
    Material mMat;

    protected override void Awake() { base.Awake(); mMat = GetComponent<Renderer>().material; }

    /// <summary>
    /// This function is called by the server when one of the players sends an RFC call.
    /// </summary>

    [RFC] void OnColor(Color c) { mMat.color = c; }

    /// <summary>
    /// Clicking on the object should change its color.
    /// </summary>

    void OnClick()
    {
        Color color = Color.red;

        if (mMat.color == Color.red) color = Color.green;
        else if (mMat.color == Color.green) color = Color.blue;

        tno.Send("OnColor", Target.AllSaved, color);
    }
}
