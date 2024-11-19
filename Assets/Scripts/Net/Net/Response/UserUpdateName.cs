using Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Message;

namespace Response {
    public class UserUpdateName : RespManager {
        public override void Execute(MsgData data) {
            // UserRegisterResponse v = UserRegisterResponse.Parser.ParseFrom(data.data);
            //UserUpdateNicknameResponse v = UserUpdateNicknameResponse.Parser.ParseFrom(data.data);
            // v.Success;
            // v.Msg
            // v.Nickname

        }

        public override ECode GetCode() {
            return ECode.UpdateName;
        }
    }
}
