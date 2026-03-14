using TMPro;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _currentWaveText;
    [SerializeField] private Image _expBarFill;
    [SerializeField] private float _barDrawingSpeed = 1;

    private Coroutine _drawingBarCoroutine;

    public void ShowCurrentWave(int wave)
    {
        _currentWaveText.text = $"Current wave - {wave}";
    }

    public void StartDrawingBar(int exp, int levelCost)
    {
        if (_drawingBarCoroutine != null)
            StopCoroutine(_drawingBarCoroutine);
        _drawingBarCoroutine = StartCoroutine(DrawHealthBar(exp, levelCost));
    }

    private IEnumerator DrawHealthBar(float exp, float levelCost)
    {
        while (_expBarFill.fillAmount != (exp / levelCost))
        {
            _expBarFill.fillAmount = Mathf.MoveTowards(_expBarFill.fillAmount, exp / levelCost, _barDrawingSpeed / 100);
            yield return null;
        }
        _drawingBarCoroutine = null;
    }
}
