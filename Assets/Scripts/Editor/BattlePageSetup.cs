using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

public class BattlePageSetup : EditorWindow
{
    [MenuItem("Tools/Setup Battle Page UI")]
    public static void SetupBattlePageUI()
    {
        GameObject battlePageObj = GameObject.Find("Battle Page");
        if (battlePageObj == null)
        {
            Debug.LogError("[BattlePageSetup] 'Battle Page'를 찾을 수 없습니다.");
            return;
        }

        BattlePage battlePage = battlePageObj.GetComponent<BattlePage>();
        if (battlePage == null)
        {
            Debug.LogError("[BattlePageSetup] BattlePage 컴포넌트가 없습니다.");
            return;
        }

        //플레이어 영역 생성
        GameObject playerArea = CreatePlayerArea(battlePageObj.transform);

        //몬스터 영역 생성
        GameObject monsterArea = CreateMonsterArea(battlePageObj.transform);

        //버튼 영역 생성
        GameObject buttonArea = CreateButtonArea(battlePageObj.transform);

        //Card Layout 찾기 (핸드 컨테이너)
        Transform cardLayout = battlePageObj.transform.Find("Card Background/Card Layout");

        //BattlePage 컴포넌트에 참조 연결
        ConnectReferences(battlePage, playerArea, monsterArea, buttonArea, cardLayout);

        EditorUtility.SetDirty(battlePageObj);
        Debug.Log("[BattlePageSetup] Battle Page UI 설정 완료!");
    }

    private static GameObject CreatePlayerArea(Transform parent)
    {
        //Player Area (좌측 하단)
        GameObject playerArea = new GameObject("Player Area");
        playerArea.transform.SetParent(parent, false);
        playerArea.layer = LayerMask.NameToLayer("UI");

        RectTransform playerRect = playerArea.AddComponent<RectTransform>();
        playerRect.anchorMin = new Vector2(0, 0);
        playerRect.anchorMax = new Vector2(0, 0);
        playerRect.pivot = new Vector2(0, 0);
        playerRect.anchoredPosition = new Vector2(30, 280);
        playerRect.sizeDelta = new Vector2(250, 150);

        //배경 이미지
        Image bgImage = playerArea.AddComponent<Image>();
        bgImage.color = new Color(0, 0, 0, 0.5f);

        //Player HP
        GameObject hpObj = CreateTextObject("Player HP Text", playerArea.transform,
            new Vector2(0, 1), new Vector2(1, 1), new Vector2(0.5f, 1),
            new Vector2(0, -10), new Vector2(-20, 35), "HP: 50 / 50", 22);

        //Player Cost
        GameObject costObj = CreateTextObject("Player Cost Text", playerArea.transform,
            new Vector2(0, 1), new Vector2(1, 1), new Vector2(0.5f, 1),
            new Vector2(0, -50), new Vector2(-20, 30), "Cost: 3 / 3", 20);

        //Player Job
        GameObject jobObj = CreateTextObject("Player Job Text", playerArea.transform,
            new Vector2(0, 1), new Vector2(1, 1), new Vector2(0.5f, 1),
            new Vector2(0, -85), new Vector2(-20, 25), "초보자", 18);

        //Player Buff Container
        GameObject buffContainer = new GameObject("Player Buff Container");
        buffContainer.transform.SetParent(playerArea.transform, false);
        buffContainer.layer = LayerMask.NameToLayer("UI");

        RectTransform buffRect = buffContainer.AddComponent<RectTransform>();
        buffRect.anchorMin = new Vector2(0, 0);
        buffRect.anchorMax = new Vector2(1, 0);
        buffRect.pivot = new Vector2(0.5f, 0);
        buffRect.anchoredPosition = new Vector2(0, 10);
        buffRect.sizeDelta = new Vector2(-20, 40);

        HorizontalLayoutGroup buffLayout = buffContainer.AddComponent<HorizontalLayoutGroup>();
        buffLayout.spacing = 5;
        buffLayout.childAlignment = TextAnchor.MiddleLeft;
        buffLayout.childControlWidth = false;
        buffLayout.childControlHeight = false;

        return playerArea;
    }

