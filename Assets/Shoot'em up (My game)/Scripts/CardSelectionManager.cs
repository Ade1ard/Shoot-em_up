using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardSelectionManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int _cardsToShow = 3;

    [Header("Prefabs")]
    [SerializeField] private CardWidget _cardPrefab;
    [SerializeField] private GameObject _selectionPanel;

    [Header("Parent objects")]
    [SerializeField] private Transform _cardsParent;
    [SerializeField] private GridLayoutGroup _gridLayout;

    [Header("Cards pool")]
    [SerializeField] private List<CardEffect> _commonCards;
    [SerializeField] private List<CardEffect> _epicCards;
    [SerializeField] private List<CardEffect> _legendCards;

    [Header("Chance")]
    [SerializeField][Range(0, 100)] private float _epicChance = 30f;
    [SerializeField][Range(0, 100)] private float _legendChance = 10f;

    private PlayerStats _playerStats;

    private void Start()
    {
        _playerStats = FindAnyObjectByType<PlayerStats>();

        _gridLayout.constraintCount = _cardsToShow;

        _selectionPanel.SetActive(false);
        _selectionPanel.transform.localScale = new Vector3(0, 0, 0);
    }

    public void ShowCardSelection()
    {
        ClearOldCards();

        List<CardEffect> selectedEffects = GenerateCards(_cardsToShow);
        _selectionPanel.transform.localScale = new Vector3(1, 1, 1);
        _selectionPanel.SetActive(true);

        for (int i = 0; i < selectedEffects.Count; i++)
        {
            CreateCard(selectedEffects[i], i);
        }

        Time.timeScale = 1f;
    }

    private List<CardEffect> GenerateCards(int count)
    {
        List<CardEffect> result = new List<CardEffect>();
        List<CardEffect> pool = new List<CardEffect>();

        pool.AddRange(_commonCards);

        if (Random.Range(0f, 100f) < _epicChance)
            pool.AddRange(_epicCards);

        if (Random.Range(0f, 100f) < _legendChance)
            pool.AddRange(_legendCards);

        for (int i = 0; i < count; i++)
        {
            if (pool.Count == 0) break;

            int randomIndex = Random.Range(0, pool.Count);
            result.Add(pool[randomIndex]);
            pool.RemoveAt(randomIndex);
        }

        return result;
    }

    private void CreateCard(CardEffect effect, int index)
    {
        CardWidget card = Instantiate(_cardPrefab, _cardsParent);

        float delay = index * 1f;
        StartCoroutine(ShowCardWithDelay(card, effect, delay));
    }

    private System.Collections.IEnumerator ShowCardWithDelay(CardWidget card, CardEffect effect, float delay)
    {
        card.transform.localScale = new Vector3(0,0,0);

        yield return new WaitForSeconds(delay);

        card.gameObject.SetActive(true);
        card.transform.DOScale(card._originalScale, card._showingDuration);

        card.Initialize(effect, OnCardSelected);
    }

    private void OnCardSelected(CardEffect selectedEffect)
    {
        ApplyEffect(selectedEffect);

        StartCoroutine(CloseSelection());
    }

    private void ApplyEffect(CardEffect effect)
    {
        if (_playerStats == null) return;

        switch (effect.effectType)
        {
            case EffectType.MaxHealth:
                _playerStats._maxHealth += (int)effect.baseValue;
                _playerStats._currentHealth += (int)effect.baseValue;
                break;

            case EffectType.Damage:
                _playerStats.damage += (int)effect.baseValue;
                break;

            case EffectType.AttackSpeed:
                _playerStats.attackSpeed *= effect.baseValue;
                break;

            case EffectType.ProjectileCount:
                _playerStats.projectileCount++;
                break;

            case EffectType.SpecialAbility:
                //playerStats.unlockSpecialAbility = true;
                break;
        }

        if (effect.spawnVFX != null)
        {
            Instantiate(effect.spawnVFX, _playerStats.transform.position, Quaternion.identity);
        }
    }

    private System.Collections.IEnumerator CloseSelection()
    {
        yield return _selectionPanel.transform.DOScale(new Vector3(0, 0, 0), 0.5f).WaitForCompletion();
        _selectionPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    private void ClearOldCards()
    {
        foreach (Transform child in _cardsParent)
        {
            Destroy(child.gameObject);
        }
    }
}
