using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TNet;

[RequireComponent(typeof(TNObject))]
public class NetThirdPerson : ThirdPersonUserControl
{
    public TNObject tno;    // TNObject 引用

    public Rigidbody m_rigidbody;

    /// <summary>
    /// 输入数据的同步频率 (次/秒)
    /// </summary>
    [Range(1f, 20f)]
    public float inputUpdateCountPerSecond = 10f;

    /// <summary>
    /// 刚体数据校验频率 (次/秒)
    /// </summary>
    [Range(0.25f, 5f)]
    public float rigidbodyUpdateCountPerSecond = 1f;


    protected NetInput  m_lastInput;
    protected float m_lastInputSendTime = 0f;
    protected float m_nextRigidbody = 0f;
    protected float m_nextPosition = 0f;

    protected void Awake()
    {
        if(tno == null)
            tno = this.GetComponent<TNObject>();
        if(m_rigidbody == null)
            m_rigidbody = this.GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update

    // Update is called once per frame
    protected override void Update()
    {
        if (!tno.isMine)    // 不是自己直接返回
        {
            return;
        }
        // 更新输入数据
        base.Update();

        // 被销毁了
        if (tno.hasBeenDestroyed)
            return;

        float curTime = Time.time;
        float deltaTime = curTime - m_lastInputSendTime;
        float delay = 1f / inputUpdateCountPerSecond;

        // 每秒发送频率控制在 20 次以下
        if(deltaTime > 0.05f)
        {
            float threshold = Mathf.Clamp01(deltaTime - delay) * 0.5f;

            if( Tools.IsNotEqual(m_lastInput.h, m_input.h, threshold) ||
                Tools.IsNotEqual(m_lastInput.v, m_input.v, threshold)||
                m_lastInput.jump != m_input.jump||
                m_lastInput.crouch != m_input.crouch||
                m_lastInput.shift != m_input.shift)
            {
                m_lastInput = m_input;
                m_lastInputSendTime = curTime;
                tno.Send("SetAxis", Target.OthersSaved, m_input);
            }
        }

        if(m_nextRigidbody < curTime)
        {
            m_nextRigidbody = curTime + 1f / rigidbodyUpdateCountPerSecond;
            tno.SendQuickly("SetRigidbody", Target.OthersSaved, m_rigidbody.position, m_rigidbody.rotation, m_rigidbody.velocity, m_rigidbody.angularVelocity);
        }
    }

    [RFC]
    protected void SetAxis(NetInput input)
    {
        m_input = input;
    }

    [RFC]
    protected void SetRigidbody(Vector3 pos, Quaternion rot, Vector3 vel, Vector3 angVel)
    {
        m_rigidbody.position = pos;
        m_rigidbody.rotation = rot;
        m_rigidbody.velocity = vel;
        m_rigidbody.angularVelocity = angVel;
    }
}
