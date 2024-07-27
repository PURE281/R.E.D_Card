using CSVToolKit;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using zFramework.Extension;
using static EnumMgr;

public class CsvManager : Singleton<CsvManager>
{


    public List<CardInfoBean> GetAllCards()
    {
        try
        {
            return CsvUtility.Read<CardInfoBean>((GlobalConfig.Instance?.GetPath() + "/StreamingAssets") + "/" + "CardData.csv");
        }
        catch (Exception ex)
        {
            Debug.Log($"����Ϊ��{ex.Message}");
            return new List<CardInfoBean>();
        }
    }
    public List<PlayerCardBean> GetBattleCards()
    {
        try
        {
            return CsvUtility.Read<PlayerCardBean>((GlobalConfig.Instance?.GetPath() + "/StreamingAssets") + "/" + "BattleCardData.csv");
        }
        catch (Exception ex)
        {
            Debug.Log($"����Ϊ��{ex.Message}");
            return new List<PlayerCardBean>();
        }
    }
    public List<PlayerCardBean> GetPlayerCards()
    {
        try
        {
            return CsvUtility.Read<PlayerCardBean>((GlobalConfig.Instance?.GetPath() + "/StreamingAssets") + "/" + "PlayerCardData.csv");
        }
        catch (Exception ex)
        {
            Debug.Log($"����Ϊ��{ex.Message}");
            return new List<PlayerCardBean>();
        }
    }
    public List<CharacterBean> GetCharactersInfo()
    {
        try
        {
            return CsvUtility.Read<CharacterBean>((GlobalConfig.Instance?.GetPath() + "/StreamingAssets") + "/" + "CharacterData.csv");
        }
        catch (Exception ex)
        {
            Debug.Log($"����Ϊ��{ex.Message}");
            return new List<CharacterBean>();
        }
    }

    public void AddPlayerCard(int id)
    {
        //�ȼ���Ƿ��Ѿ������id�ˣ��еĻ��򲻽������
        try
        {
            PlayerCardBean playerCard = CsvUtility.Read<PlayerCardBean>((GlobalConfig.Instance?.GetPath() + "/StreamingAssets") + "/" + "PlayerCardData.csv", v => v.cid == id);
            ToastManager.Instance?.CreatToast("��ǰ�û��Ѵ��ڸÿ���");
        }
        catch (Exception ex)
        {
            Debug.Log($"����Ϊ��{ex.Message}");
            //�������
            PlayerCardBean bean = new PlayerCardBean();
            bean.cid = id;
            bean.proficiency = 0;
            CsvUtility.Write(new List<PlayerCardBean>() { bean }, (GlobalConfig.Instance?.GetPath() + "/StreamingAssets") + "/" + "PlayerCardData.csv");
        }
           
    }

    public void AddBattleCard(int id)
    {//�ȼ���Ƿ��Ѿ������id�ˣ��еĻ��򲻽������
        try
        {
            PlayerCardBean playerCard = CsvUtility.Read<PlayerCardBean>((GlobalConfig.Instance?.GetPath() + "/StreamingAssets") + "/" + "BattleCardData.csv", v => v.cid == id);
            ToastManager.Instance?.CreatToast("��ǰ�û��Ѵ��ڸÿ���");
        }
        catch (Exception ex)
        {
            Debug.Log($"����Ϊ��{ex.Message}");
            //�������
            PlayerCardBean bean = new PlayerCardBean();
            bean.cid = id;
            bean.proficiency = 0;
            CsvUtility.Write(new List<PlayerCardBean>() { bean }, (GlobalConfig.Instance?.GetPath() + "/StreamingAssets") + "/" + "BattleCardData.csv");
        }
    }
}
