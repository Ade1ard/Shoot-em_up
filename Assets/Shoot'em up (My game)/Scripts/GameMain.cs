using UnityEngine;

public class GameMain : IInitializable
{
    private CardSelectionManager _cardManager;
    private Player _player;
    private GameOverUI _gameOverUI;
    private UIView _uiView;

    public void Init()
    {
        _cardManager = G.Get<CardSelectionManager>();
        _player = G.Get<Player>();
        _gameOverUI = G.Get<GameOverUI>();
        _uiView = G.Get<UIView>();

        _player.OnLevelUp += ShowCardSelection;
        _player.OnPlayerDied += GameOver;
        _cardManager.OnSelectionClosed += CloseSelection;
        _cardManager.OnCardApplied += ApplyEffect;
    }

    public void StartGame()
    {
        G.Get<EnemySpawner>().StartSpawning();
    }

    private void ShowCardSelection()
    {
        Time.timeScale = 0;

        _cardManager.ShowCardSelection();
    }

    private void CloseSelection()
    {
        Time.timeScale = 1;
        ClearScene();
    }

    private void GameOver()
    {
        _uiView.ShowUI(false);
        _gameOverUI.ShowGameOver(_player.score, (int)Time.time);
        Time.timeScale = 0;
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
