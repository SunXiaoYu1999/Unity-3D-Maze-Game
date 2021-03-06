//-------------------------------------------------
//                    TNet 3
// Copyright © 2012-2018 Tasharen Entertainment Inc
//-------------------------------------------------

using TNet;
using UnityEngine;

/// <summary>
/// This simple example script shows how to improve latency in a scene by disabling the Nagle's buffering algorithm.
/// If you run Example 3 on a mobile device using only TCP and without this script, other players may notice that
/// the mobile player is "lagging". This is because by default the buffering algorithm is on, and seems to be overly
/// aggressive on some devices. For games that require quick response times, turning on "improveLatency" flag will
/// improve performance. http://en.wikipedia.org/wiki/Nagle's_algorithm
/// 
/// Note that using UDP for frequently sent data is usually a better approach than turning on 'noDelay'.
/// For more information, look at how TNObject's SendQuickly function is used in the DraggedObject script.
/// </summary>

public class ImproveLatency : MonoBehaviour
{
    public enum Target
    {
        OnlyOnMobiles,
        Everywhere,
    }

    public Target target = Target.OnlyOnMobiles;

    void OnEnable()
    {
        TNManager.onJoinChannel += OnJoinChannel;
        TNManager.onLeaveChannel += OnLeaveChannel;
    }

    void OnDisable()
    {
        TNManager.onJoinChannel -= OnJoinChannel;
        TNManager.onLeaveChannel -= OnLeaveChannel;
    }

    void OnJoinChannel(int channelID, bool success, string error)
    {
        if (enabled && success && !TNManager.canUseUDP)
        {
            if (Application.platform == RuntimePlatform.Android ||
                Application.platform == RuntimePlatform.IPhonePlayer ||
                target == Target.Everywhere)
            {
                TNManager.noDelay = true;
            }
        }
    }

    void OnLeaveChannel(int channelID)
    {
        TNManager.noDelay = false;
    }
}
