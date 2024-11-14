using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    [SerializeField]
    private GameObject spawnerMarker;
    [SerializeField]
    private GameObject finalPanel;

    private PlayerMovement playerMovement;

    public event Action FinalEvent;

    private void Awake()
    {
        finalPanel.SetActive(false);
        spawnerMarker.SetActive(false);
        playerMovement = FindObjectOfType<PlayerMovement>();
        playerMovement.TeleportImpossible += ShowFinalPanel;
        playerMovement.TeleportPossibleChanged += SetActiveForSpawnerMarker;
    }

    public void SetActiveForSpawnerMarker(bool active)
    {
        spawnerMarker.SetActive(active);
    }

    public void ShowFinalPanel()
    {
        FinalEvent?.Invoke();
        playerMovement.FinalAction();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        finalPanel.SetActive(true);
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
[Serializable]
public class DeadZonePoint
{
    public Transform deadzone;
    public Transform target;
    [HideInInspector]
    public Vector3 startPosition;
}
