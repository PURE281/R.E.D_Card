using Message;
using Net;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Utils;

namespace Response {
    /// <summary>
    /// 消息处理 管理类; 当服务器 发送消息到客户端,客户端 拆包为 MsgData 之后,交给这个类来处理;
    /// </summary>
    /// <typeparam name="T">处理的消息类型</typeparam>
    public abstract class RespManager {

        public static Dictionary<int, RespManager> dic = new Dictionary<int, RespManager>();

        // 当客户端收到消息后,就会调用这个方法;从 dic 字典中 取出对应的类来处理 这个消息
        public static void execute(MsgData data) {
            if (dic.ContainsKey(data.code)) {
                dic[data.code].Execute(data);
            } else {
                Debug.LogError("不存在的code");
            }
        }


        public abstract void Execute(MsgData data);

        public abstract ECode GetCode();

        public static void Start() {
            var types = Assembly.GetCallingAssembly().GetTypes();
            var aType = typeof(RespManager);

            foreach (var type in types) {
                var baseType = type.BaseType;  //获取基类
                while (baseType != null) { //获取所有基类

                    if (baseType.Name == aType.Name) {
                        Type objtype = Type.GetType(type.FullName, true);
                        object obj = Activator.CreateInstance(objtype);
                        if (obj != null) {
                            RespManager info = obj as RespManager;
                            dic.Add((int)info.GetCode(), info);
                        }
                        break;
                    } else {
                        baseType = baseType.BaseType;
                    }
                }
            }
        }
    }
}
