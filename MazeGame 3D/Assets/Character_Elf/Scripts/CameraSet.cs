using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gaia;

public class CameraSet : MonoBehaviour
{
    private bool first = true;
    protected void Awake()
    {
        if (first)
        {
            CameraController.m_camera = this.transform;
            first = false;
        }
    }
}

