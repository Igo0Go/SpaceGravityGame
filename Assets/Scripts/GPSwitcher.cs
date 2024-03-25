using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GPSwitcher : MonoBehaviour
{
    [SerializeField]
    private List<GravityPointActivator> activators;

    [SerializeField]
    private List<SpriteRenderer> oldParts;
    [SerializeField]
    private List<SpriteRenderer> newParts;

    [SerializeField]
    private UnityEvent switchHandler;

    public void TakeSignal(GravityPointActivator activator)
    {
        if (activators.Contains(activator))
        {
            activators.Remove(activator);
            if (activators.Count < 1)
            {
                StartCoroutine(SmoothCrossFadeForParts());
                switchHandler.Invoke();
            }
        }
    }

    private IEnumerator SmoothCrossFadeForParts()
    {
        float t = 0;

        while (t < 1)
        {
            t += Time.deltaTime;

            foreach (var part in oldParts)
            {
                part.color = new Color(part.color.r, part.color.b, part.color.b, 1-t);
            }

            foreach (var part in newParts)
            {
                part.color = new Color(part.color.r, part.color.b, part.color.b, t);
            }

            yield return null;
        }


        foreach (var part in oldParts)
        {
            part.color = new Color(part.color.r, part.color.b, part.color.b, 0);
        }

        foreach (var part in newParts)
        {
            part.color = new Color(part.color.r, part.color.b, part.color.b, 1);
        }
    }

}
