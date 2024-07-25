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

    public void InitInfo(BattleEnermyBean characterBean)
    {
        this._character = characterBean;
        float enermyOrignX;
        enermyOrignX = this.transform.localPosition.x + 200;
        this.transform.DOLocalMoveX(enermyOrignX, 0);
        //��ʼ״̬
        this.GetComponent<CanvasGroup>().DOFade(0, 0);
        //�볡
        this.GetComponent<CanvasGroup>().DOFade(1, 0.5f);
        this.transform.DOLocalMoveX(0, 1f);
        this._hpSlider = this.GetComponentInChildren<Slider>();
        _atkPanel = this.transform.Find("AtkPanel").gameObject;
        _defPanel = this.transform.Find("DefPanel").gameObject;
        _atkPanel.GetComponentInChildren<Text>().text = $"{this.Character._curAtk}";
        _defPanel.GetComponentInChildren<Text>().text = $"{this.Character._curAtk}";
    }
    public CharacterBean Character { get => _character; }

    /// <summary>
    /// ���ݴ������ֵ����Ѫ��������
    /// </summary>
    /// <param name="value"></param>
    public void UpdateHp(float value)
    {
        //��Ҫ�ж�һ�£�����۳���Ѫ�����ڻ�������Ѫ���۳���������������
        if (value < 0 && Math.Abs(value) <= this.Character._curDef)
        {
            ToastManager.Instance?.CreatToast("�˺����ڻ���ֵ���޷�����˺�~~~");
            return;
        }
        if (value< 0 && Math.Abs(value) > this.Character._curDef)
        {
            value = -(Math.Abs(value)-this.Character._curDef);
        }
        this.Character._curHP += value;
        this._hpSlider.DOValue(this.Character._curHP / this.Character._maxHP, 0.5f);
    }
    /// <summary>
    /// ���ݴ������ֵ���й�����������
    /// </summary>
    /// <param name="value"></param>
    public void UpdateAtk(float value)
    {
        float tem = this.Character._curAtk;
        this.Character._curAtk += value;
        _atkPanel.GetComponentInChildren<Text>().text = $"{this.Character._curAtk}";
        DOTween.To(() => tem, x => tem = x, this.Character._curAtk, 0.5f).SetEase(Ease.Linear).Play();
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
    /// ���ݴ������ֵ����Ѫ��������
    /// </summary>
    /// <param name="value"></param>
    public void UpdateDef(float value)
    {
        float tem = this.Character._curDef;
        this.Character._curDef += value;
        _defPanel.GetComponentInChildren<Text>().text = $"{this.Character._curDef}";
        DOTween.To(() => tem, x => tem = x, this.Character._curDef, 0.5f).SetEase(Ease.Linear).Play();
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
        //�򵥶�������ǰ�ƶ�����
        float temPosX = this.transform.localPosition.x - 1000;
        Sequence sequence = DOTween.Sequence();
        sequence.AppendCallback(() =>
        {
            this.transform.DOLocalMoveX(temPosX, 0.2f);
            this.transform.DOShakeScale(0.2f);
            float ranAtk = new MinMaxRandomFloat(3, 10).GetRandomValue();
            BattlePlayerInfo.Instance?.UpdateHp(-(ranAtk + this.Character._curAtk));
        })
        .AppendInterval(0.2f)
        .AppendCallback(() =>
        {
            //�����ظ�
            this.transform.DOLocalMoveX(0, 0.2f);
            action();
        });
    }
}
