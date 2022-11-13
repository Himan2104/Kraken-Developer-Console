using UnityEngine;

public class Ball : MonoBehaviour
{
    Vector3 targetPos = Vector3.zero;
    public float speed = 1.0f;

    private void Start()
    {
        targetPos = transform.position;
    }


    void Update()
    {
        if(transform.position == targetPos)
        {
            targetPos = new Vector3(Random.Range(-10, 10), Random.Range(0, 6), Random.Range(-10, 10));
        }

        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
    }
}
