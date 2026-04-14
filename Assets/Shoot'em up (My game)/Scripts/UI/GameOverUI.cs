using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup _gameOverScreen;
    [SerializeField] private TextMeshProUGUI _gameOverText;
    [SerializeField] private TextMeshProUGUI _time;
    [SerializeField] private TextMeshProUGUI _scoreAmount;
    [SerializeField] private Image _scoreBar;

    public void ShowGameOver(int scoreAmount, int runTime)
    {
        _gameOverScreen.gameObject.SetActive(true);

        _time.text = $"{runTime / 60}:{(runTime % 60).ToString("D2")}";
        _scoreAmount.text = scoreAmount.ToString();
        _scoreBar.DOColor(GetRandomColor(), 1);
    }

    private void Start()
    {
        _gameOverScreen.alpha = 0f;
        _gameOverScreen.gameObject.SetActive(false);
    }

    private Color GetRandomColor() { return Color.HSVToRGB(Random.Range(0, 1f), 0.47f, 1); }
}
