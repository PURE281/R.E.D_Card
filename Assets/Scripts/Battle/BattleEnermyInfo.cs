using DG.Tweening;
using RandomElementsSystem.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleEnermyInfo : MonoSington<BattleEnermyInfo>
{
    [SerializeField]
    private CharacterBean _character;

    private Slider _hpSlider;
    private GameObject _atkPanel;
    private GameObject _defPanel;

    public void InitInfo(CharacterBean characterBean)
    {
        this._character = characterBean;
        float enermyOrignX;
        enermyOrignX = this.transform.localPosition.x + 200;
        this.transform.DOLocalMoveX(enermyOrignX, 0);
        //初始状态
        this.GetComponent<CanvasGroup>().DOFade(0, 0);
        //入场
        this.GetComponent<CanvasGroup>().DOFade(1, 0.5f);
        this.transform.DOLocalMoveX(0, 1f);
        this._hpSlider = this.GetComponentInChildren<Slider>();
        _atkPanel = this.transform.Find("AtkPanel").gameObject;
        _defPanel = this.transform.Find("DefPanel").gameObject;
        _atkPanel.GetComponentInChildren<Text>().text = $"{this.Character.curAtk}";
        _defPanel.GetComponentInChildren<Text>().text = $"{this.Character.curAtk}";
    }
    public CharacterBean Character { get => _character; }

    /// <summary>
    /// 根据传入的数值进行血量的增减
    /// </summary>
    /// <param name="value"></param>
    public void UpdateHp(float value)
    {
        //需要判断一下，如果扣除的血量低于护甲则不作血量扣除并进行友善提醒
        if (value < 0 && Math.Abs(value) <= this.Character.curDef)
        {
            ToastManager.Instance?.CreatToast("伤害低于护甲值，无法造成伤害~~~");
            return;
        }
        if (value< 0 && Math.Abs(value) > this.Character.curDef)
        {
            value = -(Math.Abs(value)-this.Character.curDef);
        }
        this.Character.curHP += value;
        this._hpSlider.DOValue(this.Character.curHP / this.Character.maxHP, 0.5f);
    }
    /// <summary>
    /// 根据传入的数值进行攻击力的增减
    /// </summary>
    /// <param name="value"></param>
    public void UpdateAtk(float value)
    {
        float tem = this.Character.curAtk;
        this.Character.curAtk += value;
        _atkPanel.GetComponentInChildren<Text>().text = $"{this.Character.curAtk}";
        DOTween.To(() => tem, x => tem = x, this.Character.curAtk, 0.5f).SetEase(Ease.Linear).Play();
        Sequence sequence = DOTween.Sequence();
        sequence.AppendCallback(() =>
        {
            _atkPanel.transform.DOScale(1.2f, 1f);
        }).AppendInterval(1).AppendCallback(() =>
        {
            _atkPanel.transform.DOScale(1f, 0);
        });
    }
    /// <summary>
    /// 根据传入的数值进行血量的增减
    /// </summary>
    /// <param name="value"></param>
    public void UpdateDef(float value)
    {
        float tem = this.Character.curDef;
        this.Character.curDef += value;
        _defPanel.GetComponentInChildren<Text>().text = $"{this.Character.curDef}";
        DOTween.To(() => tem, x => tem = x, this.Character.curDef, 0.5f).SetEase(Ease.Linear).Play();
        Sequence sequence = DOTween.Sequence();
        sequence.AppendCallback(() =>
        {
            _defPanel.transform.DOScale(1.2f, 1f);
        }).AppendInterval(1).AppendCallback(() =>
        {
            _defPanel.transform.DOScale(1f, 0);
        });
    }

    public void Attack(Action action)
    {
        //简单动画，向前移动，震动
        float temPosX = this.transform.localPosition.x - 1000;
        Sequence sequence = DOTween.Sequence();
        sequence.AppendCallback(() =>
        {
            this.transform.DOLocalMoveX(temPosX, 0.2f);
            this.transform.DOShakeScale(0.2f);
            float ranAtk = new MinMaxRandomFloat(3, 10).GetRandomValue();
            BattlePlayerInfo.Instance?.UpdateHp(-(ranAtk + this.Character.curAtk));
        })
        .AppendInterval(0.2f)
        .AppendCallback(() =>
        {
            //在最后回复
            this.transform.DOLocalMoveX(0, 0.2f);
            action();
        });
    }
}
