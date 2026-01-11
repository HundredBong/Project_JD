using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardDeletePage : UIPage
{
    [Header("UI 요소")]
    [SerializeField] private TextMeshProUGUI _ratioText;
    [SerializeField] private Transform _cardContainer;
    [SerializeField] private GameObject _deletableCardPrefab;
    [SerializeField] private Button _backButton;
    [SerializeField] private ScrollRect _scrollRect;

    private List<DeletableCardUI> _displayedCards = new List<DeletableCardUI>();

    protected override void Awake()
    {
        base.Awake();

        if (_backButton != null)
        {
            _backButton.onClick.AddListener(OnBackClicked);
        }
    }

    private void OnEnable()
    {
        RefreshDeckView();
    }

    private void RefreshDeckView()
    {
        ClearDisplayedCards();

        if (BattleManager.Instance == null || BattleManager.Instance.DeckManager == null)
        {
            return;
        }

        DeckManager deckManager = BattleManager.Instance.DeckManager;

        //비율 표시
        UpdateRatioText(deckManager);

        //카드 목록 표시
        foreach (CardData card in deckManager.FullDeck)
        {
            GameObject cardObj = Instantiate(_deletableCardPrefab, _cardContainer);
            DeletableCardUI deletableCard = cardObj.GetComponent<DeletableCardUI>();

            if (deletableCard != null)
            {
                deletableCard.Setup(card);
                deletableCard.OnCardDeleted += OnCardDeleted;
                _displayedCards.Add(deletableCard);
            }
        }

        //스크롤 위치 초기화
        if (_scrollRect != null)
        {
            _scrollRect.verticalNormalizedPosition = 1f;
        }
    }

    private void UpdateRatioText(DeckManager deckManager)
    {
        if (_ratioText == null)
        {
            return;
        }

        Dictionary<CardType, float> ratios = deckManager.GetCardTypeRatios();

        int warriorPercent = Mathf.FloorToInt(ratios[CardType.Warrior] * 100);
        int magePercent = Mathf.FloorToInt(ratios[CardType.Mage] * 100);
        int neutralPercent = Mathf.FloorToInt(ratios[CardType.Neutral] * 100);

        _ratioText.text = $"전사: {warriorPercent}% 법사: {magePercent}% 중립: {neutralPercent}%";
    }

    private void ClearDisplayedCards()
    {
        foreach (DeletableCardUI card in _displayedCards)
        {
            card.OnCardDeleted -= OnCardDeleted;
            Destroy(card.gameObject);
        }
        _displayedCards.Clear();
    }

    private void OnCardDeleted(CardData card)
    {
        GameManager.Instance.DeleteCardFromDeck(card);
        UIManager.Instance.PageOpen<BattlePage>();
    }

    private void OnBackClicked()
    {
        GameManager.Instance.CancelCardDelete();
        UIManager.Instance.PageOpen<RewardPage>();
    }

    private void OnDisable()
    {
        ClearDisplayedCards();
    }
}
