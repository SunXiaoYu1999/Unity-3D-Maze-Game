//-------------------------------------------------
//                    TNet 3
// Copyright © 2012-2018 Tasharen Entertainment Inc
//-------------------------------------------------

using TNet;
using UnityEngine;

/// <summary>
/// Attach this script to a renderer that's a child of a rigidbody in order to make its update
/// smooth even at times of high network latency.
/// </summary>

public class SpringTransform : MonoBehaviour
{
    /// <summary>
    /// Spring's strength controls how quickly the position adapts to various changes.
    /// The higher the value, the stronger the spring, and the faster it will adapt to changes.
    /// The lower the value, the smoother the transition will be.
    /// </summary>

    public float springStrength = 10f;

    /// <summary>
    /// Whether this script's effect will be ignored on the hosting player.
    /// </summary>

    public bool ignoreOnHost = true;

    bool mStarted = false;
    bool mWasHosting = false;
    Transform mParent;
    Transform mTrans;
    Vector3 mPos;
    Quaternion mRot;
    TNObject mTNO;

    /// <summary>
    /// Reset the transform's position and rotation to match the parent.
    /// </summary>

    public void Reset()
    {
        mStarted = true;
        mTrans = transform;
        mParent = mTrans.parent;

        if (mParent != null)
        {
            mPos = mParent.position;
            mRot = mParent.rotation;
        }
        else Destroy(this);
    }

    void OnEnable()
    {
        if (mStarted) Reset();
        mTNO = GetComponentInParent<TNObject>();
        TNManager.onJoinChannel += OnJoinChannel;
    }

    void OnDisable() { TNManager.onJoinChannel -= OnJoinChannel; }
    void Start() { Reset(); }
    void OnJoinChannel(int channelID, bool success, string error) { Reset(); }

    /// <summary>
    /// Update the position and rotation, smoothly interpolating it to the target destination using spring logic.
    /// </summary>

    void LateUpdate()
    {
        if (!mStarted) return;

        if (ignoreOnHost && (mTNO != null && mTNO.isMine))
        {
            if (!mWasHosting)
            {
                mTrans.position = mParent.position;
                mTrans.rotation = mParent.rotation;
                mWasHosting = true;
            }
        }
        else
        {
            float delta = Mathf.Clamp01(Time.deltaTime * springStrength);

            mPos = Vector3.Lerp(mPos, mParent.position, delta);
            mRot = Quaternion.Slerp(mRot, mParent.rotation, delta);

            mTrans.position = mPos;
            mTrans.rotation = mRot;
            mWasHosting = false;
        }
    }
}
