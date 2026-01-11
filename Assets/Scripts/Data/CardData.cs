using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCard", menuName = "Game/Card Data")]
public class CardData : ScriptableObject
{
    [Header("기본 정보")]
    public string cardName;
    public CardType cardType;
    public int cost;

    [Header("효과")]
    [TextArea(2, 4)]
    public string effectDescription;
    public CardEffect[] effects;
}

[Serializable]
public class CardEffect
{
    public CardEffectType effectType;
    public int value;           //기본 수치
    public int multiplier;      //배율 (콤보 기반 효과용)
    public int maxValue;        //최대 수치 (제한이 있는 경우)
    public bool consumesBuff;   //버프 소모 여부

    [Header("조건부 효과")]
    public bool hasCondition;           //조건 존재 여부
    public BuffType conditionBuffType;  //조건 버프 타입
    public int conditionThreshold;      //조건 임계값
    public CardEffectType conditionalEffectType; //조건 충족시 효과 타입
    public int conditionalValue;        //조건 충족시 효과 값
}
