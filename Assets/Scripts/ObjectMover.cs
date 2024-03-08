using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMover : MonoBehaviour
{
    [SerializeField]
    private Transform objectOnMove;
    [SerializeField]
    private Transform startTransform;
    [SerializeField]
    private Transform endTransform;

    [SerializeField]
    [Min(1)]
    private float objectSpeed;

    private Vector3 moveVector;
    void Start()
    {
        moveVector = (endTransform.position - startTransform.position).normalized;
        objectOnMove.position = startTransform.position;
        //геймобджект
        //два трансформа между которыми будет движение геймобжекта
        
    }

    void Update()
    {
        //двигай объект по вектору пока растояние между объектами не станет меньше diff
        if (Vector2.Distance(endTransform.position, objectOnMove.position)>objectSpeed* Time.deltaTime*2)
        {
            objectOnMove.position += moveVector * objectSpeed * Time.deltaTime;
        }
        else
        {
            objectOnMove.position = startTransform.position;
        }
    }

    private void OnDrawGizmos()
    {
        if(startTransform != null && endTransform != null) 
        { 
            Gizmos.color = Color.red;
            Gizmos.DrawLine(startTransform.position, endTransform.position);
        }
    }
}
