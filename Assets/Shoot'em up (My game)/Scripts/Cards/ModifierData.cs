using UnityEngine;

[System.Serializable]
public class ModifierData
{
    public StatAffected Stat;
    public ModifierOperation Operation;
    public float Value;
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