using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlanetControlScript : MonoBehaviour
{
    [SerializeField]
    private GameObject editModePanel;
    [SerializeField]
    private Slider sliderPlanetMass;
    [SerializeField]
    private Slider sliderPlanetRadius;
    [SerializeField]
    private Text textPlanetMass;
    [SerializeField]
    private Text textPlanetRadius;
    [SerializeField]
    private Slider sliderPlayer;
    [SerializeField]
    private Text textPlayerStats;
    [SerializeField]
    private PlayerMovement playerMovement;

    private Rigidbody2D currentRB;
    private Transform currentTransform;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sliderPlanetMass.onValueChanged.AddListener(OnSliderPlanetMassChanged);
        sliderPlanetRadius.onValueChanged.AddListener(OnSliderPlanetRadiusChanged);
        sliderPlayer.onValueChanged.AddListener(OnSliderPlayerChanged);
        editModePanel.gameObject.SetActive(false);
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);

            if (hit && hit.collider.CompareTag("GravityPoint"))
            {
                if (currentRB != null)
                {
                    currentRB.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
                }
                currentRB = hit.rigidbody;
                currentTransform = currentRB.transform;
                currentRB.gameObject.GetComponent<SpriteRenderer>().color = Color.red;
                DrawPlanetStats();
            }
        }

        if(Input.GetKeyDown(KeyCode.Z))
        {
            SetEditModeValue(!editModePanel.activeSelf);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    private void SetEditModeValue(bool value)
    {
        editModePanel.SetActive(value);
        Cursor.lockState = value? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = value;
        if (value)
        {
            DrawPlanetStats();
        }
    }

    private void OnSliderPlanetMassChanged(float newValue)
    {
        if(currentRB != null)
        {
            currentRB.mass = newValue;
            DrawPlanetStats();
        }
    }
    private void OnSliderPlanetRadiusChanged(float newValue)
    {
        if (currentRB != null)
        {
            currentTransform.localScale = Vector3.one * newValue;
            DrawPlanetStats();
        }
    }
    private void OnSliderPlayerChanged(float newValue)
    {
        playerMovement.frictionInSpace = newValue;
        textPlayerStats.text = "Сопротивление в космосе: " + playerMovement.frictionInSpace;
    }

    private void DrawPlanetStats()
    {
        if(currentRB != null)
        {
            textPlanetMass.text  = "Масса: " + currentRB.mass;
            textPlanetRadius.text = "Радиус: " + currentTransform.localScale.x;
        }
    }
}
