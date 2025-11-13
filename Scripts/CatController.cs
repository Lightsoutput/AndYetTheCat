using System.Collections;
using UnityEngine;

public class CatController : MonoBehaviour
{
    // 1.获取到人物相关组件
    [SerializeField] private GameObject cat;
    private SpriteRenderer catSR;
    private Rigidbody2D catRB;
    private BoxCollider2D catCld;
    private Animator catAnim;

    // 2.移动相关参数
    // x方向输入
    private float inputX;
    // 是否在移动
    private bool isMoving;
    // 是否在地上（方便跳跃判断）
    public bool isGround;
    // 移动速度修改
    [SerializeField] private float moveSpeed;

    // 3.动画参数字符串
    private string isMovingStr = "isMoving";
    private string isGroundStr = "isGround";
    private string isCalledStr = "isCalled";
    public bool isCalled;

    // 4.获取到人物、鱼
    public GameObject player;
    public GameObject fish;
    private SpriteRenderer fishSR;
    [SerializeField] private PlayerController pcl;
    [SerializeField] private PlayerThrowController ptcl;

    void Start()
    {
        // 人物的图片、刚体、碰撞等组件
        catSR = cat.GetComponent<SpriteRenderer>();
        catRB = cat.GetComponent<Rigidbody2D>();
        catCld = cat.GetComponent<BoxCollider2D>();
        catAnim = cat.GetComponent<Animator>();
        // 速度初始化
        moveSpeed = 5f;
        // 状态初始化
        isMoving = false;
        isGround = true;
        // 获取人物、鱼
        player = GameObject.Find("Player");
        fish = GameObject.Find("Fish");
        fishSR = fish.GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        GetInput();
        CatMove();
        GroundRayCast();
    }

    void GetInput()
    {
        // 获取x方向输入 A/D键 并判断是否在移动
        inputX = Input.GetAxis("Horizontal");
        isMoving = inputX != 0;
        // 设置动画参数
        catAnim.SetBool(isMovingStr, isMoving);
    }

    void CatMove()
    {
        // 根据输入方向翻转人物图片
        catSR.flipX = inputX < 0 ? true : (inputX > 0 ? false : catSR.flipX);
        // 根据输入设置人物速度 实现移动
        // 如果是呼唤模式 则不用这个输入的速度
        if (!isCalled)
        {
            catRB.velocity = new Vector2(inputX * moveSpeed, catRB.velocity.y);
        }
    }

    private void GroundRayCast()
    {
        // 从人物脚下发射一条射线 判断是否在地上
        Vector3 rayOrigin = catCld.bounds.center + new Vector3(0, -catCld.bounds.extents.y + 0.1f, 0);
        // 只检测地面图层（Layer 3）
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, 1f, 1 << 3);
        // hit到，则说明在地上
        // 另一种方法：通过碰撞体检测地面
        isGround = catRB.IsTouchingLayers(1 << 3) || hit.collider;
        catAnim.SetBool(isGroundStr, isGround);
    }

    // 人物呼唤时 猫咪跳跃 这里作为动画事件调用函数
    public void CatJump()
    {
        // 开始呼唤 这时开始不适用输入的水平速度
        isCalled = true;
        // 斜向上加一个力
        float rightForce = pcl.isCatAtLeft ? 15f : -15f; // 注意水平力的朝向 让猫咪往人跳
        float upForce = 25f;
        Vector2 jumpForce = new Vector2(rightForce, upForce);
        catRB.AddForce(jumpForce, ForceMode2D.Impulse);
        Debug.Log("catRB velocity: " + catRB.velocity);

        // 协程异步执行 解决跳跃动画播放问题
        StartCoroutine(ProcessCalling());
    }

    // 协程 处理猫跳跃的状态
    IEnumerator ProcessCalling()
    {
        // 等待几秒 防止一开始错误错判断
        yield return new WaitForSeconds(0.1f);
        while (!isGround)
        {
            yield return null;
        }
        // 等待猫咪落地后，再关闭猫咪跳跃动画状态
        isCalled = false;
        catAnim.SetBool(isCalledStr, isCalled);
    }

    // 猫咪吃鱼动画事件 在动画中调用
    public void CatEatFishAnim() {
        StartCoroutine(CatEatFishAnimIE());
    }
    private IEnumerator CatEatFishAnimIE()
    {
        float time = 0f;
        float totalTime = 4f;
        float nowAlpha = fishSR.color.a;
        float alpha = fishSR.color.a;
        // 4s内 透明度降到0
        while (time < totalTime)
        {
            // 如果在移动或者被呼唤 则停止吃鱼 透明度立刻降到0
            if(isMoving || isCalled) 
            {
                fishSR.color = new Color(fishSR.color.r, fishSR.color.g, fishSR.color.b, 0);
                yield break;
            }
            // 如果重新扔 立刻再回到满不透明度
            if(ptcl.isThrowing || ptcl.isGrabing)
            {
                fishSR.color = new Color(fishSR.color.r, fishSR.color.g, fishSR.color.b, 1);
                yield break;
            }
            // 按时间线性插值计算透明度
            nowAlpha = Mathf.Lerp(alpha, 0, time / totalTime);
            fishSR.color = new Color(fishSR.color.r, fishSR.color.g, fishSR.color.b, nowAlpha);
            time += Time.deltaTime;
            yield return null;
        }
    }
}
