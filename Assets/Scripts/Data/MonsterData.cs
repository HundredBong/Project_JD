using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewMonster", menuName = "Game/Monster Data")]
public class MonsterData : ScriptableObject
{
    [Header("기본 정보")]
    public string monsterName;
    public int baseHealth;
    public bool isBoss;

    [Header("패턴")]
    public MonsterPattern[] patterns;   //패턴 배열
    public int loopStartIndex;          //반복 시작 인덱스 (예: 5~6이면 4)
    public int loopEndIndex;            //반복 끝 인덱스 (예: 5~6이면 5)
}

[Serializable]
public class MonsterPattern
{
    public MonsterActionType actionType;
    public int value;
}

public enum MonsterActionType
{
    Attack,     //공격
    Defense     //방어
}
