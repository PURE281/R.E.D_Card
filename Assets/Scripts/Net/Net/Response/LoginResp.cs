using Message;
using Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Response
{
    public class LoginResp : RespManager
    {

        public override void Execute(MsgData data)
        {
            var v = LoginResponse.Parser.ParseFrom(data.data);
            if (!v.Success)
            {
                // PopUp.PopEnter("登录失败", "账号或者密码错误", "确定");
                return;
            }
            // RoleManager.Role = (v.Role);

            // 判断 玩家当前是否在对局中;不在就进入大厅
            /*            if (v.Loca == 0) {
                            UiMain.Instance.UserModelUpdate();
                            UiMain.Instance.EnterHall();
                        } else if(v.Loca == 1){
                            // 对战数据,
                            BattleManager.Instance.RelinkBattle(v.Data);
                        }*/

            Debug.LogFormat("登录成功：{0}",v.Msg);

        }

        public override ECode GetCode()
        {
            return ECode.Login;
        }
    }
}




