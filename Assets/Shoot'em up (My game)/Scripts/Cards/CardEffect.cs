using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCardEffect", menuName = "Scriptable Objects/Card Effect")]
public class CardEffect : ScriptableObject
{
    [Header("View")]
    public string effectName;
    public string description;
    public Sprite icon;

    [Header("Rarity")]
    public Rarity rarity;

    [Header("Effect type")]
    public EffectType effectType;

    [Header("Modifiers (if effect type is StatModificate)")]
    public List<ModifierData> modifiers = new List<ModifierData>();

    [Header("Spawn Pattern (if effect type is PJSpawnPattern)")]
    public SpawnPatternType spawnPatternType;

    [Header("Movement type (if effect type is PJMovementType)")]
    public MovementType movementType;

    [Header("EventSetup (if effect type is Event)")]
    public List<Player.EventSetup> events = new List<Player.EventSetup>();

    [Header("Visual effects")]
    public AudioClip pickUpSound;

    [Header("Limit")]
    public bool _haveLimit = false;
    public int _chooselimit = 3;
    [NonSerialized] public int _chooseCount = 0;
}

public enum EffectType
{
    StatModificate,
    Heal,
    PJSpawnPattern,
    PJMovementType,
    Event,
}

public enum SpawnPatternType
{
    line,
    Cross,
    Semicircle,
    PairByCircle
}

public enum MovementType
{
    Linear,
    Sine,
}

public static class SpawnPatternMap
{
    public static readonly Dictionary<SpawnPatternType, Type> Types = new()
    {
        { SpawnPatternType.line, typeof(SpawnLine) },
        { SpawnPatternType.Cross, typeof(SpawnCross) },
        { SpawnPatternType.Semicircle, typeof(SpawnSemicircle) },
        { SpawnPatternType.PairByCircle, typeof(SpawnPairByCircle) },
    };

    public static readonly Dictionary<Type, SpawnPatternType> Reverse = new()
    {
        { typeof(SpawnLine), SpawnPatternType.line },
        { typeof(SpawnCross), SpawnPatternType.Cross },
        { typeof(SpawnSemicircle), SpawnPatternType.Semicircle },
        { typeof(SpawnPairByCircle), SpawnPatternType.PairByCircle },
    };
}

public static class MovementTypeMap
{
    public static readonly Dictionary<MovementType, Type> Types = new()
    {
        { MovementType.Linear, typeof(LinearMove) },
        { MovementType.Sine, typeof(CurveLinearMove) },
    };

    public static readonly Dictionary<Type, MovementType> Reverse = new()
    {
        { typeof(LinearMove), MovementType.Linear },
        { typeof(CurveLinearMove), MovementType.Sine },
    };
}

public enum Rarity
{
    common,
    epic,
    legend,
    special,
}