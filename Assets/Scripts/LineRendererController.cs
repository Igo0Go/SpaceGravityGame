using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LineRendererController : MonoBehaviour
{
    public Transform start;
    public Transform end;

    private LineRenderer line;

    private void Awake()
    {
        line = GetComponent<LineRenderer>();
        line.positionCount = 2;
        line.useWorldSpace = true;
    }

    private void LateUpdate()
    {
        line.SetPosition(0, start.position);
        line.SetPosition(1, end.position);
    }

    public Vector3 Vector
    {
        get
        {
            return end.position - start.position;
        }
        set
        {
            start.up = value;
            end.position = start.position + value;
        }
    }
}
