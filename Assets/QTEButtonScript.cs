using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class QTEButtonScript : MonoBehaviour
{
    public InputActionReference stepAction;

    public moveSystemScript moveScript;

    private bool leftSide;
    private int correctButton;
    private int pressedButton;

    public Image player1Image, Player2Image;

    //Images
    // Player 1 (left side)
    public Sprite[] player1Keys = new Sprite[4]; // W, A, S, D

    // Player 2 (right side)
    public Sprite[] player2Keys = new Sprite[4]; // Up, Down, Left, Right

    void Awake()
    {
        leftSide = Random.Range (0, 2) == 0;
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
        }
        else
        {
            correctButton = Random.Range(4, 8);
            Player2Image.sprite = player2Keys[correctButton];
        }
    }
    void Update()
    {
        if (stepAction != null && stepAction.action.WasPressedThisFrame())
        {
            moveScript.Step();
        }
    }

    void OnDisable()
    {
        if (stepAction != null)
            stepAction.action.Disable();
    }
}
