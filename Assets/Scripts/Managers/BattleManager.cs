using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance { get; private set; }

    [Header("설정")]
    [SerializeField] private MonsterData[] _normalMonsters;
    [SerializeField] private MonsterData _bossMonster;

    private Player _player;                                   //GameManager에서 설정된 플레이어
    private Monster _currentMonster;                          //현재 전투중인 몬스터
    private DeckManager _deckManager;
    private CardEffectExecutor _cardEffectExecutor;           //카드 효과 실행기
    private BattlePhase _currentPhase;                        //현재 전투 페이즈
    private bool _isBossBattle;

    public Player Player => _player;
    public Monster CurrentMonster => _currentMonster;
    public DeckManager DeckManager => _deckManager;
    public BattlePhase CurrentPhase => _currentPhase;
    public bool IsBossBattle => _isBossBattle;

    public event Action<BattlePhase> OnPhaseChanged;
    public event Action<Monster> OnMonsterSpawned;
    public event Action<bool> OnBattleEnd; //true = 승리, false = 패배

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        _deckManager = new DeckManager();
        _cardEffectExecutor = new CardEffectExecutor();
    }

    public void Initialize(Player player)
    {
        //GameManager가 플레이어를 만들지 않았을 수 있으므로 별도 초기화 메서드로 설정
        _player = player;
    }

    public void StartBattle(bool isBoss, float statMultiplier)
    {
        _isBossBattle = isBoss;

        //몬스터 생성
        MonsterData monsterData;
        if (isBoss)
        {
            //보스 고정
            monsterData = _bossMonster;
        }
        else
        {
            //랜덤하게 선택한 일반 몬스터
            int randomIndex = UnityEngine.Random.Range(0, _normalMonsters.Length);
            monsterData = _normalMonsters[randomIndex];
        }

        //몬스터 인스턴스 생성 및 이벤트 구독
        _currentMonster = new Monster(monsterData, statMultiplier);
        _currentMonster.OnMonsterDied += OnMonsterDied;

        //카드 효과 실행기 초기화
        _cardEffectExecutor.SetContext(_player, _currentMonster, _deckManager);

        OnMonsterSpawned?.Invoke(_currentMonster);

        //전투용 덱 초기화
        _deckManager.ResetForNewBattle();
        _player.ResetForNewBattle();

        //첫 턴 시작
        StartTurn();
    }

    private void StartTurn()
    {
        SetPhase(BattlePhase.TurnStart);

        //플레이어 턴 시작 처리
        _player.OnTurnStart();

        //카드 드로우
        _deckManager.DrawCardsForTurnStart();

        //이후 플레이어 턴으로 전환
        SetPhase(BattlePhase.PlayerTurn);
    }

    public bool TryPlayCard(CardData card)
    {
        //플레이어 턴이 아닐 경우 실행 불가
        if (_currentPhase != BattlePhase.PlayerTurn)
        {
            return false;
        }
        //코스트 부족하면 사용 불가
        if (!_cardEffectExecutor.CanPlayCard(card))
        {
            return false;
        }

        //카드 효과 실행
        _cardEffectExecutor.ExecuteCard(card);

        //카드를 핸드에서 버린 카드 더미로 이동
        _deckManager.PlayCard(card);

        //몬스터가 죽었는지 체크
        if (_currentMonster != null && _currentMonster.IsDead)
        {
            EndBattle(true);
        }

        return true;
    }

    public void EndTurn()
    {
        if (_currentPhase != BattlePhase.PlayerTurn)
        {
            return;
        }

        SetPhase(BattlePhase.TurnEnd);

        //핸드에 남은 카드 버리기
        _deckManager.DiscardHand();

        //몬스터 턴 (몬스터 행동 후 플레이어 턴 종료 처리)
        MonsterTurn().Forget();
    }

    private async UniTaskVoid MonsterTurn()
    {
        SetPhase(BattlePhase.EnemyTurn);

        await UniTask.Delay(TimeSpan.FromSeconds(0.5f));

        //몬스터 행동
        if (_currentMonster != null && !_currentMonster.IsDead)
        {
            //몬스터 턴 시작 (이전 턴 방어력 초기화)
            _currentMonster.OnTurnStart();
            _currentMonster.ExecuteAction(_player);
            _currentMonster.OnTurnEnd();
        }

        //몬스터 행동 후 플레이어 턴 종료 처리 (방어력 초기화 등)
        _player.OnTurnEnd();

        await UniTask.Delay(TimeSpan.FromSeconds(0.5f));

        //플레이어가 죽었는지 체크
        if (_player.CurrentHealth <= 0)
        {
            EndBattle(false);
            return;
        }

        //다음 플레이어 턴 시작
        StartTurn();
    }

    private void OnMonsterDied()
    {
        if (_currentPhase != BattlePhase.BattleEnd)
        {
            EndBattle(true);
        }
    }

    private void EndBattle(bool victory)
    {
        SetPhase(BattlePhase.BattleEnd);

        if (_currentMonster != null)
        {
            //몬스터 죽음 이벤트 구독 해제
            _currentMonster.OnMonsterDied -= OnMonsterDied;
        }

        //GameManager 쪽에서 처리할 수 있도록 이벤트 발생, GameManager.HandleBattleEnd 실행됨
        OnBattleEnd?.Invoke(victory);
    }

    private void SetPhase(BattlePhase phase)
    {
        _currentPhase = phase;
        OnPhaseChanged?.Invoke(_currentPhase);
    }

    public MonsterPattern GetMonsterNextAction()
    {
        if (_currentMonster == null)
        {
            return null;
        }
        return _currentMonster.GetCurrentPattern();
    }
}
