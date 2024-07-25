using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//泛型约束T为Singleton自身或其子类（where <要约束的泛型符号例如T> : <约束的具体描述>）
//泛型约束的作用顾名思义就是约束泛型传入的类型，一共有六种约束方式这里不展开讲。
public abstract class MonoSington<T> : MonoBehaviour where T : MonoBehaviour
{
    public bool global = false;

    static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = (T)FindObjectOfType<T>();
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (global)
        {
            if (instance != null && instance != this.gameObject.GetComponent<T>())
            {
                Destroy(this.gameObject);
                return;
            }
            DontDestroyOnLoad(this.gameObject);
            instance = this.gameObject.GetComponent<T>();
        }
        this.OnStart();
    }

    protected virtual void OnStart()
    {

    }
}