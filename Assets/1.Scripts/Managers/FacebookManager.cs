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
            Debug.Log("초기화 시작");
            FB.Init(InitComplete);
        }
        else
        {
            Debug.Log("초기화 이미 되어있음.");
            LoginProcess();
        }
    }

    void InitComplete()
    {
        if (FB.IsInitialized)
        {
            Debug.Log("초기화 성공");
            LoginProcess();
        }
        else
            Debug.LogError("초기화 실패");
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
            Debug.Log("페이스북 로그인 성공");
            AccessToken acT = AccessToken.CurrentAccessToken;

            Debug.Log(acT.UserId);
            _loginInteractive.SetLogInOut(true);

            FB.API("/me?fields=id,name", HttpMethod.GET, GetUserInfoCallBack);
            FB.API(acT.UserId + "/picture", HttpMethod.GET, GetUserInfoCallBackForGraph);
        }
        else
        {
            if (result.Error != null)
                Debug.LogFormat("페이스북 로그인 실패 : Error Code [{0}]", result.Error);
            else
                Debug.Log("페이스북 로그인 실패(User Canceled)");
        }
    }
    void GetUserInfoCallBack(IResult result)
    {
        if (result.Cancelled || !string.IsNullOrEmpty(result.Error))
        {
            Debug.Log("유저 정보 읽기 실패");
            return;
        }

        Dictionary<string, object> userInfo
            = Facebook.MiniJSON.Json.Deserialize(result.RawResult) as Dictionary<string, object>;

        if (userInfo == null)
            Debug.LogError("유저 정보의 파싱에 실패했습니다. Reason:" + result.RawResult);

        if (userInfo.ContainsKey("name"))
        {
            string tempStr = userInfo["name"].ToString();
            //tempStr 출력
            _loginInteractive.SetName(tempStr);
        }
        if (userInfo.ContainsKey("id"))
        {
            string tempStr = userInfo["id"].ToString();
            //tempStr 출력
            _loginInteractive.SetId(tempStr);
        }

    }
    void GetUserInfoCallBackForGraph(IGraphResult result)
    {
        if (result.Cancelled || !string.IsNullOrEmpty(result.Error))
        {
            Debug.Log("유저 정보 읽기 실패");
            return;
        }
        Texture2D texture = result.Texture;
        //texture 출력
        _loginInteractive.SetImage(texture);
    }

    public void OnLogOut()
    {
        FB.LogOut();
        _loginInteractive.SetLogInOut(false);
    }
}
