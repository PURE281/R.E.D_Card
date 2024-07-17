using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventCenter : MonoSingleton<EventCenter>
{
    private Dictionary<string, UnityEvent<object>> _dic;

    // Start is called before the first frame update
    public void Init()
    {
        this._dic = new Dictionary<string, UnityEvent<object>>();
    }

    public void listen(string evtName, UnityAction<object> action)
    {
        if (this._dic.ContainsKey(evtName))
        {
            this._dic[evtName].AddListener(action);
        }
        else
        {
            this._dic.Add(evtName, new UnityEvent<object>());
            this._dic[evtName].AddListener(action);
        }
    }

    public void remove(string evtName, UnityAction<object> action)
    {
        if (this._dic.ContainsKey(evtName))
        {
            this._dic[evtName].RemoveListener(action);
        }
    }

    public void removeAll(string evtName)
    {
        if (this._dic.ContainsKey(evtName))
        {
            this._dic[evtName].RemoveAllListeners();
        }
    }

    public void dispatch(string evtName, object payload = null)
    {
        if (this._dic.ContainsKey(evtName))
        {
            this._dic[evtName].Invoke(payload);
        }
    }
}
