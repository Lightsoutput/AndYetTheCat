using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FragmentsController : MonoBehaviour
{
    // 其实如果有多个碎片的话，可以考虑用数组、结构体、类来管理
    // 先注册三个碎片
    [SerializeField] private GameObject fg1;
    [SerializeField] private GameObject fg2;
    [SerializeField] private GameObject fg3;
    private Animator fgAnim1;
    private Animator fgAnim2;
    private Animator fgAnim3;
    private SpriteRenderer fgSpr1;
    private SpriteRenderer fgSpr2;
    private SpriteRenderer fgSpr3;

    // 碎片的动画参数
    private string exitStr = "exit";
    private bool exit1 = false;
    private bool exit2 = false;
    private bool exit3 = false;
    // 碎片渐隐和上升的速度
    [SerializeField] private float fadeSpeed = 0.5f;
    [SerializeField] private float riseSpeed = 2f;

    // Start is called before the first frame update
    void Start()
    {
        fgAnim1 = fg1.GetComponent<Animator>();
        fgAnim2 = fg2.GetComponent<Animator>();
        fgAnim3 = fg3.GetComponent<Animator>();
        fgSpr1 = fg1.GetComponent<SpriteRenderer>();
        fgSpr2 = fg2.GetComponent<SpriteRenderer>();
        fgSpr3 = fg3.GetComponent<SpriteRenderer>();

        // 为三个碎片注册触发器监听器 使得在另一个脚本中也能监听到碎片的触发器事件后，执行这里的脚本
        TriggerListener tl1 = fg1.GetComponent<TriggerListener>();
        tl1.OnTriggerEnterEvent += OnFg1TriggerEnter2D;
        TriggerListener tl2 = fg2.GetComponent<TriggerListener>();
        tl2.OnTriggerEnterEvent += OnFg2TriggerEnter2D;
        TriggerListener tl3 = fg3.GetComponent<TriggerListener>();
        tl3.OnTriggerEnterEvent += OnFg3TriggerEnter2D;
    }

    private void OnFg1TriggerEnter2D(Collider2D cld)
    {
        if(exit1) return;
        // Debug.Log("已收集碎片1");
        exit1 = true;
        fgAnim1.SetBool(exitStr, exit1);
        fgAnim1.Update(0f);              // 立即刷新 Animator
        fgAnim1.enabled = false;         // 禁用 Animator，让动画停止在当前状态
        FgExitAnim(fg1);
    }
    private void OnFg2TriggerEnter2D(Collider2D cld)
    {
        if(exit2) return;
        // Debug.Log("已收集碎片2");
        exit2 = true;
        fgAnim2.SetBool(exitStr, exit2);
        fgAnim2.Update(0f);              // 立即刷新 Animator
        fgAnim2.enabled = false;         // 禁用 Animator，让动画停止在当前状态
        FgExitAnim(fg2);
    }
    private void OnFg3TriggerEnter2D(Collider2D cld)
    {
        if(exit3) return;
        // Debug.Log("已收集碎片3");
        exit3 = true;
        fgAnim3.SetBool(exitStr, exit3);
        fgAnim3.Update(0f);              // 立即刷新 Animator
        fgAnim3.enabled = false;         // 禁用 Animator，让动画停止在当前状态
        FgExitAnim(fg3);
    }

    // 处理碰撞后 碎片向上渐隐和注销的逻辑
    private void FgExitAnim(GameObject fragment)
    {
        StartCoroutine(FragmentFadeIE(fragment));
    }

    IEnumerator FragmentFadeIE(GameObject fragment)
    {
        SpriteRenderer fsr = fragment.GetComponent<SpriteRenderer>();
        Color c = fsr.color;
        while (c.a > 0f)
        {
            fragment.transform.Translate(Vector3.up * riseSpeed * Time.deltaTime);
            c.a -= fadeSpeed * Time.deltaTime;
            // 原来的c是局部变量 需要重新赋值回去
            fsr.color = c;
            yield return null;
        }
    }
}
