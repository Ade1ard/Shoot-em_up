using TMPro;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIView : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI _currentWaveText;
    [SerializeField] private Image _expBarFill;
    [SerializeField] private CanvasGroup _gamePlayUI;
    [Header("Parameters")]
    [SerializeField] private float _barDrawingSpeed = 1;
    [SerializeField] private float _fadeSpeed = 1;

    private Coroutine _drawingBarCoroutine;
    private Coroutine _fadingUICoroutine;

    public void ShowCurrentWave(int wave)
    {
        _currentWaveText.text = $"Current wave - {wave}";
    }

    public void ShowUI(bool _bool)
    {
        if (_fadingUICoroutine != null)
            StopCoroutine(_fadingUICoroutine);
        _fadingUICoroutine = StartCoroutine(FadeUI(_bool));
    }

    private IEnumerator FadeUI(bool _bool)
    {
        int amount = _bool ? 1 : 0;
        while (_gamePlayUI.alpha != amount)
        {
            _gamePlayUI.alpha = Mathf.MoveTowards(_gamePlayUI.alpha, amount, _fadeSpeed / 100);
            yield return null;
        }
        _fadingUICoroutine = null;
    }

    public void StartDrawingBar(int exp, int levelCost)
    {
        if (_drawingBarCoroutine != null)
            StopCoroutine(_drawingBarCoroutine);
        _drawingBarCoroutine = StartCoroutine(DrawExpBar(exp, levelCost));
    }

    private IEnumerator DrawExpBar(float exp, float levelCost)
    {
        while (_expBarFill.fillAmount != (exp / levelCost))
        {
            _expBarFill.fillAmount = Mathf.MoveTowards(_expBarFill.fillAmount, exp / levelCost, _barDrawingSpeed / 100);
            yield return null;
        }
        _drawingBarCoroutine = null;
    }

    private void Start()
    {
        _gamePlayUI.alpha = 0;
        ShowUI(true);
        _expBarFill.fillAmount = 0;
    }
}
