using UnityEngine;
using UnityEditor;

public class CardUIPrefabCreator : EditorWindow
{
    [MenuItem("Tools/Setup BattlePage UI References")]
    public static void SetupBattlePageUI()
    {
        BattlePage battlePage = GameObject.FindObjectOfType<BattlePage>(true);
        if (battlePage == null)
        {
            Debug.LogError("BattlePage not found!");
            return;
        }

        SerializedObject so = new SerializedObject(battlePage);

        // Player Defense Text 찾기 또는 생성
        Transform playerArea = battlePage.transform.Find("Player Area");
        if (playerArea != null)
        {
            // Player Defense Text
            Transform defenseTextTransform = playerArea.Find("Player Defense Text");
            TMPro.TextMeshProUGUI defenseText = defenseTextTransform?.GetComponent<TMPro.TextMeshProUGUI>();
            if (defenseText != null)
            {
                SerializedProperty prop = so.FindProperty("_playerDefenseText");
                if (prop != null)
                {
                    prop.objectReferenceValue = defenseText;
                    Debug.Log("BattlePage: Player Defense Text assigned");
                }
            }

            // Player Buff Text
            Transform buffTextTransform = playerArea.Find("Player Buff Text");
            TMPro.TextMeshProUGUI buffText = buffTextTransform?.GetComponent<TMPro.TextMeshProUGUI>();
            if (buffText != null)
            {
                SerializedProperty prop = so.FindProperty("_playerBuffText");
                if (prop != null)
                {
                    prop.objectReferenceValue = buffText;
                    Debug.Log("BattlePage: Player Buff Text assigned");
                }
            }
        }

        so.ApplyModifiedProperties();
        EditorUtility.SetDirty(battlePage);
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());

        Debug.Log("BattlePage UI setup complete!");
    }

    [MenuItem("Tools/Create Card UI Prefabs")]
    public static void CreateCardUIPrefabs()
    {
        string sourcePath = "Assets/Prefabs/UI/CardUI.prefab";
        GameObject sourcePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(sourcePath);

        if (sourcePrefab == null)
        {
            Debug.LogError("CardUI.prefab not found!");
            return;
        }

        // SelectableCardUI
        CreateVariantPrefab(sourcePrefab, "SelectableCardUI", typeof(CardUI), typeof(SelectableCardUI));

        // DeletableCardUI
        CreateVariantPrefab(sourcePrefab, "DeletableCardUI", typeof(CardUI), typeof(DeletableCardUI));

        // CardDisplayUI (Button 제거)
        CreateDisplayPrefab(sourcePrefab);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Card UI Prefabs created successfully!");

        // 페이지에 프리팹 연결
        AssignPrefabsToPages();
    }

    private static void AssignPrefabsToPages()
    {
        // CardSelectPage에 SelectableCardUI 프리팹 연결
        CardSelectPage cardSelectPage = GameObject.FindObjectOfType<CardSelectPage>(true);
        if (cardSelectPage != null)
        {
            SerializedObject so = new SerializedObject(cardSelectPage);
            SerializedProperty prop = so.FindProperty("_selectableCardPrefab");
            if (prop != null)
            {
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/UI/SelectableCardUI.prefab");
                prop.objectReferenceValue = prefab;
                so.ApplyModifiedProperties();
                Debug.Log("CardSelectPage: SelectableCardUI prefab assigned");
            }
        }

        // CardDeletePage에 DeletableCardUI 프리팹 연결
        CardDeletePage cardDeletePage = GameObject.FindObjectOfType<CardDeletePage>(true);
        if (cardDeletePage != null)
        {
            SerializedObject so = new SerializedObject(cardDeletePage);
            SerializedProperty prop = so.FindProperty("_deletableCardPrefab");
            if (prop != null)
            {
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/UI/DeletableCardUI.prefab");
                prop.objectReferenceValue = prefab;
                so.ApplyModifiedProperties();
                Debug.Log("CardDeletePage: DeletableCardUI prefab assigned");
            }
        }

        // DeckViewPopup에 CardDisplayUI 프리팹 연결
        DeckViewPopup deckViewPopup = GameObject.FindObjectOfType<DeckViewPopup>(true);
        if (deckViewPopup != null)
        {
            SerializedObject so = new SerializedObject(deckViewPopup);
            SerializedProperty prop = so.FindProperty("_cardDisplayPrefab");
            if (prop != null)
            {
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/UI/CardDisplayUI.prefab");
                prop.objectReferenceValue = prefab;
                so.ApplyModifiedProperties();
                Debug.Log("DeckViewPopup: CardDisplayUI prefab assigned");
            }
        }

        // Game Over Page 비활성화
        GameOverPage gameOverPage = GameObject.FindObjectOfType<GameOverPage>(true);
        if (gameOverPage != null && gameOverPage.gameObject.activeSelf)
        {
            gameOverPage.gameObject.SetActive(false);
            EditorUtility.SetDirty(gameOverPage.gameObject);
            Debug.Log("GameOverPage: Deactivated");
        }

        // 씬 저장
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
    }

    private static void CreateVariantPrefab(GameObject source, string name, System.Type removeType, System.Type addType)
    {
        string path = $"Assets/Prefabs/UI/{name}.prefab";

        // 기존 프리팹 삭제
        if (AssetDatabase.LoadAssetAtPath<GameObject>(path) != null)
        {
            AssetDatabase.DeleteAsset(path);
        }

        // 인스턴스 생성
        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(source);
        PrefabUtility.UnpackPrefabInstance(instance, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);

        // 컴포넌트 교체
        Component oldComponent = instance.GetComponent(removeType);
        if (oldComponent != null)
        {
            DestroyImmediate(oldComponent);
        }

        instance.AddComponent(addType);

        // 새 프리팹으로 저장
        PrefabUtility.SaveAsPrefabAsset(instance, path);
        DestroyImmediate(instance);

        Debug.Log($"Created: {path}");
    }

    private static void CreateDisplayPrefab(GameObject source)
    {
        string path = "Assets/Prefabs/UI/CardDisplayUI.prefab";

        // 기존 프리팹 삭제
        if (AssetDatabase.LoadAssetAtPath<GameObject>(path) != null)
        {
            AssetDatabase.DeleteAsset(path);
        }

        // 인스턴스 생성
        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(source);
        PrefabUtility.UnpackPrefabInstance(instance, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);

        // CardUI 제거
        CardUI cardUI = instance.GetComponent<CardUI>();
        if (cardUI != null)
        {
            DestroyImmediate(cardUI);
        }

        // Button 제거 (표시 전용이므로)
        UnityEngine.UI.Button button = instance.GetComponent<UnityEngine.UI.Button>();
        if (button != null)
        {
            DestroyImmediate(button);
        }

        // CardDisplayUI 추가
        instance.AddComponent<CardDisplayUI>();

        // 새 프리팹으로 저장
        PrefabUtility.SaveAsPrefabAsset(instance, path);
        DestroyImmediate(instance);

        Debug.Log($"Created: {path}");
    }
}
