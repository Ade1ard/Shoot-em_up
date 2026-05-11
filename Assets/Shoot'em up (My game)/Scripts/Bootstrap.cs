using System.Linq;
using UnityEngine;

public class Bootstrap : MonoBehaviour
{
    void Awake()
    {
        G.Register<GameMain>(new GameMain());
        G.Register<InputManager>(FindFirstObjectByType<InputManager>());
        G.Register<EnemySpawner>(FindFirstObjectByType<EnemySpawner>());
        G.Register<CardSelectionManager>(FindFirstObjectByType<CardSelectionManager>());
        G.Register<Player>(FindFirstObjectByType<Player>());
        G.Register<PlayerMovement>(FindFirstObjectByType<PlayerMovement>());
        G.Register<UIView>(FindFirstObjectByType<UIView>());
        G.Register<ScoreUI>(FindFirstObjectByType<ScoreUI>());
        G.Register<GameOverUI>(FindAnyObjectByType<GameOverUI>());
        G.Register(FindFirstObjectByType<CameraShake>());

        G._player = FindFirstObjectByType<Player>();

        G._bottomBoundary = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0)).y;
        G._leftBoundary = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0)).x;

        InitAll();
    }

    void InitAll()
    {
        var Initializables = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<IInitializable>().ToList();

        G.Get<GameMain>().Init();
        foreach (var init in Initializables)
            init.Init();
    }

    private void Start()
    {
        G.Get<GameMain>().StartGame();
    }
}
