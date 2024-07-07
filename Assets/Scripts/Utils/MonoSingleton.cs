using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//����Լ��TΪSingleton����������ࣨwhere <ҪԼ���ķ��ͷ�������T> : <Լ���ľ�������>��
//����Լ�������ù���˼�����Լ�����ʹ�������ͣ�һ��������Լ����ʽ���ﲻչ������
public abstract class MonoSingleton<T> : MonoBehaviour where T: MonoSingleton<T> 
{
    protected static T _instance;
    public static T Instance{
        get{
            //��ȡ����ʵ��ʱ���ʵ��Ϊ��
            if(_instance == null){
                //�����ڳ�����Ѱ���Ƿ�����object���ص�ǰ�ű�
                _instance = FindObjectOfType<T>();
                //���������û�й��ص�ǰ�ű���ô������һ���յ�gameobject�����ش˽ű�
                if(_instance == null){
                    //���������������ڴ���ʱ���������Ͻű���Awake������T��Awake��T��Awakeʵ�����Ǽ̳еĸ���ģ�
                    //���Դ�ʱ����Ϊ_instance��ֵ�������Awake�и�ֵ��
                    new GameObject("singleton of "+typeof(T)).AddComponent<T>();
                }
            }
            return _instance;
        }
    }
    //����Ϸ�ʼʱ����Awake �����ǰ�ű��Ѿ����ص���gameobject����Ὣ_instance��ֵΪ�ű�����
    private void Awake() {
        _instance = this as T;        
    }
}
