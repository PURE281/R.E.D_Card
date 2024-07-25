using CSVToolKit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static EnumMgr;

public class CsvManager : Singleton<CsvManager>
{

    public Dictionary<string, CardInfoBean> ReadCardInfoCSVFile()
    {
        Dictionary<string, CardInfoBean> keyValuePairs = new Dictionary<string, CardInfoBean>();
        List<List<string>> data = CSVParser.Instance.ReadData("/StreamingAssets", "CardData.csv");
        //for (int i = 1; i < data.Count - 1; i++)
        for (int i = 1; i < 10; i++)
        {
            CardInfoBean cardInfo = new CardInfoBean();
            cardInfo.id = data[i][0];
            cardInfo.name = data[i][1];
            cardInfo.type = string2CardType(data[i][2]);
            cardInfo.value = int.Parse(data[i][3]);
            cardInfo.cast = data[i][4];
            cardInfo.description = data[i][5];
            cardInfo.spritePath = data[i][6];
            cardInfo.clipPath = data[i][7];
            cardInfo.upgrade_id = data[i][8];
            cardInfo.combo_id = data[i][9];
            cardInfo.fusion_id = data[i][10];
            cardInfo.proficiency = data[i][11];
            cardInfo.probability = data[i][13].Replace("\r", "");
            keyValuePairs.Add(cardInfo.id, cardInfo);
        }
        return keyValuePairs;
    }

    public void AddContents(string fileName, List<string> infoList)
    {
        CSVParser.Instance.AddData("/StreamingAssets", fileName, infoList);
        //ReadCardInfoCSVFile(fileName);
    }

    public Dictionary<int, CharacterBean> ReadCharacterInfoCSVFile()
    {
        Dictionary<int, CharacterBean> keyValuePairs = new Dictionary<int, CharacterBean>();
        List<List<string>> data = CSVParser.Instance.ReadData("/StreamingAssets", "CharacterData.csv");
        for (int i = 1; i < data.Count - 1; i++)
        {
            switch (int.Parse(data[i][3]))
            {
                case 0:
                    //Íæ¼Ò
                    BattlePlayerBean playerInfo = new BattlePlayerBean();
                    playerInfo._id = int.Parse(data[i][0]);
                    playerInfo._name = data[i][1];
                    playerInfo._level = (int.Parse(data[i][2]));
                    playerInfo._type = int.Parse(data[i][3]);
                    playerInfo._maxHP = int.Parse(data[i][4]);
                    playerInfo._curHP = int.Parse(data[i][5]);
                    playerInfo._oriAtk = int.Parse(data[i][6]);
                    playerInfo._curAtk = int.Parse(data[i][7]);
                    playerInfo._oriDef = int.Parse(data[i][8]);
                    playerInfo._curDef = int.Parse(data[i][9]);
                    playerInfo._description = data[i][10].Replace("\r", "");
                    keyValuePairs.Add(playerInfo._type, playerInfo);
                    break;
                case 1:
                    //Íæ¼Ò
                    BattleEnermyBean enmeryInfo = new BattleEnermyBean();
                    enmeryInfo._id = int.Parse(data[i][0]);
                    enmeryInfo._name = data[i][1];
                    enmeryInfo._level = (int.Parse(data[i][2]));
                    enmeryInfo._type = int.Parse(data[i][3]);
                    enmeryInfo._maxHP = int.Parse(data[i][4]);
                    enmeryInfo._curHP = int.Parse(data[i][5]);
                    enmeryInfo._oriAtk = int.Parse(data[i][6]);
                    enmeryInfo._curAtk = int.Parse(data[i][7]);
                    enmeryInfo._oriDef = int.Parse(data[i][8]);
                    enmeryInfo._curDef = int.Parse(data[i][9]);
                    enmeryInfo._description = data[i][10].Replace("\r", "");
                    keyValuePairs.Add(enmeryInfo._type, enmeryInfo);
                    break;
            }
        }
        return keyValuePairs;
    }
    CardType string2CardType(string type)
    {
        switch (type)
        {
            case "0":
                return CardType.Atk;
            case "1":
                return CardType.AtkUp;
            case "2":
                return CardType.AtkDown;
            case "3":
                return CardType.DefUp;
            case "4":
                return CardType.DefDown;
            case "5":
                return CardType.Sleep;
            case "6":
                return CardType.Cover;
            case "7":
                return CardType.None;
        }
        return CardType.None;
    }
}
