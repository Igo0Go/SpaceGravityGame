using UnityEngine;

public class Collectable : MonoBehaviour
{
    [Range(1, 100)]
    [SerializeField]
    private int score = 1;

    [SerializeField]
    private AudioClip bonusClip;
    [SerializeField]
    private AudioSource collectableSource;
    [SerializeField]
    private ScoreController scoreController;
    //todo Нужно дописать Счёт
    public void Collect()
    {
        if (collectableSource != null && bonusClip != null)
        {
            collectableSource.PlayOneShot(bonusClip);
        }
        if (scoreController != null)
        {
            scoreController.AddScore(score);

        }
        Destroy(gameObject);
    }
}
