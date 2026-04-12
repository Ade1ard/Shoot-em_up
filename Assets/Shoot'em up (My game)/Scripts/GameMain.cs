using UnityEngine;

public class GameMain
{
    public void StartGame()
    {
        G.Get<EnemySpawner>().StartSpawning();
    }
}
