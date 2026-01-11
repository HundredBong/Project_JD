using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameOverPage : UIPage
{
    [Header("UI 요소")]
    [SerializeField] private TextMeshProUGUI _resultText;
    [SerializeField] private TextMeshProUGUI _statsText;
    [SerializeField] private Button _retryButton;

    protected override void Awake()
    {
        base.Awake();

        if (_retryButton != null)
        {
            _retryButton.onClick.AddListener(OnRetryClicked);
        }
    }

    private void OnEnable()
    {
        UpdateStats();
    }

    private void UpdateStats()
    {
        if (GameManager.Instance == null)
        {
            return;
        }

        if (_resultText != null)
        {
            _resultText.text = "게임 오버";
        }

        if (_statsText != null)
        {
            int loop = GameManager.Instance.CurrentLoop;
            int stage = GameManager.Instance.CurrentStage;
            string jobName = GetJobName(GameManager.Instance.Player.CurrentJob);

            _statsText.text = $"루프: {loop}\n스테이지: {stage}\n직업: {jobName}";
        }
    }

    private void OnRetryClicked()
    {
        GameManager.Instance.StartNewRun();
        UIManager.Instance.PageOpen<BattlePage>();
    }

    private string GetJobName(JobType job)
    {
        switch (job)
        {
            case JobType.Warrior:
                return "전사";
            case JobType.Mage:
                return "법사";
            default:
                return "초보자";
        }
    }
}
