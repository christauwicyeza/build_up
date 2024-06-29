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
    public Toggle directTransitionToggle; // Toggle to enable/disable direct transition

    // Prefab for Next and Previous buttons (assign these in the Inspector)
    public Button nextButtonPrefab;
    public Button previousButtonPrefab;

    private int Counter = -1; // This should start at -1 to ensure the first move is correct
    private float transitionSpeed = 2f;
    private bool directTransition = false; // Direct transition flag

    private void Start()
    {
        DisableAllCanvases();
        ExitButton.SetActive(true); // Ensure ExitButton is active at start
        initialCanvas.gameObject.SetActive(true); // Ensure initial canvas is active at start

        // Set up slider listener for transition speed
        transitionSpeedSlider.onValueChanged.AddListener(UpdateTransitionSpeed);
        transitionSpeedSlider.value = transitionSpeed; // Initialize slider value

        // Set up toggle listener for direct transition
        directTransitionToggle.onValueChanged.AddListener(UpdateDirectTransition);
        directTransitionToggle.isOn = false; // Ensure the toggle starts off

        // Add buttons to each canvas
        foreach (Canvas canvas in roomCanvases)
        {
            AddNavigationButtons(canvas);
        }
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

    private void UpdateDirectTransition(bool isOn)
    {
        directTransition = isOn;
    }

    private void MoveCamera()
    {
        if (Counter >= 0 && Counter < roomPositions.Length && roomPositions[Counter] != null)
        {
            if (directTransition)
            {
                // Directly move camera without animation
                transform.position = roomPositions[Counter].transform.position;
                transform.rotation = roomPositions[Counter].transform.rotation;
            }
            else
            {
                // Move camera with animation
                transform.DOMove(roomPositions[Counter].transform.position, transitionSpeed).SetEase(Ease.InOutSine);
                transform.DORotateQuaternion(roomPositions[Counter].transform.rotation, transitionSpeed).SetEase(Ease.InOutSine);
            }
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
            // Restart the game loop
            Counter = 0;
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

    private void AddNavigationButtons(Canvas canvas)
    {
        // Instantiate Next button and set its properties
        Button nextButton = Instantiate(nextButtonPrefab, canvas.transform);
        nextButton.name = "NextButton";
        nextButton.onClick.AddListener(() => NextScene());
        RectTransform nextButtonRectTransform = nextButton.GetComponent<RectTransform>();
        nextButtonRectTransform.anchorMin = new Vector2(1, 0);
        nextButtonRectTransform.anchorMax = new Vector2(1, 0);
        nextButtonRectTransform.pivot = new Vector2(1, 0);
        nextButtonRectTransform.anchoredPosition = new Vector2(-10, 10);

        // Instantiate Previous button and set its properties
        Button previousButton = Instantiate(previousButtonPrefab, canvas.transform);
        previousButton.name = "PreviousButton";
        previousButton.onClick.AddListener(() => Previous());
        RectTransform previousButtonRectTransform = previousButton.GetComponent<RectTransform>();
        previousButtonRectTransform.anchorMin = new Vector2(0, 0);
        previousButtonRectTransform.anchorMax = new Vector2(0, 0);
        previousButtonRectTransform.pivot = new Vector2(0, 0);
        previousButtonRectTransform.anchoredPosition = new Vector2(10, 10);
    }
}