    private static GameObject CreateMonsterArea(Transform parent)
    {
        //Monster Area (상단 중앙)
        GameObject monsterArea = new GameObject("Monster Area");
        monsterArea.transform.SetParent(parent, false);
        monsterArea.layer = LayerMask.NameToLayer("UI");

        RectTransform monsterRect = monsterArea.AddComponent<RectTransform>();
        monsterRect.anchorMin = new Vector2(0.5f, 1);
        monsterRect.anchorMax = new Vector2(0.5f, 1);
        monsterRect.pivot = new Vector2(0.5f, 1);
        monsterRect.anchoredPosition = new Vector2(0, -30);
        monsterRect.sizeDelta = new Vector2(400, 200);

        //배경 이미지
        Image bgImage = monsterArea.AddComponent<Image>();
        bgImage.color = new Color(0.3f, 0, 0, 0.5f);

        //Monster Name
        GameObject nameObj = CreateTextObject("Monster Name Text", monsterArea.transform,
            new Vector2(0, 1), new Vector2(1, 1), new Vector2(0.5f, 1),
            new Vector2(0, -10), new Vector2(-20, 35), "몬스터", 24);

        //Monster HP
        GameObject hpObj = CreateTextObject("Monster HP Text", monsterArea.transform,
            new Vector2(0, 1), new Vector2(1, 1), new Vector2(0.5f, 1),
            new Vector2(0, -50), new Vector2(-20, 30), "HP: 40 / 40", 20);

        //Monster Defense
        GameObject defenseObj = CreateTextObject("Monster Defense Text", monsterArea.transform,
            new Vector2(0, 1), new Vector2(1, 1), new Vector2(0.5f, 1),
            new Vector2(0, -85), new Vector2(-20, 25), "방어: 0", 18);

        //Monster Next Action
        GameObject actionObj = CreateTextObject("Monster Next Action Text", monsterArea.transform,
            new Vector2(0, 1), new Vector2(1, 1), new Vector2(0.5f, 1),
            new Vector2(0, -120), new Vector2(-20, 35), "다음 행동: 공격 10", 20);
        TextMeshProUGUI actionTmp = actionObj.GetComponent<TextMeshProUGUI>();
        actionTmp.color = Color.yellow;

        //Monster Buff Container
        GameObject buffContainer = new GameObject("Monster Buff Container");
        buffContainer.transform.SetParent(monsterArea.transform, false);
        buffContainer.layer = LayerMask.NameToLayer("UI");

        RectTransform buffRect = buffContainer.AddComponent<RectTransform>();
        buffRect.anchorMin = new Vector2(0, 0);
        buffRect.anchorMax = new Vector2(1, 0);
        buffRect.pivot = new Vector2(0.5f, 0);
        buffRect.anchoredPosition = new Vector2(0, 10);
        buffRect.sizeDelta = new Vector2(-20, 40);

        HorizontalLayoutGroup buffLayout = buffContainer.AddComponent<HorizontalLayoutGroup>();
        buffLayout.spacing = 5;
        buffLayout.childAlignment = TextAnchor.MiddleCenter;
        buffLayout.childControlWidth = false;
        buffLayout.childControlHeight = false;

        return monsterArea;
    }

    private static GameObject CreateButtonArea(Transform parent)
    {
        //Button Area (우측)
        GameObject buttonArea = new GameObject("Button Area");
        buttonArea.transform.SetParent(parent, false);
        buttonArea.layer = LayerMask.NameToLayer("UI");

        RectTransform buttonRect = buttonArea.AddComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(1, 0);
        buttonRect.anchorMax = new Vector2(1, 0);
        buttonRect.pivot = new Vector2(1, 0);
        buttonRect.anchoredPosition = new Vector2(-30, 280);
        buttonRect.sizeDelta = new Vector2(180, 150);

        //End Turn Button
        GameObject endTurnBtn = CreateButton("End Turn Button", buttonArea.transform,
            new Vector2(0, 1), new Vector2(1, 1), new Vector2(0.5f, 1),
            new Vector2(0, -10), new Vector2(0, 60), "턴 종료", 24,
            new Color(0.2f, 0.6f, 0.2f));

        //View Deck Button
        GameObject viewDeckBtn = CreateButton("View Deck Button", buttonArea.transform,
            new Vector2(0, 1), new Vector2(1, 1), new Vector2(0.5f, 1),
            new Vector2(0, -80), new Vector2(0, 50), "덱 보기", 20,
            new Color(0.3f, 0.3f, 0.6f));

        return buttonArea;
    }

