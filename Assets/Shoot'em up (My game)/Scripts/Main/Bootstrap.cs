using System.Linq;
using UnityEngine;

public class Bootstrap : MonoBehaviour
{
    void Awake()
    {
        G.Register<GameMain>(new GameMain());
        G.Register<ProjectilePool>(new ProjectilePool());
        G.Register<VFXPool>(new VFXPool());
        G.Register<CardEffectsManager>(new CardEffectsManager());
        G.Register<InputManager>(FindAnyObjectByType<InputManager>());
        G.Register<EnemySpawner>(FindAnyObjectByType<EnemySpawner>());
        G.Register<CardSelectionManager>(FindAnyObjectByType<CardSelectionManager>());
        G.Register<Player>(FindAnyObjectByType<Player>());
        G.Register<PlayerMovement>(FindAnyObjectByType<PlayerMovement>());
        G.Register<UIView>(FindAnyObjectByType<UIView>());
        G.Register<ScoreUI>(FindAnyObjectByType<ScoreUI>());
        G.Register<GameOverUI>(FindAnyObjectByType<GameOverUI>());
        G.Register(FindAnyObjectByType<CameraShake>());

        G._player = FindAnyObjectByType<Player>();
        G.VFXPool = G.Get<VFXPool>();

        G._bottomBoundary = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0)).y;
        G._leftBoundary = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0)).x;

        InitAll();
    }

    void InitAll()
    {
        var Initializables = FindObjectsByType<MonoBehaviour>().OfType<IInitializable>().ToList();

        G.Get<GameMain>().Init();
        G.Get<CardEffectsManager>().Init();

        foreach (var init in Initializables)
            init.Init();
    }

    private void Start()
    {
        G.Get<GameMain>().StartGame();
    }
}
