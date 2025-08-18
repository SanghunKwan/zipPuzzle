using DefineCoroutine;
using System.IO;
using TMPro;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.UI;

public class UISettlementWindow : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _timeText;
    [SerializeField] TextMeshProUGUI _scoreText;

    [SerializeField] TextMeshProUGUI _popUpTimeText;
    [SerializeField] TextMeshProUGUI _popUpScoreText;

    [SerializeField] TextMeshProUGUI _titleText;
    [SerializeField] Image _titleBackground;

    [SerializeField] Color _failColor;
    [SerializeField] Color _failBackgroundColor;



    float _playTime;
    int _score;

    int _scoreDigits;
    int _addDigits;

    private void Update()
    {
        _scoreText.text = Random.Range(0, Mathf.Pow(10, _scoreDigits)).ToString("N0");
        _popUpScoreText.text = Random.Range(0, Mathf.Pow(10, _addDigits)).ToString("N0");
    }


    public void InitWnd(float playTime, int score, bool isClear)
    {
        if (!isClear)
        {
            _titleText.text = "Failed";
            ColorUtility.TryParseHtmlString("#B73E3E", out Color color);
            _titleText.color = color;
            _titleBackground.color = _failColor;

        }

        _playTime = playTime;
        _score = score;

        SetTime(((int)(playTime / 60)).ToString("D2") + ":" + ((int)(playTime % 60)).ToString("D2"));
        SetScore(score.ToString("N0"));

        StartCoroutine(CoroutineUtility.DelayAction(2, () =>
        {
            _timeText.enabled = true;
            _scoreText.enabled = true;
        }));

        if (!isClear) return;

        StartCoroutine(CoroutineUtility.DelayAction(3, () =>
        {
            PopUp();
        }));
        StartCoroutine(CoroutineUtility.DelayAction(4, () =>
        {
            CalculateScore();
        }));
    }

    public void SetTime(in string str)
    {
        _timeText.text = str;
    }
    public void SetScore(in string str)
    {
        _scoreText.text = str;
    }

    public void PopUp()
    {
        int spareTime = (int)(420f - _playTime);
        int addScore = Mathf.FloorToInt(spareTime / 10f) * 10;
        _score += addScore;
        _scoreDigits = (_score == 0) ? 1 : Mathf.FloorToInt(Mathf.Log10(Mathf.Abs(_score)) + 1);
        if (_score < 0) _scoreDigits *= -1;

        _addDigits = (addScore == 0) ? 1 : Mathf.FloorToInt(Mathf.Log10(Mathf.Abs(addScore)) + 1);
        if (addScore < 0) _addDigits *= -1;

        _popUpTimeText.text = (spareTime % 60).ToString("D2") + ":" + (spareTime / 60).ToString("D2");
        _popUpScoreText.text = addScore.ToString("N0");
        SaveData();
        _popUpScoreText.enabled = true;
        _popUpTimeText.enabled = true;
    }

    void SaveData()
    {
        DonDestroyData data = GameObject.FindWithTag("DontDestroyData").GetComponent<DonDestroyData>();

        string id = data._recordData.recordDataList[data._currentPlayerIndex]._id;
        DataLoader.RecordDataList list = data._recordData;

        list.recordDataList[data._currentPlayerIndex]._records = _score;
        string jsonData = JsonUtility.ToJson(list);

        using (StreamWriter sw = new StreamWriter(DataLoader._dataPath))
        {
            sw.Write(jsonData);
        }
    }


    public void CalculateScore()
    {
        StartCoroutine(CoroutineUtility.DelayAction(1, SettleEffectEnd));

        enabled = true;
    }
    void SettleEffectEnd()
    {
        enabled = false;

        _scoreText.text = _score.ToString("N0");
        _popUpScoreText.text = 0.ToString("N" + _addDigits.ToString());
    }


    public void OnReplayButtonClicked()
    {
        InGameManager._Instance.RestartGame();
    }

    public void OnLogOutButtonClicked()
    {
        InGameManager._Instance.LogOut();
    }

    public void OnQuitButtonClicked()
    {
        InGameManager._Instance.QuitGame();
    }


}
