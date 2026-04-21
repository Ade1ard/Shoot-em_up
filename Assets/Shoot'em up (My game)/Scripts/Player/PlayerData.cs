using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Scriptable Objects/PlayerData")]
public class PlayerData : ScriptableObject
{
    public float maxHealth = 100;
    public int damage = 10;
    public float shootDelay = 1f;
    public int projectileCount = 1;
    public int levelXPCost = 100;
    public float levelMultiplier = 1.2f;
}
