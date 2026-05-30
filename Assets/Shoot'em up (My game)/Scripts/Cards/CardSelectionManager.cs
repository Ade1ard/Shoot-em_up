using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardSelectionManager : MonoBehaviour, IInitializable
{
    [Header("Settings")]
    [SerializeField] private int _cardsToShow = 3;

    [Header("Prefabs")]
    [SerializeField] private CardWidget _cardPrefab;

    [Header("Parent objects")]
    [SerializeField] private Transform _cardsParent;
    [SerializeField] private GridLayoutGroup _gridLayout;

    [Header("Chance")]
    [SerializeField][Range(0, 100)] private float _epicChance = 30f;
    [SerializeField][Range(0, 100)] private float _legendChance = 10f;

    public event Action<CardEffect> OnCardApplied;
    public event Action OnSelectionClosed;

    private UIView _UIView;
    private CardEffectsManager _effectsManager;
    private List<CardWidget> _cards;

    public void Init()
    {
        _UIView = G.Get<UIView>();
        _effectsManager = G.Get<CardEffectsManager>();

        _gridLayout.constraintCount = _cardsToShow;
    }

    public void ShowCardSelection()
    {
        _UIView.ShowUI(false);
        ClearOldCards();

        List<CardEffect> selectedEffects = _effectsManager.GetCards(_cardsToShow, _epicChance, _legendChance);

        _cards = new List<CardWidget>();

        for (int i = 0; i < selectedEffects.Count; i++)
        {
            _cards.Add(CreateCard(selectedEffects[i], i));
        }
    }

    private CardWidget CreateCard(CardEffect effect, int index)
    {
        CardWidget card = Instantiate(_cardPrefab, _cardsParent);

        float delay = index * 0.5f;
        StartCoroutine(card.Initialization(effect, delay, OnCardSelected));
        return card;
    }

    private void OnCardSelected(CardEffect selectedEffect)
    {
        OnCardApplied?.Invoke(selectedEffect);

        StartCoroutine(CloseSelection());
    }

    private System.Collections.IEnumerator CloseSelection()
    {
        foreach (CardWidget card in _cards)
        {
            card.Close();
        }
        yield return new WaitForSecondsRealtime(0.5f);

        OnSelectionClosed?.Invoke();
        _UIView.ShowUI(true);
    }

    private void ClearOldCards()
    {
        foreach (Transform child in _cardsParent)
        {
            Destroy(child.gameObject);
        }
    }
}
