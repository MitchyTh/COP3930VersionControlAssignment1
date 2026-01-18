using UnityEngine;

public class moveSystemScript : MonoBehaviour
{
    public Animator animator;
    private bool walking;
    private bool success;
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
        transform.position += new Vector3(1, 0, 0);
    }
}
