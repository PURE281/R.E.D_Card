using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//泛型约束T为Singleton自身或其子类（where <要约束的泛型符号例如T> : <约束的具体描述>）
//泛型约束的作用顾名思义就是约束泛型传入的类型，一共有六种约束方式这里不展开讲。
public abstract class MonoSingleton<T> : MonoBehaviour where T: MonoSingleton<T> 
{
    protected static T _instance;
    public static T Instance{
        get{
            //获取单例实例时如果实例为空
            if(_instance == null){
                //首先在场景中寻找是否已有object挂载当前脚本
                _instance = FindObjectOfType<T>();
                //如果场景中没有挂载当前脚本那么则生成一个空的gameobject并挂载此脚本
                if(_instance == null){
                    //如果创建对象，则会在创建时调用其身上脚本的Awake即调用T的Awake（T的Awake实际上是继承的父类的）
                    //所以此时无需为_instance赋值，其会在Awake中赋值。
                    new GameObject("singleton of "+typeof(T)).AddComponent<T>();
                }
            }
            return _instance;
        }
    }
    //在游戏最开始时调用Awake 如果当前脚本已经挂载到了gameobject上则会将_instance赋值为脚本自身
    private void Awake() {
        _instance = this as T;        
    }
}
