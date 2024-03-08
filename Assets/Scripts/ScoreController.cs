using TMPro;
using UnityEngine;

public class ScoreController : MonoBehaviour
{
    [SerializeField]
    private TMP_Text text;
    private int currentScore = 0;
    private void Awake()
    {
        AddScore(0);
    }
    public void AddScore(int score)
    {
        this.currentScore += score;
        text.text = currentScore.ToString();
    }

}
