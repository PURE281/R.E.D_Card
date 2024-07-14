using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static AssetsBundlesMgr;
using static System.Net.Mime.MediaTypeNames;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;

public enum BattleType
{
    Init,
    PlayerTurn,
    EnermyTurn,
    Winner,
    Lose,
    End
}
/// <summary>
/// 回合制对战系统的管理脚本
/// </summary>
public class BattleSystemMgr : MonoBehaviour
{
    private List<GameObject> cards = new List<GameObject>();

    //private List<Sprite> sprite_cards_list = new List<Sprite>();

    private Dictionary<string,CardInfo> cardinfos = new Dictionary<string,CardInfo>();
    
    private BattleType _battleType = BattleType.Init;
    // Start is called before the first frame update
    void Start()
    {
        _battleType = BattleType.Init;
        SwitchBattleType(BattleType.Init);
        SwitchBattleType(BattleType.PlayerTurn);
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
        //加载卡片信息
        yield return CreateCardGo(3);
        yield return null;
        for (int i = cards.Count - 1; i >= cards.Count-3; i--)
        {
            cards[i].GetComponent<CardTurnOver>().StartFront();
            cards[i].AddComponent<CardItem>();
            cards[i].GetComponent<CardItem>()._index = i;
            CardInfo cardInfo = new CardInfo();
            cardInfo.id = cardinfos.ElementAt(i).Value.id;
            cardInfo.name = cardinfos.ElementAt(i).Value.name;
            cardInfo.cast = cardinfos.ElementAt(i).Value.cast;
            cardInfo.value = cardinfos.ElementAt(i).Value.value;
            cardInfo.type = (cardinfos.ElementAt(i).Value.type);
            cardInfo.description = cardinfos.ElementAt(i).Value.description;
            cardInfo.clipPath = cardinfos.ElementAt(i).Value.clipPath;
            cardInfo.spritePath = cardinfos.ElementAt(i).Value.spritePath;
            cards[i].GetComponent<CardItem>().Init(cardInfo);
            //cardinfos.Remove(cardinfos.ElementAt(i).Key);
        }
    }
    IEnumerator InitBattleScene()
    {
        yield return StartCoroutine(CreateCardGo(5));
        //sprite_cards_list = AssetsBundlesMgr.Instance.Sprite_cards_list;
        cardinfos = CsvManager.Instance?.ReadCardInfoCSVFile("CardData.csv");

        // 步骤1: 将Dictionary的键值对添加到列表中  
        List<KeyValuePair<string, CardInfo>> cardList = cardinfos.ToList();

        // 步骤2: 随机打乱列表  
        ShuffleList(cardList);
        RollCards();
    }
    void RollCards()
    {
        for (int i = 0; i < cards.Count; i++)
        {
            cards[i].GetComponent<CardTurnOver>().StartFront();
            cards[i].AddComponent<CardItem>();
            cards[i].GetComponent<CardItem>()._index = i;
            CardInfo cardInfo = new CardInfo();
            cardInfo.id = cardinfos.ElementAt(i).Value.id;
            cardInfo.name = cardinfos.ElementAt(i).Value.name;
            cardInfo.cast = cardinfos.ElementAt(i).Value.cast;
            cardInfo.value= cardinfos.ElementAt(i).Value.value; 
            cardInfo.type= (cardinfos.ElementAt(i).Value.type);
            cardInfo.description = cardinfos.ElementAt(i).Value.description;
            cardInfo.clipPath = cardinfos.ElementAt(i).Value.clipPath;
            cardInfo.spritePath = cardinfos.ElementAt(i).Value.spritePath;
            cards[i].GetComponent<CardItem>().Init(cardInfo);
            //cardinfos.Remove(cardinfos.ElementAt(i).Key);
        }
    }
    void ShuffleList(List<KeyValuePair<string, CardInfo>> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = UnityEngine.Random.Range(0, n + 1); // 注意Unity的Random.Range是包含上限的  
            KeyValuePair<string, CardInfo> temp = list[k];
            list[k] = list[n];
            list[n] = temp;
        }
        var shuffledCardDict = new Dictionary<string, CardInfo>(list.Count);
        foreach (var kvp in list)
        {
            shuffledCardDict.Add(kvp.Key, kvp.Value);
        }

