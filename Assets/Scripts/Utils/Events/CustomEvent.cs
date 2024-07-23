using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomEvent
{
    #region 关于战斗场景中UI有关的委托
    public const string BATTLE_UI_SHOW_CARD_DETAIL = "BattleShowCardDetail";
    public const string BATTLE_UI_CLOSE_CARD_DETAIL = "BattleCloseCardDetail";
    public const string BATTLE_UI_REFRESH_CARDS = "BattleRefreshCards";
    public const string BATTLE_UI_SHOW_CARD_EFFECT = "ShowCardEffect";
    public const string BATTLE_UI_RESET_CARDS = "BattleResetCards";
    public const string BATTLE_UI_UPDATE_CARDS = "BattleUpdateCards";
    public const string BATTLE_UI_COMBO_CARDS = "BattleComboCards";
    public const string BATTLE_UI_FUSION_CARDS = "BattleFusionCards";
    public const string BATTLE_UI_SHOW_MENU = "BattleShowMenu";
    public const string BATTLE_UI_CLOSE_MENU = "BattleCloseMenu";
    public const string BATTLE_UI_ACTIVATE_CARDSINHAND = "ActivateCardsInHand";
    public const string BATTLE_UI_INACTIVATE_CARDSINHAND = "InActivateCardsInHand";

    public const string BATTLE_UI_PLAYER_WIN = "PlayerWin";
    public const string BATTLE_UI_PLAYER_LOSE = "PlayerLose";
    #endregion
    public const string BATTLE_REFRESH_PLAYER_INFO = "RefreshPlayerInfo";
    public const string BATTLE_REFRESH_ENERMY_INFO = "RefreshEnermyInfo";

}
