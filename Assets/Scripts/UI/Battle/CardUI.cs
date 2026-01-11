using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardUI : MonoBehaviour
{
    [Header("UI 요소")]
    [SerializeField] private TextMeshProUGUI _costText;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _typeText;
    [SerializeField] private TextMeshProUGUI _descriptionText;
    [SerializeField] private Image _backgroundImage;
    [SerializeField] private Button _button;

    [Header("타입별 색상")]
    [SerializeField] private Color _neutralColor = Color.gray;
    [SerializeField] private Color _warriorColor = new Color(0.8f, 0.2f, 0.2f);
    [SerializeField] private Color _mageColor = new Color(0.2f, 0.4f, 0.8f);

    private CardData _cardData;
    private bool _isInteractable = true;

    public CardData CardData => _cardData;
    public event Action<CardUI> OnCardClicked;

    private void Awake()
    {
        if (_button != null)
        {
            _button.onClick.AddListener(HandleClick);
        }
    }

    public void Setup(CardData cardData)
    {
        _cardData = cardData;

        if (_costText != null)
        {
            _costText.text = cardData.cost.ToString();
        }

        if (_nameText != null)
        {
            _nameText.text = cardData.cardName;
        }

        if (_typeText != null)
        {
            _typeText.text = GetTypeName(cardData.cardType);
        }

        if (_descriptionText != null)
        {
            _descriptionText.text = cardData.effectDescription;
        }

        UpdateBackgroundColor();
        UpdateInteractableState();
    }

    private void UpdateBackgroundColor()
    {
        if (_backgroundImage == null || _cardData == null)
        {
            return;
        }

        switch (_cardData.cardType)
        {
            case CardType.Warrior:
                _backgroundImage.color = _warriorColor;
                break;
            case CardType.Mage:
                _backgroundImage.color = _mageColor;
                break;
            default:
                _backgroundImage.color = _neutralColor;
                break;
        }
    }

    public void SetInteractable(bool interactable)
    {
        _isInteractable = interactable;
        UpdateInteractableState();
    }

    private void UpdateInteractableState()
    {
        if (_button == null)
        {
            return;
        }

        bool canAfford = true;
        if (_cardData != null && GameManager.Instance != null && GameManager.Instance.Player != null)
        {
            canAfford = GameManager.Instance.Player.CurrentCost >= _cardData.cost;
        }

        _button.interactable = _isInteractable && canAfford;
    }

    private void HandleClick()
    {
        OnCardClicked?.Invoke(this);
    }

    private string GetTypeName(CardType type)
    {
        switch (type)
        {
            case CardType.Warrior:
                return "전사";
            case CardType.Mage:
                return "법사";
            default:
                return "중립";
        }
    }

    private void OnDestroy()
    {
        if (_button != null)
        {
            _button.onClick.RemoveListener(HandleClick);
        }
    }
}
