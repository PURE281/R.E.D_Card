using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// �����������Ĵ��룬������г����л�������
/// </summary>
public class MainSceneMgr : MonoSingleton<MainSceneMgr>
{
    public GameObject _btnPanel;
    // Start is called before the first frame update
    void Start()
    {
        ///��ʼ��
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
            //ToastManager.Instance?.CreatToast("�����ڴ�");
            //return;
            ToBattleScene();
        });
        btns[2].onClick.AddListener(() =>
        {
            ToastManager.Instance.CreatToast("�����ڴ�");
        });
        btns[3].onClick.AddListener(() =>
        {
            ToIntroScene();
        });
        ToastManager.Instance.CreatToast("��ʼ���ɹ�");
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
