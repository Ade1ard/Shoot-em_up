using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardWidget : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private SpriteRenderer _backgroundCard;
    [SerializeField] private Image _iconImage;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _descriptionText;
    [SerializeField] private Button _selectButton;

    [Header("Animation")]
    [SerializeField] private float _hoverScale = 1.1f;
    [SerializeField] private float _animationDuration = 0.2f;
    public float _showingDuration = 1f;

    [Header("Colors")]
    [SerializeField] private Color _common;
    [SerializeField] private Color _epic;
    [SerializeField] private Color _legend;

    private CardEffect _cardData;
    private Action<CardEffect> _onSelected;
    [NonSerialized] public Vector3 _originalScale;

    private void Awake()
    {
        _originalScale = transform.localScale;
        _selectButton.onClick.AddListener(OnCardClicked);
    }

    public void Initialize(CardEffect effect, Action<CardEffect> callback)
    {
        _cardData = effect;
        _onSelected = callback;

        if (_iconImage != null )
            _iconImage.sprite = effect.icon;

        _nameText.text = effect.effectName;
        _descriptionText.text = effect.description;

        SetRarityColor(effect.cost);
    }

    private void SetRarityColor(int cost)
    {
        switch (cost)
        {
            case 1: // common
                _backgroundCard.color = _common;
                break;
            case 2: // epic
                _backgroundCard.color = _epic;
                break;
            case 3: // legend
                _backgroundCard.color = _legend;
                break;
        }
    }

    public void OnHoverEnter()
    {
        transform.DOScale(_originalScale * _hoverScale, _animationDuration);
    }

    public void OnHoverExit()
    {
        transform.DOScale(_originalScale, _animationDuration);
    }

    private void OnCardClicked()
    {
        _selectButton.interactable = false;

        if (_cardData.pickUpSound != null)
            AudioSource.PlayClipAtPoint(_cardData.pickUpSound, Camera.main.transform.position);

        _onSelected?.Invoke(_cardData);
    }
}
