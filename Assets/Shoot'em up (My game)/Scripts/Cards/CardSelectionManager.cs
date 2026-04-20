using System;
using System.Collections.Generic;
using System.Linq;
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

    private List<CardEffect> _commonCards = new List<CardEffect>();
    private List<CardEffect> _epicCards = new List<CardEffect>();
    private List<CardEffect> _legendCards = new List<CardEffect>();

    public event Action<CardEffect> OnCardApplied;
    public event Action OnSelectionClosed;

    private UIView _UIView;
    private List<CardWidget> _cards;

    public void Init()
    {
        _UIView = G.Get<UIView>();

        _gridLayout.constraintCount = _cardsToShow;
        LoadCardEffects();
    }

    private void LoadCardEffects()
    {
        foreach (CardEffect effect in Resources.LoadAll<CardEffect>("CardEffects"))
        {
            switch (effect.rarity)
            {
                case Rarity.common:
                    _commonCards.Add(effect);
                    break;
                case Rarity.epic:
                    _epicCards.Add(effect);
                    break;
                case Rarity.legend:
                    _legendCards.Add(effect);
                    break;
            }
        }
    }

    public void ShowCardSelection()
    {
        _UIView.ShowUI(false);
        ClearOldCards();

        List<CardEffect> selectedEffects = GenerateCards(_cardsToShow);

        _cards = new List<CardWidget>();

        for (int i = 0; i < selectedEffects.Count; i++)
        {
            _cards.Add(CreateCard(selectedEffects[i], i));
        }
    }

    private List<CardEffect> GenerateCards(int count)
    {
        List<CardEffect> result = new List<CardEffect>();
        List<CardEffect> pool = new List<CardEffect>();

        pool.AddRange(_commonCards);

        if (UnityEngine.Random.Range(0f, 100f) < _epicChance)
            pool.AddRange(_epicCards);

        if (UnityEngine.Random.Range(0f, 100f) < _legendChance)
            pool.AddRange(_legendCards);

        while (result.Count < count)
        {
            if (pool.Count == 0) break;

            CardEffect effect = pool[UnityEngine.Random.Range(0, pool.Count)];
            
            if (effect._chooseCount < effect._chooselimit)
                result.Add(effect);

            pool.Remove(effect);
        }

        return result;
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
