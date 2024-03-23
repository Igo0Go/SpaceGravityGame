using System;
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
    private string deadZoneTag;
    [SerializeField]
    private Transform cameraTransform;
    [SerializeField, Range(0, 4)]
    private float deflectionForce = 1;

    [HideInInspector]
    public Rigidbody2D rb2D;
    private Rigidbody2D currentGravityPoint;
    private GravityPointController lastGravityPoint;
    private Transform myTransform;
    [HideInInspector]
    public Vector2 currentImpulseVector;
    [HideInInspector]
    public Vector2 resultVectorInSpace;
    [SerializeField]
    private Transform finalCameraPoint;

    public event Action<bool> TeleportPossibleChanged;
    public event Action TeleportImpossible;

    public float DeflectionForce
    {
        get
        {
            return deflectionForce;
        }
        set
        {
            deflectionForce = value;
            if (deflectionForce < 0)
            {
                deflectionForce = 0;
            }
            if (deflectionForce > 4)
            {
                deflectionForce = 4;
            }
        }
    }

    private bool active = true;

    void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();

        myTransform = transform;
    }

    private void FixedUpdate()
    {
        if (currentGravityPoint != null)
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
        if (active)
        {
            currentImpulseVector = context.ReadValue<Vector2>();
        }
    }

    public void OnRespawnAction(InputAction.CallbackContext context)
    {
        if (context.started && active)
        {
            Respawn();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(gravityPointTag))
        {
            resultVectorInSpace = Vector2.zero;

            if(lastGravityPoint != null)
            {
                lastGravityPoint.OnGravityPointDestroyed -= OnLostSaveZone;
            }

            currentGravityPoint = collision.GetComponent<Rigidbody2D>();
            lastGravityPoint = currentGravityPoint.GetComponent<GravityPointController>();
            lastGravityPoint.OnGravityPointDestroyed += OnLostSaveZone;
            TeleportPossibleChanged?.Invoke(true);

            StopAllCoroutines();
            StartCoroutine(ChangeCameraZValueCoroutine(collision.transform.localScale.x));

            collision.GetComponent<GravityPointActivator>().Activate();
        }
        else if (collision.CompareTag(deadZoneTag))
        {
            if(active)
            {
                StartCoroutine(RespawnCoroutine());
            }
        }
        else if (collision.CompareTag("Collectable"))
        {
            Collectable bonus = collision.GetComponent<Collectable>();
            bonus.Collect();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Rigidbody2D>() == currentGravityPoint)
        {
            currentGravityPoint = null;
        }
    }

    public void FinalAction()
    {
        active = false;
        StartCoroutine(MoveCameraToFinalPoint());
    }

    private IEnumerator MoveCameraToFinalPoint()
    {
        cameraTransform.parent = null;
        Vector3 startPosition = cameraTransform.position;
        Vector3 endPosition = finalCameraPoint.position;

        float t = 0f;
        while (t < 1)
        {
            t += Time.deltaTime/5;
            cameraTransform.position = Vector3.Lerp(startPosition, endPosition, t);
            yield return null;
        }
        cameraTransform.position = endPosition;
    }

    private IEnumerator ChangeCameraZValueCoroutine(float planetRadius)
    {
        float minimalZ = -10;
        float startZ = cameraTransform.position.z;
        float targetZ = minimalZ - (planetRadius - 1);

        float t = 0;

        while (t < 1)
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
            resultVectorInSpace = Quaternion.Euler(0, 0, -90) * rb2D.velocity.normalized;
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

    private void Respawn()
    {
        if (lastGravityPoint == null)
        {
            TeleportImpossible?.Invoke();
            Destroy(this);
            return;
        }

        myTransform.position = lastGravityPoint.transform.position;
        rb2D.velocity = Vector2.zero;
    }

    private IEnumerator RespawnCoroutine()
    {
        float minimalZ = -10;
        float startZ = cameraTransform.position.z;
        float targetZ = minimalZ;

        float t = 0;

        while (t < 1)
        {
            t += Time.deltaTime;
            cameraTransform.localPosition =
                new Vector3(0, 0, Mathf.Lerp(startZ, targetZ, t));

            yield return null;
        }

        cameraTransform.localPosition = new Vector3(0, 0, targetZ);

        Respawn();
    }

    private void OnLostSaveZone()
    {
        lastGravityPoint = null;
        TeleportPossibleChanged?.Invoke(false);
    }
}
