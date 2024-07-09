using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 管理主场景的代码，负责进行场景切换和设置
/// </summary>
public class MainSceneMgr : MonoSingleton<MainSceneMgr>
{
    public GameObject _btnPanel;
    // Start is called before the first frame update
    void Start()
    {
        ///初始化
        if (_btnPanel == null)
        {
            return;
        }
        Button[] btns = _btnPanel.GetComponentsInChildren<Button>();
        btns[0].onClick.AddListener(() =>
        {
            ToCardScene();
        });
        btns[1].onClick.AddListener(() =>
        {
            //ToastManager.Instance?.CreatToast("敬请期待");
            //return;
            ToBattleScene();
        });
        btns[2].onClick.AddListener(() =>
        {
            ToastManager.Instance.CreatToast("敬请期待");
        });
        btns[3].onClick.AddListener(() =>
        {
            ToIntroScene();
        });
        ToastManager.Instance.CreatToast("初始化成功");
    }

    public void ToCardScene()
    {
        SceneManager.LoadScene("CardScene");
    }
    public void ToBattleScene()
    {
        SceneManager.LoadScene("BattleScene");
    }
    public void ToIntroScene()
    {
        SceneManager.LoadScene("IntroductScene");
    }

    public void ToMainScene()
    {
        AssetsBundlesMgr.Instance?.UnloadAllAssetBundles();
        SceneManager.LoadScene("MainScene");
    }
}
