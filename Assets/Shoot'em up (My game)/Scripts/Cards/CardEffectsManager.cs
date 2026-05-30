using System.Collections.Generic;
using UnityEngine;

public class CardEffectsManager  : IInitializable
{
    private List<CardEffect> _commonCards = new List<CardEffect>();
    private List<CardEffect> _epicCards = new List<CardEffect>();
    private List<CardEffect> _legendCards = new List<CardEffect>();

    private Player _player;

    public void Init()
    {
        _player = G.Get<Player>();

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

    public List<CardEffect> GetCards(int count, float epicChance, float legendChance)
    {
        List<CardEffect> result = new List<CardEffect>();
        List<CardEffect> pool = new List<CardEffect>();

        pool.AddRange(_commonCards);

        if (Random.Range(0f, 100f) < epicChance)
            pool.AddRange(_epicCards);

        if (Random.Range(0f, 100f) < legendChance)
            pool.AddRange(_legendCards);

        pool = FilterActivePatterns(pool);

        while (result.Count < count)
        {
            if (pool.Count == 0) break;

            CardEffect effect = pool[Random.Range(0, pool.Count)];

            if (effect._chooseCount < effect._chooselimit)
                result.Add(effect);

            pool.Remove(effect);
        }

        return result;
    }

    private List<CardEffect> FilterActivePatterns(List<CardEffect> pool)
    {
        if (_player?.CurrentWeaponConfig?.SpawnPattern == null)
            return pool;

        var activeType = _player.CurrentWeaponConfig.SpawnPattern.GetType();

        SpawnPatternType activePattern = SpawnPatternMap.Reverse[activeType];

        pool.RemoveAll(card =>
            card.effectType == EffectType.PJSpawnPattern &&
            card.spawnPatternType == activePattern
        );

        return pool;
    }

    public void ClearAllChooseLimits()
    {
        ClearLimitsInList(_commonCards);
        ClearLimitsInList(_epicCards);
        ClearLimitsInList(_legendCards);
    }

    private void ClearLimitsInList(List<CardEffect> cards)
    {
        foreach (CardEffect card in cards)
        {
            if (card._haveLimit)
                card._chooseCount = 0;
        }
    }
}
