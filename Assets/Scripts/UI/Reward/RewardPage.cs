using UnityEngine;
using UnityEngine.UI;

public class RewardPage : UIPage
{
    [Header("버튼")]
    [SerializeField] private Button _addCardButton;
    [SerializeField] private Button _deleteCardButton;

    protected override void Awake()
    {
        base.Awake();

        if (_addCardButton != null)
        {
            _addCardButton.onClick.AddListener(OnAddCardClicked);
        }

        if (_deleteCardButton != null)
        {
            _deleteCardButton.onClick.AddListener(OnDeleteCardClicked);
        }
    }

    private void OnAddCardClicked()
    {
        GameManager.Instance.SelectRewardAddCard();
        UIManager.Instance.PageOpen<CardSelectPage>();
    }

    private void OnDeleteCardClicked()
    {
        GameManager.Instance.SelectRewardDeleteCard();
        UIManager.Instance.PageOpen<CardDeletePage>();
    }
}
