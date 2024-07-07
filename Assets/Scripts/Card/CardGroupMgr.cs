using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;

/// <summary>
/// 这是用来管理抽卡的卡组的管理类
/// 实现功能，打乱卡牌顺序，随机按照顺序依次出现，然后依次转出正面，在特别的图片中需要有特定的处理
/// </summary>
public class CardGroupMgr : MonoSingleton<CardGroupMgr>
{
    public List<CardTurnOver> cards = new List<CardTurnOver>();

    public List<Sprite> sprite_cards_list = new List<Sprite>();

    private bool isActive = false;

    public Button _startCardsbtn;

    void Awake()
    {

        _startCardsbtn.interactable = false;
        StartCoroutine(InitIE());

    }
    IEnumerator InitIE()
    {
        yield return StartCoroutine(InitCard());
        StartCoroutine(LoadPicture(() =>
        {
            RollCards();
            _startCardsbtn.interactable = true;
        }));
    }
    IEnumerator InitCard()
    {

        yield return StartCoroutine(LoadAB("prefabs", "Card", (cardgo) =>
        {
            for (int i = 0; i < 10; i++)
            {
                GameObject gameObject1 = Instantiate(cardgo);
                if (GlobalConfig.Instance.Platform == 1)
                {
                    gameObject1.transform.SetParent(GameObject.FindWithTag("MainCanvas").transform.Find("PC/CardPanel"));
                    gameObject1.transform.localScale = Vector3.one;
                    cards.Add(gameObject1.GetComponent<CardTurnOver>());
                }
                else if (GlobalConfig.Instance.Platform == 2)
                {
                    gameObject1.transform.SetParent(GameObject.FindWithTag("MainCanvas").transform.Find("Mobile/CardPanel"));
                    gameObject1.transform.localScale = Vector3.one;
                    cards.Add(gameObject1.GetComponent<CardTurnOver>());
                }
            }
        }));
    }
    IEnumerator LoadAB(string abname, string filename, Action<GameObject> action)
    {
        //加载卡牌预制体
        var uri = new System.Uri(Path.Combine(Application.streamingAssetsPath, abname));
        using (UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(uri))
        {
            yield return request.SendWebRequest();
            if (request.isNetworkError || request.isHttpError)
            {
                Debug.Log(request.error);
                Debug.Log(request.error);
            }
            else
            {
                AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(request);
                GameObject prefab = bundle.LoadAsset<GameObject>(filename);
                loadedAssetBundles.Add(abname, bundle);
                action?.Invoke(prefab);
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {

    }


    public void GetCards()
    {
        StartCoroutine(GetCardsFrontIE());
    }


    public void RestartGetCards()
    {
        if (isActive)
            return;
        //重新打乱卡组
        //回到背面
        _startCardsbtn.interactable = false;
        GetCardsBackIE(() =>
        {
            RollCards();
            StartCoroutine(GetCardsFrontIE());
        });
    }
    IEnumerator GetCardsFrontIE()
    {
        isActive = true;
        for (int i = 0; i < cards.Count; i++)
        {
            yield return cards[i].ToFront();
            yield return 0.5f;
        }
        isActive = false;
        _startCardsbtn.interactable = true;
    }

    void GetCardsBackIE(Action action)
    {

        for (int i = 0; i < cards.Count; i++)
        {
            cards[i].mFront.transform.DORotate(new Vector3(0, 90, 0), 0);
            cards[i].mBack.transform.DORotate(new Vector3(0, 0, 0), 0);
        }
        action?.Invoke();
    }
    void RollCards()
    {
        //打乱顺序
        Shuffle<Sprite>(sprite_cards_list);
        for (int i = 0; i < cards.Count; i++)
        {
            cards[i].gameObject.transform.GetChild(0).GetComponent<Image>().sprite = sprite_cards_list[i];
            AdjustImageToAspectFit(cards[i].gameObject.transform.GetChild(0).GetComponent<Image>(), cards[i].gameObject.transform.GetChild(0).GetComponent<RectTransform>());
        }
    }
    // 假设你有一个方法来获取图片的原始尺寸  
    Vector2 GetOriginalImageSize(Sprite sprite)
    {
        return new Vector2(sprite.rect.width, sprite.rect.height);
    }

    // 然后，你可以根据目标容器的宽高比来调整Image的RectTransform  
    void AdjustImageToAspectFit(Image image, RectTransform container)
    {
        Sprite sprite = image.sprite;
        if (sprite == null) return;

        Vector2 originalSize = GetOriginalImageSize(sprite);
        float aspectRatio = originalSize.x / originalSize.y;

        // 假设我们想要保持图片的宽度，根据容器的宽度来调整高度  
        float targetWidth = container.rect.width;
        float targetHeight = targetWidth / aspectRatio;

        // 现在，我们需要调整RectTransform的锚点（Anchors）和大小（SizeDelta）  
        // 这里假设容器已经设置了合适的锚点和pivot来适应内容  
        // 我们只调整SizeDelta  
        image.rectTransform.sizeDelta = new Vector2(targetWidth, targetHeight);

        // 注意：如果你想要保持高度并调整宽度，只需交换width和height的计算即可  
    }
    void Shuffle<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = UnityEngine.Random.Range(0, n + 1); // 注意Unity的Random.Range是包含上限的  
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    /// <summary>
    ///   //获取文件夹下所有jpg/png图片路径，并把图片转换为sprite，存进指定的集合
    /// </summary>
    /// <param 文件夹路径="path"></param>
    /// <param 用于存放图片的集合="PhotoList"></param>
    IEnumerator LoadPicture(Action action)
    {

        var uri = new System.Uri(Path.Combine(Application.streamingAssetsPath, "CardInfo.jsonl"));
        string[] lists = null;

        UnityWebRequest request = UnityWebRequest.Get(uri);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
        {
            Debug.Log(request.error);
        }
        else
        {
            string list = request.downloadHandler.text;
            lists = list.Split("\n");

        }
        if (lists.Length > 0)
        {
            uri = new System.Uri(Path.Combine(Application.streamingAssetsPath, "card"));
            UnityWebRequest request1 = UnityWebRequestAssetBundle.GetAssetBundle(uri);

            yield return request1.SendWebRequest();

            if (request1.isNetworkError || request1.isHttpError)
            {
                Debug.Log(request1.error);
                Debug.Log(request1.error);
            }
            AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(request1);

            loadedAssetBundles.Add("card", bundle);
            foreach (var item in lists)
            {
                //print(item.Name);
                string pathname = item.Replace("\r", "");
                string suffixName = pathname.Substring(pathname.Length - 3, 3);   //获取文件后缀名，用以判断是否为图片
                suffixName = suffixName.ToLower();
                if (suffixName == "jpg" || suffixName == "png")
                {
                    string path = Path.Combine(Application.streamingAssetsPath, pathname);
                    //photoNames.Add(item.Name);
                    //LoadImageByIO(path + @"\" + item.Name, sprite_cards_list);
                    //yield return StartCoroutine(ReadTexture(pathname, (texture2D) =>
                    //{
                    //    Sprite tempSprite = Sprite.Create((Texture2D)texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(10, 10));
                    //    sprite_cards_list.Add(tempSprite);
                    //}));
                    Texture texture2D = bundle.LoadAsset<Texture>(pathname);
                    Sprite tempSprite = Sprite.Create((Texture2D)texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(10, 10));
                    sprite_cards_list.Add(tempSprite);
                }


            }
        }
        else
        {
            print("文件夹为空:" + uri);
        }
        action?.Invoke();
        //return sprite_cards_list;

    }
    private Dictionary<string, AssetBundle> loadedAssetBundles = new Dictionary<string, AssetBundle>();
    public void UnloadAssetBundle(string assetBundleName)
    {
        if (loadedAssetBundles.ContainsKey(assetBundleName))
        {
            AssetBundle assetBundle = loadedAssetBundles[assetBundleName];
            assetBundle.Unload(true); // false表示不卸载AssetBundle中的Assets，仅卸载AssetBundle本身  
            loadedAssetBundles.Remove(assetBundleName);
            Debug.Log("Unloaded AssetBundle: " + assetBundleName);
        }
    }
    public void UnloadAllAssetBundles()
    {
        List<string> keys = new List<string>(loadedAssetBundles.Keys);
        foreach (string key in keys)
        {
            UnloadAssetBundle(key);
        }
    }

    private void OnDestroy()
    {
        
    }
}
