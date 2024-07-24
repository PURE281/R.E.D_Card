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
    }
    public CharacterBean Character { get => _character; }

    /// <summary>
    /// ���ݴ������ֵ����Ѫ��������
    /// </summary>
    /// <param name="value"></param>
    public void UpdateHp(float value)
    {
        this.Character._curHP += value;
        this.GetComponentInChildren<Slider>().DOValue(this.Character._curHP / this.Character._maxHP, 0.5f);
    }
    /// <summary>
    /// ���ݴ������ֵ���й�����������
    /// </summary>
    /// <param name="value"></param>
    public void UpdateAtk(float value)
    {
        this.Character._curAtk += value;
    }
    /// <summary>
    /// ���ݴ������ֵ����Ѫ��������
    /// </summary>
    /// <param name="value"></param>
    public void UpdateDef(float value)
    {
        this.Character._curDef += value;
    }

    public void Attack(Action action )
    {
        //�򵥶�������ǰ�ƶ�����
        float temPosX = this.transform.localPosition.x - 1000;
        Sequence sequence = DOTween.Sequence();
        sequence.AppendCallback(() =>
        {
            this.transform.DOLocalMoveX(temPosX,0.2f);
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
