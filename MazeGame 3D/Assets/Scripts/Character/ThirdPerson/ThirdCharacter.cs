using UnityEngine;

namespace sy
{
	[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(CapsuleCollider))]
	[RequireComponent(typeof(Animator))]
	public class ThirdCharacter : MonoBehaviour
	{
		[SerializeField] float m_MovingTurnSpeed = 360;
		[SerializeField] float m_StationaryTurnSpeed = 180;
		[SerializeField] float m_JumpPower = 12f;
		[Range(1f, 4f)] [SerializeField] float m_GravityMultiplier = 2f;
		[SerializeField] float m_MoveSpeedMultiplier = 1f;
		[SerializeField] float m_AnimSpeedMultiplier = 1f;
		[SerializeField] float m_GroundCheckDistance = 0.10f;   /* ������ʱ�����µľ��� */

		Rigidbody		m_Rigidbody;				/* ����������� */
		Animator		m_Animator;					/* Animator ���� */
		bool			m_IsGrounded;				/* ��ɫ�Ƿ��ڵ��� */
		float			m_OrigGroundCheckDistance;	/* ������ʱ��ľ���(Inspector �����õ�Ĭ�Ͼ���) */
		float			m_TurnAmount;				/* Animator �е� Turn ��ֵ */
		float			m_ForwardAmount;			/* Animator �е� Forward ��ֵ */
		Vector3			m_GroundNormal;				/* ��ɫ���µ���ķ����� */



		void Start()
		{
			/* ��ȡ������� */
			m_Animator = GetComponent<Animator>();
			m_Rigidbody = GetComponent<Rigidbody>();

			/* �������� X, Y, Z ����ת */
			m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
			
			m_OrigGroundCheckDistance = m_GroundCheckDistance;
		}


		public void Move(Vector3 move, bool crouch, bool jump)
		{
			/* �����������������������ϵ������� Move ������Ҫת������ɫ���ص�����ϵ�µ� move */
			if (move.magnitude > 1)
				move.Normalize();
			move = transform.InverseTransformDirection(move);   // ����������ת��Ϊ��������
			CheckGroundStatus();								// ʹ�����߼�� ����ʹ�Ƿ��ڵ��棬���µ��淨��
			move = Vector3.ProjectOnPlane(move, m_GroundNormal);// �� move ��������棨���ﲻһ����ƽ�棩��ͶӰ
			m_TurnAmount = Mathf.Atan2(move.x, move.z);			// �����ǰ�������󣨻��ң���ƫת�Ƕ�ֵ 
			m_ForwardAmount = move.z;

			/* �����������ٹ��� */
			ApplyExtraTurnRotation();

			/* ����һ��ʼ�����ڿ��У������ڿ����������ƶ���ʽ������ */
			if (m_IsGrounded)
			{
				HandleGroundedMovement(jump);
			}
			else
			{
				HandleAirborneMovement();
			}

			// �����ݴ���Animator�У� ���ж�������
			UpdateAnimator(move);
		}
		void UpdateAnimator(Vector3 move)
		{
			/* ���� Animator ���� */
			m_Animator.SetFloat("Forward", m_ForwardAmount, 0.1f, Time.deltaTime);
			m_Animator.SetFloat("Turn", m_TurnAmount, 0.1f, Time.deltaTime);
 
			/* �� Inspector �п��Զ�̬���������ļ��٣����ܲ�������·ʱ�� */
			if (m_IsGrounded && move.magnitude > 0)
			{
				m_Animator.speed = m_AnimSpeedMultiplier;
			}
			else
			{
				// �ڿ��еĻ���������ʧЧ
				m_Animator.speed = 1;
			}
		}


		void HandleAirborneMovement()
		{
			// ����ͨ�� Inspector �������������Ӱ��
			Vector3 extraGravityForce = (Physics.gravity * m_GravityMultiplier) - Physics.gravity;
			m_Rigidbody.AddForce(extraGravityForce);
		}


		void HandleGroundedMovement(bool jump)
		{
			/* ���������ڵ���ʱ�������� */
			if (jump && m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Grounded"))
			{
				m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x, m_JumpPower, m_Rigidbody.velocity.z);	// ���ø�����ٶ�
				m_IsGrounded = false;																				// ���µ���״̬
				m_Animator.applyRootMotion = false;																	// ????																
			}
		}

		void ApplyExtraTurnRotation()
		{
			/* �Զ����е���ת���е��� */
			float turnSpeed = Mathf.Lerp(m_StationaryTurnSpeed, m_MovingTurnSpeed, m_ForwardAmount);
			transform.Rotate(0, m_TurnAmount * turnSpeed * Time.deltaTime, 0);
		}


		public void OnAnimatorMove()
		{
			/* ���� Animator �е�״̬������ٶȣ����õ�������*/
			if (m_IsGrounded && Time.deltaTime > 0)
			{
				Vector3 v = (m_Animator.deltaPosition * m_MoveSpeedMultiplier) / Time.deltaTime;

				// we preserve the existing y part of the current velocity.
				v.y = m_Rigidbody.velocity.y;
				m_Rigidbody.velocity = v;
			}
		}


		void CheckGroundStatus()
		{
			RaycastHit hitInfo;
#if UNITY_EDITOR
			/* �ڱ༭���л���������� */
			Debug.DrawLine(transform.position + (Vector3.up * m_GroundCheckDistance * 0.5f), transform.position + (Vector3.down * m_GroundCheckDistance * 0.5f));
#endif
			/* ���߼�ӽ�ɫλ�� �Ϸ��� m_GroundCheckDistance/2 ���뿪ʼ �� ��ɫλ�õ��·� m_GroundCheckDistance/2 �����������Ҫ���ݽ�ɫλ�õ��� */
			if (Physics.Raycast(transform.position + (Vector3.up * m_GroundCheckDistance * 0.5f), Vector3.down, out hitInfo, m_GroundCheckDistance))
			{
				m_GroundNormal = hitInfo.normal;
				m_IsGrounded = true;
				m_Animator.applyRootMotion = true;
			}
			else
			{
				m_IsGrounded = false;
				m_GroundNormal = Vector3.up;
				m_Animator.applyRootMotion = false;
			}
		}
	}
}
