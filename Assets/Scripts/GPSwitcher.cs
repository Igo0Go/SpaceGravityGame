using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GPSwitcher : MonoBehaviour
{
    [SerializeField]
    private List<GravityPointActivator> activators;
    [SerializeField]
    private UnityEvent switchHandler;

    public void TakeSignal(GravityPointActivator activator)
    {
        if (activators.Contains(activator))
        {
            activators.Remove(activator);
            if (activators.Count < 1)
            {
                switchHandler.Invoke();
                //¬ключить некст и отключить

            }
        }
    }

}
