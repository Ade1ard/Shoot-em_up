using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class PlayerModifiers
{
    private PlayerData _baseData;
    private PlayerRuntimeStats Stats;

    private List<ModifierInstance> _modifiers = new();

    public void Init(PlayerData baseData, PlayerRuntimeStats runtimeStats)
    {
        _baseData = baseData;
        Stats = runtimeStats;

        Recalculate();
    }

    public void AddModifier(ModifierData data)
    {
        var instance = new ModifierInstance
        {
            Data = data,
            FinalValue = data.GetValue()
        };

        _modifiers.Add(instance);
        Recalculate();
    }

    public void RemoveModifier(ModifierData data)
    {
        _modifiers.RemoveAll(m => m.Data == data);
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

        Debug.Log($"Damage {Stats.Damage} : MaxHP {Stats.MaxHP} : CurrnetHP {Stats.CurrentHP} : ShootDelay {Stats.ShootDelay} : PJCount {Stats.ProjectileCountStep}");
    }

    private float Calculate(StatAffected stat, float baseValue)
    {
        var relevant = _modifiers
            .Where(m => m.Data.Stat == stat)
            .OrderBy(m => m.Data.Operation);

        float value = baseValue;
        float multiplier = 1f;

        foreach (var mod in relevant)
        {
            float modValue = mod.FinalValue;

            switch (mod.Data.Operation)
            {
                case ModifierOperation.Add:
                    value += modValue;
                    break;
                case ModifierOperation.Multiply:
                    multiplier *= (1 + modValue / 100f);
                    break;
                case ModifierOperation.Override:
                    value = modValue;
                    break;
            }
        }

        return value * multiplier;
    }
}