        // 现在 shuffledCardDict 包含与 cardList 相同的键值对，但顺序是随机的（在 Dictionary 的上下文中，这实际上没有意义）  

        // 如果你想要用新的 Dictionary 替换旧的，你可以这样做：  
        cardinfos = shuffledCardDict;
    }

    public void SwitchBattleType(BattleType battleType)
    {
        switch (battleType)
        {
            case BattleType.Init:
                //初始化卡牌
                StartCoroutine(InitBattleScene());
                break;
            case BattleType.PlayerTurn:
                //进行血量判断
                break;
            case BattleType.EnermyTurn:
                //进行血量判断

                break;
            case BattleType.Winner:
                Debug.Log("Winner");
                break;
            case BattleType.Lose:
                Debug.Log("win");
                break;
            case BattleType.End:
                Debug.Log("End");
                break;
        }
    }

    public IEnumerator CreateCardGo(int cardMax)
    {
        string assetsName = "BattleCard";
        GameObject cardGo = Resources.Load<GameObject>($"Prefabs/{assetsName}");
        for (int i = 0; i < cardMax; i++)
        {
            GameObject gameObject1 = Instantiate(cardGo);
            yield return new WaitForSeconds(0.5f);
            gameObject1.transform.SetParent(GameObject.FindWithTag("MainCanvas").transform.Find("PC/CardPanel"));
            gameObject1.transform.localScale = Vector3.one;
            cards.Add(gameObject1);
        }
    }
    public PlayerInfo _playerInfo;
    public EnermyInfo _enermyInfo;
    public void HandleCard(CardInfo cardInfo)
    {
        switch (cardInfo.type)
        {
            case CardType.Atk:
                //直接造成伤害
                //计算伤害
                int temAtk = _playerInfo._curAtk;
                _enermyInfo._curHP -= temAtk;
                string log = $"玩家使用卡片对敌人造成了{cardInfo.value}点的伤害";
                Debug.Log(log);
                //text.text = log;
                break;
            case CardType.AtkUp:
                _playerInfo._curAtk += cardInfo.value;
                log = $"玩家使用卡片提升自身{cardInfo.value}点的攻击力";
                Debug.Log(log);
                //text.text = log;
                break;
            case CardType.AtkDown:
                _enermyInfo._curAtk -= cardInfo.value;
                log = $"玩家使用卡片降低敌方{cardInfo.value}点的攻击力";
                Debug.Log(log);
                //text.text = log;
                break;
            case CardType.DefUp:
                _playerInfo._curDef += cardInfo.value;
                log = $"玩家使用卡片提升自身{cardInfo.value}点的防御力";
                Debug.Log(log);
                //text.text = log;
                break;
            case CardType.DefDown:
                _enermyInfo._curDef -= cardInfo.value;
                log = $"玩家使用卡片降低敌方{cardInfo.value}点的防御力";
                Debug.Log(log);
                //text.text = log;
                break;
            case CardType.Sleep:
                log = $"玩家使用卡片让敌方睡眠跳过一回合";
                Debug.Log(log);
                //text.text = log;
                break;
            case CardType.None:
                log = $"玩家使用卡片....无事发生";
                Debug.Log(log);
                //text.text = log;
                break;
        }
    }
}
[Serializable]
public class PlayerInfo
{
    public int _id;
    public string _name;
    public string _description;
    public int _level;
    public int _maxHP;
    public int _curHP;
    public int _oriAtk;
    public int _curAtk;
    public int _oriDef;
    public int _curDef;
}
[Serializable]
public class EnermyInfo
{
    public int _id;
    public string _name;
    public string _description;
    public int _level;
    public int _maxHP;
    public int _curHP;
    public int _oriAtk;
    public int _curAtk;
    public int _oriDef;
    public int _curDef;
}