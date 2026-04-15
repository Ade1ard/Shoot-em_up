using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class CardWidget : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image _backgroundCard;
    [SerializeField] private Image _cardOutline;
    [SerializeField] private Image _iconImage;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _descriptionText;
    [SerializeField] private Button _selectButton;
    [SerializeField] private EventTrigger _eventTrigger;

    [Header("Animation")]
    [SerializeField] private float _hoverScale = 1.1f;
    [SerializeField] private float _animationDuration = 0.2f;
    public float _showingDuration = 1f;

    [Header("Animaiton Curves")]
    public AnimationCurve _showCurve;
    public AnimationCurve _closeCurve;

    [Header("Colors")]
    [SerializeField] private Color _common;
    [SerializeField] private Color _epic;
    [SerializeField] private Color _legend;

    [Header("Sounds")]
    [SerializeField] private AudioClip _hoverEnterSound;
    [SerializeField] private AudioClip _hoverExitSound;
    [SerializeField] private AudioClip _clickSound;

    private CardEffect _cardData;
    private Action<CardEffect> _onSelected;
    [NonSerialized] public Vector3 _originalScale;
    private AudioSource _audioSource;

    private void Awake()
    {
        _originalScale = transform.localScale;
        _selectButton.onClick.AddListener(OnCardClicked);
        _eventTrigger.enabled = false;

        _audioSource = GetComponent<AudioSource>();
    }

    public System.Collections.IEnumerator Initialization(CardEffect effect, float delay, Action<CardEffect> callback)
    {
        transform.localScale = Vector3.zero;

        yield return new WaitForSecondsRealtime(delay);

        gameObject.SetActive(true);
        _cardData = effect;
        _onSelected = callback;

        if (_iconImage != null)
            _iconImage.sprite = effect.icon;

        _nameText.text = effect.effectName;
        _descriptionText.text = effect.description;

        SetRarityColor(effect.cost);

        yield return transform.DOScale(_originalScale, _showingDuration).SetEase(_showCurve).SetUpdate(true).WaitForCompletion();

        _eventTrigger.enabled = true;
    }

    public void Close()
    {
        _eventTrigger.enabled = false;
        transform.DORotate(new Vector3(0, 90, 0), 0.5f).SetEase(_closeCurve).SetUpdate(true);
        _backgroundCard.DOFade(0, 0.5f).SetEase(_closeCurve).SetUpdate(true);
    }

    private void SetRarityColor(int cost)
    {
        switch (cost)
        {
            case 1: // common
                _backgroundCard.color = _common;
                if (_cardOutline != null)
                    _cardOutline.color = _common.gamma;
                break;
            case 2: // epic
                _backgroundCard.color = _epic;
                if (_cardOutline != null)
                    _cardOutline.color = _epic.gamma;
                break;
            case 3: // legend
                _backgroundCard.color = _legend;
                if (_cardOutline != null)
                    _cardOutline.color = _legend.gamma;
                break;
        }
    }

    public void OnHoverEnter()
    {
        transform.DOScale(_originalScale * _hoverScale, _animationDuration).SetUpdate(true);
        if (_hoverEnterSound != null)
            _audioSource.PlayOneShot(_hoverEnterSound);
    }

    public void OnHoverExit()
    {
        transform.DOScale(_originalScale, _animationDuration).SetUpdate(true);
        if (_hoverExitSound != null)
            _audioSource.PlayOneShot(_hoverExitSound);
    }

    private void OnCardClicked()
    {
        _selectButton.interactable = false;

        if (_clickSound != null)
            _audioSource.PlayOneShot(_clickSound);

        if (_cardData.pickUpSound != null)
            AudioSource.PlayClipAtPoint(_cardData.pickUpSound, Camera.main.transform.position);

        _onSelected?.Invoke(_cardData);
    }
}
