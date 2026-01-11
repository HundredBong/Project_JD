using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class SceneDataSetup : EditorWindow
{
    [MenuItem("Tools/Setup Scene Data")]
    public static void SetupAllData()
    {
        SetupGameManager();
        SetupBattleManager();

        Debug.Log("[SceneDataSetup] 씬 데이터 설정 완료!");
    }

    private static void SetupGameManager()
    {
        GameManager gameManager = Object.FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("[SceneDataSetup] GameManager를 찾을 수 없습니다.");
            return;
        }

        SerializedObject serializedObject = new SerializedObject(gameManager);

        //시작 덱 설정 (공격 4장, 방어 4장)
        SerializedProperty startingDeckProp = serializedObject.FindProperty("_startingDeck");
        CardData attackCard = AssetDatabase.LoadAssetAtPath<CardData>("Assets/ScriptableObjects/Cards/Card_Attack.asset");
        CardData defenseCard = AssetDatabase.LoadAssetAtPath<CardData>("Assets/ScriptableObjects/Cards/Card_Defense.asset");

        startingDeckProp.arraySize = 8;
        for (int i = 0; i < 4; i++)
        {
            startingDeckProp.GetArrayElementAtIndex(i).objectReferenceValue = attackCard;
        }
        for (int i = 4; i < 8; i++)
        {
            startingDeckProp.GetArrayElementAtIndex(i).objectReferenceValue = defenseCard;
        }

        //전체 카드 풀 설정
        SerializedProperty allCardsProp = serializedObject.FindProperty("_allCards");
        List<CardData> allCards = LoadAllCards();
        allCardsProp.arraySize = allCards.Count;
        for (int i = 0; i < allCards.Count; i++)
        {
            allCardsProp.GetArrayElementAtIndex(i).objectReferenceValue = allCards[i];
        }

        serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(gameManager);

        Debug.Log($"[SceneDataSetup] GameManager 설정 완료 - 시작 덱: 8장, 전체 카드: {allCards.Count}장");
    }

    private static void SetupBattleManager()
    {
        BattleManager battleManager = Object.FindObjectOfType<BattleManager>();
        if (battleManager == null)
        {
            Debug.LogError("[SceneDataSetup] BattleManager를 찾을 수 없습니다.");
            return;
        }

        SerializedObject serializedObject = new SerializedObject(battleManager);

        //일반 몬스터 설정
        SerializedProperty normalMonstersProp = serializedObject.FindProperty("_normalMonsters");
        MonsterData monsterA = AssetDatabase.LoadAssetAtPath<MonsterData>("Assets/ScriptableObjects/Monsters/Monster_A.asset");
        MonsterData monsterB = AssetDatabase.LoadAssetAtPath<MonsterData>("Assets/ScriptableObjects/Monsters/Monster_B.asset");

        normalMonstersProp.arraySize = 2;
        normalMonstersProp.GetArrayElementAtIndex(0).objectReferenceValue = monsterA;
        normalMonstersProp.GetArrayElementAtIndex(1).objectReferenceValue = monsterB;

        //보스 몬스터 설정
        SerializedProperty bossMonsterProp = serializedObject.FindProperty("_bossMonster");
        MonsterData bossMonster = AssetDatabase.LoadAssetAtPath<MonsterData>("Assets/ScriptableObjects/Monsters/Monster_Boss.asset");
        bossMonsterProp.objectReferenceValue = bossMonster;

        serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(battleManager);

        Debug.Log("[SceneDataSetup] BattleManager 설정 완료 - 일반 몬스터: 2종, 보스: 1종");
    }

    private static List<CardData> LoadAllCards()
    {
        List<CardData> cards = new List<CardData>();

        //중립 카드
        cards.Add(AssetDatabase.LoadAssetAtPath<CardData>("Assets/ScriptableObjects/Cards/Card_Attack.asset"));
        cards.Add(AssetDatabase.LoadAssetAtPath<CardData>("Assets/ScriptableObjects/Cards/Card_Defense.asset"));

        //전사 카드
        cards.Add(AssetDatabase.LoadAssetAtPath<CardData>("Assets/ScriptableObjects/Cards/Card_Warrior_Kihap.asset"));
        cards.Add(AssetDatabase.LoadAssetAtPath<CardData>("Assets/ScriptableObjects/Cards/Card_Warrior_ReadyStance.asset"));
        cards.Add(AssetDatabase.LoadAssetAtPath<CardData>("Assets/ScriptableObjects/Cards/Card_Warrior_StanceSwitch.asset"));
        cards.Add(AssetDatabase.LoadAssetAtPath<CardData>("Assets/ScriptableObjects/Cards/Card_Warrior_StraightSword.asset"));
        cards.Add(AssetDatabase.LoadAssetAtPath<CardData>("Assets/ScriptableObjects/Cards/Card_Warrior_Shield.asset"));
        cards.Add(AssetDatabase.LoadAssetAtPath<CardData>("Assets/ScriptableObjects/Cards/Card_Warrior_Focus.asset"));
        cards.Add(AssetDatabase.LoadAssetAtPath<CardData>("Assets/ScriptableObjects/Cards/Card_Warrior_Finisher.asset"));
        cards.Add(AssetDatabase.LoadAssetAtPath<CardData>("Assets/ScriptableObjects/Cards/Card_Warrior_Mountain.asset"));
        cards.Add(AssetDatabase.LoadAssetAtPath<CardData>("Assets/ScriptableObjects/Cards/Card_Warrior_Chase.asset"));
        cards.Add(AssetDatabase.LoadAssetAtPath<CardData>("Assets/ScriptableObjects/Cards/Card_Warrior_Calm.asset"));
        cards.Add(AssetDatabase.LoadAssetAtPath<CardData>("Assets/ScriptableObjects/Cards/Card_Warrior_Rage.asset"));

        //법사 카드
        cards.Add(AssetDatabase.LoadAssetAtPath<CardData>("Assets/ScriptableObjects/Cards/Card_Mage_Staff.asset"));
        cards.Add(AssetDatabase.LoadAssetAtPath<CardData>("Assets/ScriptableObjects/Cards/Card_Mage_Robe.asset"));
        cards.Add(AssetDatabase.LoadAssetAtPath<CardData>("Assets/ScriptableObjects/Cards/Card_Mage_DeepBreath.asset"));
        cards.Add(AssetDatabase.LoadAssetAtPath<CardData>("Assets/ScriptableObjects/Cards/Card_Mage_Think.asset"));
        cards.Add(AssetDatabase.LoadAssetAtPath<CardData>("Assets/ScriptableObjects/Cards/Card_Mage_MagicShield.asset"));
        cards.Add(AssetDatabase.LoadAssetAtPath<CardData>("Assets/ScriptableObjects/Cards/Card_Mage_Chant.asset"));
        cards.Add(AssetDatabase.LoadAssetAtPath<CardData>("Assets/ScriptableObjects/Cards/Card_Mage_Explosion.asset"));
        cards.Add(AssetDatabase.LoadAssetAtPath<CardData>("Assets/ScriptableObjects/Cards/Card_Mage_Barrier.asset"));
        cards.Add(AssetDatabase.LoadAssetAtPath<CardData>("Assets/ScriptableObjects/Cards/Card_Mage_Prayer.asset"));
        cards.Add(AssetDatabase.LoadAssetAtPath<CardData>("Assets/ScriptableObjects/Cards/Card_Mage_QuickChant.asset"));
        cards.Add(AssetDatabase.LoadAssetAtPath<CardData>("Assets/ScriptableObjects/Cards/Card_Mage_Fireball.asset"));

        //null 체크
        cards.RemoveAll(c => c == null);

        return cards;
    }
}
