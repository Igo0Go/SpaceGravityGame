using UnityEngine;

public class Collectable : MonoBehaviour
{
    [Range(1, 100)]
    [SerializeField]
    private int score = 1;

    [SerializeField]
    private AudioClip bonusClip;
    [SerializeField]
    private ScoreController scoreController;
    //todo Нужно дописать Счёт
    public void Collect()
    {
        if (bonusClip != null)
        {
            AudioSystem.PlaySound(bonusClip);
        }
        if (scoreController != null)
        {
            scoreController.AddScore(score);
        }
        Destroy(gameObject);
    }
}
