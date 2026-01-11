using System;
using System.Collections.Generic;
using UnityEngine;

public class BuffSystem
{
    private Dictionary<BuffType, int> _buffs = new Dictionary<BuffType, int>();

    public event Action<BuffType, int> OnBuffChanged;

    public BuffSystem()
    {
        //모든 버프 타입 초기화
        foreach (BuffType buffType in Enum.GetValues(typeof(BuffType)))
        {
            _buffs[buffType] = 0;
        }
    }

    public int GetBuff(BuffType buffType)
    {
        if (_buffs.TryGetValue(buffType, out int value))
        {
            return value;
        }

        return 0;
    }

    public void AddBuff(BuffType buffType, int amount)
    {
        //버프 추가

        if (_buffs.ContainsKey(buffType) == false)
        {
            _buffs[buffType] = 0;
        }

        _buffs[buffType] += amount;
        OnBuffChanged?.Invoke(buffType, _buffs[buffType]);

        //공격 콤보 획득시 방어 콤보 소멸
        if (buffType == BuffType.AttackCombo && amount > 0)
        {
            ClearBuff(BuffType.DefenseCombo);
        }
        //방어 콤보 획득시 공격 콤보 소멸
        else if (buffType == BuffType.DefenseCombo && amount > 0)
        {
            ClearBuff(BuffType.AttackCombo);
        }
    }

    public void SetBuff(BuffType buffType, int value)
    {
        //버프 초기화

        _buffs[buffType] = value;
        OnBuffChanged?.Invoke(buffType, _buffs[buffType]);
    }

    public void MultiplyBuff(BuffType buffType, int multiplier)
    {
        //버프 배율 적용

        if (_buffs.ContainsKey(buffType))
        {
            _buffs[buffType] *= multiplier;
            OnBuffChanged?.Invoke(buffType, _buffs[buffType]);
        }
    }

    public int ConsumeBuff(BuffType buffType)
    {
        //버프 소모 및 값 반환

        int value = GetBuff(buffType);
        ClearBuff(buffType);
        return value;
    }

    public void ClearBuff(BuffType buffType)
    {
        //버프 제거 

        if (_buffs.ContainsKey(buffType) && _buffs[buffType] != 0)
        {
            _buffs[buffType] = 0;
            OnBuffChanged?.Invoke(buffType, 0);
        }
    }

    public void OnTurnEnd()
    {
        //턴 종료시 소멸되는 버프들
        ClearBuff(BuffType.AttackCombo);
        ClearBuff(BuffType.DefenseCombo);
        ClearBuff(BuffType.Defense);
        //주문은 턴 종료시 소멸되지 않음 (특정 카드 사용시에만 소멸)
    }

    public void ClearAll()
    {
        //새 전투시 또는 게임 초기화시 모든 버프 제거

        foreach (BuffType buffType in Enum.GetValues(typeof(BuffType)))
        {
            ClearBuff(buffType);
        }
    }

    public int ApplyDefense(int damage)
    {
        //방어력으로 피해 감소, 남은 피해량 반환

        int defense = GetBuff(BuffType.Defense);
        if (defense > 0)
        {
            int absorbed = Mathf.Min(defense, damage);
            AddBuff(BuffType.Defense, -absorbed);
            return damage - absorbed;
        }
        return damage;
    }
}
