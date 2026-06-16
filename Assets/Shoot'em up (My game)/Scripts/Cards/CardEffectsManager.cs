using System.Collections.Generic;
using UnityEngine;

public class CardEffectsManager  : IInitializable
{
    private List<CardEffect> _commonCards = new List<CardEffect>();
    private List<CardEffect> _epicCards = new List<CardEffect>();
    private List<CardEffect> _legendCards = new List<CardEffect>();
    private List<CardEffect> _speacialCards = new List<CardEffect>();

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
            if(!effect.avaible) continue;

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
                case Rarity.special:
                    _speacialCards.Add(effect);
                    break;
            }
        }
    }

    public List<CardEffect> GetCards(int count, float epicChance, float legendChance, float specialChance)
    {
        List<CardEffect> result = new List<CardEffect>();
        List<CardEffect> pool = new List<CardEffect>();

        pool.AddRange(_commonCards);

        if (Random.Range(0f, 100f) <= epicChance)
            pool.AddRange(_epicCards);

        if (Random.Range(0f, 100f) <= legendChance)
            pool.AddRange(_legendCards);

        if (Random.Range(0f, 100f) <= specialChance)
            pool.AddRange(_speacialCards);

        pool = FilterActivePatterns(pool);
        pool = FilterUnneeded(pool);

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
        SpawnPatternType? activePattern = null;
        MovementType? activeMovement = null;

        if (_player?.CurrentWeaponConfig?.SpawnPattern != null)
        {
            var activeType = _player.CurrentWeaponConfig.SpawnPattern.GetType();
            activePattern = SpawnPatternMap.Reverse[activeType];
        }

        if (_player?.CurrentPJMoveConfig?.MovementType != null)
        {
            var activePJType = _player.CurrentPJMoveConfig.MovementType.GetType();
            activeMovement = MovementTypeMap.Reverse[activePJType];
        }

        pool.RemoveAll(card =>
            (card.effectType == EffectType.PJSpawnPattern && activePattern.HasValue && card.spawnPatternType == activePattern.Value) ||
            (card.effectType == EffectType.PJMovementType && activeMovement.HasValue && card.movementType == activeMovement.Value)
        );

        return pool;
    }

    private List<CardEffect> FilterUnneeded(List<CardEffect> pool)
    {
        if (_player.Stats.CurrentHP == _player.Stats.MaxHP)
        {
            pool.RemoveAll(card => card.effectType == EffectType.Heal);
        }

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
