using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DefineReadonly;

public class UIShower : MonoBehaviour
{
    float _accumTimeStartedTime;
    float _timerCheckedTime;
    [SerializeField] float _timer = 7;


    [SerializeField] TextMeshProUGUI _accumulatedTime;
    [SerializeField] TextMeshProUGUI _nowScore;
    [SerializeField] TextMeshProUGUI _leftTimer;

    UISettingWindow _uiSettingWindow;


    [Header("Prefabs")]
    [SerializeField] GameObject _prefabsettingWindow;

    Animator _anim;



    private void Update()
    {
        RenewText();
    }



    public void InitUI()
    {
        _anim = GetComponent<Animator>();

        GameObject tempWnd = Instantiate(_prefabsettingWindow, InGameManager._Instance.transform);
        _uiSettingWindow = tempWnd.GetComponent<UISettingWindow>();
        tempWnd.SetActive(false);
    }
    public void Open()
    {
        _anim.SetBool(ReadOnly._animHash_isShow, true);
    }

    public void StartPuzzle(float startTime)
    {
        _accumTimeStartedTime = startTime;
        _timerCheckedTime = startTime + _timer;
        _nowScore.text = "0";
        enabled = true;
    }


    void RenewText()
    {
        float currentTime = Time.time;
        float passedTime = currentTime - _accumTimeStartedTime;
        float timerLeftTime = _timerCheckedTime - currentTime;

        _accumulatedTime.text = ((int)(passedTime / 60)).ToString("D2") + ":" + ((int)(passedTime % 60)).ToString("D2");

        if (timerLeftTime > 0)
        {
            _leftTimer.text = timerLeftTime.ToString("F4");
        }
        else
        {
            _leftTimer.text = "0.0000";
            InGameManager._Instance.OnTimeOut();
            OnShowerStop();
        }
    }

    public void RenewTimer()
    {
        _timerCheckedTime = Time.time + _timer; // Reset timer when score changes
    }
    public void SetScoreText(int newNum)
    {
        _nowScore.text = newNum.ToString("N0");
    }

    public void OnSettingButtonClicked()
    {
        _uiSettingWindow.gameObject.SetActive(!_uiSettingWindow.gameObject.activeInHierarchy);
    }

    public void OnShowerStop()
    {
        enabled = false;
    }
}
