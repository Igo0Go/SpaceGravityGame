using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{

    [SerializeField]
    [Min(1)]
    private float antiGravityForce;
    [SerializeField]
    [Min(1)]
    private float G = 5f;
    [Space]
    [SerializeField]
    private string gravityPointTag;
    [SerializeField]
    private Transform cameraTransform;
    [SerializeField, Min(0)]
    public float frictionInSpace = 1;

    private Rigidbody2D rb2D;
    private Rigidbody2D currentGravityPoint;
    private Rigidbody2D lastGravityPoint;
    private Transform myTransform;
    private Vector2 currentImpulseVector;

    private Vector3 gravityGozmos;
    private Vector3 movementGizmos;

    void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();

        myTransform = transform;
    }

    private void FixedUpdate()
    {
        if(currentGravityPoint != null)
        {
            MoveInPlanet();
        }
        else
        {
            MoveInSpace();
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {

        currentImpulseVector = context.ReadValue<Vector2>();
    }

    public void OnGravityAction(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            myTransform.position = lastGravityPoint.position;
            rb2D.velocity = Vector2.zero;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag(gravityPointTag))
        {
            currentGravityPoint = lastGravityPoint = collision.GetComponent<Rigidbody2D>();

            StopAllCoroutines();
            StartCoroutine(ChangeCameraZValueCoroutine(collision.transform.localScale.x));
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Rigidbody2D>() == currentGravityPoint)
        {
            currentGravityPoint = null;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + gravityGozmos);
        if (currentGravityPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + movementGizmos);
        }
        if(rb2D != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, rb2D.velocity);
        }
    }

    private IEnumerator ChangeCameraZValueCoroutine(float planetRadius)
    {
        float minimalZ = -10;
        float startZ = cameraTransform.position.z;
        float targetZ = minimalZ - (planetRadius - 1);

        float t = 0;

        while(t < 1)
        {
            t += Time.deltaTime;
            cameraTransform.localPosition = 
                new Vector3(0, 0, Mathf.Lerp(startZ, targetZ, t));

            yield return null;
        }

        cameraTransform.localPosition = new Vector3(0, 0, targetZ);
    }


    private void MoveInPlanet()
    {
        Vector2 gravity = currentGravityPoint.transform.position - myTransform.position;

        float gravityForce = 0;

        if (gravity.sqrMagnitude > 0)
        {
            gravityForce = G * currentGravityPoint.mass * 1 / (currentGravityPoint.transform.localScale.x / gravity.magnitude);
        }

        gravityGozmos = gravity.normalized * gravityForce * Time.fixedDeltaTime;
        movementGizmos = currentImpulseVector * antiGravityForce * Time.fixedDeltaTime;

        rb2D.AddForce(gravity.normalized * gravityForce * Time.fixedDeltaTime, ForceMode2D.Force);

        rb2D.AddForce(currentImpulseVector * antiGravityForce * Time.fixedDeltaTime, ForceMode2D.Impulse);

        rb2D.AddForce(-rb2D.velocity.normalized * Time.fixedDeltaTime * currentGravityPoint.mass, ForceMode2D.Force);
    }

    private void MoveInSpace()
    {
        rb2D.AddForce(currentImpulseVector * antiGravityForce * Time.fixedDeltaTime, ForceMode2D.Impulse);

        rb2D.AddForce(-rb2D.velocity.normalized * frictionInSpace, ForceMode2D.Force);
    }
}
