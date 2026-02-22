using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardSelectionManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int cardsToShow = 3;

    [Header("Prefabs")]
    [SerializeField] private CardWidget cardPrefab;
    [SerializeField] private GameObject selectionPanel;

    [Header("Parent objects")]
    [SerializeField] private Transform cardsParent;
    [SerializeField] private GridLayoutGroup gridLayout;

    [Header("Cards pool")]
    [SerializeField] private List<CardEffect> _commonCards;
    [SerializeField] private List<CardEffect> _epicCards;
    [SerializeField] private List<CardEffect> _legendCards;

    [Header("Chance")]
    [SerializeField][Range(0, 100)] private float _epicChance = 30f;
    [SerializeField][Range(0, 100)] private float _legendChance = 10f;

    private PlayerStats playerStats;

    private void Start()
    {
        playerStats = Object.FindAnyObjectByType<PlayerStats>();

        selectionPanel.SetActive(false);
        selectionPanel.transform.localScale = new Vector3(0, 0, 0);
    }

    public void ShowCardSelection()
    {
        ClearOldCards();

        List<CardEffect> selectedEffects = GenerateCards(cardsToShow);

        selectionPanel.transform.localScale = new Vector3(1, 1, 1);
        selectionPanel.SetActive(true);

        for (int i = 0; i < selectedEffects.Count; i++)
        {
            CreateCard(selectedEffects[i], i);
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

    private void CreateCard(CardEffect effect, int index)
    {
        CardWidget card = Instantiate(cardPrefab, cardsParent);

        float delay = index * 0.1f;
        StartCoroutine(ShowCardWithDelay(card, effect, delay));
    }

    private System.Collections.IEnumerator ShowCardWithDelay(CardWidget card, CardEffect effect, float delay)
    {
        card.gameObject.SetActive(false);
        card.transform.localScale = new Vector3(0,0,0);

        yield return new WaitForSeconds(delay);

        card.transform.DOScale(new Vector3(1, 1, 1), card._showingDuration);
        card.gameObject.SetActive(true);

        card.Initialize(effect, OnCardSelected);
    }

    private void OnCardSelected(CardEffect selectedEffect)
    {
        ApplyEffect(selectedEffect);

        StartCoroutine(CloseSelection());
    }

    private void ApplyEffect(CardEffect effect)
    {
        if (playerStats == null) return;

        switch (effect.effectType)
        {
            case EffectType.MaxHealth:
                playerStats._maxHealth += (int)effect.baseValue;
                playerStats._currentHealth += (int)effect.baseValue;
                break;

            case EffectType.Damage:
                playerStats.damage += (int)effect.baseValue;
                break;

            case EffectType.AttackSpeed:
                playerStats.attackSpeed *= effect.baseValue;
                break;

            case EffectType.ProjectileCount:
                playerStats.projectileCount++;
                break;

            case EffectType.SpecialAbility:
                //playerStats.unlockSpecialAbility = true;
                break;
        }

        if (effect.spawnVFX != null)
        {
            Instantiate(effect.spawnVFX, playerStats.transform.position, Quaternion.identity);
        }
    }

    private System.Collections.IEnumerator CloseSelection()
    {
        yield return new WaitForSeconds(0.5f);

        selectionPanel.transform.DOScale(new Vector3(0, 0, 0), 1);
        selectionPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    private void ClearOldCards()
    {
        foreach (Transform child in cardsParent)
        {
            Destroy(child.gameObject);
        }
    }
}
