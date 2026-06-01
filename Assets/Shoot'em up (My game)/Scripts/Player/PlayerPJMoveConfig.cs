using UnityEngine;

[CreateAssetMenu(fileName = "PlayerPJMovementType", menuName = "Scriptable Objects/PlayerPJMovementType")]
public class PlayerPJMoveConfig : ScriptableObject
{
    [SerializeReference, SubclassSelector]
    public IMovementType MovementType;
}
