using System;
using System.Collections.Generic;
using UnityEngine;

public class DeckManager
{
    private List<CardData> _fullDeck;       //전체 카드 목록, 보상 추가/삭제용
    private List<CardData> _drawPile;       //드로우할 카드들
    private List<CardData> _hand;           //손에 든 카드들
    private List<CardData> _discardPile;    //사용하거나 버린 카드 더미

    private const int MAX_HAND_SIZE = 10;
    private const int DRAW_PER_TURN = 5;

    public IReadOnlyList<CardData> FullDeck => _fullDeck;
    public IReadOnlyList<CardData> DrawPile => _drawPile;
    public IReadOnlyList<CardData> Hand => _hand;
    public IReadOnlyList<CardData> DiscardPile => _discardPile;

    public event Action<List<CardData>> OnHandChanged;
    public event Action OnDeckShuffled;

    public DeckManager()
    {
        _fullDeck = new List<CardData>();
        _drawPile = new List<CardData>();
        _hand = new List<CardData>();
        _discardPile = new List<CardData>();
    }

    public void InitializeDeck(List<CardData> startingCards)
    {
        _fullDeck.Clear();
        _fullDeck.AddRange(startingCards);

        _drawPile.Clear();
        _drawPile.AddRange(startingCards);

        _hand.Clear();
        _discardPile.Clear();

        ShuffleDeck();
    }

    public void ShuffleDeck()
    {
        //Fisher-Yates 셔플
        for (int i = _drawPile.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            CardData temp = _drawPile[i];
            _drawPile[i] = _drawPile[j];
            _drawPile[j] = temp;
        }

        OnDeckShuffled?.Invoke();
    }

    public void DrawCards(int count)
    {
        for (int i = 0; i < count; i++)
        {
            if (_hand.Count >= MAX_HAND_SIZE)
            {
                break;
            }

            if (_drawPile.Count == 0)
            {
                ReshuffleDiscardPile();
                if (_drawPile.Count == 0)
                {
                    break;
                }
            }

            CardData card = _drawPile[0];
            _drawPile.RemoveAt(0);
            _hand.Add(card);
        }

        OnHandChanged?.Invoke(_hand);
    }

    public void DrawCardsForTurnStart()
    {
        DrawCards(DRAW_PER_TURN);
    }

    private void ReshuffleDiscardPile()
    {
        if (_discardPile.Count == 0)
        {
            return;
        }

        _drawPile.AddRange(_discardPile);
        _discardPile.Clear();
        ShuffleDeck();
    }

    public bool PlayCard(CardData card)
    {
        //카드 사용

        //손에 없는 카드면 실패
        if (_hand.Contains(card) == false)
        {
            return false;
        }

        //손에서 제거하고 버림 더미로 이동
        _hand.Remove(card);
        _discardPile.Add(card);
        OnHandChanged?.Invoke(_hand);
        return true;
    }

    public void DiscardHand()
    {
        //손에 있는 카드 모두 버림 더미로 이동

        _discardPile.AddRange(_hand);
        _hand.Clear();
        OnHandChanged?.Invoke(_hand);
    }

    public void AddCardToDeck(CardData card)
    {
        //덱에 카드 추가

        _fullDeck.Add(card);
        _drawPile.Add(card);
    }

    public bool RemoveCardFromDeck(CardData card)
    {
        bool removedFromFull = _fullDeck.Remove(card);

        //현재 덱에서도 제거
        _drawPile.Remove(card);
        _hand.Remove(card);
        _discardPile.Remove(card);

        OnHandChanged?.Invoke(_hand);
        return removedFromFull;
    }

    public void ResetForNewBattle()
    {
        _drawPile.Clear();
        _drawPile.AddRange(_fullDeck);
        _hand.Clear();
        _discardPile.Clear();
        ShuffleDeck();
    }

    public Dictionary<CardType, float> GetCardTypeRatios()
    {
        //덱 내 카드 타입별 비율 계산  

        Dictionary<CardType, int> counts = new Dictionary<CardType, int>();
        foreach (CardType type in Enum.GetValues(typeof(CardType)))
        {
            counts[type] = 0;
        }

        foreach (CardData card in _fullDeck)
        {
            counts[card.cardType]++;
        }

        Dictionary<CardType, float> ratios = new Dictionary<CardType, float>();
        int totalCards = _fullDeck.Count;

        foreach (CardType type in Enum.GetValues(typeof(CardType)))
        {
            ratios[type] = totalCards > 0 ? (float)counts[type] / totalCards : 0f;
        }

        return ratios;
    }

    public Dictionary<CardType, float> GetJobCardRatios()
    {
        //직업별 비율 계산 (중립 제외)

        int warriorCount = 0;
        int mageCount = 0;

        foreach (CardData card in _fullDeck)
        {
            if (card.cardType == CardType.Warrior)
            {
                warriorCount++;
            }
            else if (card.cardType == CardType.Mage)
            {
                mageCount++;
            }
        }

        int totalJobCards = warriorCount + mageCount;

        Dictionary<CardType, float> ratios = new Dictionary<CardType, float>();
        ratios[CardType.Warrior] = totalJobCards > 0 ? (float)warriorCount / totalJobCards : 0f;
        ratios[CardType.Mage] = totalJobCards > 0 ? (float)mageCount / totalJobCards : 0f;

        return ratios;
    }

    public JobType CheckJobPromotion()
    {
        //전직 조건 체크 (특정 직업 카드가 50% 초과)

        Dictionary<CardType, float> ratios = GetCardTypeRatios();

        if (ratios[CardType.Warrior] > 0.5f)
        {
            return JobType.Warrior;
        }
        else if (ratios[CardType.Mage] > 0.5f)
        {
            return JobType.Mage;
        }

        return JobType.None;
    }
}
