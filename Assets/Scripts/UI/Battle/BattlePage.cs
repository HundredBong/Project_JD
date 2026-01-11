using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattlePage : UIPage
{
    [Header("플레이어 UI")]
    [SerializeField] private TextMeshProUGUI _playerHealthText;
    [SerializeField] private TextMeshProUGUI _playerCostText;
    [SerializeField] private TextMeshProUGUI _playerJobText;
    [SerializeField] private TextMeshProUGUI _playerDefenseText;
    [SerializeField] private TextMeshProUGUI _playerBuffText;
    [SerializeField] private Transform _playerBuffContainer;

    [Header("몬스터 UI")]
    [SerializeField] private TextMeshProUGUI _monsterNameText;
    [SerializeField] private TextMeshProUGUI _monsterHealthText;
    [SerializeField] private TextMeshProUGUI _monsterDefenseText;
    [SerializeField] private TextMeshProUGUI _monsterNextActionText;
    [SerializeField] private Transform _monsterBuffContainer;

    [Header("핸드 UI")]
    [SerializeField] private Transform _handContainer;
    [SerializeField] private GameObject _cardUIPrefab;

    [Header("버튼")]
    [SerializeField] private Button _endTurnButton;
    [SerializeField] private Button _viewDeckButton;

    private Player _player;
    private Monster _monster;
    private List<CardUI> _handCards = new List<CardUI>();

    protected override void Awake()
    {
        base.Awake();

        if (_endTurnButton != null)
        {
            _endTurnButton.onClick.AddListener(OnEndTurnClicked);
        }

        if (_viewDeckButton != null)
        {
            _viewDeckButton.onClick.AddListener(OnViewDeckClicked);
        }
    }

    private void OnEnable()
    {
        if (BattleManager.Instance != null)
        {
            BattleManager.Instance.OnPhaseChanged += OnPhaseChanged;
            BattleManager.Instance.OnMonsterSpawned += OnMonsterSpawned;
            BattleManager.Instance.DeckManager.OnHandChanged += OnHandChanged;
        }

        if (GameManager.Instance != null && GameManager.Instance.Player != null)
        {
            _player = GameManager.Instance.Player;
            _player.OnHealthChanged += OnPlayerHealthChanged;
            _player.OnCostChanged += OnPlayerCostChanged;
            _player.OnJobChanged += OnPlayerJobChanged;
            _player.BuffSystem.OnBuffChanged += OnPlayerBuffChanged;

            UpdatePlayerUI();
        }
    }

    private void OnDisable()
    {
        if (BattleManager.Instance != null)
        {
            BattleManager.Instance.OnPhaseChanged -= OnPhaseChanged;
            BattleManager.Instance.OnMonsterSpawned -= OnMonsterSpawned;
            if (BattleManager.Instance.DeckManager != null)
            {
                BattleManager.Instance.DeckManager.OnHandChanged -= OnHandChanged;
            }
        }

        if (_player != null)
        {
            _player.OnHealthChanged -= OnPlayerHealthChanged;
            _player.OnCostChanged -= OnPlayerCostChanged;
            _player.OnJobChanged -= OnPlayerJobChanged;
            _player.BuffSystem.OnBuffChanged -= OnPlayerBuffChanged;
        }

        UnsubscribeMonster();
    }

    private void OnMonsterSpawned(Monster monster)
    {
        UnsubscribeMonster();

        _monster = monster;
        _monster.OnHealthChanged += OnMonsterHealthChanged;
        _monster.OnDefenseChanged += OnMonsterDefenseChanged;

        UpdateMonsterUI();
    }

    private void UnsubscribeMonster()
    {
        if (_monster != null)
        {
            _monster.OnHealthChanged -= OnMonsterHealthChanged;
            _monster.OnDefenseChanged -= OnMonsterDefenseChanged;
        }
    }

    private void OnPhaseChanged(BattlePhase phase)
    {
        bool isPlayerTurn = phase == BattlePhase.PlayerTurn;
        if (_endTurnButton != null)
        {
            _endTurnButton.interactable = isPlayerTurn;
        }

        //핸드 카드 상호작용 가능 여부
        foreach (CardUI card in _handCards)
        {
            card.SetInteractable(isPlayerTurn);
        }

        UpdateMonsterNextAction();
    }

    private void OnHandChanged(List<CardData> hand)
    {
        ClearHandUI();

        foreach (CardData cardData in hand)
        {
            GameObject cardObj = Instantiate(_cardUIPrefab, _handContainer);
            CardUI cardUI = cardObj.GetComponent<CardUI>();

            if (cardUI != null)
            {
                cardUI.Setup(cardData);
                cardUI.OnCardClicked += OnCardClicked;
                _handCards.Add(cardUI);
            }
        }
    }

    private void ClearHandUI()
    {
        foreach (CardUI card in _handCards)
        {
            card.OnCardClicked -= OnCardClicked;
            Destroy(card.gameObject);
        }
        _handCards.Clear();
    }

    private void OnCardClicked(CardUI cardUI)
    {
        if (BattleManager.Instance.CurrentPhase != BattlePhase.PlayerTurn)
        {
            return;
        }

        bool success = BattleManager.Instance.TryPlayCard(cardUI.CardData);

        if (success)
        {
            //카드 사용 성공시 UI 업데이트 (OnHandChanged에서 처리됨)
            UpdateMonsterNextAction();
        }
    }

    private void OnEndTurnClicked()
    {
        BattleManager.Instance.EndTurn();
    }

    private void OnViewDeckClicked()
    {
        UIManager.Instance.PopupOpen<DeckViewPopup>();
    }

    private void OnPlayerHealthChanged(int current, int max)
    {
        if (_playerHealthText != null)
        {
            _playerHealthText.text = $"{current} / {max}";
        }
    }

    private void OnPlayerCostChanged(int cost)
    {
        if (_playerCostText != null)
        {
            int maxCost = _player != null ? _player.TurnStartCost : 3;
            _playerCostText.text = $"{cost} / {maxCost}";
        }
    }

    private void OnPlayerJobChanged(JobType job)
    {
        if (_playerJobText != null)
        {
            _playerJobText.text = GetJobName(job);
        }
    }

    private void OnPlayerBuffChanged(BuffType buffType, int value)
    {
        UpdatePlayerBuffUI();
    }

    private void OnMonsterHealthChanged(int current, int max)
    {
        if (_monsterHealthText != null)
        {
            _monsterHealthText.text = $"{current} / {max}";
        }
    }

    private void OnMonsterDefenseChanged(int defense)
    {
        if (_monsterDefenseText != null)
        {
            _monsterDefenseText.gameObject.SetActive(defense > 0);
            _monsterDefenseText.text = $"방어 {defense}";
        }
    }

    private void UpdatePlayerUI()
    {
        if (_player == null)
        {
            return;
        }

        OnPlayerHealthChanged(_player.CurrentHealth, _player.MaxHealth);
        OnPlayerCostChanged(_player.CurrentCost);
        OnPlayerJobChanged(_player.CurrentJob);
        UpdatePlayerBuffUI();
    }

    private void UpdatePlayerBuffUI()
    {
        if (_player == null)
        {
            return;
        }

        //방어력 표시
        int defense = _player.BuffSystem.GetBuff(BuffType.Defense);
        if (_playerDefenseText != null)
        {
            _playerDefenseText.gameObject.SetActive(defense > 0);
            _playerDefenseText.text = $"방어 {defense}";
        }

        //버프 텍스트 표시 (공격콤보, 방어콤보, 주문)
        if (_playerBuffText != null)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            int attackCombo = _player.BuffSystem.GetBuff(BuffType.AttackCombo);
            int defenseCombo = _player.BuffSystem.GetBuff(BuffType.DefenseCombo);
            int spell = _player.BuffSystem.GetBuff(BuffType.Spell);

            if (attackCombo > 0)
            {
                sb.Append($"공콤 {attackCombo} ");
            }
            if (defenseCombo > 0)
            {
                sb.Append($"방콤 {defenseCombo} ");
            }
            if (spell > 0)
            {
                sb.Append($"주문 {spell}");
            }

            _playerBuffText.text = sb.ToString();
            _playerBuffText.gameObject.SetActive(sb.Length > 0);
        }
    }

    private void UpdateMonsterUI()
    {
        if (_monster == null)
        {
            return;
        }

        if (_monsterNameText != null)
        {
            _monsterNameText.text = _monster.Name;
        }

        OnMonsterHealthChanged(_monster.CurrentHealth, _monster.MaxHealth);
        OnMonsterDefenseChanged(_monster.Defense);
        UpdateMonsterNextAction();
    }

    private void UpdateMonsterNextAction()
    {
        if (_monsterNextActionText == null || _monster == null)
        {
            return;
        }

        MonsterPattern pattern = BattleManager.Instance.GetMonsterNextAction();
        if (pattern != null)
        {
            string actionName = pattern.actionType == MonsterActionType.Attack ? "공격" : "방어";
            int scaledValue = _monster.GetScaledValue(pattern.value);
            _monsterNextActionText.text = $"{actionName} {scaledValue}";
        }
    }

    private string GetJobName(JobType job)
    {
        switch (job)
        {
            case JobType.Warrior:
                return "전사";
            case JobType.Mage:
                return "법사";
            default:
                return "초보자";
        }
    }
}
