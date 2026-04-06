using DG.Tweening;
using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI _scoreAmountText;
    [SerializeField] private CanvasGroup _scoreCanvasGroup;

    public void UpdateScoreAmount(int amount)
    { 
        _scoreAmountText.text = amount.ToString(); 
    }



    private void Start()
    {
        UpdateScoreAmount(0);
    }
}
