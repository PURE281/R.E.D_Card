using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class UserManager : Singleton<UserManager>
{


    public void Login(string username, string password, Action<string> action)
    {
        MainSceneMgr.Instance?.StartCoroutine(LoginIE(username, password, action));
    }

    IEnumerator LoginIE(string username, string password, Action<string> action)
    {

        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);
        using (UnityWebRequest request = UnityWebRequest.Post(string.Format("{0}/{1}", GlobalConfig._baseUrlDevelop, "User/login"), form))
        {
            yield return request.SendWebRequest();
            if (request.isNetworkError || request.isHttpError)
            {
                ToastManager.Instance?.CreatToast("网络异常");
            }
            action(request.downloadHandler.text);
        }
    }


    public void Register(string username, string password, string nickname, int gender, Action<string> action)
    {
        MainSceneMgr.Instance?.StartCoroutine(RegisterIE(username, password, nickname, gender, action));
    }

    IEnumerator RegisterIE(string username, string password, string nickname, int gender, Action<string> action)
    {

        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);
        form.AddField("nickname", nickname);
        using (UnityWebRequest request = UnityWebRequest.Post(string.Format("{0}/{1}", GlobalConfig._baseUrlDevelop, "User/register"), form))
        {
            yield return request.SendWebRequest();
            if (request.isNetworkError || request.isHttpError)
            {
                ToastManager.Instance?.CreatToast("网络异常");
            }
            Debug.Log(request.downloadHandler.text);
            action(request.downloadHandler.text);
        }
    }
    public void UpdateUser(string nickname, Action<string> action)
    {
        MainSceneMgr.Instance?.StartCoroutine(UpdateUserIE(nickname, action));
    }

    IEnumerator UpdateUserIE(string nickname, Action<string> action)
    {

        WWWForm form = new WWWForm();
        form.AddField("id", GlobalConfig.Instance._curUserModel.id);
        form.AddField("nickname", nickname);
        using (UnityWebRequest request = UnityWebRequest.Post(string.Format("{0}/{1}", GlobalConfig._baseUrlDevelop, "User/updateUser"), form))
        {
            yield return request.SendWebRequest();
            if (request.isNetworkError || request.isHttpError)
            {
                ToastManager.Instance?.CreatToast("网络异常");
            }
            Debug.Log(request.downloadHandler.text);
            action(request.downloadHandler.text);
        }
    }
}

[Serializable]
public class UserModel
{
    public int id;
    public string username;
    public string password;
    public string nickname;
    public int gender;
    public int status;

}
