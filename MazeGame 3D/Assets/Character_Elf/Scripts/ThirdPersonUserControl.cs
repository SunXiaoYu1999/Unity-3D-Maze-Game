using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

[RequireComponent(typeof(ThirdPersonCharacter))]
public class ThirdPersonUserControl : MonoBehaviour
{
    private ThirdPersonCharacter m_Character; // A reference to the ThirdPersonCharacter on the object
    public Transform m_Cam;                  // A reference to the main camera in the scenes transform
    private Vector3 m_CamForward;             // The current forward direction of the camera
    private Vector3 m_Move;
    private bool m_Jump;                      // the world-relative desired move direction, calculated from the camForward and user input.

    // 网络模块进行网络同步时需要
    public NetInput m_input;


    public struct NetInput
    {
        public Vector3 move;    // 使用 move 进行人物移动的同步，不再是 h, v
        public float h;    // 水平轴
        public float v;    // 竖直轴
        public bool jump;  // 跳跃
        public bool crouch;// 下蹲按键
        public bool shift; // 奔跑
    }


    protected virtual void Start()
    {
        // get the transform of the main camera
        if (Camera.main != null)
        {
            m_Cam = Camera.main.transform;
        }
        else
        {
            Debug.LogWarning(
                "Warning: no main camera found. Third person character needs a Camera tagged \"MainCamera\", for camera-relative controls.", gameObject);
            // we use self-relative controls in this case, which probably isn't what the user wants, but hey, we warned them!
        }

        // get the third person character ( this should never be null due to require component )
        m_Character = GetComponent<ThirdPersonCharacter>();
        m_input = new NetInput();
    }


    protected virtual void Update()
    {
//        if (!m_input.jump)
//        {
            m_input.jump = CrossPlatformInputManager.GetButtonDown("Jump");
//                m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
//        }
        m_input.h = CrossPlatformInputManager.GetAxis("Horizontal");
        m_input.v = CrossPlatformInputManager.GetAxis("Vertical");
        m_input.crouch = Input.GetKey(KeyCode.C);
        m_input.shift = Input.GetKey(KeyCode.LeftShift);

        if (m_Cam != null)
        {
            // calculate camera relative direction to move:
            m_CamForward = Vector3.Scale(m_Cam.forward, new Vector3(1, 0, 1)).normalized;   /* 获得摄像机方向向量的 x, z坐标 */
            m_Move = m_input.v * m_CamForward + m_input.h * m_Cam.right;
        }
        else
        {
            // we use world-relative directions in the case of no main camera
            m_Move = m_input.v * Vector3.forward + m_input.h * Vector3.right;
        }
        m_Move *= 0.5f;

        // walk speed multiplier
        if (m_input.shift) 
            m_Move *= 2f;
        m_input.move = m_Move;
    }


    // Fixed update is called in sync with physics
    protected virtual void FixedUpdate()
    {
        // read inputs
        //m_input.h = CrossPlatformInputManager.GetAxis("Horizontal");
        //m_input.v = CrossPlatformInputManager.GetAxis("Vertical");
        //m_input.crouch = Input.GetKey(KeyCode.C);
        //            float h = CrossPlatformInputManager.GetAxis("Horizontal");
        //            float v = CrossPlatformInputManager.GetAxis("Vertical");
        // bool crouch = Input.GetKey(KeyCode.C);

        // calculate move direction to pass to character
  //      if (m_Cam != null)
  //      {
  //          // calculate camera relative direction to move:
  //          m_CamForward = Vector3.Scale(m_Cam.forward, new Vector3(1, 0, 1)).normalized;   /* 获得摄像机方向向量的 x, z坐标 */
  //          m_Move = m_input.v * m_CamForward + m_input.h * m_Cam.right;
  //      }
  //      else
  //      {
  //          // we use world-relative directions in the case of no main camera
  //          m_Move = m_input.v * Vector3.forward + m_input.h * Vector3.right;
  //      }
  //      m_Move *= 0.5f;

		//// walk speed multiplier
	 //   if (m_input.shift) m_Move *= 2f;

        // pass all parameters to the character control script
        m_Character.Move(m_input.move, m_input.crouch, m_input.jump);
        m_input.jump = false;
    }
}

