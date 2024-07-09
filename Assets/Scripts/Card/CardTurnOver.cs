using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;//����������

//����״̬�����桢����
public enum CardState
{
    Front,
    Back
}
public class CardTurnOver : MonoSingleton<CardTurnOver>
{
    public GameObject mFront;//��������
    public GameObject mBack;//���Ʊ���
    public CardState mCardState = CardState.Front;//���Ƶ�ǰ��״̬�������滹�Ǳ��棿
    public float mTime = 0.3f;
    private bool isActive = false;//true��������ִ�з�ת���������
    private void Start()
    {
    }

    /// <summary>
    /// ���������õĽӿ�
    /// </summary>
    public void StartBack()
    {
        if (isActive)
            return;
        StartCoroutine(ToBack());
    }
    /// <summary>
    /// ���������õĽӿ�
    /// </summary>
    public void StartFront()
    {
        if (isActive)
            return;
        StartCoroutine(ToFront());
    }
    /// <summary>
    /// ��ת������
    /// </summary>
    public IEnumerator ToBack()
    {
        isActive = true; 
        mFront.transform.DORotate(new Vector3(0, 90, 0), mTime);
        for (float i = mTime; i >= 0; i -= Time.deltaTime)
            yield return 0;
        mBack.transform.DORotate(new Vector3(0, 0, 0), mTime);
        isActive = false;

    }
    /// <summary>
    /// ��ת������
    /// </summary>
    public IEnumerator ToFront()
    {
        isActive = true;
        mBack.transform.DORotate(new Vector3(0, 90, 0), mTime);
        for (float i = mTime; i >= 0; i -= Time.deltaTime)
            yield return 0;
        mFront.transform.DORotate(new Vector3(0, 0, 0), mTime);
        isActive = false;
    }


}
