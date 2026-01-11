using UnityEngine;
using UnityEngine.UI;
using TMPro;

//표시 전용 카드 UI (클릭 불가)
public class CardDisplayUI : MonoBehaviour
{
    [Header("UI 요소")]
    [SerializeField] private TextMeshProUGUI _costText;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _typeText;
    [SerializeField] private TextMeshProUGUI _descriptionText;
    [SerializeField] private Image _backgroundImage;

    [Header("타입별 색상")]
    [SerializeField] private Color _neutralColor = Color.gray;
    [SerializeField] private Color _warriorColor = new Color(0.8f, 0.2f, 0.2f);
    [SerializeField] private Color _mageColor = new Color(0.2f, 0.4f, 0.8f);

    private CardData _cardData;

    public CardData CardData => _cardData;

    private void Awake()
    {
        AutoBindComponents();
    }

    private void AutoBindComponents()
    {
        if (_backgroundImage == null)
        {
            _backgroundImage = GetComponent<Image>();
        }

        if (_costText == null || _nameText == null || _typeText == null || _descriptionText == null)
        {
            TextMeshProUGUI[] texts = GetComponentsInChildren<TextMeshProUGUI>(true);
            foreach (TextMeshProUGUI text in texts)
            {
                string lowerName = text.gameObject.name.ToLower();
                if (lowerName.Contains("cost") && _costText == null)
                {
                    _costText = text;
                }
                else if (lowerName.Contains("name") && _nameText == null)
                {
                    _nameText = text;
                }
                else if (lowerName.Contains("type") && _typeText == null)
                {
                    _typeText = text;
                }
                else if (lowerName.Contains("desc") && _descriptionText == null)
                {
                    _descriptionText = text;
                }
            }
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
}
