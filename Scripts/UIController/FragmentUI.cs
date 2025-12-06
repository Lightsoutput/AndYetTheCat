using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FragmentUI : MonoBehaviour
{
    // 碎片UI
    [SerializeField] Image fragmentUI_item1;
    [SerializeField] Image fragmentUI_item2;
    [SerializeField] Image fragmentUI_item3;
    // 实际的三个碎片
    [SerializeField] private GameObject fg1;
    [SerializeField] private GameObject fg2;
    [SerializeField] private GameObject fg3;
    void Start()
    {
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
        // 碰撞后，给碎片UI添加效果，用协程实现
        StartCoroutine(UIFragementAnim(fragmentUI_item1));
    }
    private void OnFg2TriggerEnter2D(Collider2D cld)
    {
        // 碰撞后，给碎片UI添加效果，用协程实现
        StartCoroutine(UIFragementAnim(fragmentUI_item2));
    }
    private void OnFg3TriggerEnter2D(Collider2D cld)
    {
        // 碰撞后，给碎片UI添加效果，用协程实现
        StartCoroutine(UIFragementAnim(fragmentUI_item3));
    }

    IEnumerator UIFragementAnim(Image fgUI)
    {
        float nowTime = Time.time;
        float deltaTime = 0f;
        while(deltaTime < 1f)
        {
            deltaTime = Time.time - nowTime;
            // 1.逐渐放大UI视觉效果
            fgUI.transform.localScale = new Vector3(1f + 0.2f * deltaTime, 1f + 0.2f * deltaTime, 1);
            yield return null;
        }

        nowTime = Time.time;
        deltaTime = 0f;
        while (deltaTime < 1f)
        {
            deltaTime = Time.time - nowTime;
            // 2.逐渐取消黑色遮罩
            fgUI.color = new Color(deltaTime, deltaTime, deltaTime, deltaTime);
            yield return null;
        }

        nowTime = Time.time;
        deltaTime = 0f;
        while (deltaTime < 1f)
        {
            deltaTime = Time.time - nowTime;
            // 3.变回原来的大小
            fgUI.transform.localScale = new Vector3(1f - 0.1f * deltaTime, 1f - 0.1f * deltaTime, 1);
            yield return null;
        }

        yield return null;
    }
}
