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
/// ������������鿨�Ŀ���Ĺ�����
/// ʵ�ֹ��ܣ����ҿ���˳���������˳�����γ��֣�Ȼ������ת�����棬���ر��ͼƬ����Ҫ���ض��Ĵ���
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
        //���ؿ���Ԥ����
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
        //���´��ҿ���
        //�ص�����
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
        //����˳��
        Shuffle<Sprite>(sprite_cards_list);
        for (int i = 0; i < cards.Count; i++)
        {
            cards[i].gameObject.transform.GetChild(0).GetComponent<Image>().sprite = sprite_cards_list[i];
            AdjustImageToAspectFit(cards[i].gameObject.transform.GetChild(0).GetComponent<Image>(), cards[i].gameObject.transform.GetChild(0).GetComponent<RectTransform>());
        }
    }
    // ��������һ����������ȡͼƬ��ԭʼ�ߴ�  
    Vector2 GetOriginalImageSize(Sprite sprite)
    {
        return new Vector2(sprite.rect.width, sprite.rect.height);
    }

    // Ȼ������Ը���Ŀ�������Ŀ�߱�������Image��RectTransform  
    void AdjustImageToAspectFit(Image image, RectTransform container)
    {
        Sprite sprite = image.sprite;
        if (sprite == null) return;

        Vector2 originalSize = GetOriginalImageSize(sprite);
        float aspectRatio = originalSize.x / originalSize.y;

        // ����������Ҫ����ͼƬ�Ŀ�ȣ����������Ŀ���������߶�  
        float targetWidth = container.rect.width;
        float targetHeight = targetWidth / aspectRatio;

        // ���ڣ�������Ҫ����RectTransform��ê�㣨Anchors���ʹ�С��SizeDelta��  
        // ������������Ѿ������˺��ʵ�ê���pivot����Ӧ����  
        // ����ֻ����SizeDelta  
        image.rectTransform.sizeDelta = new Vector2(targetWidth, targetHeight);

        // ע�⣺�������Ҫ���ָ߶Ȳ�������ȣ�ֻ�轻��width��height�ļ��㼴��  
    }
    void Shuffle<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = UnityEngine.Random.Range(0, n + 1); // ע��Unity��Random.Range�ǰ������޵�  
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    /// <summary>
    ///   //��ȡ�ļ���������jpg/pngͼƬ·��������ͼƬת��Ϊsprite�����ָ���ļ���
    /// </summary>
    /// <param �ļ���·��="path"></param>
    /// <param ���ڴ��ͼƬ�ļ���="PhotoList"></param>
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
                string suffixName = pathname.Substring(pathname.Length - 3, 3);   //��ȡ�ļ���׺���������ж��Ƿ�ΪͼƬ
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
            print("�ļ���Ϊ��:" + uri);
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
            assetBundle.Unload(true); // false��ʾ��ж��AssetBundle�е�Assets����ж��AssetBundle����  
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
