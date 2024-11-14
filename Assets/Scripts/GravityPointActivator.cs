using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class GravityPointActivator : MonoBehaviour
{
    [SerializeField]
    private GPSwitcher _switcher;
    [SerializeField]
    private Color targetColor = Color.yellow;

    private bool active = false;

    public void Activate()
    {
        if(!active)
        {
            if (_switcher == null)
            {
                Debug.Log("Не передан GPSwitcher");
                return;
            }
            _switcher.TakeSignal(this);
            StartCoroutine(SmoothChangeColorCoroutine());
            active = true;
        }
    }


    private IEnumerator SmoothChangeColorCoroutine()
    {
        Color color = GetComponent<SpriteRenderer>().color;
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();

        float t = 0;

        while (t <1)
        {
            t += Time.deltaTime/2;
            renderer.color = Color.Lerp(color, targetColor, t);
            yield return null;
        }
        renderer.color = targetColor;
    }
}
