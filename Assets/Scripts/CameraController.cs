using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class CameraController : MonoBehaviour
{
    public GameObject[] roomPositions; // Array of GameObjects for camera positions
    public Canvas[] roomCanvases; // Array of Canvases for each room
    public Canvas initialCanvas; // Reference to the initial canvas
    public GameObject ExitButton;
    public Slider transitionSpeedSlider; // Slider to control transition speed

    private int Counter = -1; // This should start at -1 to ensure the first move is correct
    private float transitionSpeed = 2f;

    private void Start()
    {
        DisableAllCanvases();
        ExitButton.SetActive(true); // Ensure ExitButton is active at start
        initialCanvas.gameObject.SetActive(true); // Ensure initial canvas is active at start

        // Set up slider listener for transition speed
        transitionSpeedSlider.onValueChanged.AddListener(UpdateTransitionSpeed);
    }

    private void DisableAllCanvases()
    {
        foreach (Canvas canvas in roomCanvases)
        {
            canvas.gameObject.SetActive(false);
        }
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ActualStart()
    {
        Counter = 0; // Ensure Counter starts at the first room
        MoveCamera();
    }

    private void UpdateTransitionSpeed(float value)
    {
        transitionSpeed = value;
    }

    private void MoveCamera()
    {
        if (Counter >= 0 && Counter < roomPositions.Length && roomPositions[Counter] != null)
        {
            transform.DOMove(roomPositions[Counter].transform.position, transitionSpeed).SetEase(Ease.InOutSine);
            transform.rotation = roomPositions[Counter].transform.rotation; // Set camera rotation

            ActivateCanvas(Counter);
        }
    }

    private void ActivateCanvas(int index)
    {
        if (index >= 0 && index < roomCanvases.Length)
        {
            DisableAllCanvases();
            roomCanvases[index].gameObject.SetActive(true);
        }
    }

    public void NextScene()
    {
        Counter++;
        if (Counter >= roomPositions.Length)
        {
            Counter = 0; // Loop back to the first room
            MoveCamera();
            initialCanvas.gameObject.SetActive(true); // Activate the initial canvas

            // Deactivate all other canvases except the initial canvas
            foreach (Canvas canvas in roomCanvases)
            {
                if (canvas != initialCanvas)
                {
                    canvas.gameObject.SetActive(false);
                }
            }

            // Restart the application
            RestartApplication();
        }
        else
        {
            MoveCamera();
        }

        // Do not deactivate ExitButton
        ExitButton.SetActive(true);
    }

    public void Previous()
    {
        Counter--;
        if (Counter < 0)
        {
            Counter = roomPositions.Length - 1; // Loop back to the last room
        }
        MoveCamera();
    }

    public void ExitApp()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

    private void RestartApplication()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
