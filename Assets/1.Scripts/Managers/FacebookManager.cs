using UnityEngine;
using Facebook.Unity;
using System.Collections.Generic;

public class FacebookManager : MonoBehaviour
{
    [SerializeField] UILoginInteractive _loginInteractive;

    public void InitFacebook()
    {
        if (!FB.IsInitialized)
        {
            Debug.Log("�ʱ�ȭ ����");
            FB.Init(InitComplete);
        }
        else
        {
            Debug.Log("�ʱ�ȭ �̹� �Ǿ�����.");
            LoginProcess();
        }
    }

    void InitComplete()
    {
        if (FB.IsInitialized)
        {
            Debug.Log("�ʱ�ȭ ����");
            LoginProcess();
        }
        else
            Debug.LogError("�ʱ�ȭ ����");
    }
    void LoginProcess()
    {
        FB.ActivateApp();
        string[] param = { "public_profile", "email" };
        FB.LogInWithPublishPermissions(param, LoginComplete);

    }
    void LoginComplete(ILoginResult result)
    {
        if (FB.IsLoggedIn)
        {
            Debug.Log("���̽��� �α��� ����");
            AccessToken acT = AccessToken.CurrentAccessToken;

            Debug.Log(acT.UserId);
            _loginInteractive.SetLogInOut(true);

            FB.API("/me?fields=id,name", HttpMethod.GET, GetUserInfoCallBack);
            FB.API(acT.UserId + "/picture", HttpMethod.GET, GetUserInfoCallBackForGraph);
        }
        else
        {
            if (result.Error != null)
                Debug.LogFormat("���̽��� �α��� ���� : Error Code [{0}]", result.Error);
            else
                Debug.Log("���̽��� �α��� ����(User Canceled)");
        }
    }
    void GetUserInfoCallBack(IResult result)
    {
        if (result.Cancelled || !string.IsNullOrEmpty(result.Error))
        {
            Debug.Log("���� ���� �б� ����");
            return;
        }

        Dictionary<string, object> userInfo
            = Facebook.MiniJSON.Json.Deserialize(result.RawResult) as Dictionary<string, object>;

        if (userInfo == null)
            Debug.LogError("���� ������ �Ľ̿� �����߽��ϴ�. Reason:" + result.RawResult);

        if (userInfo.ContainsKey("name"))
        {
            string tempStr = userInfo["name"].ToString();
            //tempStr ���
            _loginInteractive.SetName(tempStr);
        }
        if (userInfo.ContainsKey("id"))
        {
            string tempStr = userInfo["id"].ToString();
            //tempStr ���
            _loginInteractive.SetId(tempStr);
        }

    }
    void GetUserInfoCallBackForGraph(IGraphResult result)
    {
        if (result.Cancelled || !string.IsNullOrEmpty(result.Error))
        {
            Debug.Log("���� ���� �б� ����");
            return;
        }
        Texture2D texture = result.Texture;
        //texture ���
        _loginInteractive.SetImage(texture);
    }

    public void OnLogOut()
    {
        FB.LogOut();
        _loginInteractive.SetLogInOut(false);
    }
}