    private static GameObject CreateTextObject(string name, Transform parent,
        Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot,
        Vector2 anchoredPos, Vector2 sizeDelta, string text, int fontSize)
    {
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(parent, false);
        textObj.layer = LayerMask.NameToLayer("UI");

        RectTransform rect = textObj.AddComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.pivot = pivot;
        rect.anchoredPosition = anchoredPos;
        rect.sizeDelta = sizeDelta;

        TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;

        return textObj;
    }

    private static GameObject CreateButton(string name, Transform parent,
        Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot,
        Vector2 anchoredPos, Vector2 sizeDelta, string text, int fontSize, Color bgColor)
    {
        GameObject btnObj = new GameObject(name);
        btnObj.transform.SetParent(parent, false);
        btnObj.layer = LayerMask.NameToLayer("UI");

        RectTransform rect = btnObj.AddComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.pivot = pivot;
        rect.anchoredPosition = anchoredPos;
        rect.sizeDelta = sizeDelta;

        Image btnImage = btnObj.AddComponent<Image>();
        btnImage.color = bgColor;

        Button btn = btnObj.AddComponent<Button>();
        btn.targetGraphic = btnImage;

        //버튼 텍스트
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(btnObj.transform, false);
        textObj.layer = LayerMask.NameToLayer("UI");

        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        textRect.anchoredPosition = Vector2.zero;

        TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;
        tmp.fontStyle = FontStyles.Bold;

        return btnObj;
    }

    private static void ConnectReferences(BattlePage battlePage,
        GameObject playerArea, GameObject monsterArea, GameObject buttonArea, Transform cardLayout)
    {
        SerializedObject so = new SerializedObject(battlePage);

        //플레이어 UI
        so.FindProperty("_playerHealthText").objectReferenceValue =
            playerArea.transform.Find("Player HP Text")?.GetComponent<TextMeshProUGUI>();
        so.FindProperty("_playerCostText").objectReferenceValue =
            playerArea.transform.Find("Player Cost Text")?.GetComponent<TextMeshProUGUI>();
        so.FindProperty("_playerJobText").objectReferenceValue =
            playerArea.transform.Find("Player Job Text")?.GetComponent<TextMeshProUGUI>();
        so.FindProperty("_playerBuffContainer").objectReferenceValue =
            playerArea.transform.Find("Player Buff Container");

        //몬스터 UI
        so.FindProperty("_monsterNameText").objectReferenceValue =
            monsterArea.transform.Find("Monster Name Text")?.GetComponent<TextMeshProUGUI>();
        so.FindProperty("_monsterHealthText").objectReferenceValue =
            monsterArea.transform.Find("Monster HP Text")?.GetComponent<TextMeshProUGUI>();
        so.FindProperty("_monsterDefenseText").objectReferenceValue =
            monsterArea.transform.Find("Monster Defense Text")?.GetComponent<TextMeshProUGUI>();
        so.FindProperty("_monsterNextActionText").objectReferenceValue =
            monsterArea.transform.Find("Monster Next Action Text")?.GetComponent<TextMeshProUGUI>();
        so.FindProperty("_monsterBuffContainer").objectReferenceValue =
            monsterArea.transform.Find("Monster Buff Container");

        //핸드 UI
        so.FindProperty("_handContainer").objectReferenceValue = cardLayout;

        //CardUI 프리팹 연결
        GameObject cardUIPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/UI/CardUI.prefab");
        so.FindProperty("_cardUIPrefab").objectReferenceValue = cardUIPrefab;

        //버튼
        so.FindProperty("_endTurnButton").objectReferenceValue =
            buttonArea.transform.Find("End Turn Button")?.GetComponent<Button>();
        so.FindProperty("_viewDeckButton").objectReferenceValue =
            buttonArea.transform.Find("View Deck Button")?.GetComponent<Button>();

        so.ApplyModifiedProperties();
        EditorUtility.SetDirty(battlePage);

        Debug.Log("[BattlePageSetup] BattlePage 참조 연결 완료!");
    }
}
