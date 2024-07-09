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
    private List<GameObject> cards = new List<GameObject>();

    private List<Sprite> sprite_cards_list = new List<Sprite>();

    private bool isActive = false;

    public Button _startCardsbtn;

    void Awake()
    {

    }

    public IEnumerator InitCarddScene()
    {

        if (_startCardsbtn)
        {
            _startCardsbtn.interactable = false;
            yield return StartCoroutine(AssetsBundlesMgr.Instance.InitIE(10));
            sprite_cards_list = AssetsBundlesMgr.Instance.Sprite_cards_list;
            cards = AssetsBundlesMgr.Instance.Cards;
            RollCards();
            _startCardsbtn.interactable = true;
        }
    }

   

    // Start is called before the first frame update
    void Start()
    {
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
            yield return cards[i].GetComponent<CardTurnOver>().ToFront();
            yield return 0.5f;
        }
        isActive = false;
        _startCardsbtn.interactable = true;
    }

    void GetCardsBackIE(Action action)
    {

        for (int i = 0; i < cards.Count; i++)
        {
            cards[i].GetComponent<CardTurnOver>().mFront.transform.DORotate(new Vector3(0, 90, 0), 0);
            cards[i].GetComponent<CardTurnOver>().mBack.transform.DORotate(new Vector3(0, 0, 0), 0);
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
            cards[i].GetComponent<CardTurnOver>().StartFront();
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
