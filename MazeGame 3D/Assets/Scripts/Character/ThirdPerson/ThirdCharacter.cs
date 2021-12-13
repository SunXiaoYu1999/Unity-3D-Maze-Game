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
		[SerializeField] float m_GroundCheckDistance = 0.10f;   /* 地面检测时候向下的距离 */

		Rigidbody		m_Rigidbody;				/* 刚体组件引用 */
		Animator		m_Animator;					/* Animator 引用 */
		bool			m_IsGrounded;				/* 角色是否在地面 */
		float			m_OrigGroundCheckDistance;	/* 检测地面时候的距离(Inspector 中设置的默认距离) */
		float			m_TurnAmount;				/* Animator 中的 Turn 数值 */
		float			m_ForwardAmount;			/* Animator 中的 Forward 数值 */
		Vector3			m_GroundNormal;				/* 角色脚下地面的法向量 */



		void Start()
		{
			/* 获取组件引用 */
			m_Animator = GetComponent<Animator>();
			m_Rigidbody = GetComponent<Rigidbody>();

			/* 刚体锁定 X, Y, Z 的旋转 */
			m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
			
			m_OrigGroundCheckDistance = m_GroundCheckDistance;
		}


		public void Move(Vector3 move, bool crouch, bool jump)
		{
			/* 根据摄像机（或者世界坐标系）算出的 Move 向量需要转换到角色本地的坐标系下的 move */
			if (move.magnitude > 1)
				move.Normalize();
			move = transform.InverseTransformDirection(move);   // 将世界坐标转换为本地坐标
			CheckGroundStatus();								// 使用射线检测 人物使是否在地面，更新地面法线
			move = Vector3.ProjectOnPlane(move, m_GroundNormal);// 将 move 向量向地面（这里不一定是平面）做投影
			m_TurnAmount = Mathf.Atan2(move.x, move.z);			// 计算从前方向向左（或右）的偏转角度值 
			m_ForwardAmount = move.z;

			/* 帮助动画快速过渡 */
			ApplyExtraTurnRotation();

			/* 人物一开始被放在空中，人物在空中与地面的移动方式有区别 */
			if (m_IsGrounded)
			{
				HandleGroundedMovement(jump);
			}
			else
			{
				HandleAirborneMovement();
			}

			// 将数据传入Animator中， 进行动画处理
			UpdateAnimator(move);
		}
		void UpdateAnimator(Vector3 move)
		{
			/* 更新 Animator 参数 */
			m_Animator.SetFloat("Forward", m_ForwardAmount, 0.1f, Time.deltaTime);
			m_Animator.SetFloat("Turn", m_TurnAmount, 0.1f, Time.deltaTime);
 
			/* 在 Inspector 中可以动态调整动画的加速（在跑步或者走路时候） */
			if (m_IsGrounded && move.magnitude > 0)
			{
				m_Animator.speed = m_AnimSpeedMultiplier;
			}
			else
			{
				// 在空中的话动画加速失效
				m_Animator.speed = 1;
			}
		}


		void HandleAirborneMovement()
		{
			// 可以通过 Inspector 界面调整重力的影响
			Vector3 extraGravityForce = (Physics.gravity * m_GravityMultiplier) - Physics.gravity;
			m_Rigidbody.AddForce(extraGravityForce);
		}


		void HandleGroundedMovement(bool jump)
		{
			/* 处理人物在地面时的跳起处理 */
			if (jump && m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Grounded"))
			{
				m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x, m_JumpPower, m_Rigidbody.velocity.z);	// 设置刚体的速度
				m_IsGrounded = false;																				// 更新地面状态
				m_Animator.applyRootMotion = false;																	// ????																
			}
		}

		void ApplyExtraTurnRotation()
		{
			/* 对动画中的旋转进行调整 */
			float turnSpeed = Mathf.Lerp(m_StationaryTurnSpeed, m_MovingTurnSpeed, m_ForwardAmount);
			transform.Rotate(0, m_TurnAmount * turnSpeed * Time.deltaTime, 0);
		}


		public void OnAnimatorMove()
		{
			/* 根据 Animator 中的状态计算出速度，设置到刚体中*/
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
			/* 在编辑器中画出检测射线 */
			Debug.DrawLine(transform.position + (Vector3.up * m_GroundCheckDistance * 0.5f), transform.position + (Vector3.down * m_GroundCheckDistance * 0.5f));
#endif
			/* 射线检从角色位置 上方的 m_GroundCheckDistance/2 距离开始 到 角色位置的下方 m_GroundCheckDistance/2 距离结束，需要根据角色位置调整 */
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
