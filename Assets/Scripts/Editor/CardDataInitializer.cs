using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class CardDataInitializer : EditorWindow
{
    [MenuItem("Tools/Initialize Card Data")]
    public static void InitializeAllCards()
    {
        InitializeNeutralCards();
        InitializeWarriorCards();
        InitializeMageCards();
        InitializeMonsterPatterns();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("[CardDataInitializer] 모든 카드 및 몬스터 데이터 초기화 완료!");
    }

    private static void InitializeNeutralCards()
    {
        //공격
        CardData attack = AssetDatabase.LoadAssetAtPath<CardData>("Assets/ScriptableObjects/Cards/Card_Attack.asset");
        if (attack != null)
        {
            attack.effects = new CardEffect[]
            {
                new CardEffect { effectType = CardEffectType.Damage, value = 6 }
            };
            EditorUtility.SetDirty(attack);
        }

        //방어
        CardData defense = AssetDatabase.LoadAssetAtPath<CardData>("Assets/ScriptableObjects/Cards/Card_Defense.asset");
        if (defense != null)
        {
            defense.effects = new CardEffect[]
            {
                new CardEffect { effectType = CardEffectType.GainDefense, value = 4 }
            };
            EditorUtility.SetDirty(defense);
        }

        Debug.Log("[CardDataInitializer] 중립 카드 초기화 완료");
    }

    private static void InitializeWarriorCards()
    {
        //기합
        CardData kihap = AssetDatabase.LoadAssetAtPath<CardData>("Assets/ScriptableObjects/Cards/Card_Warrior_Kihap.asset");
        if (kihap != null)
        {
            kihap.effects = new CardEffect[]
            {
                new CardEffect { effectType = CardEffectType.Draw, value = 3 },
                new CardEffect { effectType = CardEffectType.GainDefenseCombo, value = 1 },
                new CardEffect
                {
                    effectType = CardEffectType.GainCost,
                    value = 0,
                    hasCondition = true,
                    conditionBuffType = BuffType.DefenseCombo,
                    conditionThreshold = 2,
                    conditionalEffectType = CardEffectType.GainCost,
                    conditionalValue = 1
                }
            };
            EditorUtility.SetDirty(kihap);
        }

        //준비 자세
        CardData readyStance = AssetDatabase.LoadAssetAtPath<CardData>("Assets/ScriptableObjects/Cards/Card_Warrior_ReadyStance.asset");
        if (readyStance != null)
        {
            readyStance.effects = new CardEffect[]
            {
                new CardEffect { effectType = CardEffectType.GainCost, value = 2 },
                new CardEffect { effectType = CardEffectType.GainAttackCombo, value = 1 },
                new CardEffect
                {
                    effectType = CardEffectType.Draw,
                    value = 0,
                    hasCondition = true,
                    conditionBuffType = BuffType.AttackCombo,
                    conditionThreshold = 2,
                    conditionalEffectType = CardEffectType.Draw,
                    conditionalValue = 1
                }
            };
            EditorUtility.SetDirty(readyStance);
        }

        //태세 전환
        CardData stanceSwitch = AssetDatabase.LoadAssetAtPath<CardData>("Assets/ScriptableObjects/Cards/Card_Warrior_StanceSwitch.asset");
        if (stanceSwitch != null)
        {
            stanceSwitch.effects = new CardEffect[]
            {
                new CardEffect { effectType = CardEffectType.ConvertCombo }
            };
            EditorUtility.SetDirty(stanceSwitch);
        }

        //직검
        CardData straightSword = AssetDatabase.LoadAssetAtPath<CardData>("Assets/ScriptableObjects/Cards/Card_Warrior_StraightSword.asset");
        if (straightSword != null)
        {
            straightSword.effects = new CardEffect[]
            {
                new CardEffect { effectType = CardEffectType.Damage, value = 9 },
                new CardEffect { effectType = CardEffectType.GainAttackCombo, value = 2 }
            };
            EditorUtility.SetDirty(straightSword);
        }

        //방패
        CardData shield = AssetDatabase.LoadAssetAtPath<CardData>("Assets/ScriptableObjects/Cards/Card_Warrior_Shield.asset");
        if (shield != null)
        {
            shield.effects = new CardEffect[]
            {
                new CardEffect { effectType = CardEffectType.GainDefense, value = 6 },
                new CardEffect { effectType = CardEffectType.GainDefenseCombo, value = 2 }
            };
            EditorUtility.SetDirty(shield);
        }

        //집중
        CardData focus = AssetDatabase.LoadAssetAtPath<CardData>("Assets/ScriptableObjects/Cards/Card_Warrior_Focus.asset");
        if (focus != null)
        {
            focus.effects = new CardEffect[]
            {
                new CardEffect { effectType = CardEffectType.DamageByAttackCombo, multiplier = 4 },
                new CardEffect { effectType = CardEffectType.DefenseByDefenseCombo, multiplier = 4 }
            };
            EditorUtility.SetDirty(focus);
        }

        //마무리
        CardData finisher = AssetDatabase.LoadAssetAtPath<CardData>("Assets/ScriptableObjects/Cards/Card_Warrior_Finisher.asset");
        if (finisher != null)
        {
            finisher.effects = new CardEffect[]
            {
                new CardEffect { effectType = CardEffectType.DamageByAttackCombo, multiplier = 10 },
                new CardEffect { effectType = CardEffectType.ConsumeAllAttackCombo }
            };
            EditorUtility.SetDirty(finisher);
        }

        //태산
        CardData mountain = AssetDatabase.LoadAssetAtPath<CardData>("Assets/ScriptableObjects/Cards/Card_Warrior_Mountain.asset");
        if (mountain != null)
        {
            mountain.effects = new CardEffect[]
            {
                new CardEffect { effectType = CardEffectType.DefenseByDefenseCombo, multiplier = 10 },
                new CardEffect { effectType = CardEffectType.ConsumeAllDefenseCombo }
            };
            EditorUtility.SetDirty(mountain);
        }

        //추격
        CardData chase = AssetDatabase.LoadAssetAtPath<CardData>("Assets/ScriptableObjects/Cards/Card_Warrior_Chase.asset");
        if (chase != null)
        {
            chase.effects = new CardEffect[]
            {
                new CardEffect { effectType = CardEffectType.DamageByDamageTaken, maxValue = -1 } //-1은 공격 콤보로 제한
            };
            EditorUtility.SetDirty(chase);
        }

        //침착
        CardData calm = AssetDatabase.LoadAssetAtPath<CardData>("Assets/ScriptableObjects/Cards/Card_Warrior_Calm.asset");
        if (calm != null)
        {
            calm.effects = new CardEffect[]
            {
                new CardEffect { effectType = CardEffectType.DefenseByDamageTaken, maxValue = -1 } //-1은 방어 콤보로 제한
            };
            EditorUtility.SetDirty(calm);
        }

        //분노
        CardData rage = AssetDatabase.LoadAssetAtPath<CardData>("Assets/ScriptableObjects/Cards/Card_Warrior_Rage.asset");
        if (rage != null)
        {
            rage.effects = new CardEffect[]
            {
                new CardEffect { effectType = CardEffectType.GainAttackComboByDamageTaken }
            };
            EditorUtility.SetDirty(rage);
        }

        Debug.Log("[CardDataInitializer] 전사 카드 초기화 완료");
    }

    private static void InitializeMageCards()
    {
        //지팡이
        CardData staff = AssetDatabase.LoadAssetAtPath<CardData>("Assets/ScriptableObjects/Cards/Card_Mage_Staff.asset");
        if (staff != null)
        {
            staff.effects = new CardEffect[]
            {
                new CardEffect { effectType = CardEffectType.Damage, value = 6 },
                new CardEffect { effectType = CardEffectType.GainSpell, value = 1 }
            };
            EditorUtility.SetDirty(staff);
        }

        //로브
        CardData robe = AssetDatabase.LoadAssetAtPath<CardData>("Assets/ScriptableObjects/Cards/Card_Mage_Robe.asset");
        if (robe != null)
        {
            robe.effects = new CardEffect[]
            {
                new CardEffect { effectType = CardEffectType.GainDefense, value = 4 },
                new CardEffect { effectType = CardEffectType.GainSpell, value = 2 }
            };
            EditorUtility.SetDirty(robe);
        }

        //심호흡
        CardData deepBreath = AssetDatabase.LoadAssetAtPath<CardData>("Assets/ScriptableObjects/Cards/Card_Mage_DeepBreath.asset");
        if (deepBreath != null)
        {
            deepBreath.effects = new CardEffect[]
            {
                new CardEffect { effectType = CardEffectType.GainCost, value = 2 },
                new CardEffect { effectType = CardEffectType.GainDefense, value = 3 },
                new CardEffect { effectType = CardEffectType.GainSpell, value = 3 }
            };
            EditorUtility.SetDirty(deepBreath);
        }

        //사고
        CardData think = AssetDatabase.LoadAssetAtPath<CardData>("Assets/ScriptableObjects/Cards/Card_Mage_Think.asset");
        if (think != null)
        {
            think.effects = new CardEffect[]
            {
                new CardEffect { effectType = CardEffectType.Draw, value = 3 },
                new CardEffect { effectType = CardEffectType.GainDefense, value = 3 },
                new CardEffect { effectType = CardEffectType.GainSpell, value = 3 }
            };
            EditorUtility.SetDirty(think);
        }

        //실드
        CardData magicShield = AssetDatabase.LoadAssetAtPath<CardData>("Assets/ScriptableObjects/Cards/Card_Mage_MagicShield.asset");
        if (magicShield != null)
        {
            magicShield.effects = new CardEffect[]
            {
                new CardEffect { effectType = CardEffectType.GainDefense, value = 15 },
                new CardEffect { effectType = CardEffectType.GainSpell, value = 5 }
            };
            EditorUtility.SetDirty(magicShield);
        }

        //영창
        CardData chant = AssetDatabase.LoadAssetAtPath<CardData>("Assets/ScriptableObjects/Cards/Card_Mage_Chant.asset");
        if (chant != null)
        {
            chant.effects = new CardEffect[]
            {
                new CardEffect { effectType = CardEffectType.GainSpell, value = 10 }
            };
            EditorUtility.SetDirty(chant);
        }

        //폭발
        CardData explosion = AssetDatabase.LoadAssetAtPath<CardData>("Assets/ScriptableObjects/Cards/Card_Mage_Explosion.asset");
        if (explosion != null)
        {
            explosion.effects = new CardEffect[]
            {
                new CardEffect
                {
                    effectType = CardEffectType.DamageBySpell,
                    value = 3,          //기본 데미지
                    multiplier = 2,     //주문 배율
                    maxValue = 20,      //threshold (20 이하면 기본 데미지)
                    consumesBuff = true
                },
                new CardEffect { effectType = CardEffectType.ConsumeAllSpell }
            };
            EditorUtility.SetDirty(explosion);
        }

        //결계
        CardData barrier = AssetDatabase.LoadAssetAtPath<CardData>("Assets/ScriptableObjects/Cards/Card_Mage_Barrier.asset");
        if (barrier != null)
        {
            barrier.effects = new CardEffect[]
            {
                new CardEffect { effectType = CardEffectType.DefenseBySpell, multiplier = 1 }
            };
            EditorUtility.SetDirty(barrier);
        }

        //기도
        CardData prayer = AssetDatabase.LoadAssetAtPath<CardData>("Assets/ScriptableObjects/Cards/Card_Mage_Prayer.asset");
        if (prayer != null)
        {
            prayer.effects = new CardEffect[]
            {
                new CardEffect { effectType = CardEffectType.MultiplySpell }
            };
            EditorUtility.SetDirty(prayer);
        }

        //단문 영창
        CardData quickChant = AssetDatabase.LoadAssetAtPath<CardData>("Assets/ScriptableObjects/Cards/Card_Mage_QuickChant.asset");
        if (quickChant != null)
        {
            quickChant.effects = new CardEffect[]
            {
                new CardEffect { effectType = CardEffectType.GainSpell, value = 3 }
            };
            EditorUtility.SetDirty(quickChant);
        }

        //화염
        CardData fireball = AssetDatabase.LoadAssetAtPath<CardData>("Assets/ScriptableObjects/Cards/Card_Mage_Fireball.asset");
        if (fireball != null)
        {
            fireball.effects = new CardEffect[]
            {
                new CardEffect
                {
                    effectType = CardEffectType.DamageBySpell,
                    value = 3,          //기본 데미지
                    multiplier = 1,     //주문 배율
                    maxValue = 10       //최대 데미지
                }
            };
            EditorUtility.SetDirty(fireball);
        }

        Debug.Log("[CardDataInitializer] 법사 카드 초기화 완료");
    }

    private static void InitializeMonsterPatterns()
    {
        //Monster A
        MonsterData monsterA = AssetDatabase.LoadAssetAtPath<MonsterData>("Assets/ScriptableObjects/Monsters/Monster_A.asset");
        if (monsterA != null)
        {
            monsterA.patterns = new MonsterPattern[]
            {
                new MonsterPattern { actionType = MonsterActionType.Attack, value = 10 },
                new MonsterPattern { actionType = MonsterActionType.Defense, value = 6 },
                new MonsterPattern { actionType = MonsterActionType.Attack, value = 12 },
                new MonsterPattern { actionType = MonsterActionType.Defense, value = 8 },
                new MonsterPattern { actionType = MonsterActionType.Attack, value = 15 },
                new MonsterPattern { actionType = MonsterActionType.Defense, value = 10 }
            };
            EditorUtility.SetDirty(monsterA);
        }

        //Monster B
        MonsterData monsterB = AssetDatabase.LoadAssetAtPath<MonsterData>("Assets/ScriptableObjects/Monsters/Monster_B.asset");
        if (monsterB != null)
        {
            monsterB.patterns = new MonsterPattern[]
            {
                new MonsterPattern { actionType = MonsterActionType.Defense, value = 3 },
                new MonsterPattern { actionType = MonsterActionType.Defense, value = 5 },
                new MonsterPattern { actionType = MonsterActionType.Attack, value = 10 },
                new MonsterPattern { actionType = MonsterActionType.Defense, value = 8 },
                new MonsterPattern { actionType = MonsterActionType.Defense, value = 10 },
                new MonsterPattern { actionType = MonsterActionType.Attack, value = 25 }
            };
            EditorUtility.SetDirty(monsterB);
        }

        //Boss
        MonsterData boss = AssetDatabase.LoadAssetAtPath<MonsterData>("Assets/ScriptableObjects/Monsters/Monster_Boss.asset");
        if (boss != null)
        {
            boss.patterns = new MonsterPattern[]
            {
                new MonsterPattern { actionType = MonsterActionType.Attack, value = 15 },
                new MonsterPattern { actionType = MonsterActionType.Defense, value = 10 },
                new MonsterPattern { actionType = MonsterActionType.Attack, value = 20 },
                new MonsterPattern { actionType = MonsterActionType.Defense, value = 15 },
                new MonsterPattern { actionType = MonsterActionType.Attack, value = 40 },
                new MonsterPattern { actionType = MonsterActionType.Defense, value = 10 }
            };
            EditorUtility.SetDirty(boss);
        }

        Debug.Log("[CardDataInitializer] 몬스터 패턴 초기화 완료");
    }
}
