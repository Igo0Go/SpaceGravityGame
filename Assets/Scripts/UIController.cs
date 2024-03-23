using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    [SerializeField]
    private GameObject spawnerMarker;
    [SerializeField]
    private GameObject finalPanel;

    [SerializeField]
    private List<DeadZonePoint> deadZones;
    [SerializeField]
    private List<PlayableDirector> playables;

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
        StartCoroutine(ShowFinalPanelCoroutone());
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Exit()
    {
        Application.Quit();
    }

    private IEnumerator ShowFinalPanelCoroutone()
    {
        FinalEvent?.Invoke();
        playerMovement.FinalAction();

        float t = 0;

        foreach (var player in playables)
        {
            player.Stop();
        }

        foreach (DeadZonePoint deadZone in deadZones)
        {
            deadZone.startPosition = deadZone.deadzone.position;
        }


        while (t < 1)
        {
            t += Time.deltaTime/5;

            foreach (DeadZonePoint deadZone in deadZones)
            {
                deadZone.deadzone.position = Vector3.Lerp(deadZone.startPosition, deadZone.target.position, t);
            }

            yield return null;
        }

    
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        finalPanel.SetActive(true);

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
