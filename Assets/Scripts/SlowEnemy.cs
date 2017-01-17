using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowEnemy : MonoBehaviour
{

    public Transform[] patrolPoints;
    int currentPoint;
    public float speed = 0.5f;
    public float timeStill = 2f;

    private Animator anim;

    // Use this for initialization
    void Start()
    {
        anim = GetComponent<Animator>();
        StartCoroutine("Patrol");
        anim.SetBool("Walking", true);
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator Patrol()
    {
        while (true)
        {
            if (transform.position.x == patrolPoints[currentPoint].position.x)
            {
                currentPoint++;
                anim.SetBool("Walking", false);
                yield return new WaitForSeconds(timeStill);
                anim.SetBool("Walking", true);
            }

            if (currentPoint >= patrolPoints.Length)
            {
                currentPoint = 0;
            }

            transform.position = Vector2.MoveTowards(transform.position, new Vector2(patrolPoints[currentPoint].position.x, transform.position.y), speed);

            if (transform.position.x < patrolPoints[currentPoint].position.x)
                transform.localScale = new Vector3(-1, 1, 1);
            else if (transform.position.x > patrolPoints[currentPoint].position.x)
                transform.localScale = Vector3.one;

            yield return null;
        }
    }

}
