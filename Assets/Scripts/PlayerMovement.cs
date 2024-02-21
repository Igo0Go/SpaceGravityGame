using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

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
    [SerializeField, Range(0,2)]
    private float deflectionForce = 1;

    [HideInInspector]
    public Rigidbody2D rb2D;
    private Rigidbody2D currentGravityPoint;
    private Rigidbody2D lastGravityPoint;
    private Transform myTransform;
    [HideInInspector]
    public Vector2 currentImpulseVector;
    [HideInInspector]
    public Vector2 resultVectorInSpace;


    public float DeflectionForce
    {
        get
        {
            return deflectionForce;
        }
        set
        {
            deflectionForce = value;
            if(deflectionForce < 0)
            {
                deflectionForce = 0;
            }
            if(deflectionForce > 2)
            {
                deflectionForce = 2;
            }
        }
    }

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
            if(lastGravityPoint == null)
            {
                Debug.LogError("Точки сохранения ещё не было!!!");
                return;
            }

            myTransform.position = lastGravityPoint.position;
            rb2D.velocity = Vector2.zero;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag(gravityPointTag))
        {
            resultVectorInSpace = Vector2.zero;
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

        rb2D.AddForce(gravity.normalized * gravityForce * Time.fixedDeltaTime, ForceMode2D.Force);

        rb2D.AddForce(currentImpulseVector * antiGravityForce * Time.fixedDeltaTime, ForceMode2D.Impulse);

        rb2D.AddForce(-rb2D.velocity.normalized * Time.fixedDeltaTime * currentGravityPoint.mass, ForceMode2D.Force);
    }

    private void MoveInSpace()
    {
        float currentAngle = Vector2.SignedAngle(rb2D.velocity, currentImpulseVector.normalized);
        resultVectorInSpace = currentImpulseVector.normalized;

        if (currentAngle < 0)
        {
            resultVectorInSpace = Quaternion.Euler(0,0,-90)*rb2D.velocity.normalized;
        }
        else if (currentAngle > 0)
        {
            resultVectorInSpace = Quaternion.Euler(0, 0, 90) * rb2D.velocity.normalized;
        }
        else
        {
            resultVectorInSpace = Vector2.zero;
            return;
        }

        rb2D.AddForce(resultVectorInSpace.normalized * deflectionForce * Time.fixedDeltaTime, ForceMode2D.Impulse);
    }
}
