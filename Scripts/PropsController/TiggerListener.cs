using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerListener:MonoBehaviour
{
    // 作用：将OnTriggerEnter2D 回调“封装 + 转发”为一个可订阅的事件。

    // 当触发 OnTriggerEnter2D 时，会把“碰到 Trigger 的对象 Collider2D”作为参数传出去
    public event Action<Collider2D> OnTriggerEnterEvent;

    // 如果有人订阅了事件，就执行；否则什么都不做
    private void OnTriggerEnter2D(Collider2D collision)
    {
        OnTriggerEnterEvent?.Invoke(collision);
    }
}
