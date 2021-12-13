using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace sy
{
    [RequireComponent(typeof(ThirdCharacter))]
    public class ThirdUserControl : MonoBehaviour
    {
        private ThirdCharacter m_Character; /* 角色引用 */
        public Transform m_Cam;            /* 主摄像机的变换组件 */
        private Vector3 m_CamForward;       /* 摄像机的前方向 */
        private Vector3 m_Move;             /* 根据用户输入最终形成的 move 向量 */
        private void Start()
        {
            /* 尝试获取摄像机组件 */
            if (Camera.main != null && m_Cam == null)
            {
                m_Cam = Camera.main.transform;
            }
            else
            {
                Debug.LogWarning(
                    "没有找到摄像机！！！", gameObject);
                /* 使用自主控制，不使用摄像机的前方向了 */
            }

            /* 由 [RequireComponent(typeof(ThirdCharacter))] 限制，获取角色组件肯定不为空*/
            m_Character = GetComponent<ThirdCharacter>();
        }

        // Fixed update is called in sync with physics
        private void FixedUpdate()
        {
            /* 获取用户输入 */
            float h = CrossPlatformInputManager.GetAxis("Horizontal");
            float v = CrossPlatformInputManager.GetAxis("Vertical");

            /* 根据用户输入计算 move 向量 */
            if (m_Cam != null)
            {
                m_CamForward = Vector3.Scale(m_Cam.forward, new Vector3(1, 0, 1)).normalized;   /* 获得摄像机方向向量的 x, z坐标 */
                m_Move = v * m_CamForward + h * m_Cam.right;
            }
            else
            {
                /* 没有摄像机的话，基于世界坐标进行移动 */
                m_Move = v * Vector3.forward + h * Vector3.right;
            }

            /* 传入角色中 */
            m_Character.Move(m_Move, false, false);
        }
    }
}
