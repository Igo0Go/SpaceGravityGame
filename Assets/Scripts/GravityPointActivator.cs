using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(SpriteRenderer))]
public class GravityPointActivator : MonoBehaviour
{
    [SerializeField]
    private GPSwitcher _switcher;
    [SerializeField]
    private Color targetColor = Color.yellow;

    public void Activate()
    {
        if (_switcher == null)
        {
            Debug.Log("Не передан GPSwitcher");
            return;
        }
        _switcher.TakeSignal(this);
        GetComponent<SpriteRenderer>().color = targetColor;
    }


}
