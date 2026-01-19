using UnityEngine;

public class moveSystemScript : MonoBehaviour
{
    public Animator animator;
    private bool walking;
    private bool success;
    public int stepsToWin = 15;
    private int stepsTaken = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Step()
    {
        transform.position += new Vector3(0.4f, 0, 0);
        stepsTaken++;
        if (stepsTaken >= stepsToWin) { Debug.Log("You Win"); }
    }
}
