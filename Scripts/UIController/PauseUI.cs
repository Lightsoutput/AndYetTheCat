using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseUI : MonoBehaviour
{
    // 游戏内的暂停按钮
    [SerializeField] Button btn_Pause;
    [SerializeField] GameObject PauseMenuUI;
    // 暂停界面的按钮
    [SerializeField] Button btn1_Back;
    [SerializeField] Button btn2_Resume;
    [SerializeField] Button btn2_RePlay;
    [SerializeField] Button btn2_Back2Menu;
    [SerializeField] Button btn2_SaveGame;
    [SerializeField] Button btn2_LoadGame;
    [SerializeField] Button btn2_Setting;
    // 配置悬停时候的图片和文字颜色
    // 这里自己配置字颜色的时候，一定要注意不透明度啊
    // 改了半天没效果，原来不透明度一直是0，一直是透明的。。。
    [SerializeField] Sprite defaultSprite;
    [SerializeField] Sprite hoverSprite;
    [SerializeField] Color defaultTextColor;
    [SerializeField] Color hoverTextColor;


    // 给每个按钮的OnClick填上下面的方法
    private void Start()
    {
        // 注册监听的方法
        btn_Pause.onClick.AddListener(BtnPauseOnClick);
        btn1_Back.onClick.AddListener(BtnBackOnClick);
        btn2_Resume.onClick.AddListener(BtnBackOnClick);
        btn2_RePlay.onClick.AddListener(Btn2RePlayOnClick);
        btn2_Back2Menu.onClick.AddListener(Btn2Back2Menu);

        // 菜单内按钮 添加鼠标悬浮进入和退出动画
        BindHoverEvents(btn2_Resume);
        BindHoverEvents(btn2_RePlay);
        BindHoverEvents(btn2_Back2Menu);
        BindHoverEvents(btn2_SaveGame);
        BindHoverEvents(btn2_LoadGame);
        BindHoverEvents(btn2_Setting);
    }
    private void BtnPauseOnClick()
    {
        PauseMenuUI.SetActive(true);
        Time.timeScale = 0;
    }

    private void BtnBackOnClick()
    {
        PauseMenuUI.SetActive(false);
        Time.timeScale = 1;
    }

    private void Btn2RePlayOnClick()
    {
        StartCoroutine(LoadLevelAsync("Level1Scene"));
        Time.timeScale = 1; //记得恢复时间
    }

    private void Btn2Back2Menu()
    {
        StartCoroutine(LoadLevelAsync("MainMenu"));
        Time.timeScale = 1; //记得恢复时间
    }

    private void BindHoverEvents(Button btn)
    {
        // 监听鼠标事件组件
        EventTrigger t = btn.gameObject.GetComponent<EventTrigger>();
        if (t == null)
            t = btn.gameObject.AddComponent<EventTrigger>();
        // 对应Button的图片和文字组件
        Image img = btn.GetComponent<Image>();
        Text txt = btn.GetComponentInChildren<Text>();

        // 进入事件
        EventTrigger.Entry enter = new EventTrigger.Entry();
        enter.eventID = EventTriggerType.PointerEnter;
        enter.callback.AddListener((data) =>
        {
            txt.color = hoverTextColor;      // 改字颜色
            img.sprite = hoverSprite;       // 换背景
        });

        // 离开事件
        EventTrigger.Entry exit = new EventTrigger.Entry();
        exit.eventID = EventTriggerType.PointerExit;
        exit.callback.AddListener((data) =>
        {
            txt.color = defaultTextColor;     // 恢复字颜色
            img.sprite = defaultSprite;       // 恢复背景
        });

        // 点击事件 点下去也要恢复颜色 不然下次打开还有问题（除非每次打开都初始化一遍）
        EventTrigger.Entry press = new EventTrigger.Entry();
        press.eventID = EventTriggerType.PointerClick;
        press.callback.AddListener((data) =>
        {
            txt.color = defaultTextColor;     // 恢复字颜色
            img.sprite = defaultSprite;       // 恢复背景
        });

        t.triggers.Add(enter);
        t.triggers.Add(exit);
        t.triggers.Add(press);
    }

    // 异步加载场景的协程
    private IEnumerator LoadLevelAsync(string sceneName)
    {
        AsyncOperation asyncOp = SceneManager.LoadSceneAsync(sceneName);

        // 先可以允许自动切换场景 无加载动画
        asyncOp.allowSceneActivation = true;

        while (!asyncOp.isDone)
        {
            yield return null;
        }
    }
}
