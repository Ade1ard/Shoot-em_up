using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI _scoreAmountText;
    [SerializeField] private CanvasGroup _scoreCanvasGroup;

    private Coroutine _coroutine;
    private Coroutine _fadingCoroutine;
    private Tween _tween;

    private readonly WaitForSeconds _wait = new WaitForSeconds(1);

    public void UpdateScoreAmount(int score, int amount)
    { 
        _scoreAmountText.text = amount.ToString(); 
        if ( _coroutine != null)
        {
            StopCoroutine(_coroutine);
            DOTween.Kill(_scoreCanvasGroup.transform);
        }
        StartCoroutine(ScoreAnimation(score, amount));
    }

    private IEnumerator ScoreAnimation(int score, int amount)
    {
        _fadingCoroutine = StartCoroutine(ScoreFade(true));

        _scoreCanvasGroup.transform.DOShakePosition(1.5f, 2);
        _scoreCanvasGroup.transform.DOShakeRotation(1.5f, 2);
        yield return StartCoroutine(ScoreAdding(score, amount));
        yield return _wait;

        if (_fadingCoroutine != null)
            StopCoroutine(_fadingCoroutine);
        _fadingCoroutine = StartCoroutine(ScoreFade(false));

        _coroutine = null;
    }

    private IEnumerator ScoreFade(bool _bool)
    {
        int amount = _bool ? 1 : 0;
        while (_scoreCanvasGroup.alpha != amount)
        {
            _scoreCanvasGroup.alpha = Mathf.MoveTowards(_scoreCanvasGroup.alpha, amount, 0.01f);
            yield return null;
        }
        _fadingCoroutine = null;
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
            yield return null;
        }
        _scoreAmountText.text = (score += amount).ToString();
    }

    private void Start()
    {
        _scoreAmountText.text = "0";
    }
}
