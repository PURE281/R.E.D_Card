using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToastManager : MonoSingleton<ToastManager>
{


    public GameObject ToastPrefab;
    public List<ToastHandler> ToastList = new List<ToastHandler>();

    private float Timer = 0;
    public float Interval;

    public Transform Parent;


    //通过这里来创建Toast预制体
    public void CreatToast(string str)
    {
        var Toa = Instantiate(ToastPrefab, Parent.transform.position, Parent.transform.rotation, Parent.transform);
        var comp = Toa.GetComponent<ToastHandler>();
        ToastList.Insert(0, comp);
        comp.InitToast(str, () => {
            ToastList.Remove(comp);
        });

        Timer = 0;

        //有新的Toast出现，之前的Toast向上移动
        ToastMove(0.2f);
    }

    public void ToastMove(float speed)
    {
        for (int i = 0; i < ToastList.Count; i++)
        {
            ToastList[i].Move(speed, i + 1);
        }
    }
}