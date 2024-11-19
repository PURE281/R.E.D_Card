using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net {
    public enum ECode {
        Port = 52711,
        // 注册
        Register = 1,
        Login = 2,
        UpdateName = 3,
        UpdateModel = 4,        // 改变角色 形象


        LeaveGame = 9,

        Matching = 10,
        Look = 11,      // 观战

        LeaveBattle = 20,


        // 以下是对局    
        BattleBegin = 50,       // 通知客户端,对局开启; 需要通知 客户端 双方的 昵称,ID,等数据  
        SendRollRoll = 51,       // 通知 所有玩家,该某个角色 roll了
        RoleRoll = 52,           // 请求roll点 以及返回

        MoveTargetEnd = 53,         // 玩家通知服务器,自己到达位置了
        FuncRequest = 54,           // 根据玩家在 所在的位置 发出请求;升级建筑/建筑新的建筑/购买道具等
        BattleShop = 55,            // 商店购买对局道具
        BattleUseItem = 56,        // 对局使用了道具


        BattleOver = 60,            // 有人对局结束
        Relink = 61,                // 重连

    }
}
