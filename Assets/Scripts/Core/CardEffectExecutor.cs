using UnityEngine;

/// <summary>
/// 카드 효과 실행기 클래스
/// </summary>
public class CardEffectExecutor
{
    private Player _player;
    private Monster _monster;
    private DeckManager _deckManager;

    public void SetContext(Player player, Monster monster, DeckManager deckManager)
    {
        //참조 설정

        _player = player;
        _monster = monster;
        _deckManager = deckManager;
    }

    public bool CanPlayCard(CardData card)
    {
        //플레이어의 현재 코스트가 카드 코스트 이상인지 확인

        return _player.CurrentCost >= card.cost;
    }

    public void ExecuteCard(CardData card)
    {
        //카드 효과 실행

        //카드 실행 가능 여부 확인
        if (CanPlayCard(card) == false)
        {
            return;
        }

        //코스트 차감
        _player.SpendCost(card.cost);

        //각 효과 실행
        foreach (CardEffect effect in card.effects)
        {
            ExecuteEffect(effect);
        }
    }

    private void ExecuteEffect(CardEffect effect)
    {
        //조건부 효과 체크
        if (effect.hasCondition)
        {
            //조건 버프 수치 확인
            int buffValue = _player.BuffSystem.GetBuff(effect.conditionBuffType);
            if (buffValue >= effect.conditionThreshold)
            {
                //조건 충족시 추가 효과 실행
                ExecuteConditionalEffect(effect.conditionalEffectType, effect.conditionalValue);
            }
        }

        //기본 효과 실행
        switch (effect.effectType)
        {
            //대미지    
            case CardEffectType.Damage:
                DealDamage(effect.value);
                break;

            //방어력 획득
            case CardEffectType.GainDefense:
                GainDefense(effect.value);
                break;

            //카드 뽑기
            case CardEffectType.Draw:
                _deckManager.DrawCards(effect.value);
                break;

            //코스트 획득
            case CardEffectType.GainCost:
                _player.AddCost(effect.value);
                break;

            //공격 콤보 획득
            case CardEffectType.GainAttackCombo:
                GainBuff(BuffType.AttackCombo, effect.value);
                break;

            //방어 콤보 획득
            case CardEffectType.GainDefenseCombo:
                GainBuff(BuffType.DefenseCombo, effect.value);
                break;

            //주문 획득
            case CardEffectType.GainSpell:
                GainBuff(BuffType.Spell, effect.value);
                break;

            //콤보 전환
            case CardEffectType.ConvertCombo:
                ConvertCombo();
                break;

            //콤보 기반 대미지
            case CardEffectType.DamageByAttackCombo:
                DamageByCombo(BuffType.AttackCombo, effect.multiplier);
                break;

            //콤보 기반 방어
            case CardEffectType.DefenseByDefenseCombo:
                DefenseByCombo(BuffType.DefenseCombo, effect.multiplier);
                break;

            //받은 피해 기반 대미지
            case CardEffectType.DamageByDamageTaken:
                DamageByDamageTaken(effect.maxValue);
                break;

            //받은 피해 기반 방어
            case CardEffectType.DefenseByDamageTaken:
                DefenseByDamageTaken(effect.maxValue);
                break;

            //받은 피해 기반 공격 콤보 획득
            case CardEffectType.GainAttackComboByDamageTaken:
                GainAttackComboByDamageTaken();
                break;

            //모든 공격 콤보 소모
            case CardEffectType.ConsumeAllAttackCombo:
                _player.BuffSystem.ConsumeBuff(BuffType.AttackCombo);
                break;

            //모든 방어 콤보 소모
            case CardEffectType.ConsumeAllDefenseCombo:
                _player.BuffSystem.ConsumeBuff(BuffType.DefenseCombo);
                break;

            //모든 주문 소모
            case CardEffectType.ConsumeAllSpell:
                _player.BuffSystem.ConsumeBuff(BuffType.Spell);
                break;

            //주문 기반 대미지
            case CardEffectType.DamageBySpell:
                DamageBySpell(effect.value, effect.multiplier, effect.maxValue, effect.consumesBuff);
                break;

            //주문 기반 방어
            case CardEffectType.DefenseBySpell:
                DefenseBySpell(effect.multiplier);
                break;

            //기도 카드용: 주문 배율 변경
            case CardEffectType.MultiplySpell:
                MultiplySpell();
                break;
        }
    }

    private void ExecuteConditionalEffect(CardEffectType effectType, int value)
    {
        //조건부 효과 실행
        switch (effectType)
        {
            //코스트 획득
            case CardEffectType.GainCost:
                _player.AddCost(value);
                break;

            //카드 뽑기
            case CardEffectType.Draw:
                _deckManager.DrawCards(value);
                break;

            //방어 획득
            case CardEffectType.GainDefense:
                GainDefense(value);
                break;

            //대미지
            case CardEffectType.Damage:
                DealDamage(value);
                break;

            //공격 콤보 획득
            case CardEffectType.GainAttackCombo:
                GainBuff(BuffType.AttackCombo, value);
                break;

            //방어 콤보 획득
            case CardEffectType.GainDefenseCombo:
                GainBuff(BuffType.DefenseCombo, value);
                break;

            //주문 획득
            case CardEffectType.GainSpell:
                GainBuff(BuffType.Spell, value);
                break;
        }
    }

