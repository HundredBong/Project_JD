using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("게임 설정")]
    [SerializeField] private int _playerMaxHealth = 50;
    [SerializeField] private int _turnStartCost = 3;
    [SerializeField] private int _stagesPerChapter = 5;
    [SerializeField] private float _statMultiplierPerLoop = 1.1f;

    [Header("시작 덱")]
    [SerializeField] private CardData[] _startingDeck;

    [Header("카드 풀")]
    [SerializeField] private CardData[] _allCards;

    private Player _player;
    private GameState _currentState;
    private int _currentStage;
    private int _currentLoop;
    private float _currentStatMultiplier;

    public Player Player => _player;
    public GameState CurrentState => _currentState;
    public int CurrentStage => _currentStage;
    public int CurrentLoop => _currentLoop;
    public CardData[] AllCards => _allCards;

    public event Action<GameState> OnStateChanged;
    public event Action<int> OnStageChanged;
    public event Action<int> OnLoopChanged;
    public event Action OnGameOver;

    private void Awake()
    {
        Debug.Log("[GameManager] Awake");
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        Debug.Log("[GameManager] Start");
        InitializeGame();
        StartCoroutine(DelayedStartNewRun()); // 프로토타입용: 모든 Start() 완료 후 게임 시작
    }

    private System.Collections.IEnumerator DelayedStartNewRun()
    {
        Debug.Log("[GameManager] DelayedStartNewRun - waiting one frame");
        yield return null; // 한 프레임 대기
        Debug.Log("[GameManager] DelayedStartNewRun - calling StartNewRun");
        StartNewRun();
    }

    public void InitializeGame()
    {
        //게임 초기화 메서드

        //1회차 0스테이지, 배율 1배
        _currentLoop = 1;
        _currentStage = 0;
        _currentStatMultiplier = 1f;

        //플레이어 생성 및 죽었을 때 이벤트 구독
        _player = new Player(_playerMaxHealth, _turnStartCost);
        _player.OnPlayerDied += HandlePlayerDeath;

        //BattleManager 초기화 및 전투 종료 이벤트 구독
        BattleManager.Instance.Initialize(_player);
        BattleManager.Instance.OnBattleEnd += HandleBattleEnd;

        //시작 덱 설정
        List<CardData> startingCards = new List<CardData>(_startingDeck);
        BattleManager.Instance.DeckManager.InitializeDeck(startingCards);

        //대기 상태로 시작
        SetState(GameState.Idle);
    }

    public void StartNewRun()
    {
        Debug.Log("[GameManager] StartNewRun called");

        //새 게임 시작

        //1회차 0스테이지, 배율 1배
        _currentLoop = 1;
        _currentStage = 0;
        _currentStatMultiplier = 1f;

        //플레이어 완전히 리셋
        _player.FullReset(_playerMaxHealth, _turnStartCost);

        //덱 초기화
        List<CardData> startingCards = new List<CardData>(_startingDeck);
        BattleManager.Instance.DeckManager.InitializeDeck(startingCards);

        //UI에 알림
        OnLoopChanged?.Invoke(_currentLoop);
        OnStageChanged?.Invoke(_currentStage);

        Debug.Log("[GameManager] Calling StartNextStage");
        //첫 스테이지 시작
        StartNextStage();
    }

    public void StartNextStage()
    {
        Debug.Log("[GameManager] StartNextStage");
        _currentStage++;
        OnStageChanged?.Invoke(_currentStage);

        //일반 스테이지를 5번 이상 클리어했으면 보스전
        bool isBoss = _currentStage > _stagesPerChapter;

        Debug.Log($"[GameManager] SetState(Battle), isBoss={isBoss}");
        SetState(GameState.Battle);
        BattleManager.Instance.StartBattle(isBoss, _currentStatMultiplier);
    }

    private void HandleBattleEnd(bool victory)
    {
        if (victory)
        {
            if (BattleManager.Instance.IsBossBattle)
            {
                //보스 클리어, 전직 체크, 보상 선택 후 다음 루프
                HandleBossClear();
            }
            else
            {
                //일반 전투 승리, 보상 선택
                SetState(GameState.Reward);
            }
        }
        else
        {
            //패배
            HandlePlayerDeath();
        }
    }

    private void HandleBossClear()
    {
        //전직 체크
        JobType newJob = BattleManager.Instance.DeckManager.CheckJobPromotion();
        if (newJob != JobType.None)
        {
            _player.SetJob(newJob);
        }

        //다음 루프 시작
        _currentLoop++;
        _currentStage = 0;
        _currentStatMultiplier *= _statMultiplierPerLoop;

        OnLoopChanged?.Invoke(_currentLoop);
        OnStageChanged?.Invoke(_currentStage);

        //보상 선택으로 이동
        SetState(GameState.Reward);
    }

    private void HandlePlayerDeath()
    {
        SetState(GameState.GameOver);
        OnGameOver?.Invoke();
    }

    public void SelectRewardAddCard()
    {
        SetState(GameState.CardSelect);
    }

    public void SelectRewardDeleteCard()
    {
        SetState(GameState.CardDelete);
    }

    public void AddCardToDeck(CardData card)
    {
        BattleManager.Instance.DeckManager.AddCardToDeck(card);
        StartNextStage();
    }

    public void SkipCardSelection()
    {
        StartNextStage();
    }

    public void DeleteCardFromDeck(CardData card)
    {
        BattleManager.Instance.DeckManager.RemoveCardFromDeck(card);
        StartNextStage();
    }

    public void CancelCardDelete()
    {
        SetState(GameState.Reward);
    }


    public List<CardData> GetRandomCardChoices(int count = 3)
    {
        //카드 선택지용 랜덤 카드 3장 반환

        List<CardData> choices = new List<CardData>();
        List<CardData> availableCards = new List<CardData>(_allCards);

        for (int i = 0; i < count && availableCards.Count > 0; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, availableCards.Count);
            choices.Add(availableCards[randomIndex]);
            availableCards.RemoveAt(randomIndex);
        }

        return choices;
    }

    private void SetState(GameState state)
    {
        _currentState = state;
        OnStateChanged?.Invoke(_currentState);
    }

    private void OnDestroy()
    {
        if (_player != null)
        {
            _player.OnPlayerDied -= HandlePlayerDeath;
        }

        if (BattleManager.Instance != null)
        {
            BattleManager.Instance.OnBattleEnd -= HandleBattleEnd;
        }
    }
}
