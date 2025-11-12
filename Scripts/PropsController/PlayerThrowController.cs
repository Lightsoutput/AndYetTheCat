using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerThrowController : MonoBehaviour
{
    // 鼠标位置的世界坐标
    private Vector2 mouse_Pos;

    // 轨迹线相关
    public LineRenderer line1;
    public int line1Num = 10;
    Vector3[] points1;

    // 力度线相关
    public LineRenderer line2;
    public float maxForce = 2;
    int line2Num = 2;
    Vector3[] points2;

    // 刚体组件与力度参数
    Rigidbody2D rb2D;
    public float releaseForce;
    Vector2 release_Velocity;
    float S;
    float t;
    // 这里我们的Untiy编辑器设置的重力是50
    float g = 50f;
    public Transform ground;
    float height;
    float xUnit = .1f;

    // 碰撞组件
    BoxCollider2D fishCld;
    public PhysicsMaterial2D guanghuaBound;
    public PhysicsMaterial2D fishBound;

    // 地面图层掩码
    public LayerMask groundLayer;
    // 力度拖拽示意点物体对象
    public GameObject dragPoint;
    // 图片渲染组件
    SpriteRenderer fishSR;
    // 轨迹线的起点颜色
    Vector4 fadeLine = new Vector4(1, 1, 1, 1);

    // 人物相关信息
    public GameObject player;
    private Vector3 playerPos;
    private Animator playerAnim;
    private SpriteRenderer playerSR;
    // 猫咪相关信息
    public GameObject cat;

    // 是否开始投掷或落地状态
    public bool isThrowing;
    public bool isGrabing;
    public bool isGround;

    // 状态枚举
    //void OnMouseDown()  => next_state = STATE.GRAB;
    //void OnMouseUp() => next_state = STATE.RELEASE;
    enum STATE
    {
        NONE = -1,
        IDLE = 0,    // 静止
        GRAB,        // 抓取
        DRAG,        // 拖拽
        RELEASE,     // 松手
        LAND,        // 落地
        NUM
    }

    private STATE state = STATE.IDLE;     // 初始状态
    private STATE next_state = STATE.NONE; // 下一个状态

    void Start()
    {
        // 设置Line Renderer的绘制点数
        line1.positionCount = line1Num;
        line2.positionCount = line2Num;

        // 根据点数确定数组大小
        points1 = new Vector3[line1Num];
        points2 = new Vector3[line2Num];

        // 获取刚体、图片组件
        rb2D = GetComponent<Rigidbody2D>();
        fishCld = GetComponent<BoxCollider2D>();
        fishSR = GetComponent<SpriteRenderer>();

        // 获取player的位置
        player = GameObject.Find("Player");
        playerAnim = player.GetComponent<Animator>();
        playerSR = player.GetComponent<SpriteRenderer>();
        cat = GameObject.Find("Cat");
        fishSR.enabled = false;
        line1.enabled = false;
        line2.enabled = false;
    }

    void Update()
    {
        // 鼠标位置debug
        // mousePosDebug();
        playerPos = player.transform.position;

        // 调用监听开始扔鱼干按钮的方法
        ThrowFishListener();

        // 获取鼠标位置的世界坐标
        // 注：这里本来获得的世界坐标，如果不改是不动的，必须和screen的（这个才是变化的）鼠标坐标有个投影
        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = -Camera.main.transform.position.z; // 摄像机到z=0平面的距离
        mouse_Pos = Camera.main.ScreenToWorldPoint(mouseScreenPos);

        // 检测状态改变 持续进行
        switch (state)
        {
            case STATE.DRAG:
                // 在这里根据鼠标方向 改变人物的朝向
                // 鼠标在左边 人朝右边 鱼也在左边
                if (mouse_Pos.x < player.transform.position.x)
                {
                    playerSR.flipX = false;
                    this.transform.position = new Vector3(playerPos.x - 1, playerPos.y + 1, playerPos.z);
                }
                // 鼠标在右边 人朝左边 鱼也在右边
                else
                {
                    playerSR.flipX = true;
                    this.transform.position = new Vector3(playerPos.x + 1, playerPos.y + 1, playerPos.z);
                }
                break;
            case STATE.RELEASE:
                // 放手状态中，如果接触到地面且竖直速度比较小了，则进入落地状态
                if (rb2D.IsTouchingLayers(groundLayer) && rb2D.velocity.y <= 0.5f)
                {
                    next_state = STATE.LAND;
                }
                break;
            case STATE.LAND:
                // 落地状态中，如果小球静止，则进入静止状态
                if (rb2D.velocity.x <= 0.2f && rb2D.velocity.y <= 0.2f)
                {
                    // 准备开协程 让小猫过去
                    isGround = true;
                    StartCoroutine(CatMoveToFish());
                    next_state = STATE.IDLE;
                }
                break;
        }

        // 状态初始化（有状态改变才执行一次）
        if (next_state != STATE.NONE)
        {
            // 打印当前和下一个状态
            // Debug.Log("当前" + state.ToString() + "，下一个状态" + next_state.ToString());
            state = next_state;
            next_state = STATE.NONE;

            switch (state)
            {
                case STATE.IDLE:
                    // 重置
                    isThrowing = false;
                    isGrabing = false;
                    isGround = false;
                    playerAnim.SetBool("isGrabing", isGrabing);
                    // 使刚体静止 清除之前的速度
                    rb2D.velocity = Vector2.zero;
                    // 关闭重力影响
                    rb2D.gravityScale = 0;
                    line1.enabled = false;
                    line2.enabled = false;
                    dragPoint.SetActive(false);
                    break;
                case STATE.GRAB:
                    isThrowing = true;
                    // 开始抓取 准备动画
                    isGrabing = true;
                    playerAnim.SetBool("isGrabing", isGrabing);
                    // 获取小球离地高度
                    height = transform.position.y - ground.position.y;
                    // 重新显示小鱼图片，设置透明度为1（不透明）
                    fishSR.enabled = true;
                    fishSR.color = new Color(fishSR.color.r, fishSR.color.g,
                                            fishSR.color.b, 1);
                    // 重新显示抛物线
                    line1.enabled = true;
                    line1.startColor = new Vector4(1, 1, 1, 1);
                    line2.enabled = true;
                    dragPoint.SetActive(true);
                    // 准备进入拖拽阶段
                    StartCoroutine(EnterDragNextFrame());
                    next_state = STATE.DRAG;
                    break;
                case STATE.RELEASE:
                    isGrabing = false;
                    playerAnim.SetBool("isGrabing", isGrabing);
                    // 设置小球的刚体属性
                    rb2D.drag = 0;          // 线性阻力
                    rb2D.gravityScale = 1;  // 重力影响
                    rb2D.velocity = release_Velocity; // 初速度
                    fishCld.sharedMaterial = guanghuaBound; // 扔出时换成光滑材质
                    // 隐藏力度线
                    line2.enabled = false;
                    dragPoint.SetActive(false);
                    break;
                case STATE.LAND:
                    // 对小球附加额外阻力，使其可以停下
                    // 落地时更换材质为原有摩擦材质
                    fishCld.sharedMaterial = fishBound;
                    rb2D.drag = 1.8f;
                    break;
            }

            // 这里得放在状态改变后来判断 否则会覆盖下面的初始化状态
            if (isGrabing)
            {
                fishSR.enabled = true;
                // 让鱼干位置变到人的位置
                this.transform.position = new Vector3(playerPos.x - 1, playerPos.y + 1, playerPos.z);
            }
        }

        // 状态执行
        switch (state)
        {
            case STATE.DRAG:
                // 连接鼠标和小球位置的线段
                points2[0] = transform.position;
                points2[1] = mouse_Pos;

                // 如果超过最大长度，则限制为最大长度
                if (Vector3.Distance(points2[0], points2[1]) > maxForce)
                {
                    points2[1] = points2[0] + (points2[1] - points2[0]).normalized * maxForce;
                }
                line2.SetPositions(points2);

                // 末端小点的位置
                dragPoint.transform.position = points2[1];

                // 轨迹抛物线-初速度
                release_Velocity = (points2[0] - points2[1]) * releaseForce;

                // 落地的最大水平位移
                // S = Vx * t  这里 t 通过 h~vt+1/2gt² 用求根公式算出来
                S = release_Velocity.x * (release_Velocity.y / g
                    + Mathf.Sqrt((release_Velocity.y * release_Velocity.y / g / g) + 2 * height / g));

                // 根据水平位移确定轨迹的X轴绘制间隔
                xUnit = S / line1Num;

                // 结合LineRender绘制图像
                for (int i = 0; i < line1Num; i++)
                {
                    points1[i].x = transform.position.x + i * xUnit;
                    points1[i].y = GetFuncPathY(points1[i].x);
                }
                line1.SetPositions(points1);
                break;
            case STATE.RELEASE:
                // 轨迹线逐渐消失
                fadeLine.w -= Time.deltaTime * 2;
                Mathf.Clamp01(fadeLine.w);
                line1.startColor = fadeLine;
                break;
        }
    }

    // 获取函数Y坐标，即抛物线的轨迹方程
    // 二维抛体运动方程（Projectile Motion Equation）的解析形式
    // “x”来表示时间 t 的替代，从而得到 y 关于 x 的函数。
    float GetFuncPathY(float x)
    {
        float y;
        y = (release_Velocity.y / release_Velocity.x) * (x - transform.position.x)
            - (g * (x - transform.position.x) * (x - transform.position.x)) / (2 * release_Velocity.x * release_Velocity.x)
            + transform.position.y;
        return y;
    }

    // 暂停协程 
    IEnumerator EnterDragNextFrame()
    {
        yield return null;  // 等待一帧
        state = STATE.DRAG;
    }

    // 控制扔鱼干监听事件
    private void ThrowFishListener()
    {
        // 按下T以后，切换开始扔鱼干
        if (Input.GetKeyDown(KeyCode.T))
        {
            if(state == STATE.IDLE)
            {
                next_state = STATE.GRAB;
            }
            else if(state == STATE.GRAB || state == STATE.DRAG)
            {
                // 重新隐藏鱼
                fishSR.enabled = false;
                next_state = STATE.IDLE;
            }
        }
        // 松手后，扔出鱼干
        if (Input.GetMouseButtonUp(0) && state == STATE.DRAG)
        {
            next_state = STATE.RELEASE;
        }
    }

    // 协程 用于在鱼干落地1s后 猫移动过去
    private IEnumerator CatMoveToFish()
    {
        float time = 0f;
        float moveTime = 1f;
        bool isCatMoving;

        // 等待1s
        while (time <= moveTime)
        {
            time += Time.deltaTime;
            yield return null;
        }

        Transform catTrans = cat.transform;
        Transform fishTrans = this.transform;
        float catMoveSpeed = 5f;
        Animator catAnim = cat.GetComponent<Animator>();
        SpriteRenderer catSR = cat.GetComponent<SpriteRenderer>();
        string isMovingStr = "isMoving";

        // 先定位置 然后每帧都要往前移动
        while (Vector3.Distance(catTrans.position, fishTrans.position) > 1.6f)
        {
            isCatMoving = true;
            catAnim.SetBool(isMovingStr, isCatMoving);
            Vector3 dir = fishTrans.position - catTrans.position;
            dir.Normalize();

            // 看dir的x反转图片
            catSR.flipX = dir.x < 0f;
            // MoveTowards平滑移动
            catTrans.position = Vector3.MoveTowards(catTrans.position, fishTrans.position, catMoveSpeed * Time.deltaTime);
            yield return null;
        }
        isCatMoving = false;
        catAnim.SetBool(isMovingStr, isCatMoving);
        // 猫到达，开始吃鱼
        // 连接过去用触发器 回来勾选ExitTime=2 吃两次就结束
        catAnim.SetTrigger("isArrivedAtFish");
    }

    //// 按下鼠标，进入抓取状态 OnMouseDown()
    //private void OnMouseDown()
    //{
    //    next_state = STATE.GRAB;
    //}

    //// 松开鼠标，进入放手状态 OnMouseUp()
    //private void OnMouseUp()
    //{
    //    next_state = STATE.RELEASE;
    //}


    // 鼠标位置Debug
    void mousePosDebug()
    {
        // 鼠标位置debug
        Debug.Log($"state={state}, mousePos={mouse_Pos}");
        if (Camera.main == null)
        {
            Debug.LogError("Camera.main is NULL!");
        }
        else
        {
            Debug.Log($"mousePos screen = {Input.mousePosition}, world = {Camera.main.ScreenToWorldPoint(Input.mousePosition)}");
        }
    }

}