    private void DealDamage(int damage)
    {
        //몬스터에게 데미지 입히기

        if (_monster != null)
        {
            _monster.TakeDamage(damage);
        }
    }

    private void GainDefense(int amount)
    {
        //플레이어에게 방어 버프 획득

        _player.BuffSystem.AddBuff(BuffType.Defense, amount);
    }

    private void GainBuff(BuffType buffType, int amount)
    {
        //직업 보너스 적용
        int adjustedAmount = _player.ApplyJobBonus(buffType, amount);
        _player.BuffSystem.AddBuff(buffType, adjustedAmount);
    }

    private void ConvertCombo()
    {
        //콤보 전환

        int attackCombo = _player.BuffSystem.GetBuff(BuffType.AttackCombo);
        int defenseCombo = _player.BuffSystem.GetBuff(BuffType.DefenseCombo);

        //임시로 값 저장 후 교체
        _player.BuffSystem.SetBuff(BuffType.AttackCombo, defenseCombo);
        _player.BuffSystem.SetBuff(BuffType.DefenseCombo, attackCombo);
    }

    private void DamageByCombo(BuffType comboType, int multiplier)
    {
        //콤보 기반 대미지 계산

        int comboValue = _player.BuffSystem.GetBuff(comboType);
        int damage = comboValue * multiplier;
        DealDamage(damage);
    }

    private void DefenseByCombo(BuffType comboType, int multiplier)
    {
        //콤보 기반 방어 계산

        int comboValue = _player.BuffSystem.GetBuff(comboType);
        int defense = comboValue * multiplier;
        GainDefense(defense);
    }

    private void DamageByDamageTaken(int maxDamage)
    {
        //받은 피해량 기반 대미지 계산

        //int damageTaken = _player.DamageTakenThisTurn;
        int damageTaken = _player.MaxHealth - _player.CurrentHealth;
        int attackCombo = _player.BuffSystem.GetBuff(BuffType.AttackCombo);

        //최대 데미지는 공격 콤보 수치로 제한
        int damage = Mathf.Min(damageTaken, attackCombo);
        if (maxDamage > 0)
        {
            damage = Mathf.Min(damage, maxDamage);
        }
        DealDamage(damage);
    }

    private void DefenseByDamageTaken(int maxDefense)
    {
        //받은 피해량 기반 방어 계산

        //int damageTaken = _player.DamageTakenThisTurn;
        int damageTaken = _player.MaxHealth - _player.CurrentHealth;
        int defenseCombo = _player.BuffSystem.GetBuff(BuffType.DefenseCombo);

        //최대 방어는 방어 콤보 수치로 제한
        int defense = Mathf.Min(damageTaken, defenseCombo);
        if (maxDefense > 0)
        {
            defense = Mathf.Min(defense, maxDefense);
        }
        GainDefense(defense);
    }

    private void GainAttackComboByDamageTaken()
    {
        //받은 피해량 기반 공격 콤보 획득

        int damageTaken = _player.DamageTakenThisTurn;
        GainBuff(BuffType.AttackCombo, damageTaken);
    }

    //폭발 카드용: 주문 20 이하면 3 데미지, 21 이상이면 (주문 * 2) 데미지
    private void DamageBySpell(int baseDamage, int multiplier, int threshold, bool consumeSpell)
    {
        int spell = _player.BuffSystem.GetBuff(BuffType.Spell);
        int damage;

        if (threshold > 0 && spell <= threshold)
        {
            damage = baseDamage;
        }
        else
        {
            damage = spell * multiplier;
        }

        DealDamage(damage);

        if (consumeSpell)
        {
            _player.BuffSystem.ConsumeBuff(BuffType.Spell);
        }
    }

    //결계 카드용: (주문 * 1) 방어 획득
    private void DefenseBySpell(int multiplier)
    {
        int spell = _player.BuffSystem.GetBuff(BuffType.Spell);
        int defense = spell * multiplier;
        GainDefense(defense);
    }

    //화염 카드용: (주문 * 1) + 3 데미지, 최대 10
    public void ExecuteFireballDamage(int baseDamage, int spellMultiplier, int maxDamage)
    {
        int spell = _player.BuffSystem.GetBuff(BuffType.Spell);
        int damage = (spell * spellMultiplier) + baseDamage;

        if (maxDamage > 0)
        {
            damage = Mathf.Min(damage, maxDamage);
        }

        DealDamage(damage);
    }

    //기도 카드용: 주문을 1~3배로 변경
    private void MultiplySpell()
    {
        int multiplier = Random.Range(1, 4);
        _player.BuffSystem.MultiplyBuff(BuffType.Spell, multiplier);
    }
}
