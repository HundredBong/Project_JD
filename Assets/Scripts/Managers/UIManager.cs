using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    private List<UIPopup> _popups = new List<UIPopup>();
    private List<UIPage> _pages = new List<UIPage>();
    private Stack<UIPopup> openPopups = new Stack<UIPopup>();
    private UIPage _currentPage;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        //-------------------------------------------------------------------

        SceneManager.sceneLoaded += RegisterUI;
    }

    private void Start()
    {
        Debug.Log("[UIManager] Start");
        // 초기 씬에서 UI 등록 (sceneLoaded가 호출되지 않을 수 있음)
        if (_pages.Count == 0)
        {
            Debug.Log("[UIManager] Pages count is 0, calling RegisterUI");
            RegisterUI(SceneManager.GetActiveScene(), LoadSceneMode.Single);
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= RegisterUI;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnStateChanged -= HandleGameStateChanged;
        }
    }

    private void Init()
    {
        foreach (UIPopup p in _popups)
        {
            p.gameObject.SetActive(false);
        }

        foreach (UIPage p in _pages)
        {
            p.gameObject.SetActive(false);
        }

        // GameManager 상태 변화 구독
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnStateChanged += HandleGameStateChanged;

            // 이미 상태가 설정되어 있으면 해당 페이지 열기
            if (GameManager.Instance.CurrentState != GameState.Idle)
            {
                HandleGameStateChanged(GameManager.Instance.CurrentState);
            }
        }
    }

    private void HandleGameStateChanged(GameState state)
    {
        Debug.Log($"[UIManager] HandleGameStateChanged: {state}");
        switch (state)
        {
            case GameState.Battle:
                PageOpen<BattlePage>();
                break;
            case GameState.Reward:
                PageOpen<RewardPage>();
                break;
            case GameState.CardSelect:
                PageOpen<CardSelectPage>();
                break;
            case GameState.CardDelete:
                PageOpen<CardDeletePage>();
                break;
            case GameState.GameOver:
                PageOpen<GameOverPage>();
                break;
        }
    }

    //T 타입을 받고, 그 타입의 객체를 반환
    //var page = UIManager.Instance.PageOpen<UIHome>(); 하면 T는 UIHome이 됨,
    //Where T : UIPage => T는 UIPage를 상속받아야 함. 
    public T PageOpen<T>() where T : UIPage
    {
        //페이지는 항상 하나만 떠야함. 

        //Debug.Log("[UIManager] 페이지 오픈");

        if (_currentPage != null)
        {
            _currentPage.Close();
        }

        T page = _pages.Find(p => p is T) as T;

        if (page != null)
        {
            page.Open();
            _currentPage = page;
        }

        return page;
    }

    public void PageClose()
    {
        while (openPopups.Count > 0)
        {
            PopupClose();
        }

        if (_currentPage != null)
        {
            _currentPage.Close();
            _currentPage = null;
        }
    }

    public T PopupOpen<T>() where T : UIPopup
    {
        //팝업은 여러개 떠도 됨, 알람 팝업이나 설정 팝업같은거, 닫을 때 마지막에 켜진거부터 닫아야 하니 스택에 저장

        //popup리스트에서 타입이 일치하는 첫번째 객체를 찾고, 그 객체를 다운캐스팅해서 반환
        T popup = _popups.Find(p => p is T) as T;

        if (popup != null)
        {
            popup.Open();
            openPopups.Push(popup);
        }

        return popup;
    }

    public void PopupClose()
    {
        if (openPopups.Count > 0)
        {
            //마지막에 켜진 팝업 순서대로 꺼짐
            UIPopup popup = openPopups.Pop();
            popup.Close();
        }
    }

    public void HandleBack()
    {
        //팝업 큐에 남아있는 팝업이 있으면 팝업을 닫음.
        if (openPopups.Count > 0)
        {
            PopupClose();
        }
        //큐에 남아있지 않고, 현재 페이지가 있으면 페이지를 닫음.
        else if (_currentPage != null)
        {
            PageClose();
        }
    }

    public bool TryGetPage<T>(out T page) where T : UIPage
    {
        foreach (UIPage p in _pages)
        {
            if (p is T target)
            {
                page = target;
                return true;
            }
        }

        page = null;
        return false;
    }

    private void RegisterUI(Scene scene, LoadSceneMode mode)
    {
        if (scene.name.Contains("Loading"))
        {
            return;
        }

        _popups.Clear();
        _pages.Clear();
        openPopups.Clear();
        _currentPage = null;

        UIPage[] foundPages = FindObjectsOfType<UIPage>(true);

        foreach (var page in foundPages)
        {
            _pages.Add(page);
        }

        UIPopup[] foundPopus = FindObjectsOfType<UIPopup>(true);

        foreach (var popup in foundPopus)
        {
            _popups.Add(popup);
        }

        Init();
    }
}
