using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardSelectPage : UIPage
{
    [Header("UI 요소")]
    [SerializeField] private Transform _cardContainer;
    [SerializeField] private GameObject _selectableCardPrefab;
    [SerializeField] private Button _skipButton;

    private List<SelectableCardUI> _cardChoices = new List<SelectableCardUI>();

    protected override void Awake()
    {
        base.Awake();

        if (_skipButton != null)
        {
            _skipButton.onClick.AddListener(OnSkipClicked);
        }
    }

    private void OnEnable()
    {
        RefreshCardChoices();
    }

    private void RefreshCardChoices()
    {
        ClearCardChoices();

        if (GameManager.Instance == null)
        {
            return;
        }

        List<CardData> choices = GameManager.Instance.GetRandomCardChoices(3);

        foreach (CardData card in choices)
        {
            GameObject cardObj = Instantiate(_selectableCardPrefab, _cardContainer);
            SelectableCardUI selectableCard = cardObj.GetComponent<SelectableCardUI>();

            if (selectableCard != null)
            {
                selectableCard.Setup(card);
                selectableCard.OnCardSelected += OnCardSelected;
                _cardChoices.Add(selectableCard);
            }
        }
    }

    private void ClearCardChoices()
    {
        foreach (SelectableCardUI card in _cardChoices)
        {
            card.OnCardSelected -= OnCardSelected;
            Destroy(card.gameObject);
        }
        _cardChoices.Clear();
    }

    private void OnCardSelected(CardData card)
    {
        GameManager.Instance.AddCardToDeck(card);
        UIManager.Instance.PageOpen<BattlePage>();
    }

    private void OnSkipClicked()
    {
        GameManager.Instance.SkipCardSelection();
        UIManager.Instance.PageOpen<BattlePage>();
    }

    private void OnDisable()
    {
        ClearCardChoices();
    }
}
