using UnityEngine;

[CreateAssetMenu(fileName = "PlayerWeaponConfig", menuName = "Scriptable Objects/PlayerWeaponConfig")]
public class PlayerWeaponConfig : ScriptableObject
{
    [SerializeReference, SubclassSelector]
    public ISpawnFormation SpawnPattern;

    [Header("Settings")]
    public int MaxProjectileCount = 3;

    [SerializeReference, SubclassSelector]
    public IDirectionGenerator DirGenerator;
}