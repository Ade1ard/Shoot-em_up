using UnityEngine;

[System.Serializable]
public class ModifierData
{
    [Header("Settings")]
    public StatAffected Stat;
    public ModifierOperation Operation;
    [SerializeField] float Value;

    [Header("Randomness")]
    public bool RandomModifier;
    [SerializeField] float minValue;
    [SerializeField] float maxValue;

    public float GetValue()
    {
        if(!RandomModifier)
            return Value;

        return Random.Range(minValue, maxValue);
    }
}

public class ModifierInstance
{
    public ModifierData Data;
    public float FinalValue;
}

public enum StatAffected
{
    MaxHP,
    Damage,
    AttackSpeed,
    ProjectileCount,
}

public enum ModifierOperation
{
    Add,
    Multiply,
    Override,
}