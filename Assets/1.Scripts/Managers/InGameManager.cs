using DefineCoroutine;
using Facebook.Unity;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameManager : MonoBehaviour
{
    public static InGameManager _Instance { get; private set; }

    int _currentScore;

    float _puzzleStartTime;


    UIShower _uiShower;
    SlotManager _uiSlotManager;

    [Header("Prefabs")]
    [SerializeField] GameObject _prefabMessageBox;
    [SerializeField] GameObject _prefabSettlementWnd;

    UIMessageBox _uiMessageBox;
    UISettlementWindow _uiSettlementWindow;



    private void Awake()
    {
        _Instance = this;
    }
    private void Start()
    {
        InitManager();
    }

    public void InitManager()
    {
        _uiSlotManager = transform.Find("Tiles").GetComponent<SlotManager>();
        _uiShower = transform.Find("UIShower").GetComponent<UIShower>();

        _uiSlotManager.InitManager(10, 10);
        _uiShower.InitUI();

        GameObject tempBox = Instantiate(_prefabMessageBox, transform);
        _uiMessageBox = tempBox.GetComponent<UIMessageBox>();
        _uiMessageBox.InitBox();

        StartCoroutine(CoroutineUtility.DelayAction(1, () =>
        {
            _uiShower.Open();
            _uiSlotManager.ScatterSlots();
            _uiMessageBox.ShowBox(UIMessageBox.ShowType.Slide, UIMessageBox.ShowType.Slide, "Start!");
            _uiMessageBox.SetText("Ready~");
            _uiMessageBox.AnimationEndCallback += PuzzleStart;
        }));
        _currentScore = 200;
    }

    void PuzzleStart()
    {
        Debug.Log("Puzzle Start!");
        _uiSlotManager.SetPuzzleEnable(true);
        _puzzleStartTime = Time.time;
        _uiShower.StartPuzzle(_puzzleStartTime);
    }

    public void OnTimeOut()
    {
        Debug.Log("Time Out!");
        _uiMessageBox.ShowBox(UIMessageBox.ShowType.PopUp, UIMessageBox.ShowType.PopUp);
        _uiMessageBox.SetText("Time's Up.");
        _uiSlotManager.SetPuzzleEnable(false);

        float playTime = Time.time - _puzzleStartTime;
        _uiMessageBox.AnimationEndCallback += () => OpenSettlementWindow(playTime, false);
        //°á»êÃ¢ ÆË¾÷.
    }
    public void OnPuzzleClear()
    {
        Debug.Log("Puzzle Clear!");
        _uiMessageBox.ShowBox(UIMessageBox.ShowType.Slide, UIMessageBox.ShowType.PopUp);
        _uiMessageBox.SetText("Finished!");
        _uiSlotManager.SetPuzzleEnable(false);
        _uiShower.OnShowerStop();

        float playTime = Time.time - _puzzleStartTime;
        _uiMessageBox.AnimationEndCallback += () => OpenSettlementWindow(playTime, true);
        //°á»êÃ¢ ÆË¾÷.
    }

    void OpenSettlementWindow(float playTime, bool isClear)
    {
        _uiSettlementWindow = Instantiate(_prefabSettlementWnd, transform).GetComponent<UISettlementWindow>();

        _uiSettlementWindow.InitWnd(playTime, _currentScore, isClear);
    }

    public void AddScore(int score)
    {
        _currentScore += score;
        _uiShower.RenewTimer();
        _uiShower.SetScoreText(_currentScore);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
    public void LogOut()
    {
        if (FB.IsLoggedIn)
        {
            Debug.Log("·Î±×¾Æ¿ô ÇØ¾ß´ï");
            FB.LogOut();
        }
        else
            Debug.Log("·Î±×¾Æ¿ô ¾È ÇØµµ µÊ");
        SceneManager.LoadScene(0);
    }
    public void RestartGame()
    {
        SceneManager.LoadScene(1);
    }

}
