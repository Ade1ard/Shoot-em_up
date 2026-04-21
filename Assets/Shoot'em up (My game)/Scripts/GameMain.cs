using System.Collections;
using UnityEngine;

public enum GameState
{
    Running,
    CardSelection,
    GameOver,
}

public class GameMain : IInitializable
{
    private CardSelectionManager _cardManager;
    private Player _player;
    private PlayerMovement _playerMovement;
    private InputManager _inputManager;
    private GameOverUI _gameOverUI;
    private UIView _uiView;
    private ScoreUI _scoreUI;
    private EnemySpawner _enemySpawner;

    private GameState _currentState;

    private float _runStartTime;
    private bool _isRunning;
    private float _runTime => _isRunning? Time.time - _runStartTime : 0f;

    private WaitForSeconds _wait = new WaitForSeconds(0.1f);

    public void Init()
    {
        _cardManager = G.Get<CardSelectionManager>();
        _player = G.Get<Player>();
        _playerMovement = G.Get<PlayerMovement>();
        _inputManager = G.Get<InputManager>();
        _gameOverUI = G.Get<GameOverUI>();
        _uiView = G.Get<UIView>();
        _scoreUI = G.Get<ScoreUI>();
        _enemySpawner = G.Get<EnemySpawner>();

        _player.OnLevelUp += ShowCardSelection;
        _player.OnPlayerDied += GameOver;
        _player.OnXPChanged += UpdateXP;
        _player.OnScoreChanged += UpdateScore;
        _cardManager.OnSelectionClosed += CloseSelection;
        _cardManager.OnCardApplied += ApplyEffect;
        _inputManager.OnGameRestarted += RestartGame;
    }

    public void SetGameState(GameState newState)
    {
        if (_currentState == newState) return;

        ExitState(_currentState);
        _currentState = newState;
        EnterState(newState);
    }

    private void EnterState(GameState state)
    {
        switch (state)
        {
            case GameState.Running:
                Time.timeScale = 1;
                PlayerMovementEnable(true);
                _uiView.ShowUI(true);
                _enemySpawner.AllEnemiesUIVisible(true);
                _player.UIVisible(true);

                break;

            case GameState.CardSelection:
                _cardManager.ShowCardSelection();

                break;

            case GameState.GameOver:
                _gameOverUI.ShowGameOver(_player.score, (int)_runTime);
                _inputManager.RestartEnable(true);

                break;
        }
    }

    private void ExitState(GameState state)
    {
        switch (state)
        {
            case GameState.Running:
                Time.timeScale = 0;
                PlayerMovementEnable(false);

                _uiView.ShowUI(false);
                _enemySpawner.AllEnemiesUIVisible(false);
                _player.UIVisible(false);

                break;

            case GameState.CardSelection:
                ClearScene();
                break;

            case GameState.GameOver:

                break;
        }
    }

    public void StartGame()
    {
        _runStartTime = Time.time;
        _isRunning = true;

        _uiView.StartCoroutine(RunTime());
        _enemySpawner.StartSpawning();

        EnterState(GameState.Running);
    }

    private void ShowCardSelection() => SetGameState(GameState.CardSelection);

    private void CloseSelection() => SetGameState(GameState.Running);

    private void GameOver() => SetGameState(GameState.GameOver);

    private void RestartGame()
    {
        _runStartTime = Time.time;
        _inputManager.RestartEnable(false);

        _cardManager.ClearAllChooseLimits();
        _player.LoadBasicStats();

        _gameOverUI.CloseGameOver();
        _uiView.UpdateXP(0, 60);

        ClearScene();
        _enemySpawner.ClearAllEnemies();
        _enemySpawner.SetBasicDifficulty();

        SetGameState(GameState.Running);
    }



    private void UpdateXP(int exp, int levelCost) => _uiView.UpdateXP(exp, levelCost);

    private void UpdateScore(int score, int amount) => _scoreUI.UpdateScoreAmount(score, amount);

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

    private IEnumerator RunTime()
    {
        while (_isRunning)
        {
            _uiView.ShowTime((int)_runTime);
            yield return _wait;
        }
    }
}