using UnityEngine;
using TNet;
namespace Gaia
{
    /// <summary>
    /// This camera controller is adapted from here: https://ruhrnuklear.de/fcc/ and provided here as a convenience.
    /// </summary>
    public class CameraController : MonoBehaviour
    {
        public static Transform m_camera;       // 相机的 transform
        public GameObject target;               // Target to follow 
        public float targetHeight = 1.84f;      // Vertical offset adjustment 
        public float distance = 12.0f;          // Default Distance 
        public float offsetFromWall = 0.1f;     // Bring camera away from any colliding objects 
        public float maxDistance = 20f;         // Maximum zoom Distance 
        public float minDistance = 0.6f;        // Minimum zoom Distance 
        public float xSpeed = 200.0f;           // Orbit speed (Left/Right) 
        public float ySpeed = 200.0f;           // Orbit speed (Up/Down) 
        public float yMinLimit = -80f;          // Looking up limit 
        public float yMaxLimit = 80f;           // Looking down limit 
        public float zoomRate = 40f;            // Zoom Speed 
        public float rotationDampening = 0.5f;  // Auto Rotation speed (higher = faster) 
        public float zoomDampening = 5.0f;      // Auto Zoom speed (Higher = faster) 
        public LayerMask collisionLayers = 3841;// What the camera will collide with 
        public bool lockToRearOfTarget = false; // Lock camera to rear of target    
        public bool allowMouseInputX = true;    // Allow player to control camera angle on the X axis (Left/Right) 
        public bool allowMouseInputY = true;    // Allow player to control camera angle on the Y axis (Up/Down) 
        

        private float xDeg = 0.0f;
        private float yDeg = 0.0f;
        private float currentDistance;
        private float desiredDistance;
        private float correctedDistance;
        public bool rotateBehind = false;
        private bool mouseSideButton = false;
        private float pbuffer = 0.0f;       //Cooldownpuffer for SideButtons 
        //private float coolDown = 0.5f;    //Cooldowntime for SideButtons  


        public bool allowAdjustMouseSpeedRuntime = false;    
    
        void Start()
        {
            if(target.GetComponent<TNObject>() == null || !target.GetComponent<TNObject>().isMine)
            {
                Destroy(this);
            }

            Vector3 angles = transform.eulerAngles;
            xDeg = angles.x;
            yDeg = angles.y;
            currentDistance = distance;
            desiredDistance = distance;
            correctedDistance = distance;

            // Make the rigid body not change rotation 
//        if (rigidbody)
//            rigidbody.freezeRotation = true;

            if (lockToRearOfTarget)
                rotateBehind = true;

            //Grab target
            if (target == null)
            {
                target = GameObject.FindGameObjectWithTag("Player") as GameObject;
            }

            gameObject.layer = 2;

            /* 允许在运行时改变鼠标灵敏度 */
            if(allowAdjustMouseSpeedRuntime)
            {
                xSpeed = ConstVariable.mouseXSpeed;
                ySpeed = ConstVariable.mouseYSpeed;
            }

        }

        //Only Move camera after everything else has been updated 
        private void LateUpdate()
        {
            Apply();
        }

