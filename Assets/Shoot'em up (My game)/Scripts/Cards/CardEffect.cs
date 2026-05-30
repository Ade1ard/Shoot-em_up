using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCardEffect", menuName = "Scriptable Objects/Card Effect")]
public class CardEffect : ScriptableObject
{
    [Header("Settings")]
    public string effectName;
    public string description;
    public Sprite icon;
    public Rarity rarity;

    [Header("Effect type")]
    public EffectType effectType;

    [Header("Values")]
    public float baseValue = 10f;

    [Header("Spawn Pattern (if effect type is PJSpawnPattern)")]
    public SpawnPatternType spawnPatternType;

    [Header("Visual effects")]
    public AudioClip pickUpSound;

    [Header("Limit")]
    public bool _haveLimit = false;
    public int _chooselimit = 3;
    [NonSerialized] public int _chooseCount = 0;
}

public enum EffectType
{
    MaxHealth,
    CurrentHealth,
    Damage,
    AttackSpeed,
    ProjectileCount,
    PJSpawnPattern,
}

public enum SpawnPatternType
{
    line,
    Cross,
    Semicircle
}

public static class SpawnPatternMap
{
    public static readonly Dictionary<SpawnPatternType, Type> Types = new()
    {
        { SpawnPatternType.line, typeof(SpawnLine) },
        { SpawnPatternType.Cross, typeof(SpawnCross) },
        { SpawnPatternType.Semicircle, typeof(SpawnSemicircle) },
    };

    public static readonly Dictionary<Type, SpawnPatternType> Reverse = new()
    {
        { typeof(SpawnLine), SpawnPatternType.line },
        { typeof(SpawnCross), SpawnPatternType.Cross },
        { typeof(SpawnSemicircle), SpawnPatternType.Semicircle },
    };
}

public enum Rarity
{
    common,
    epic,
    legend,
}