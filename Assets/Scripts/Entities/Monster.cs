using System;
using UnityEngine;

public class Monster
{
    private MonsterData _data;
    private int _maxHealth;
    private int _currentHealth;
    private int _defense;
    private int _currentPatternIndex;
    private float _statMultiplier;

    public MonsterData Data => _data;
    public string Name => _data.monsterName;
    public int MaxHealth => _maxHealth;
    public int CurrentHealth => _currentHealth;
    public int Defense => _defense;
    public bool IsBoss => _data.isBoss;
    public bool IsDead => _currentHealth <= 0;

    public event Action<int, int> OnHealthChanged;  //currentHealth, maxHealth
    public event Action<int> OnDefenseChanged;
    public event Action OnMonsterDied;

    public Monster(MonsterData data, float statMultiplier = 1f)
    {
        _data = data;
        _statMultiplier = statMultiplier;
        _maxHealth = Mathf.FloorToInt(data.baseHealth * statMultiplier);
        _currentHealth = _maxHealth;
        _defense = 0;
        _currentPatternIndex = 0;
    }

    public MonsterPattern GetCurrentPattern()
    {
        if (_data.patterns == null || _data.patterns.Length == 0)
        {
            return null;
        }

        return _data.patterns[_currentPatternIndex];
    }

    public MonsterPattern GetNextPattern()
    {
        //현재 패턴 반환 후 다음 인덱스로 이동
        MonsterPattern pattern = GetCurrentPattern();
        AdvancePattern();
        return pattern;
    }

    private void AdvancePattern()
    {
        _currentPatternIndex++;

        //패턴 끝에 도달하면 반복 구간으로 돌아감
        if (_currentPatternIndex > _data.loopEndIndex)
        {
            _currentPatternIndex = _data.loopStartIndex;
        }
    }

    public int GetScaledValue(int baseValue)
    {
        return Mathf.FloorToInt(baseValue * _statMultiplier);
    }

    public void TakeDamage(int damage)
    {
        //방어력으로 피해 감소
        int actualDamage = damage;
        if (_defense > 0)
        {
            int absorbed = Mathf.Min(_defense, damage);
            _defense -= absorbed;
            actualDamage = damage - absorbed;
            OnDefenseChanged?.Invoke(_defense);
        }

        if (actualDamage > 0)
        {
            _currentHealth -= actualDamage;
            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);

            if (_currentHealth <= 0)
            {
                _currentHealth = 0;
                OnMonsterDied?.Invoke();
            }
        }
    }

    public void AddDefense(int amount)
    {
        _defense += Mathf.FloorToInt(amount * _statMultiplier);
        OnDefenseChanged?.Invoke(_defense);
    }

    public void OnTurnStart()
    {
        //몬스터 방어력은 턴 시작시 초기화 (이전 턴에 쌓은 방어력이 플레이어 턴 동안 유지됨)
        _defense = 0;
        OnDefenseChanged?.Invoke(_defense);
    }

    public void OnTurnEnd()
    {
        //턴 종료시 처리할 것이 있으면 여기에 추가
    }

    public void ExecuteAction(Player target)
    {
        MonsterPattern pattern = GetNextPattern();
        if (pattern == null)
        {
            return;
        }

        int scaledValue = GetScaledValue(pattern.value);

        switch (pattern.actionType)
        {
            case MonsterActionType.Attack:
                target.TakeDamage(scaledValue);
                break;

            case MonsterActionType.Defense:
                AddDefense(pattern.value); //AddDefense 내부에서 스케일링 처리
                break;
        }
    }
}
