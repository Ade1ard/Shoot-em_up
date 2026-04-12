using UnityEngine;

public class GameMain : IInitializable
{
    private CardSelectionManager _cardManager;
    private Player _player;

    public void Init()
    {
        _cardManager = G.Get<CardSelectionManager>();
        _player = G.Get<Player>();

        _player.OnLevelUp += ShowCardSelection;
        _cardManager.OnSelectionClosed += CloseSelection;
        _cardManager.OnCardApplied += ApplyEffect;
    }

    public void StartGame()
    {
        G.Get<EnemySpawner>().StartSpawning();
    }

    private void ShowCardSelection()
    {
        Time.timeScale = 0f;

        _cardManager.ShowCardSelection();
    }

    private void CloseSelection()
    {
        Time.timeScale = 1f;
        ClearScene();
    }

    private void ApplyEffect(CardEffect effect)
    {
        if (_player == null) return;

        switch (effect.effectType)
        {
            case EffectType.MaxHealth:
                _player.AddMaxHP((int)effect.baseValue);
                break;

            case EffectType.Damage:
                _player.AddDamage((int)effect.baseValue);
                break;

            case EffectType.AttackSpeed:
                _player.AddAttackSpeed(effect.baseValue);
                break;

            case EffectType.ProjectileCount:
                _player.AddPrjcCount();
                break;

            case EffectType.SpecialAbility:
                //playerStats.unlockSpecialAbility = true;
                break;
        }
        if (effect._haveLimit)
            effect._chooseCount++;

        _player.UpdateStats();
    }

    private void ClearScene()
    {
        foreach (ProjectileCont prj in Object.FindObjectsByType<ProjectileCont>(FindObjectsSortMode.None))
            prj.Clear();
    }
}
