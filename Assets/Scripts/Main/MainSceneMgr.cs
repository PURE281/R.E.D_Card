using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using zFramework.Extension;

/// <summary>
/// 管理主场景的代码，负责进行场景切换和设置
/// </summary>
public class MainSceneMgr : MonoSington<MainSceneMgr>
{
    public GameObject _mainBtnPanel;
    public GameObject _battleBtnPanel;
    // Start is called before the first frame update
    void Start()
    {
        Init();
        ///初始化
        if (_mainBtnPanel == null)
        {
            return;
        }
        Button[] btns = _mainBtnPanel.GetComponentsInChildren<Button>();
        btns[0].onClick.AddListener(() =>
        {
            ToCardScene();
        });
        btns[1].onClick.AddListener(() =>
        {
            this._battleBtnPanel.SetActive(true);
        });
        btns[2].onClick.AddListener(() =>
        {
            ToDeckScene();
        });
        btns[3].onClick.AddListener(() =>
        {
            ToastManager.Instance.CreatToast("敬请期待");
        });
        btns[4].onClick.AddListener(() =>
        {
            ToIntroScene();
        });
        btns[4].onClick.AddListener(() =>
        {
            Application.Quit();
        });
        ToastManager.Instance.CreatToast("初始化成功");


        Button[] battleBtns = _battleBtnPanel.GetComponentsInChildren<Button>();
        battleBtns[0].onClick.AddListener(() =>
        {
            this.SwitchBattleMode(1);
            ToBattleScene();
        });
        battleBtns[1].onClick.AddListener(() =>
        {
            this.SwitchBattleMode(2);
            ToBattleScene();
        });
        battleBtns[2].onClick.AddListener(() =>
        {
            this._battleBtnPanel.SetActive(false);
        });
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
        //SceneManager.LoadScene("IntroductScene");
        ToastManager.Instance?.CreatToast("敬请期待");
    }
    public void ToDeckScene()
    {
        SceneManager.LoadScene("DeckScene");
    }
    public void ToMainScene()
    {
        //AssetsBundlesMgr.Instance?.UnloadAllAssetBundles();
        SceneManager.LoadScene("MainScene");
    }

    public void ToChrome(string url)
    {
        Application.OpenURL(url);
    }

    public void SwitchBattleMode(int battleMode)
    {
        GlobalConfig.Instance.BattleMode = battleMode;
    }


    public void Init()
    {
        //需要在这里初始化配置文件
        //当前有三个csv文件需要添加
        //carddata
        string _temCardDataPath = (GlobalConfig.Instance.GetPath() + "/StreamingAssets") + "/" + "CardData.csv";
        //从streamingassets中拉取最新的csv文件
        this.copy("CardData.csv");
        
        _temCardDataPath = (GlobalConfig.Instance.GetPath() + "/StreamingAssets") + "/" + "CharacterData.csv";
        //characterdata
        this.copy("CharacterData.csv");
        _temCardDataPath = (GlobalConfig.Instance.GetPath() + "/StreamingAssets") + "/" + "PlayerCardData.csv";
        if (!File.Exists(_temCardDataPath))
        {
            //初始化
            FileStream fileStream = File.Create(_temCardDataPath);
            fileStream.Close();
            try
            {
                //添加数据
                List<PlayerCardBean> playerCardBeans = new List<PlayerCardBean>();
                playerCardBeans.Add(new PlayerCardBean(3,0));
                playerCardBeans.Add(new PlayerCardBean(6,0));
                playerCardBeans.Add(new PlayerCardBean(44,0));
                playerCardBeans.Add(new PlayerCardBean(46,0));
                playerCardBeans.Add(new PlayerCardBean(49,0));
                playerCardBeans.Add(new PlayerCardBean(50,0));
                playerCardBeans.Add(new PlayerCardBean(52,0));
                playerCardBeans.Add(new PlayerCardBean(55,0));
                playerCardBeans.Add(new PlayerCardBean(56,0));
                playerCardBeans.Add(new PlayerCardBean(71,0));
                playerCardBeans.Add(new PlayerCardBean(72,0));
                playerCardBeans.Add(new PlayerCardBean(75,0));
                playerCardBeans.Add(new PlayerCardBean(90,0));
                playerCardBeans.Add(new PlayerCardBean(91,0));
                playerCardBeans.Add(new PlayerCardBean(99,0));
                playerCardBeans.Add(new PlayerCardBean(100,0));
                playerCardBeans.Add(new PlayerCardBean(117,0));
                playerCardBeans.Add(new PlayerCardBean(118,0));
                playerCardBeans.Add(new PlayerCardBean(137,0));
                CsvUtility.Write(playerCardBeans, _temCardDataPath);
#if UNITY_EDITOR
                UnityEditor.AssetDatabase.Refresh();
#endif
            }
            catch (Exception ex)
            {
                Debug.LogError($"Something went wrong while writing content{ex.Message}");
            }
        }
        _temCardDataPath = (GlobalConfig.Instance.GetPath() + "/StreamingAssets") + "/" + "BattleCardData.csv";
        if (!File.Exists(_temCardDataPath))
        {
            //初始化
            FileStream fileStream = File.Create(_temCardDataPath);
            fileStream.Close();
            try
            {
                //添加数据
                CsvUtility.Write(new List<PlayerCardBean> { }, _temCardDataPath);
#if UNITY_EDITOR
                UnityEditor.AssetDatabase.Refresh();
#endif
            }
            catch (Exception ex)
            {
                Debug.LogError($"Something went wrong while writing content{ex.Message}");
            }
        }

        MainUIManager.Instance?.Init();
        //playerdata
    }

    public void copy(string fileName = "test.txt")
    {
        //文件地址
        string url;
        //Mac，Windows或Linux平台
#if UNITY_EDITOR || UNITY_STANDALONE
        url = $"{Application.dataPath}/StreamingAssets/{fileName}";
        //ios平台路径
#elif UNITY_IPHONE
            url = $"file://{Application.dataPath}/Raw/{fileName}";
    //安卓路径
#elif UNITY_ANDROID
            url = $"jar:file://{Application.dataPath}!/assets/{fileName}";
#elif UNITY_WEBGL
            url =  (GlobalConfig.Instance.GetPath() + "/StreamingAssets") + "/" + "CardData.csv";
 
#endif
        if (!Directory.Exists($"{Application.persistentDataPath}/StreamingAssets"))
        {
            Directory.CreateDirectory($"{Application.persistentDataPath}/StreamingAssets");
        }
        string persistentUrl = $"{Application.persistentDataPath}/StreamingAssets/{fileName}";

        if (!File.Exists(persistentUrl))
        {
            Debug.Log($"{persistentUrl} 文件不存在,从StreamingAssets中Copy!");
            WWW www = new WWW(url);
            while (true)
            {
                if (www.isDone)
                {
                    if (www.error == null)
                    {
                        //本次读的文本 
                        File.WriteAllText(persistentUrl, www.text);
                        Debug.Log($"持久化目录: {persistentUrl}");
                        break;
                    }
                    else
                    {
                        Debug.LogWarning($"没得到StreamingAssets的文件 : {fileName}");
                    }
                }
            }
        }
        else
        {
            Debug.Log($"{persistentUrl} 文件已存在!");
            //删除,重新拉,以此保证每次都是最新的
            File.Delete(persistentUrl);
            this.copy(fileName);
        }
    }
}
