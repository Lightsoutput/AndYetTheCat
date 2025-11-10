using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZhongliShiban : MonoBehaviour
{
    private Animator shizhuAnmi;
    private Animator shibanAnmi;
    // Start is called before the first frame update
    void Start()
    {
        GameObject shizhu = GameObject.Find("Shizhu");
        GameObject zhongliShiban = GameObject.Find("ZhongliShiban");

        shizhuAnmi = shizhu.GetComponent<Animator>();
        shibanAnmi = zhongliShiban.GetComponent<Animator>();
    }

    // 猫咪碰撞到 触发器形式
    private void OnTriggerEnter2D(Collider2D collision)
    {
        string isCollideStr = "isCollided";
        bool isCollided = false;

        // 如果碰撞对象是猫咪
        if(collision.gameObject.CompareTag("Cat"))
        {
            isCollided = true;
            shibanAnmi.SetBool(isCollideStr, isCollided);
            // 等待0.5s
            StartCoroutine(waitHalfSecound());
            shizhuAnmi.SetBool(isCollideStr, isCollided);
        }
    }

    private IEnumerator waitHalfSecound()
    {
        yield return new WaitForSeconds(0.5f);
    }
}
