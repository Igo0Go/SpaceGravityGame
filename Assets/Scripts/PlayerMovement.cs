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
    private string gravityPointTag;
    [SerializeField]
    [Min(1)]
    private float antiGravityForce;
    [SerializeField]
    private InputActionAsset GravityGameActionAsset;

    [SerializeField]
    private float G = 5f;


    private Rigidbody2D rb2D;
    private Rigidbody2D currentGravityPoin;
    private Rigidbody2D lastGravityPoint;
    private Transform myTransform;
    private InputActionMap playerActionMap;
    private InputAction gravityAction;
    private Vector2 currentImpulseVector;

    private Vector3 gravityGozmos;
    private Vector3 movementGizmos;

    void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();

        myTransform = transform;
        playerActionMap = GravityGameActionAsset.FindActionMap("Player");
        gravityAction = playerActionMap.FindAction("GravityAction");
        gravityAction.performed += OnGravityAction;
    }

    private void OnDestroy()
    {
        gravityAction.performed -= OnGravityAction;
    }

    private void FixedUpdate()
    {
        if(currentGravityPoin != null)
        {
            Vector2 gravity = currentGravityPoin.transform.position - myTransform.position;

            float gravityForce = 0;

            if (gravity.sqrMagnitude > 0)
            {
                gravityForce = G * currentGravityPoin.mass * 1/(currentGravityPoin.transform.localScale.x / gravity.magnitude);
            }

            gravityGozmos = gravity.normalized * gravityForce * Time.fixedDeltaTime;
            movementGizmos = currentImpulseVector * antiGravityForce * Time.fixedDeltaTime;

            rb2D.AddForce(gravity.normalized * gravityForce * Time.fixedDeltaTime, ForceMode2D.Force);

            rb2D.AddForce(currentImpulseVector * antiGravityForce * Time.fixedDeltaTime, ForceMode2D.Impulse);

            rb2D.AddForce(-rb2D.velocity.normalized * Time.fixedDeltaTime * currentGravityPoin.mass, ForceMode2D.Force);
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            myTransform.position = lastGravityPoint.position;
            rb2D.velocity = Vector2.zero;
        }
    }

    private void OnMove(InputValue inputValue)
    {
        currentImpulseVector = inputValue.Get<Vector2>();
    }

    public void OnGravityAction(InputAction.CallbackContext context)
    {
        Debug.Log("Action");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag(gravityPointTag))
        {
            currentGravityPoin = lastGravityPoint = collision.GetComponent<Rigidbody2D>();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Rigidbody2D>() == currentGravityPoin)
        {
            currentGravityPoin = null;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + gravityGozmos);
        if (currentGravityPoin != null)
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
}
