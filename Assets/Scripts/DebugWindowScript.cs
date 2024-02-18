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
    private Slider sliderPlayerAngle;
    [SerializeField]
    private Text textPlayerAngle;
    [SerializeField]
    private InputField playerRotateInputField;

    [SerializeField]
    private PlayerMovement playerMovement;

    [SerializeField]
    private DebugLinesPack linesPack;

    private Rigidbody2D currentRB;
    private Transform currentTransform;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sliderPlanetMass.onValueChanged.AddListener(OnPlanetMassSliderChanged);
        sliderPlanetRadius.onValueChanged.AddListener(OnPlanetRadiusSliderChanged);
        sliderPlayerAngle.onValueChanged.AddListener(OnRotateForceSliderChanged);
        playerRotateInputField.onValueChanged.AddListener(OnPlayerRotateForceInputFieldChanget);
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
        linesPack.DrawAngle(playerMovement);
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
            DrawAngleStats();
            linesPack.DrawAngle(playerMovement);
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
    private void OnRotateForceSliderChanged(float newValue)
    {
        playerRotateInputField.text = newValue.ToString();
        playerMovement.rotateForce = newValue;
        textPlayerAngle.text = "Угол отклонения: " + playerMovement.rotateForce;
        linesPack.DrawAngle(playerMovement);
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
    private void OnPlayerRotateForceInputFieldChanget(string newValue)
    {
        float value = float.Parse(newValue);
        sliderPlayerAngle.value = value;
        playerMovement.rotateForce = value;
        textPlayerAngle.text = "Угол отклонения: " + playerMovement.rotateForce;
        linesPack.DrawAngle(playerMovement);
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
    private void DrawAngleStats()
    {
        sliderPlayerAngle.value = playerMovement.rotateForce;
        playerRotateInputField.text = playerMovement.rotateForce.ToString();
        textPlayerAngle.text = "Угол отклонения: " + playerMovement.rotateForce;
    }
}

[System.Serializable]
public class DebugLinesPack
{
    public GameObject debugPack;

    public LineRendererController lineAngleLeft;
    public LineRendererController lineAngleRight;
    public LineRendererController lineInputVector;
    public LineRendererController lineAngleResultVector;
    public LineRendererController lineVelocityVector;

    public void DrawAngle(PlayerMovement playerMovement)
    {
        lineAngleRight.Vector = playerMovement.RotateVector2(playerMovement.rb2D.velocity.normalized, -playerMovement.rotateForce);
        lineAngleLeft.Vector = playerMovement.RotateVector2(playerMovement.rb2D.velocity.normalized, playerMovement.rotateForce);
    }
    public void DrawDirections(PlayerMovement playerMovement)
    {
        lineInputVector.Vector = playerMovement.currentImpulseVector;
        lineAngleResultVector.Vector = playerMovement.resultVectorInSpace;
        lineVelocityVector.Vector = playerMovement.rb2D.velocity;
    }
}


