using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace sy
{
    [RequireComponent(typeof(ThirdCharacter))]
    public class ThirdUserControl : MonoBehaviour
    {
        private ThirdCharacter m_Character; /* ��ɫ���� */
        public Transform m_Cam;            /* ��������ı任��� */
        private Vector3 m_CamForward;       /* �������ǰ���� */
        private Vector3 m_Move;             /* �����û����������γɵ� move ���� */
        private void Start()
        {
            /* ���Ի�ȡ�������� */
            if (Camera.main != null && m_Cam == null)
            {
                m_Cam = Camera.main.transform;
            }
            else
            {
                Debug.LogWarning(
                    "û���ҵ������������", gameObject);
                /* ʹ���������ƣ���ʹ���������ǰ������ */
            }

            /* �� [RequireComponent(typeof(ThirdCharacter))] ���ƣ���ȡ��ɫ����϶���Ϊ��*/
            m_Character = GetComponent<ThirdCharacter>();
        }

        // Fixed update is called in sync with physics
        private void FixedUpdate()
        {
            /* ��ȡ�û����� */
            float h = CrossPlatformInputManager.GetAxis("Horizontal");
            float v = CrossPlatformInputManager.GetAxis("Vertical");

            /* �����û�������� move ���� */
            if (m_Cam != null)
            {
                m_CamForward = Vector3.Scale(m_Cam.forward, new Vector3(1, 0, 1)).normalized;   /* ������������������ x, z���� */
                m_Move = v * m_CamForward + h * m_Cam.right;
            }
            else
            {
                /* û��������Ļ�������������������ƶ� */
                m_Move = v * Vector3.forward + h * Vector3.right;
            }

            /* �����ɫ�� */
            m_Character.Move(m_Move, false, false);
        }
    }
}
