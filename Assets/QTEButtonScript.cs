using System.Collections;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class QTEButtonScript : MonoBehaviour
{
    public InputActionReference stepAction;

    public moveSystemScript moveScript;

    private bool leftSide;
    private int correctButton = -1;
    private bool inputLocked = false;

    public Image player1Image, Player2Image;
    public TextMeshProUGUI winText;
    public GameObject redBorder;

    //Images
    // Player 1 (left side)
    public Sprite[] player1Keys = new Sprite[4]; // W, A, S, D

    // Player 2 (right side)
    public Sprite[] player2Keys = new Sprite[4]; // Up, Down, Left, Right

    Key[] playerButtons = { Key.W, Key.A, Key.S, Key.D, Key.UpArrow, Key.DownArrow, Key.LeftArrow, Key.RightArrow };
    void Awake()
    {
        leftSide = Random.Range (0, 2) == 0;
        winText.enabled = false;
        redBorder.SetActive(false);

        if (stepAction == null)
        {
            Debug.LogError("StepAction not assigned in Inspector", this);
            return;
        }

        stepAction.action.Enable();
    }

    public void chooseButton()
    {
        if (leftSide)
        {
            correctButton = Random.Range(0, 4);
            player1Image.sprite = player1Keys[correctButton];
            Player2Image.sprite = null;
        }
        else
        {
            correctButton = Random.Range(4, 8);
            Player2Image.sprite = player2Keys[correctButton - 4];
            player1Image.sprite = null;
        }
    }

    public void selectButton()
    {
        if (inputLocked) return; // Skip if waiting

        if (correctButton == -1)
        {
            chooseButton();
        }
        else
        {
            Key expectedKey = playerButtons[correctButton];
            if (Keyboard.current[expectedKey].wasPressedThisFrame)
            {
                moveScript.Step();
                chooseButton();
                leftSide = Random.Range(0, 2) == 0;
            }
            else if (Keyboard.current.anyKey.wasPressedThisFrame)
            {
                // Lock input and start fail coroutine
                StartCoroutine(InputFail(1f));
            }
        }
    }
    void Update()
    {
        selectButton();
        if (moveScript.won == true)
        {
            winText.enabled = true;
            Time.timeScale = 0f;
        }
    }

    void OnDisable()
    {
        if (stepAction != null)
            stepAction.action.Disable();
    }

    public IEnumerator InputFail(float waitTime)
    {
        inputLocked = true; // Lock input
        redBorder.SetActive(true);
        stepAction.action.Disable();

        yield return new WaitForSeconds(waitTime);

        player1Image.sprite = null;
        Player2Image.sprite = null;
        correctButton = -1;

        stepAction.action.Enable();
        inputLocked = false; // Unlock input
        redBorder.SetActive(false);

    }
}