        public void Apply()
        {
            // Don't do anything if target is not defined 
            if (target == null)
                return;

            // pushbuffer 
            if (pbuffer > 0)
                pbuffer -= Time.deltaTime;
            if (pbuffer < 0)
                pbuffer = 0;

            //Sidebuttonmovement 
//        if ((Input.GetAxis("Toggle Move") != 0) && (pbuffer == 0))
//        {
//            pbuffer = coolDown;
//            mouseSideButton = !mouseSideButton;
//        }

            //if (mouseSideButton && Input.GetAxis("Vertical") != 0)
            //    mouseSideButton = false;

            Vector3 vTargetOffset;

            /*
             * 以下鼠标操作的逻辑：
             *          1. 设置 lockToRearOfTarget 变量来让摄像机始终在 target 后方
             *          2. 鼠标按下后移动 鼠标可以调整摄像机x，y轴的旋转，鼠标只要不放下，就不会把摄像机旋转到对象后方
             *          3. 鼠标不按下移动 鼠标移动无效，摄像机始终要移动到 target 的后方
            */
            // If either mouse buttons are down, let the mouse govern camera position 
            /* 屏蔽GUI的鼠标操作 */
            if (GUIUtility.hotControl == 0)
            {
                if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
                {
                    //Check to see if mouse input is allowed on the axis 
                    if (allowMouseInputX)
                        xDeg += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
                    else
                        RotateBehindTarget();
                    if (allowMouseInputY)
                        yDeg -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;



                    //Interrupt rotating behind if mouse wants to control rotation 
                    if (!lockToRearOfTarget)
                        rotateBehind = false;
                }

                // otherwise, ease behind the target if any of the directional keys are pressed 
                else if (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0 || rotateBehind
                         )
                {
                    RotateBehindTarget();
                }
            }

            yDeg = ClampAngle(yDeg, yMinLimit, yMaxLimit);

            // Set camera rotation 
            Quaternion rotation = Quaternion.Euler(yDeg, xDeg, 0);

            // Calculate the desired distance 
            desiredDistance -= Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * zoomRate *
                               Mathf.Abs(desiredDistance);
            desiredDistance = Mathf.Clamp(desiredDistance, minDistance, maxDistance);
            correctedDistance = desiredDistance;

            // Calculate desired camera position 
            vTargetOffset = new Vector3(0, -targetHeight, 0);       
            /* 原来人物坐标原点在脚上，这时候设置目标高度为身高可以起到盯着人物头部看的效果 */
            /*目标坐标 - （摄像机的旋转方向 + 偏移） 作为摄像机的位置向量 */
            Vector3 position = target.transform.position - (rotation * Vector3.forward * desiredDistance + vTargetOffset);

            // Check for collision using the true target's desired registration point as set by user using height 
            RaycastHit collisionHit;
            Vector3 trueTargetPosition = new Vector3(target.transform.position.x, target.transform.position.y + targetHeight, target.transform.position.z);

            // If there was a collision, correct the camera position and calculate the corrected distance 
            var isCorrected = false;
            /* 在 观察点（因为有高度偏移，所以可以将观察点偏移到角色头部） 和 计算出的摄像机坐标 之间进行射线检测，若有遮挡物则
             * 将遮挡点设置在遮挡物之后
             */
            if (Physics.Linecast(trueTargetPosition, position, out collisionHit, collisionLayers))
            {
                // Calculate the distance from the original estimated position to the collision location, 
                // subtracting out a safety "offset" distance from the object we hit.  The offset will help 
                // keep the camera from being right on top of the surface we hit, which usually shows up as 
                // the surface geometry getting partially clipped by the camera's front clipping plane.
                correctedDistance = Vector3.Distance(trueTargetPosition, collisionHit.point) - offsetFromWall;
                isCorrected = true;
            }

            /*  */
           if( correctedDistance > currentDistance)
            {
                Debug.Log("找到了！！！");
            }

            // For smoothing, lerp distance only if either distance wasn't corrected, or correctedDistance is more than currentDistance
            currentDistance = !isCorrected || correctedDistance > currentDistance
                ? Mathf.Lerp(currentDistance, correctedDistance, Time.deltaTime * zoomDampening)
                : correctedDistance;

            // Keep within limits 
            currentDistance = Mathf.Clamp(currentDistance, minDistance, maxDistance);

            // Recalculate position based on the new currentDistance 
            position = target.transform.position - (rotation * Vector3.forward * currentDistance + vTargetOffset);

            //Finally Set rotation and position of camera 
            m_camera.rotation = rotation;
            m_camera.position = position;
        }

        /* 摄像机移动到 target 后方 */
        private void RotateBehindTarget()
        {
            float targetRotationAngle = target.transform.eulerAngles.y;  /* 目标的欧拉角 */
            float currentRotationAngle = transform.eulerAngles.y;        /* 当前的欧拉角 */
            xDeg = Mathf.LerpAngle(currentRotationAngle, targetRotationAngle, rotationDampening * Time.deltaTime);  /* 使用移动速度控制时间进行插值 */

            // Stop rotating behind if not completed 
            /* 不能直接比较，应该去比较两个差值小于某个值 */
 //           if (targetRotationAngle == currentRotationAngle)
            if(Mathf.Abs(currentRotationAngle - targetRotationAngle) <= 0.2f)
            {
                xDeg = targetRotationAngle;
                if (!lockToRearOfTarget)
                    rotateBehind = false;
            }
            else
                rotateBehind = true;
        }

        private float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360f)
                angle += 360f;
            if (angle > 360f)
                angle -= 360f;
            return Mathf.Clamp(angle, min, max);
        }
    }
}