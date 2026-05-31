using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Scriptable Objects/PlayerData")]
public class PlayerData : ScriptableObject
{
    [Header("Stats")]
    public float maxHealth = 100;
    public int damage = 10;
    public float shootDelay = 1f;
    public int projectileCountStep = 1;
    [Header("Leveling")]
    public int levelXPCost = 100;
    public float levelMultiplier = 1.2f;
    [Header("Weapon")]
    public PlayerWeaponConfig weaponConfig;
}