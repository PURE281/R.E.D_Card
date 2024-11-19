using Net;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Message;
using Response;
using TMPro;

public class UiMain : MonoSingleton<UiMain> {

    public TMP_InputField username;
    public TMP_InputField password;
    public TMP_InputField nickname;

    public Button Login;
    public Button Register;

    protected override void OnStart() {
        RespManager.Start();
        NetClient.Instance.Init("127.0.0.1", ((int)ECode.Port));
        //Register.onClick.AddListener(SendRegister);
        Login.onClick.AddListener(SendLogin);
        Register.onClick.AddListener(SendRegister);
    }


    private void SendRegister() {
        RegisterRequest request = new RegisterRequest() {
            Username = username.text,
            Password = password.text,
            Nickname = nickname.text
        };

        NetClient.Instance.Send(ECode.Register, request);
    }

    private void SendLogin()
    {
        LoginRequest request = new LoginRequest()
        {
            Username = username.text,
            Password = password.text
        };

        NetClient.Instance.Send(ECode.Login, request);
    }

}
