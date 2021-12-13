using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sy
{
    public class CameraController : MonoBehaviour
    {
        public GameObject targetGO;             /* 被观测目标 */
        public float targetHeight = 1.8f;       /* 被观测目标的高度（用于调整观测点） */
        public float defaultDistance = 3.0f;    /* 默认观测距离 */
        public float maxDistance = 20.0f;       /* 最大观测距离（使用鼠标滚轮调整） */
        public float minDistance = 1.5f;        /* 最小观测距离 */
        public float xSpeed = 200.0f;           /* 鼠标拖动摄像机移动速度（左右） */
        public float ySpeed = 200.0f;           /* 鼠标拖动摄像机移动速度（上下） */
        public float yAngleMinLimit = -80.0f;   /* 俯仰角的下届 */
        public float yAngleMaxLimit = 80.0f;    /* 俯仰角的上届 */
        public float zoomSpeed = 40.0f;         /* 鼠标中键的速度 */
        [Range(0.01f, 1f)]
        public float zoomSmooth = 0.2f;         /* 鼠标中键移动的平滑度（越大越平滑） */
        [Range(0.01f, 1f)]
        public float rotateSmooth = 0.5f;       /* 旋转到 target 后方的平滑度（越大越平滑） */
        public bool allwaysBackOfTarget = false;/* 始终处于角色后方 */
        public bool enableMouseInputX = true;   /* 鼠标输入 X */
        public bool enableMouseInputY = true;   /* 鼠标输入 Y */
        public LayerMask collisionLayer = 3841; /* 摄像机更新位置时，会基于判断新位置与 target 中间是否有遮挡物  */
        public float offsetFormWall = 0.1f;     /* 摄像机进行碰撞检测后，更新新位置要与碰撞点有个偏移 */

        private float xDegree;
        private float yDegree;
        private float currentDistance;          /* 当前距离 */
        private float objectDiatance;           /* 计算出来的 客观距离 */
        private float finallDistance;           /* 经过碰撞判断后的最终待更新距离 */
        private bool rotateBackIsFinish = true; /* 将摄像头旋转到背后是否结束 */

        private void Start()
        {
            Vector3 eulerAngle = this.transform.eulerAngles;
            xDegree = eulerAngle.x;
            yDegree = eulerAngle.y;

            currentDistance = objectDiatance = finallDistance = defaultDistance;

            if (allwaysBackOfTarget)
                rotateBackIsFinish = false;

            /* 若没有设置目标对象，则尝试在项目中查找 tag 为 “Player” 的游戏对象*/
            if(targetGO == null)
            {
                targetGO = GameObject.FindGameObjectWithTag("Player") as GameObject;
            }

            this.gameObject.layer = 2;
        }

        private void LateUpdate()
        {
            if (targetGO == null)
                return;

            if (GUIUtility.hotControl == 0)     /* 过滤掉 GUI 的鼠标操作 */
            {
                /* 鼠标按下 */
                if (Input.GetMouseButton(0) || Input.GetMouseButton(1)) 
                {
                    if (enableMouseInputX)
                        xDegree += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
                    else
                        RotateToBackOfTarget();
                    if (enableMouseInputY)
                        yDegree -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;

                    /* 若鼠标按下时，正在进行旋转到背后操作，那么打断这个操作 */
                    if (allwaysBackOfTarget == false)
                        rotateBackIsFinish = true;

                }

                /* 1. 若鼠标没有按下，但是键盘按下啦，
                 * 2. 或者旋转到背后过程还没结束（使用插值进行平滑，所以旋转到背后可能不会在一帧中完成） */
                else if (rotateBackIsFinish == false ||          
                        Input.GetAxis("Vertical") != 0 ||
                        Input.GetAxis("Horizontal") != 0)
                {
                    RotateToBackOfTarget();
                }
            }

            yDegree = ClampAngle(yDegree, yAngleMinLimit, yAngleMaxLimit);

            /* 摄像机的旋转角度（世界坐标系） */
            Quaternion rotation = Quaternion.Euler(yDegree, xDegree, 0);

            /* 获取当前的滚轮操作准备更新距离（非线性更新）距离越远，速度越快 */
            objectDiatance -= Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * zoomSpeed * Mathf.Abs(objectDiatance);
            objectDiatance = Mathf.Clamp(objectDiatance, minDistance, maxDistance);
            finallDistance = objectDiatance;

            /* 原来人物坐标原点在脚上，这时候设置目标高度为身高可以起到盯着人物头部看的效果 */
            /*目标坐标 - （摄像机的旋转方向 + 偏移） 作为摄像机的位置向量 */
            Vector3 vecTargerOffset = new Vector3(0.0f, -targetHeight, 0.0f);
            Vector3 position = targetGO.transform.position - (rotation * Vector3.forward * objectDiatance + vecTargerOffset);

            RaycastHit resultHit;
            /* 射线起点为我们的观察点 */
            Vector3 rayStartPoint = targetGO.transform.position - vecTargerOffset;
            bool hasOtherObject = false;
            if(Physics.Linecast(rayStartPoint, position, out resultHit, collisionLayer))
            {
                finallDistance = Vector3.Distance(rayStartPoint, resultHit.point) - offsetFormWall;
                hasOtherObject = true;
            }

            /* 射线 没有 检测到碰撞  或者   用户 增加了距离 */
            if(hasOtherObject == false || finallDistance > currentDistance )
            {
                currentDistance = Mathf.Lerp(currentDistance, finallDistance, Time.deltaTime * (1.0f / zoomSmooth));
            }
            else
            {
                currentDistance = finallDistance;
            }

            /* 约束在最大最小距离内 */
            currentDistance = Mathf.Clamp(currentDistance, minDistance, maxDistance);

            /* 再次计算坐标 */
            position = targetGO.transform.position - (rotation * Vector3.forward * currentDistance + vecTargerOffset);

            /* 设定摄像机位置与旋转 */
            transform.position = position;
            transform.rotation = rotation;
        }

        /* 该函数只是完成了旋转角度的插值，并没有真正设置摄像机的旋转角 */
        private void RotateToBackOfTarget()
        {
            float targetEulerAngle = targetGO.transform.eulerAngles.y;
            float currentEulerAngle = this.transform.eulerAngles.y;
            xDegree = Mathf.LerpAngle(currentEulerAngle, targetEulerAngle, (0.25f / rotateSmooth) * Time.deltaTime);

            /* 插值接近完成 */
            if (Mathf.Abs(currentEulerAngle - targetEulerAngle) <= 0.2f)
            {
                xDegree = targetEulerAngle;
                if(allwaysBackOfTarget == false)
                {
                    rotateBackIsFinish = true;
                }
            }
            else
            {
                rotateBackIsFinish = false;
            }
        }

        private float ClampAngle(float value, float min, float max)
        {
            float ret;

            while (value <= -180.0f)    /* 将所有角度限定到 (-180,180] 的范围 */
                value += 360.0f;
            while (value > 180.0f)
                value -= 360.0f;

            while (min <= -180.0f)
                min += 360.0f;
            while (min > 180.0f)
                min -= 360.0f;

            while (max <= -180.0f)
                max += 360.0f;
            while (max > 180.0f)
                max -= 360.0f;

            ret = Mathf.Clamp(value, min, max);
            return ret;
        }

    }
}
