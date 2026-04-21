using UnityEngine;

public class GameMain : IInitializable
{
    private CardSelectionManager _cardManager;
    private Player _player;
    private PlayerMovement _playerMovement;
    private InputManager _inputManager;
    private GameOverUI _gameOverUI;
    private UIView _uiView;
    private EnemySpawner _enemySpawner;

    public void Init()
    {
        _cardManager = G.Get<CardSelectionManager>();
        _player = G.Get<Player>();
        _playerMovement = G.Get<PlayerMovement>();
        _inputManager = G.Get<InputManager>();
        _gameOverUI = G.Get<GameOverUI>();
        _uiView = G.Get<UIView>();
        _enemySpawner = G.Get<EnemySpawner>();

        _player.OnLevelUp += ShowCardSelection;
        _player.OnPlayerDied += GameOver;
        _cardManager.OnSelectionClosed += CloseSelection;
        _cardManager.OnCardApplied += ApplyEffect;
        _inputManager.OnGameRestarted += RestartGame;
    }

    public void StartGame()
    {
        _enemySpawner.StartSpawning();
        PlayerMovementEnable(true);
        _inputManager.RestartEnable(false);
    }

    private void ShowCardSelection()
    {
        Time.timeScale = 0;

        _cardManager.ShowCardSelection();
        PlayerMovementEnable(false);
    }

    private void CloseSelection()
    {
        Time.timeScale = 1;
        ClearScene();
        PlayerMovementEnable(true);
    }

    private void GameOver()
    {
        PlayerMovementEnable(false);

        _uiView.ShowUI(false);
        _enemySpawner.AllEnemiesUIVisible(false);
        _player.UIVIsible(false);

        _gameOverUI.ShowGameOver(_player.score, (int)Time.time);
        Time.timeScale = 0;

        _inputManager.RestartEnable(true);
    }

    private void RestartGame()
    {
        _inputManager.RestartEnable(false);
        Time.timeScale = 1;

        ClearScene();
        _enemySpawner.ClearAllEnemies();

        _gameOverUI.CloseGameOver();

        _uiView.ShowUI(true);

        _cardManager.ClearAllChooseLimits();
        _player.LoadBasicStats();
        PlayerMovementEnable(true);
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

    private void PlayerMovementEnable(bool enable)
    {
        _playerMovement.SetInputEnabled(enable);
        _inputManager.EnablePlayerInput(enable);
    }
}
