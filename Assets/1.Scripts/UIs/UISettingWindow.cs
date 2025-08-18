using UnityEngine;

public class UISettingWindow : MonoBehaviour
{
    public void OnLogoutButtonClicked()
    {
        InGameManager._Instance.LogOut();
    }
    public void OnCloseWindowButtonClicked()
    {
        gameObject.SetActive(false);
    }
    public void OnQuitButtonClicked()
    {
        InGameManager._Instance.QuitGame();
    }

}
