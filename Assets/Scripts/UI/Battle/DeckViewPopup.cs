using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DeckViewPopup : UIPopup
{
    [Header("UI 요소")]
    [SerializeField] private TextMeshProUGUI _ratioText;         //카드 비율 텍스트
    [SerializeField] private Transform _cardContainer;           //카드 목록 컨테이너
    [SerializeField] private GameObject _cardDisplayPrefab;      //카드 프리팹
    [SerializeField] private ScrollRect _scrollRect;             //스크롤 뷰

    private List<GameObject> _displayedCards = new List<GameObject>();

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
            GameObject cardObj = Instantiate(_cardDisplayPrefab, _cardContainer);
            CardDisplayUI display = cardObj.GetComponent<CardDisplayUI>();

            if (display != null)
            {
                display.Setup(card);
            }

            _displayedCards.Add(cardObj);
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
        foreach (GameObject card in _displayedCards)
        {
            Destroy(card);
        }
        _displayedCards.Clear();
    }

    private void OnDisable()
    {
        ClearDisplayedCards();
    }
}
