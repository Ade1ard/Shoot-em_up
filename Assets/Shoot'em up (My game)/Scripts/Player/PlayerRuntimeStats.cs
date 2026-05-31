[System.Serializable]
public class PlayerRuntimeStats
{
    public float MaxHP;
    public float CurrentHP;
    public int Damage;
    public float ShootDelay;
    public int ProjectileCountStep;
    public int Level = 1;
    public int LevelXPCost;
    public int XP;
    public int Score;

    public void LoadFromData(PlayerData data)
    {
        MaxHP = data.maxHealth;
        CurrentHP = data.maxHealth;
        Damage = data.damage;
        ShootDelay = data.shootDelay;
        ProjectileCountStep = data.projectileCountStep;
        LevelXPCost = data.levelXPCost;
        Level = 1;
        XP = 0;
        Score = 0;
    }
}