using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI _scoreAmountText;
    [SerializeField] private Image _scoreBar;
    [SerializeField] private CanvasGroup _scoreCanvasGroup;

    private Coroutine _coroutine;
    private Sequence _animaton;

    private readonly WaitForSeconds _waitShort = new WaitForSeconds(0.005f);
    private readonly WaitForSeconds _wait = new WaitForSeconds(2);

    public void UpdateScoreAmount(int score, int amount)
    {
        _scoreAmountText.text = amount.ToString();
        StopScoreShowing();
        _coroutine = StartCoroutine(ScoreAnimation(score, amount));
    }

    public void StopScoreShowing()
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            if (_animaton.IsActive())
                _animaton.Kill(true);
            DOTween.Kill(_scoreCanvasGroup);
        }
    }

    private IEnumerator ScoreAnimation(int score, int amount)
    {
        _animaton = DOTween.Sequence();
        _animaton
            .Append(_scoreCanvasGroup.DOFade(1, 0.4f))
            .Join(_scoreCanvasGroup.transform.DOShakePosition(1f, 2))
            .Join(_scoreCanvasGroup.transform.DOShakeRotation(1f, 3))
            .Join(_scoreBar.DOColor(GetRandomColor(), 1))
            .Join(_scoreAmountText.transform.DOShakeRotation(2.5f, 4))
            .Join(_scoreAmountText.transform.DOShakePosition(1.5f, 2))
            .SetUpdate(true);

        yield return StartCoroutine(ScoreAdding(score, amount));
        yield return _wait;
        yield return _scoreCanvasGroup.DOFade(0, 0.8f).WaitForCompletion();

        _coroutine = null;
    }

    private IEnumerator ScoreAdding(int score, int amount)
    {
        int add = 1;
        int increasedCount = 0;

        while (amount > add)
        {
            _scoreAmountText.text = (score += add).ToString();
            amount -= add;
            increasedCount++;

            if (increasedCount >= 40)
            {
                increasedCount = 0;
                add++;
            }
            yield return _waitShort;
        }
        _scoreAmountText.text = (score += amount).ToString();
    }

    private Color GetRandomColor() { return Color.HSVToRGB(Random.Range(0, 1f), 0.47f, 1); }
    private void Start()
    {
        _scoreAmountText.text = "0";
        _scoreCanvasGroup.alpha = 0;
    }
}
