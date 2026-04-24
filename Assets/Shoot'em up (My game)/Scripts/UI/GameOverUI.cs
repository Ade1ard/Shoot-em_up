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

    [Header("Animation")]
    [SerializeField] private AnimationCurve _showCurve;
    [SerializeField] private AnimationCurve _closeCurve;
    [SerializeField] private float _showDuration;

    private Vector3 _originalScale;
    private Vector3 _originalPosition;

    public void ShowGameOver(int scoreAmount, int runTime)
    {
        _gameOverScreen.transform.localScale = Vector3.zero;
        _gameOverScreen.gameObject.SetActive(true);
        _gameOverScreen.transform.DOScale(_originalScale, _showDuration).SetEase(_showCurve).SetUpdate(true);

        _time.text = $"{runTime / 60}:{(runTime % 60).ToString("D2")}";
        _scoreAmount.text = scoreAmount.ToString();
        _scoreBar.DOColor(GetRandomColor(), 1).SetUpdate(true);
    }

    public void CloseGameOver()
    {
        Vector3 pos = _originalPosition + new Vector3(0, 10, 0);
        _gameOverScreen.transform.DOMove(pos, _showDuration).SetEase(_closeCurve).SetUpdate(true).OnComplete(() => Reload());
    }

    private void Reload()
    {
        _gameOverScreen.gameObject.SetActive(false);
        _gameOverScreen.transform.position = _originalPosition;
    }

    private void Start()
    {
        _gameOverScreen.gameObject.SetActive(false);
        _originalScale = _gameOverScreen.transform.localScale;
        _originalPosition = _gameOverScreen.transform.position;
    }

    private Color GetRandomColor() { return Color.HSVToRGB(Random.Range(0, 1f), 0.47f, 1); }
}
