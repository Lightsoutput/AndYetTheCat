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

    // 4.获取到人物
    public GameObject player;
    [SerializeField] private PlayerController pcl;

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
        // 获取人物
        player = GameObject.Find("player");
    }

    void Update()
    {
        GetInput();
        CatMove();
        GroundRayCast();
        CatJump();
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
        catSR.flipX = inputX < 0 ? true : (inputX>0 ? false : catSR.flipX);
        // 根据输入设置人物速度 实现移动
        // 如果是呼唤模式 则不用这个输入的速度
        if (!pcl.isCalled)
        {
            catRB.velocity = new Vector2(inputX * moveSpeed, catRB.velocity.y);
        }
        
    }

    void GroundRayCast()
    {
        // 从人物脚下发射一条射线 判断是否在地上
        Vector3 rayOrigin = catCld.bounds.center + new Vector3(0, -catCld.bounds.extents.y + 0.1f, 0);
        // 只检测地面图层（Layer 3）
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, 1f, 1<<3);
        // hit到，则说明在地上
        isGround = hit.collider;
        catAnim.SetBool(isGroundStr, isGround);
    }

    void CatJump()
    {
        //// W按钮 跳跃
        //if (Input.GetKeyDown(KeyCode.W) && isGround)
        //{
        //    catRB.AddForce(Vector2.up * jumpForce);
        //}
    }
}
