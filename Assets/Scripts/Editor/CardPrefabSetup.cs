using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

public class CardPrefabSetup : EditorWindow
{
    [MenuItem("Tools/Setup Card Prefab")]
    public static void SetupCardPrefab()
    {
        //Card 오브젝트 찾기
        GameObject cardObj = GameObject.Find("Card");
        if (cardObj == null)
        {
            Debug.LogError("[CardPrefabSetup] 'Card' 오브젝트를 찾을 수 없습니다.");
            return;
        }

        RectTransform cardRect = cardObj.GetComponent<RectTransform>();
        CardUI cardUI = cardObj.GetComponent<CardUI>();
        Image backgroundImage = cardObj.GetComponent<Image>();
        Button button = cardObj.GetComponent<Button>();

        if (cardUI == null)
        {
            Debug.LogError("[CardPrefabSetup] CardUI 컴포넌트가 없습니다.");
            return;
        }

        //자식 텍스트들 찾기
        Transform costTextTr = cardObj.transform.Find("Cost Text");
        Transform nameTextTr = cardObj.transform.Find("Name Text");
        Transform typeTextTr = cardObj.transform.Find("Type Text");
        Transform descTextTr = cardObj.transform.Find("Description Text");

        if (costTextTr == null || nameTextTr == null || typeTextTr == null || descTextTr == null)
        {
            Debug.LogError("[CardPrefabSetup] 텍스트 자식 오브젝트가 없습니다.");
            return;
        }

        //Cost Text 설정 (좌상단, 원형 배경)
        SetupCostText(costTextTr);

        //Name Text 설정 (상단 중앙)
        SetupNameText(nameTextTr);

        //Type Text 설정 (이름 아래)
        SetupTypeText(typeTextTr);

        //Description Text 설정 (중앙~하단)
        SetupDescriptionText(descTextTr);

        //CardUI 참조 연결
        SerializedObject serializedCardUI = new SerializedObject(cardUI);
        serializedCardUI.FindProperty("_costText").objectReferenceValue = costTextTr.GetComponent<TextMeshProUGUI>();
        serializedCardUI.FindProperty("_nameText").objectReferenceValue = nameTextTr.GetComponent<TextMeshProUGUI>();
        serializedCardUI.FindProperty("_typeText").objectReferenceValue = typeTextTr.GetComponent<TextMeshProUGUI>();
        serializedCardUI.FindProperty("_descriptionText").objectReferenceValue = descTextTr.GetComponent<TextMeshProUGUI>();
        serializedCardUI.FindProperty("_backgroundImage").objectReferenceValue = backgroundImage;
        serializedCardUI.FindProperty("_button").objectReferenceValue = button;
        serializedCardUI.ApplyModifiedProperties();

        //Button Target Graphic 설정
        if (button != null)
        {
            button.targetGraphic = backgroundImage;
            EditorUtility.SetDirty(button);
        }

        EditorUtility.SetDirty(cardObj);
        Debug.Log("[CardPrefabSetup] Card UI 설정 완료!");

        //프리팹 저장
        SaveAsPrefab(cardObj);
    }

    private static void SetupCostText(Transform textTr)
    {
        RectTransform rect = textTr.GetComponent<RectTransform>();
        TextMeshProUGUI tmp = textTr.GetComponent<TextMeshProUGUI>();

        //좌상단 고정
        rect.anchorMin = new Vector2(0, 1);
        rect.anchorMax = new Vector2(0, 1);
        rect.pivot = new Vector2(0, 1);
        rect.anchoredPosition = new Vector2(8, -8);
        rect.sizeDelta = new Vector2(32, 32);

        //TMP 설정
        tmp.text = "3";
        tmp.fontSize = 24;
        tmp.fontStyle = FontStyles.Bold;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;

        EditorUtility.SetDirty(rect);
        EditorUtility.SetDirty(tmp);
    }

    private static void SetupNameText(Transform textTr)
    {
        RectTransform rect = textTr.GetComponent<RectTransform>();
        TextMeshProUGUI tmp = textTr.GetComponent<TextMeshProUGUI>();

        //상단 중앙 (좌우 늘림)
        rect.anchorMin = new Vector2(0, 1);
        rect.anchorMax = new Vector2(1, 1);
        rect.pivot = new Vector2(0.5f, 1);
        rect.anchoredPosition = new Vector2(0, -8);
        rect.sizeDelta = new Vector2(-16, 28);

        //TMP 설정
        tmp.text = "카드 이름";
        tmp.fontSize = 18;
        tmp.fontStyle = FontStyles.Bold;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;

        EditorUtility.SetDirty(rect);
        EditorUtility.SetDirty(tmp);
    }

    private static void SetupTypeText(Transform textTr)
    {
        RectTransform rect = textTr.GetComponent<RectTransform>();
        TextMeshProUGUI tmp = textTr.GetComponent<TextMeshProUGUI>();

        //이름 아래
        rect.anchorMin = new Vector2(0, 1);
        rect.anchorMax = new Vector2(1, 1);
        rect.pivot = new Vector2(0.5f, 1);
        rect.anchoredPosition = new Vector2(0, -38);
        rect.sizeDelta = new Vector2(-16, 20);

        //TMP 설정
        tmp.text = "중립";
        tmp.fontSize = 14;
        tmp.fontStyle = FontStyles.Normal;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = new Color(0.9f, 0.9f, 0.9f);

        EditorUtility.SetDirty(rect);
        EditorUtility.SetDirty(tmp);
    }

    private static void SetupDescriptionText(Transform textTr)
    {
        RectTransform rect = textTr.GetComponent<RectTransform>();
        TextMeshProUGUI tmp = textTr.GetComponent<TextMeshProUGUI>();

        //중앙~하단 (전체 영역에서 상단 60, 하단 10 여백)
        rect.anchorMin = new Vector2(0, 0);
        rect.anchorMax = new Vector2(1, 1);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = new Vector2(0, -25);
        rect.sizeDelta = new Vector2(-16, -75);

        //TMP 설정
        tmp.text = "카드 효과 설명";
        tmp.fontSize = 14;
        tmp.fontStyle = FontStyles.Normal;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;
        tmp.enableWordWrapping = true;
        tmp.overflowMode = TextOverflowModes.Truncate;

        EditorUtility.SetDirty(rect);
        EditorUtility.SetDirty(tmp);
    }

    private static void SaveAsPrefab(GameObject cardObj)
    {
        string folderPath = "Assets/Prefabs/UI";
        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
            {
                AssetDatabase.CreateFolder("Assets", "Prefabs");
            }
            AssetDatabase.CreateFolder("Assets/Prefabs", "UI");
        }

        string prefabPath = folderPath + "/CardUI.prefab";

        //기존 프리팹이 있으면 덮어쓰기
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(cardObj, prefabPath);

        if (prefab != null)
        {
            Debug.Log($"[CardPrefabSetup] 프리팹 저장 완료: {prefabPath}");
        }
        else
        {
            Debug.LogError("[CardPrefabSetup] 프리팹 저장 실패!");
        }
    }
}
