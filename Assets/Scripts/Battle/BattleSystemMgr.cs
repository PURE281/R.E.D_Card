using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �غ��ƶ�սϵͳ�Ĺ���ű�
/// </summary>
public class BattleSystemMgr : MonoBehaviour
{
    private List<GameObject> cards = new List<GameObject>();

    private List<Sprite> sprite_cards_list = new List<Sprite>();
    // Start is called before the first frame update
    void Start()
    {
        //��ʼ������
        StartCoroutine(InitBattleScene());
    }

    // Update is called once per frame
    void Update()
    {

    }
    IEnumerator InitBattleScene()
    {
        yield return StartCoroutine(AssetsBundlesMgr.Instance.InitIE(5));
        sprite_cards_list = AssetsBundlesMgr.Instance.Sprite_cards_list;
        cards = AssetsBundlesMgr.Instance.Cards;
        RollCards();
        //��ʼ����������
        for (int i = 0; i < cards.Count; i++)
        {
            cards[i].AddComponent<CardItem>();
            cards[i].AddComponent<CardItem>()._index = i;

        }
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
