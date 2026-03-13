using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardSelectionManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int _cardsToShow = 3;

    [Header("Prefabs")]
    [SerializeField] private CardWidget _cardPrefab;

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
    private List<CardWidget> _cards;

    private void Start()
    {
        _playerStats = FindAnyObjectByType<PlayerStats>();

        _gridLayout.constraintCount = _cardsToShow;
    }

    public void ShowCardSelection()
    {
        ClearOldCards();

        List<CardEffect> selectedEffects = GenerateCards(_cardsToShow);

        _cards = new List<CardWidget>();

        for (int i = 0; i < selectedEffects.Count; i++)
        {
            _cards.Add(CreateCard(selectedEffects[i], i));
        }

        Time.timeScale = 0f;
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

    private CardWidget CreateCard(CardEffect effect, int index)
    {
        CardWidget card = Instantiate(_cardPrefab, _cardsParent);

        float delay = index * 0.5f;
        StartCoroutine(card.Initialization(effect, delay, OnCardSelected));
        return card;
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
                _playerStats.shootDelay -= effect.baseValue;
                _playerStats.shootDelay = Mathf.Clamp(_playerStats.shootDelay, 0.17f, 1);
                break;

            case EffectType.ProjectileCount:
                _playerStats.projectileCount += 1;
                _playerStats.projectileCount = Mathf.Clamp(_playerStats.projectileCount, 1, 3);
                _playerStats.UpdateProjectileCount();
                break;

            case EffectType.SpecialAbility:
                //playerStats.unlockSpecialAbility = true;
                break;
        }
        _playerStats.GiveStats();

        if (effect.spawnVFX != null)
        {
            Instantiate(effect.spawnVFX, _playerStats.transform.position, Quaternion.identity);
        }
    }

    private System.Collections.IEnumerator CloseSelection()
    {
        foreach (CardWidget card in _cards)
        {
            card.Close();
        }
        yield return new WaitForSecondsRealtime(0.5f);
        Time.timeScale = 1f;
        ClearScene();
    }

    private void ClearOldCards()
    {
        foreach (Transform child in _cardsParent)
        {
            Destroy(child.gameObject);
        }
    }

    private void ClearScene()
    {
        foreach (ProjectileCont prj in Object.FindObjectsByType<ProjectileCont>(FindObjectsSortMode.None))
             prj.Clear();
    }
}
