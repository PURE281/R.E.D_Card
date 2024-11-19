using Message;
using Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Response {
    public class RegisterResp : RespManager {

        public override void Execute(MsgData data) {
            RegisterResponse v = RegisterResponse.Parser.ParseFrom(data.data);
            Debug.LogError(v);
            if (v.Code) {
                // 如果注册 成功 的逻辑
            } else {
                
            }
            Debug.LogError(v.Msg);

        }

        public override ECode GetCode() {
            return ECode.Register;
        }
    }
}




