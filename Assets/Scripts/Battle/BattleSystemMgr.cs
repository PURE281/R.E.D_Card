using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static AssetsBundlesMgr;

/// <summary>
/// 回合制对战系统的管理脚本
/// </summary>
public class BattleSystemMgr : MonoBehaviour
{
    private List<GameObject> cards = new List<GameObject>();

    private List<Sprite> sprite_cards_list = new List<Sprite>();

    private Dictionary<string,CardInfo> cardinfos = new Dictionary<string,CardInfo>();
    // Start is called before the first frame update
    void Start()
    {
        //初始化卡牌
        StartCoroutine(InitBattleScene());
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GetCard()
    {
        StartCoroutine(GetCardIE());
    }
    IEnumerator GetCardIE()
    {
        yield return AssetsBundlesMgr.Instance?.InitLoadCard(CardPrefabType.BattleCard,3);

        for (int i = cards.Count - 1; i >= cards.Count-3; i--)
        {
            cards[i].gameObject.transform.GetChild(0).GetChild(1).GetComponent<Image>().sprite = sprite_cards_list[i];
            AdjustImageToAspectFit(cards[i].gameObject.transform.GetChild(0).GetChild(1).GetComponent<Image>(), cards[i].gameObject.transform.GetChild(0).GetComponent<RectTransform>());
            cards[i].GetComponent<CardTurnOver>().StartFront();
            cards[i].AddComponent<CardItem>();
            cards[i].GetComponent<CardItem>()._index = i;

            CardInfo cardInfo = new CardInfo();
            cardInfo.cast = cardinfos[sprite_cards_list[i].name].cast;
            cardInfo.num = cardinfos[sprite_cards_list[i].name].num;
            cardInfo.type = (cardinfos[sprite_cards_list[i].name].type == "1" ? "攻" : "辅");
            cardInfo.description = cardinfos[sprite_cards_list[i].name].description;
            cardInfo.clipPath = cardinfos[sprite_cards_list[i].name].clipPath;
            cards[i].GetComponent<CardItem>().Init(cardInfo);
            cards[i].gameObject.transform.GetChild(0).GetChild(2).GetComponentInChildren<Text>().text = cardInfo.cast;
            cards[i].gameObject.transform.GetChild(0).GetChild(3).GetChild(0).GetComponentInChildren<Text>().text = cardInfo.num;
            cards[i].gameObject.transform.GetChild(0).GetChild(3).GetChild(1).GetComponentInChildren<Text>().text = cardInfo.type;
        }
    }
    IEnumerator InitBattleScene()
    {
        yield return StartCoroutine(AssetsBundlesMgr.Instance.InitIE(CardPrefabType.BattleCard, 5));
        sprite_cards_list = AssetsBundlesMgr.Instance.Sprite_cards_list;
        cards = AssetsBundlesMgr.Instance.Cards;
        cardinfos = AssetsBundlesMgr.Instance?.CardInfoDicts;
        RollCards();
    }
    void RollCards()
    {
        //打乱顺序
        Shuffle<Sprite>(sprite_cards_list);
        for (int i = 0; i < cards.Count; i++)
        {
            cards[i].gameObject.transform.GetChild(0).GetChild(1).GetComponent<Image>().sprite = sprite_cards_list[i];
            AdjustImageToAspectFit(cards[i].gameObject.transform.GetChild(0).GetChild(1).GetComponent<Image>(), cards[i].gameObject.transform.GetChild(0).GetComponent<RectTransform>());
            cards[i].GetComponent<CardTurnOver>().StartFront();
            cards[i].AddComponent<CardItem>();
            cards[i].GetComponent<CardItem>()._index = i;
            CardInfo cardInfo = new CardInfo();
            cardInfo.cast = cardinfos[sprite_cards_list[i].name].cast;
            cardInfo.num= cardinfos[sprite_cards_list[i].name].num; 
            cardInfo.type= (cardinfos[sprite_cards_list[i].name].type == "1" ? "攻" : "辅");
            cardInfo.description = cardinfos[sprite_cards_list[i].name].description;
            cardInfo.clipPath = cardinfos[sprite_cards_list[i].name].clipPath;
            cards[i].GetComponent<CardItem>().Init(cardInfo);
            cards[i].gameObject.transform.GetChild(0).GetChild(2).GetComponentInChildren<Text>().text = cardInfo.cast;
            cards[i].gameObject.transform.GetChild(0).GetChild(3).GetChild(0).GetComponentInChildren<Text>().text = cardInfo.num;
            cards[i].gameObject.transform.GetChild(0).GetChild(3).GetChild(1).GetComponentInChildren<Text>().text = cardInfo.type;
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
}
