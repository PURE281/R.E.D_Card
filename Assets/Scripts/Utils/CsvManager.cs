using CSVToolKit;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
            Debug.Log($"数据为零{ex.Message}");
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
            Debug.Log($"数据为零{ex.Message}");
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
            Debug.Log($"数据为零{ex.Message}");
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
            Debug.Log($"数据为零{ex.Message}");
            return new List<CharacterBean>();
        }
    }

    public void AddPlayerCard(int id)
    {
        //先检查是否已经有这个id了，有的话则不进行添加
        try
        {
            PlayerCardBean playerCard = CsvUtility.Read<PlayerCardBean>((GlobalConfig.Instance?.GetPath() + "/StreamingAssets") + "/" + "PlayerCardData.csv", v => v.cid == id);
            ToastManager.Instance?.CreatToast("当前用户已存在该卡牌");
        }
        catch (Exception ex)
        {
            Debug.Log($"数据为零{ex.Message}");
            //进行添加
            PlayerCardBean bean = new PlayerCardBean();
            bean.cid = id;
            bean.proficiency = 0;
            CsvUtility.Write(new List<PlayerCardBean>() { bean }, (GlobalConfig.Instance?.GetPath() + "/StreamingAssets") + "/" + "PlayerCardData.csv");
        }

    }

    public void AddBattleCard(int id)
    {//先检查是否已经有这个id了，有的话则不进行添加
        try
        {
            PlayerCardBean playerCard = CsvUtility.Read<PlayerCardBean>((GlobalConfig.Instance?.GetPath() + "/StreamingAssets") + "/" + "BattleCardData.csv", v => v.cid == id);
            if (playerCard != null)
            {
                ToastManager.Instance?.CreatToast("当前用户已存在该卡牌");
            }
            else
            {
                //进行添加
                PlayerCardBean bean = new PlayerCardBean();
                bean.cid = id;
                bean.proficiency = 0;
                List<PlayerCardBean> playerCardBeans = this.GetBattleCards();
                playerCardBeans.Add(bean);
                CsvUtility.Write(playerCardBeans, (GlobalConfig.Instance?.GetPath() + "/StreamingAssets") + "/" + "BattleCardData.csv");
                ToastManager.Instance?.CreatToast("已成功添加至对战卡组，可前往查看~");
            }
        }
        catch (Exception ex)
        {
            Debug.Log($"数据为零{ex.Message}");
            ToastManager.Instance?.CreatToast("已成功添加至对战卡组，可前往查看~");
            //进行添加
            PlayerCardBean bean = new PlayerCardBean();
            bean.cid = id;
            bean.proficiency = 0;
            CsvUtility.Write(new List<PlayerCardBean>() { bean }, (GlobalConfig.Instance?.GetPath() + "/StreamingAssets") + "/" + "BattleCardData.csv");
        }
    }

    public void RemoveBattleCard(int id)
    {
        try
        {
            PlayerCardBean playerCard = CsvUtility.Read<PlayerCardBean>((GlobalConfig.Instance?.GetPath() + "/StreamingAssets") + "/" + "BattleCardData.csv", v => v.cid == id);
            if (playerCard == null)
            {
                ToastManager.Instance?.CreatToast("当前用户不存在该卡牌");
            }
            else
            {
                //进行移除
                List<PlayerCardBean> playerCardBeans = this.GetBattleCards();
                playerCardBeans.Remove(playerCardBeans.Where((value)=>value.cid == id).ToList()[0]);
                CsvUtility.Write(playerCardBeans, (GlobalConfig.Instance?.GetPath() + "/StreamingAssets") + "/" + "BattleCardData.csv");
                ToastManager.Instance?.CreatToast("已成功从对战卡组中移除");
            }
        }
        catch (Exception ex)
        {
            Debug.Log($"数据为零{ex.Message}");
            //进行添加
            PlayerCardBean bean = new PlayerCardBean();
            bean.cid = id;
            bean.proficiency = 0;
            CsvUtility.Write(new List<PlayerCardBean>() { bean }, (GlobalConfig.Instance?.GetPath() + "/StreamingAssets") + "/" + "BattleCardData.csv");
        }
    }
}
