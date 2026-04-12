using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCardEffect", menuName = "Scriptable Objects/Card Effect")]
public class CardEffect : ScriptableObject
{
    [Header("Settings")]
    public string effectName;
    public string description;
    public Sprite icon;
    public int cost = 1;

    [Header("Effect type")]
    public EffectType effectType;

    [Header("Values")]
    public float baseValue = 10f;
    public float multiplierPerLevel = 1f;

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
    SpecialAbility
}
