using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindDetect : MonoBehaviour
{
    // 人、猫和鸟相关组件
    public GameObject player;
    public GameObject cat;
    public GameObject bird;
    private Animator playerAnim;
    private Animator catAnim;
    private Animator birdAnim;
    private SpriteRenderer playerSR;
    private SpriteRenderer catSR;
    private BoxCollider2D playerBC;
    private BoxCollider2D catBC;

    // 风区相关参数
    private bool isWind = false;
    private string isWindStr = "isWind";
    private string isWindedStr = "isWinded";

    // Start is called before the first frame update
    void Start()
    {
        playerAnim = player.GetComponent<Animator>();
        catAnim = cat.GetComponent<Animator>();
        birdAnim = bird.GetComponent<Animator>();
        playerBC = player.GetComponent<BoxCollider2D>();
        catBC = cat.GetComponent<BoxCollider2D>();
        playerSR = player.GetComponent<SpriteRenderer>();
        catSR = cat.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 人或猫进入风区
    private void OnCollisionEnter2D(Collision2D collision)
    {
        isWind = true;
        AnimationSet(isWind);
        StartCoroutine(windBack());
    }

    // 人或猫离开风区
    //private void OnCollisionExit2D(Collision2D collision)
    //{
    //    isWind = false;
    //    AnimationSet(isWind);
    //}

    // 动画状态设定
    private void AnimationSet(bool isWind)
    {
        playerAnim.SetBool(isWindedStr, isWind);
        catAnim.SetBool(isWindedStr, isWind);
        birdAnim.SetBool(isWindStr, isWind);
    }

    private IEnumerator windBack()
    {
        float time = 0f;
        float windDuration = 2.0f;
        playerSR.flipX = false;
        catSR.flipX = false;

        while(time <= windDuration)
        {
            player.transform.position = new Vector3(player.transform.position.x - 0.005f,
                                        player.transform.position.y,
                                        player.transform.position.z);
            cat.transform.position = new Vector3(cat.transform.position.x - 0.007f,
                                        cat.transform.position.y,
                                        cat.transform.position.z);
            time += Time.deltaTime;
            yield return null;
        }

        // 被吹风结束后 离开
        isWind = false;
        AnimationSet(isWind);
        yield return null;
    }
}
