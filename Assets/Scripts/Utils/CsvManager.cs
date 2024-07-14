using CSVToolKit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CsvManager : Singleton<CsvManager>
{

    public Dictionary<string, CardInfo> ReadCardInfoCSVFile(string fileName)
    {
        Dictionary<string, CardInfo> keyValuePairs = new Dictionary<string, CardInfo>();
        List<List<string>> data = CSVParser.Instance.ReadData(GetPath() + "/StreamingAssets", fileName);
        for (int i = 1; i < data.Count - 1; i++)
        {
            CardInfo cardInfo = new CardInfo();
            cardInfo.id = data[i][0];
            cardInfo.name = data[i][1];
            cardInfo.type = string2CardType(data[i][2]);
            cardInfo.value = int.Parse(data[i][3]);
            cardInfo.cast = data[i][4];
            cardInfo.description = data[i][5];
            cardInfo.spritePath = data[i][6];
            cardInfo.clipPath = data[i][7].Replace("\r","");
            keyValuePairs.Add(cardInfo.id, cardInfo);
        }
        return keyValuePairs;
    }

    public void AddContents(string fileName, List<string> infoList)
    {
        CSVParser.Instance.AddData(GetPath() + "/StreamingAssets", fileName, infoList);
        ReadCardInfoCSVFile(fileName);
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
    private static string GetPath()
    {
#if UNITY_EDITOR
        return Application.dataPath;
#elif UNITY_ANDROID
			return Application.persistentDataPath;
#elif UNITY_IPHONE
			return GetiPhoneDocumentsPath();
#else
			return Application.dataPath;
#endif
    }
}
