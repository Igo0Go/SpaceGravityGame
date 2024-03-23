using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class GravityPointController : MonoBehaviour
{
    public event Action OnGravityPointDestroyed;

    private void Awake()
    {
        FindObjectOfType<UIController>().FinalEvent += DestroyMe;
    }

    private void DestroyMe()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("DeadZone"))
        {
            OnGravityPointDestroyed?.Invoke();
            FindObjectOfType<UIController>().FinalEvent -= DestroyMe;
            Destroy(gameObject);
        }
    }
}
