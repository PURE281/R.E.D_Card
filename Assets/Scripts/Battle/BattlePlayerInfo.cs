using DG.Tweening;
using RandomElementsSystem.Types;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattlePlayerInfo : MonoSingleton<BattlePlayerInfo>
{
    [SerializeField]
    private CharacterBean _character;

    public void InitInfo(BattlePlayerBean characterBean)
    {
        this._character = characterBean;
        float playerOrignX;
        //简单的入场动画
        //主角入场
        playerOrignX = this.transform.localPosition.x - 200;
        this.transform.DOLocalMoveX(playerOrignX, 0);
        //初始状态
        this.GetComponent<CanvasGroup>().DOFade(0, 0);
        //入场
        this.GetComponent<CanvasGroup>().DOFade(1, 0.5f);
        this.transform.DOLocalMoveX(0, 1f);
    }
    public CharacterBean Character { get => _character; }

    /// <summary>
    /// 根据传入的数值进行血量的增减
    /// </summary>
    /// <param name="value"></param>
    public void UpdateHp(float value)
    {
        this.Character._curHP += value;
        this.GetComponentInChildren<Slider>().DOValue(this.Character._curHP / this.Character._maxHP, 0.5f);
    }
    /// <summary>
    /// 根据传入的数值进行攻击力的增减
    /// </summary>
    /// <param name="value"></param>
    public void UpdateAtk(float value)
    {
        this.Character._curAtk += value;
    }
    /// <summary>
    /// 根据传入的数值进行血量的增减
    /// </summary>
    /// <param name="value"></param>
    public void UpdateDef(float value)
    {
        this.Character._curDef += value;
    }

}
