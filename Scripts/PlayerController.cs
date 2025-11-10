using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // 1.获取到人物相关组件
    [SerializeField] private GameObject player;
    private SpriteRenderer playerSR;
    private Rigidbody2D playerRB;
    private BoxCollider2D playerCld;
    private Animator playerAnim;

    // 2.移动相关参数
    // x方向输入
    private float inputX;
    // 是否在移动
    private bool isMoving;
    // 是否在地上（方便跳跃判断）
    private bool isGround;
    private bool isJumping;
    private bool isJumpHold;
    float jumpTime = 0f;
    // 移动速度修改
    [SerializeField] private float moveSpeed;
    // 跳跃力修改
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpTimeHold;

    // 3.动画参数字符串
    private string isMovingStr = "isMoving";
    private string isGroundStr = "isGround";
    private string isJumpingStr = "isJumping";
    private string velocityYStr = "velocityY";

    void Start()
    {
        // 人物的图片、刚体、碰撞等组件
        playerSR = player.GetComponent<SpriteRenderer>();
        playerRB = player.GetComponent<Rigidbody2D>();
        playerCld = player.GetComponent<BoxCollider2D>();
        playerAnim = player.GetComponent<Animator>();
        // 速度、跳跃力度、长按跳跃时间初始化
        moveSpeed = 5f;
        jumpForce = 7.5f;
        jumpTimeHold = 0.2f;

        // 状态初始化
        isMoving = false;
        isGround = true;
        isJumping = false;
        isJumpHold = false;
    }

    void Update()
    {
        GetInput();
        PlayerMove();
        GroundRayCast();
        PlayerJump();
    }

    void GetInput()
    {
        // 获取x方向输入 A/D键 并判断是否在移动
        inputX = Input.GetAxis("Horizontal");
        isMoving = inputX != 0;
        // 设置动画参数
        playerAnim.SetBool(isMovingStr, isMoving);
    }

    void PlayerMove()
    {
        // 根据输入方向翻转人物图片
        playerSR.flipX = inputX < 0 ? true : (inputX > 0 ? false : playerSR.flipX);
        // 根据输入设置人物速度 实现移动
        playerRB.velocity = new Vector2(inputX * moveSpeed, playerRB.velocity.y);
    }

    void GroundRayCast()
    {
        // 从人物脚下发射一条射线 判断是否在地上
        Vector3 rayOriginLeft = playerCld.bounds.center + new Vector3(-playerCld.bounds.extents.x, -playerCld.bounds.extents.y, 0);
        Vector3 rayOriginRight = playerCld.bounds.center + new Vector3(playerCld.bounds.extents.x, -playerCld.bounds.extents.y, 0);

        // 只检测地面图层（Layer 3）
        RaycastHit2D hitL = Physics2D.Raycast(rayOriginLeft, Vector2.down, 0.1f, 1 << 3);
        RaycastHit2D hitR = Physics2D.Raycast(rayOriginRight, Vector2.down, 0.1f, 1 << 3);

        // hit到，则说明在地上
        isGround = hitL.collider || hitR.collider;
        isJumping = !isGround;
        // 设置isGround为true
        playerAnim.SetBool(isGroundStr, isGround);
        playerAnim.SetBool(isJumpingStr, isJumping);
    }

    void PlayerJump()
    {
        // 空格键 跳跃
        if (Input.GetKeyDown(KeyCode.Space) && isGround)
        {
            playerRB.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            // 设置isGround为false
            isGround = false;
            isJumping = true;
            isJumpHold = true;
            playerAnim.SetBool(isGroundStr, isGround);
            playerAnim.SetBool(isJumpingStr, isJumping);
            // 设置跳跃时间
            jumpTime = jumpTimeHold + Time.time;
        }

        // 单独的长按跳跃逻辑
        else if (isJumping && isJumpHold)
        {
            // 保持长按时 施加一个小一点的向上的力
            if (Input.GetKey(KeyCode.Space))
            {
                playerRB.AddForce(Vector2.up * jumpForce * 0.015f, ForceMode2D.Impulse);
            }
            // 直到按的时间超过了jumpTimeHold时间
            if (Time.time > jumpTime)
            {
                isJumpHold = false;
            }
        }

        // 设置跳跃动画的y值
        playerAnim.SetFloat(velocityYStr, playerRB.velocity.y);
    }
}
