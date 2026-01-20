using Unity.VisualScripting;
using UnityEngine;

public class moveSystemScript : MonoBehaviour
{
    public Animator animator;
    private bool walking;
    private bool success;
    public int stepsToWin = 15;
    private int stepsTaken = 0;

    public void Step()
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + new Vector3(0.2f, 0, 0);
        transform.position = Vector3.Lerp(startPos, endPos, 0.5f);
        stepsTaken++;
        if (stepsTaken >= stepsToWin) { Debug.Log("You Win"); }
    }
}
