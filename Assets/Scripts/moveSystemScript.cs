using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class moveSystemScript : MonoBehaviour
{
    public Animator animator;
    public int stepsToWin = 15;
    private int stepsTaken = 0;
    public bool won = false;

    public float moveDistance = 0.5f;
    public float stepTime = 0.3f;

    private bool stepping = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetBool("isMoving", false);
    }
    public IEnumerator Step()
    {
        if (stepping) yield break; // prevent overlapping steps
        stepping = true;

        stepsTaken++;
        if (stepsTaken >= stepsToWin)
        {
            won = true;
        }

        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + new Vector3(0, 0, moveDistance);

        float elapsed = 0f;

        while (elapsed < stepTime)
        {
            animator.SetBool("isMoving", true);
            elapsed += Time.deltaTime;
            float t = elapsed / stepTime;
            transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        transform.position = endPos;
        animator.SetBool("isMoving", false);
        stepping = false;
    }
}
