using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerModifiers
{
    private PlayerData _baseData;
    private PlayerRuntimeStats Stats;

    private List<ModifierData> _modifiers = new();

    public void Init(PlayerData baseData, PlayerRuntimeStats runtimeStats)
    {
        _baseData = baseData;
        Stats = runtimeStats;

        Recalculate();
    }

    public void AddModifier(ModifierData data)
    {
        _modifiers.Add(data);
        Recalculate();
    }

    public void RemoveModifier(ModifierData data)
    {
        _modifiers.Remove(data);
        Recalculate();
    }

    public void ClearAll()
    {
        _modifiers.Clear();
        Recalculate();
    }

    private void Recalculate()
    {
        Stats.MaxHP = Calculate(StatAffected.MaxHP, _baseData.maxHealth);
        Stats.Damage = Mathf.RoundToInt(Calculate(StatAffected.Damage, _baseData.damage));
        Stats.ShootDelay = Calculate(StatAffected.AttackSpeed, _baseData.shootDelay);
        Stats.ProjectileCountStep = Mathf.Clamp(Mathf.RoundToInt(Calculate(StatAffected.ProjectileCount, _baseData.projectileCountStep)), 1, 3);
    }

    private float Calculate(StatAffected stat, float baseValue)
    {
        var relevant = _modifiers
            .Where(m => m.Stat == stat)
            .OrderBy(m => m.Operation);

        float value = baseValue;
        float multiplier = 1f;

        foreach (var mod in relevant)
        {
            switch (mod.Operation)
            {
                case ModifierOperation.Add:
                    value += mod.Value;
                    break;
                case ModifierOperation.Multiply:
                    multiplier *= (1 + mod.Value / 100f);
                    break;
                case ModifierOperation.Override:
                    value = mod.Value;
                    break;
            }
        }

        return value * multiplier;
    }
}