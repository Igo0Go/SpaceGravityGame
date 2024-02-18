using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DebugWindowScript : MonoBehaviour
{
    [SerializeField]
    private GameObject editModePanel;

    [SerializeField]
    private Slider sliderPlanetMass;
    [SerializeField]
    private Text textPlanetMass;
    [SerializeField]
    private InputField planetMassInputField;


    [SerializeField]
    private Slider sliderPlanetRadius;
    [SerializeField]
    private Text textPlanetRadius;
    [SerializeField]
    private InputField planetRadiusInputField;

    [SerializeField]
    private PlayerMovement playerMovement;

    [SerializeField]
    private DebugLinesPack linesPack;

    private Rigidbody2D currentRB;
    private Transform currentTransform;


    private void Awake()
    {
        sliderPlanetMass.onValueChanged.AddListener(OnPlanetMassSliderChanged);
        sliderPlanetRadius.onValueChanged.AddListener(OnPlanetRadiusSliderChanged);
        planetMassInputField.onValueChanged.AddListener (OnPlanetMassInputFieldChanget);
        planetRadiusInputField.onValueChanged.AddListener(OnPlaneRadiusInputFieldChanget);
        SetEditModeValue(false);
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

    private void LateUpdate()
    {
        linesPack.DrawDirections(playerMovement);
    }

    private void SetEditModeValue(bool value)
    {
        editModePanel.SetActive(value);
        linesPack.debugPack.SetActive(value);
        Cursor.lockState = value? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = value;
        if (value)
        {
            DrawPlanetStats();
            linesPack.DrawDirections(playerMovement);
        }
    }

    private void OnPlanetMassSliderChanged(float newValue)
    {
        if(currentRB != null)
        {
            currentRB.mass = newValue;
            planetMassInputField.text = newValue.ToString();
            DrawPlanetStats();
        }
    }
    private void OnPlanetRadiusSliderChanged(float newValue)
    {
        if (currentRB != null)
        {
            currentTransform.localScale = Vector3.one * newValue;
            planetRadiusInputField.text = newValue.ToString();
            DrawPlanetStats();
        }
    }

    private void OnPlanetMassInputFieldChanget(string newValue)
    {
        if (currentRB != null)
        {
            float value = float.Parse(newValue);
            sliderPlanetMass.value = value;
            currentRB.mass = value;
            DrawPlanetStats();
        }
    }
    private void OnPlaneRadiusInputFieldChanget(string newValue)
    {
        if (currentRB != null)
        {
            float value = float.Parse(newValue);
            sliderPlanetRadius.value = value;
            currentTransform.localScale = Vector3.one * value;
            DrawPlanetStats();
        }
    }

    private void DrawPlanetStats()
    {
        if(currentRB != null)
        {
            textPlanetMass.text  = "Масса: " + currentRB.mass;
            planetMassInputField.text = currentRB.mass.ToString();
            sliderPlanetMass.value = currentRB.mass;
            textPlanetRadius.text = "Радиус: " + currentTransform.localScale.x;
            planetRadiusInputField.text = currentTransform.localScale.x.ToString();
            sliderPlanetRadius.value = currentTransform.localScale.x;
        }
    }
}

[System.Serializable]
public class DebugLinesPack
{
    public GameObject debugPack;

    public LineRendererController lineInputVector;
    public LineRendererController lineAngleResultVector;
    public LineRendererController lineVelocityVector;

    public void DrawDirections(PlayerMovement playerMovement)
    {
        lineInputVector.Vector = playerMovement.currentImpulseVector;
        lineAngleResultVector.Vector = playerMovement.resultVectorInSpace;
        lineVelocityVector.Vector = playerMovement.rb2D.velocity;
    }
}


