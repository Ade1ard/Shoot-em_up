using UnityEngine;

[CreateAssetMenu(fileName = "PlayerWeaponConfig", menuName = "Scriptable Objects/PlayerWeaponConfig")]
public class PlayerWeaponConfig : ScriptableObject
{
    [SerializeReference, SubclassSelector]
    public ISpawnFormation SpawnPattern;

    [Header("PJCountSteps")]
    public int Step1 = 1;
    public int Step2 = 2;
    public int Step3 = 3;

    [SerializeReference, SubclassSelector]
    public IDirectionGenerator DirGenerator;

    public float PJLifeTime;

    public int GetPJCount(int step)
    {
        return step switch
        {
            1 => Step1,
            2 => Step2,
            3 => Step3,
            _ => Step3,
        };
    }
}