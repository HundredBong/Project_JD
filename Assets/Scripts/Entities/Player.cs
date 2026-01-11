using System;
using UnityEngine;

public class Player
{
    private int _maxHealth;
    private int _currentHealth;
    private int _currentCost;
    private int _turnStartCost;
    private int _damageTakenThisTurn;
    private JobType _currentJob;
    private BuffSystem _buffSystem;

    public int MaxHealth => _maxHealth;
    public int CurrentHealth => _currentHealth;
    public int CurrentCost => _currentCost;
    public int TurnStartCost => _turnStartCost;
    public int DamageTakenThisTurn => _damageTakenThisTurn;
    public JobType CurrentJob => _currentJob;
    public BuffSystem BuffSystem => _buffSystem;

    public event Action<int, int> OnHealthChanged;      //currentHealth, maxHealth
    public event Action<int> OnCostChanged;
    public event Action<JobType> OnJobChanged;
    public event Action OnPlayerDied;

    public Player(int maxHealth, int turnStartCost)
    {
        _maxHealth = maxHealth;
        _currentHealth = maxHealth;
        _turnStartCost = turnStartCost;
        _currentCost = 0;
        _currentJob = JobType.None;
        _buffSystem = new BuffSystem();
        _damageTakenThisTurn = 0;
    }

    public void TakeDamage(int damage)
    {
        //방어력으로 피해 감소
        int actualDamage = _buffSystem.ApplyDefense(damage);

        if (actualDamage > 0)
        {
            //현재 체력 감소
            _currentHealth -= actualDamage;
            //이번 턴에 받은 총 피해량 증가
            _damageTakenThisTurn += actualDamage;
            //체력 변경 이벤트 호출
            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);

            //대미지를 받았을 때 체력이 0 이하가 되면 사망 처리
            if (_currentHealth <= 0)
            {
                _currentHealth = 0;
                OnPlayerDied?.Invoke();
            }
        }
    }

    public void Heal(int amount)
    {
        //오버힐 방지
        _currentHealth = Mathf.Min(_currentHealth + amount, _maxHealth);
        OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
    }

    public void AddCost(int amount)
    {
        _currentCost += amount;
        OnCostChanged?.Invoke(_currentCost);
    }

    public bool SpendCost(int amount)
    {
        if (_currentCost >= amount)
        {
            _currentCost -= amount;
            OnCostChanged?.Invoke(_currentCost);
            return true;
        }
        return false;
    }

    public void ResetCostForTurn()
    {
        _currentCost = _turnStartCost;
        OnCostChanged?.Invoke(_currentCost);
    }

    public void OnTurnStart()
    {
        ResetCostForTurn();
        _damageTakenThisTurn = 0;
    }

    public void OnTurnEnd()
    {
        //법사 직업 효과: 턴 종료시 주문 3배
        if (_currentJob == JobType.Mage)
        {
            _buffSystem.MultiplyBuff(BuffType.Spell, 3);
        }

        _buffSystem.OnTurnEnd();
    }

    public void SetJob(JobType job)
    {
        _currentJob = job;
        OnJobChanged?.Invoke(_currentJob);
    }

    //직업 효과 적용: 전사는 콤보 획득량 2배
    public int ApplyJobBonus(BuffType buffType, int amount)
    {
        if (_currentJob == JobType.Warrior)
        {
            if (buffType == BuffType.AttackCombo || buffType == BuffType.DefenseCombo)
            {
                return amount * 2;
            }
        }
        return amount;
    }

    public void ResetForNewBattle()
    {
        //새로운 전투용 버프 초기화

        _buffSystem.ClearAll();
        _damageTakenThisTurn = 0;
    }

    public void FullReset(int maxHealth, int turnStartCost)
    {
        //플레이어 완전 초기화

        _maxHealth = maxHealth;
        _currentHealth = maxHealth;
        _turnStartCost = turnStartCost;
        _currentCost = 0;
        _currentJob = JobType.None;
        _buffSystem.ClearAll();
        _damageTakenThisTurn = 0;

        OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
        OnCostChanged?.Invoke(_currentCost);
        OnJobChanged?.Invoke(_currentJob);
    }
}
