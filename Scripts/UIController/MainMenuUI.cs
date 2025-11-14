using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    // UI页面 根节点
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject levelSelect;
    private VisualElement mainMenuRoot;
    private VisualElement levelSelectRoot;

    // UI下其他组件
    private Button mainMenu_startBtn;
    private Button levelSelect_backBtn;
    List<VisualElement> levelSelect_levelItems;

    // Start is called before the first frame update
    void Start()
    {
        // 绑定相关UI和组件
        mainMenuRoot = mainMenu.GetComponent<UIDocument>().rootVisualElement;
        levelSelectRoot = levelSelect.GetComponent<UIDocument>().rootVisualElement;

        // 绑定按钮和相关方法
        mainMenu_startBtn = mainMenuRoot.Q<Button>("StartBtn");
        mainMenu_startBtn.clicked += Switch_MainMenu2LevelSelect;
        levelSelect_backBtn = levelSelectRoot.Q<Button>("BackBtn");
        levelSelect_backBtn.clicked += Switch_LevelSelect2MainMenu;

        // 获取关卡选择页面的关卡项列表
        levelSelect_levelItems = levelSelectRoot.Query<VisualElement>("LevelContainer").ToList();
        // 并为每个关卡项绑定点击事件
        for (int i = 0; i < levelSelect_levelItems.Count; i++)
        {
            // 缓存索引！！不然回调函数里用的是i的最后一个值，必须要新建变量缓存
            int index = i;
            var item = levelSelect_levelItems[i];
            // 注册点击的回调函数
            item.RegisterCallback<ClickEvent>(evt =>
            {
                Debug.Log("关卡选择：Level" + index+1);
                OnLevelItemClicked(index);  //根据关卡索引加载对应关卡
            });
        }

    }

    void Update()
    {


    }

    // UI界面切换
    private void Switch_MainMenu2LevelSelect()
    {
        Debug.Log("Switching to Level Select Menu");
        mainMenuRoot.style.display = DisplayStyle.None;
        levelSelectRoot.style.display = DisplayStyle.Flex;
    }

    private void Switch_LevelSelect2MainMenu()
    {
        Debug.Log("Switching to Main Menu");
        levelSelectRoot.style.display = DisplayStyle.None;
        mainMenuRoot.style.display = DisplayStyle.Flex;
    }

    // 关卡选择 切换场景 异步加载
    private void OnLevelItemClicked(int index)
    {
        // 第一关：Level1_Scene
        if (index == 0)
        {
            StartCoroutine(LoadLevelAsync("Level1Scene"));
        }
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
