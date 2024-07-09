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
        //����˳��
        Shuffle<Sprite>(sprite_cards_list);
        for (int i = 0; i < cards.Count; i++)
        {
            cards[i].gameObject.transform.GetChild(0).GetComponent<Image>().sprite = sprite_cards_list[i];
            AdjustImageToAspectFit(cards[i].gameObject.transform.GetChild(0).GetComponent<Image>(), cards[i].gameObject.transform.GetChild(0).GetComponent<RectTransform>());
            cards[i].GetComponent<CardTurnOver>().StartFront();
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
}
