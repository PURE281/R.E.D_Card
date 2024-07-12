using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using DG.Tweening;

public class AssetsBundlesMgr : MonoSingleton<AssetsBundlesMgr>
{
    //public int cardMax = 10;
    private List<GameObject> cards = new List<GameObject>();
    [SerializeField]
    //private List<Sprite> sprite_cards_list = new List<Sprite>();

    private Dictionary<string, CardInfo> cardInfoDicts = new Dictionary<string, CardInfo>();

    private GameObject cardGo;

    public IEnumerator InitIE(CardPrefabType cardType, int cardMax)
    {
        //UnloadAllAssetBundles();
        yield return StartCoroutine(LoadPicture());
        yield return StartCoroutine(InitLoadCard(cardType, cardMax));
    }
    public IEnumerator InitLoadCard(CardPrefabType cardType, int cardMax)
    {
        string assetsName = "";
        switch (cardType)
        {
            case CardPrefabType.Card:
                assetsName = "Card";
                break;
            case CardPrefabType.BattleCard:
                assetsName = "BattleCard";
                break;
        }
        yield return StartCoroutine(LoadAB("prefabs", assetsName));
        for (int i = 0; i < cardMax; i++)
        {
            GameObject gameObject1 = Instantiate(cardGo);
            if (GlobalConfig.Instance.Platform == 1)
            {
                yield return new WaitForSeconds(0.5f);
                gameObject1.transform.SetParent(GameObject.FindWithTag("MainCanvas").transform.Find("PC/CardPanel"));
                gameObject1.transform.localScale = Vector3.one;
                Cards.Add(gameObject1);
            }
            else if (GlobalConfig.Instance.Platform == 2)
            {
                gameObject1.transform.SetParent(GameObject.FindWithTag("MainCanvas").transform.Find("Mobile/CardPanel"));
                gameObject1.transform.localScale = Vector3.one;
                Cards.Add(gameObject1);
            }
        }
    }
    IEnumerator LoadAB(string abname, string filename)
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
                if (!loadedAssetBundles.ContainsKey(abname))
                {
                    AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(request);
                    cardGo = bundle.LoadAsset<GameObject>(filename);
                    loadedAssetBundles.Add(abname, bundle);
                }
                else
                {
                    cardGo = loadedAssetBundles[abname].LoadAsset<GameObject>(filename);
                }
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
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

    /// <summary>
    ///   //获取文件夹下所有jpg/png图片路径，并把图片转换为sprite，存进指定的集合
    /// </summary>
    /// <param 文件夹路径="path"></param>
    /// <param 用于存放图片的集合="PhotoList"></param>
    IEnumerator LoadPicture()
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
            AssetBundle bundle = null;
            if (!loadedAssetBundles.ContainsKey("card"))
            {
                bundle = DownloadHandlerAssetBundle.GetContent(request1);
                loadedAssetBundles.Add("card", bundle);
            }
            else
            {
                bundle = loadedAssetBundles["card"];
            }

            foreach (var item in lists)
            {
                //print(item.Name);
                string[] infos = item.Replace("\r", "").Split(",");
                string pathname = infos[0];
                CardInfo cardInfo = new CardInfo();
                cardInfo.cast = infos[1];
                cardInfo.num = infos[2];
                cardInfo.type = infos[3];
                cardInfo.description = infos[4].Replace("\"", "");
                if (infos.Length > 5 && !"".Equals(infos[5]))
                {
                    cardInfo.clipPath = $"{Application.dataPath}/Resources/Music/clips/{infos[5].Replace("\"", "")}";
                }
                string suffixName = pathname.Substring(pathname.Length - 3, 3);   //获取文件后缀名，用以判断是否为图片
                suffixName = suffixName.ToLower();
                Debug.Log(item + ">>>" + suffixName);
                if (suffixName == "jpg" || suffixName == "png")
                {
                    //string path = Path.Combine(Application.streamingAssetsPath, pathname);
                    Texture texture2D = bundle.LoadAsset<Texture>(pathname);
                    Sprite tempSprite = Sprite.Create((Texture2D)texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(10, 10));
                    tempSprite.name = pathname;
                    cardInfo.sprite = tempSprite;
                    cardInfo.name = pathname;
                    //Sprite_cards_list.Add(tempSprite);
                }

                CardInfoDicts.Add(pathname, cardInfo);

            }
        }
        else
        {
            print("文件夹为空:" + uri);
        }
        //return sprite_cards_list;

    }
    private Dictionary<string, AssetBundle> loadedAssetBundles = new Dictionary<string, AssetBundle>();

    //public List<Sprite> Sprite_cards_list { get => sprite_cards_list; set => sprite_cards_list = value; }
    public List<GameObject> Cards { get => cards; set => cards = value; }
    public Dictionary<string, CardInfo> CardInfoDicts { get => cardInfoDicts; set => cardInfoDicts = value; }

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

    public class CardInfo
    {
        public string name;
        public string cast;
        public string num;
        public string type;
        public string description;
        public string clipPath;
        public Sprite sprite;
    }
}
