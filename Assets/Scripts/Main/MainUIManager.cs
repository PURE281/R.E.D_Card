
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;

public class MainUIManager : MonoSington<MainUIManager>
{
    private GameObject _loginAndRegisterPanel;
    private GameObject _loginPanel;
    private GameObject _registerPanel;
    private GameObject _userPanel;

    private Transform _mainCanvas;
    public void Init()
    {
        _mainCanvas = GameObject.FindGameObjectWithTag("MainCanvas").transform;

        GameObject _temLoginAndregisterPanel = Resources.Load<GameObject>("Prefabs/LoginAndRegisterPanel");
        _loginAndRegisterPanel = Instantiate(_temLoginAndregisterPanel);
        _loginAndRegisterPanel.transform.SetParent(_mainCanvas);
        _loginAndRegisterPanel.transform.localPosition = Vector3.zero;

        _userPanel = _mainCanvas.Find("UserPanel").gameObject;
        //GameObject _temUserPanel = Resources.Load<GameObject>("Prefabs/UserPanel");
        //_userPanel = Instantiate(_temUserPanel);
        //_userPanel.transform.SetParent(_mainCanvas);
        //_userPanel.transform.localPosition = Vector3.zero;

        #region 登录功能初始化
        _loginPanel = _loginAndRegisterPanel.transform.Find("LoginPanel").gameObject;
        Button _loginbtn = _loginPanel.transform.Find("login").GetComponent<Button>();
        InputField _loginUsername = _loginPanel.transform.Find("username").GetComponent<InputField>();
        InputField _loginPassword = _loginPanel.transform.Find("password").GetComponent<InputField>();
        _loginbtn.onClick.AddListener(() =>
        {
            OnLogin(_loginUsername.text, _loginPassword.text);
        });
        _loginPanel.SetActive(false);
        #endregion
        #region 注册功能初始化
        _registerPanel = _loginAndRegisterPanel.transform.Find("RegisterPanel").gameObject;
        _registerPanel.transform.localPosition = Vector3.zero;
        Button _registerbtn = _registerPanel.transform.Find("register").GetComponent<Button>();
        InputField _registerUsername = _registerPanel.transform.Find("username").GetComponent<InputField>();
        InputField _registerPassword = _registerPanel.transform.Find("password").GetComponent<InputField>();
        InputField _registerNickname = _registerPanel.transform.Find("nickname").GetComponent<InputField>();
        //Dropdown _registerGender = _registerPanel.transform.Find("gender").GetComponent<Dropdown>();
        _registerbtn.onClick.AddListener(() =>
        {
            OnRegister(_registerUsername.text, _registerPassword.text, _registerNickname.text, 1);
        });
        _loginPanel.SetActive(false);
        #endregion
        _loginAndRegisterPanel.SetActive(false);

        #region 用户信息栏初始化
        _userPanel.transform.Find("UpdatePanel/UpdateUsernameBtn").GetComponent<Button>().onClick.AddListener(() =>
        {
            this.OnUpdateUser(_userPanel.transform.Find("UpdatePanel/UpdateUsernameInput").GetComponent<InputField>().text);
        });
        #endregion
        //检查当前是否已登录
        //是则无需登录
        //string playerjson = PlayerPrefs.GetString(GlobalConfig._playerName);
        //UserModel model = JsonConvert.DeserializeObject<UserModel>(playerjson);
        //if (model == null)
        //{
        //    //否则打开登录/注册界面
        //    Debug.Log("本地无用户信息存留");
        //    _loginAndRegisterPanel.SetActive(true);
        //}
        //else
        //{
        //    OnLogin(model.username, model.password);
        //}
    }

    public void OnLogin(string username, string password)
    {
        //信息初步筛查
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            ToastManager.Instance.CreatToast("信息未填写完全");
            return;
        }
        _mainCanvas.GetComponent<CanvasGroup>().interactable = false;
        //调用登录信息，校验账号密码是否正确
        UserManager.Instance?.Login(username, password, (result) =>
        {
            JObject jo = JObject.Parse(result);
            UserModel model = new UserModel();
            if ((int)jo["code"] == 200)
            {
                model.username = username;
                model.password = password;
                model.nickname = jo["data"]["nickname"].ToString();
                model.status = (int)jo["data"]["status"];
                model.id = (int)jo["data"]["id"];
                GlobalConfig.Instance._curUserModel = model;
                //校验成功，进入主场景，显示用户名称等信息
                ToastManager.Instance.CreatToast("登录成功");
                //记录到本地
                string playerinfo = JsonConvert.SerializeObject(model);
                PlayerPrefs.SetString(GlobalConfig._playerName, playerinfo);
                this._loginAndRegisterPanel.SetActive(false);
                GameObject.Find("Username").GetComponent<Text>().text = model.nickname;
            }
            else
            {
                ToastManager.Instance?.CreatToast(jo["msg"].ToString());
                //填写失败，返回登录界面

                //this._loginAndRegisterPanel.SetActive(true);
            }
            _mainCanvas.GetComponent<CanvasGroup>().interactable = true;
        });
    }

    public void OnRegister(string username, string password, string nickname, int gender)
    {
        //信息初步筛查
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(nickname) || password.Length < 6)
        {
            ToastManager.Instance.CreatToast("信息未填写完全/密码小于6位");
            return;
        }
        _mainCanvas.GetComponent<CanvasGroup>().interactable = false;
        UserManager.Instance.Register(username, password, nickname, gender, (result) =>
        {
            JObject jo = JObject.Parse(result);
            if ((int)jo["code"] == 200)
            {
                //校验成功，进入主场景，显示用户名称等信息
                ToastManager.Instance.CreatToast("注册成功");
                this._loginPanel.SetActive(true);
                this._registerPanel.SetActive(false);
            }
            else
            {
                ToastManager.Instance?.CreatToast(jo["msg"].ToString());
            }
            _mainCanvas.GetComponent<CanvasGroup>().interactable = true;
        });
    }

    public void OnUpdateUser(string nickname)
    {
        //信息初步筛查
        if (string.IsNullOrEmpty(nickname))
        {
            ToastManager.Instance.CreatToast("不能为空！");
            return;
        }
        _mainCanvas.GetComponent<CanvasGroup>().interactable = false;
        UserManager.Instance.UpdateUser(nickname, (result) =>
        {
            JObject jo = JObject.Parse(result);
            if ((int)jo["code"] == 200)
            {
                //校验成功，进入主场景，显示用户名称等信息
                ToastManager.Instance.CreatToast("修改成功");
                this._userPanel.transform.Find("Username").GetComponent<Text>().text = nickname;
                this._userPanel.transform.Find("UpdatePanel").gameObject.SetActive(false);
            }
            else
            {
                ToastManager.Instance?.CreatToast(jo["msg"].ToString());
            }
            _mainCanvas.GetComponent<CanvasGroup>().interactable = true;
        });
    }

}
