using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour {

    [HideInInspector] public bool facingRight = true;
    [HideInInspector] public bool jump = false;

    public float moveForce = 365f;
    public float maxSpeed = 5f;
    public float jumpForce = 10f;
    public Transform groundCheck;
    public bool isDigging = false;
    public float boxDistance = 1.5f;
    public float barkDistance = 5f;
    public LayerMask boxMask;
    public GameObject spark;
    public GameObject restartText;

    //Lifes
    public int lives;
    public bool isImmune;
    public float immuneCounter;
    public float immuneTime;

    //Jump
    private float timer;
    float timeLeft = 10f;

    private bool grounded = false;
    private bool boxGrounded = false;
    private bool isPushing = false;
    private Animator anim;
    private Rigidbody2D rb2d;
    private GameObject box;
    private GameObject[] digSpots;
    private GameObject[] sparkItems;

    private bool isDead = false;

    // Use this for initialization
    void Awake () {

        anim = GetComponent <Animator>();
        rb2d = GetComponent<Rigidbody2D>();
    }
	
	// Update is called once per frame
	void Update () {

        grounded = Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground"));
        boxGrounded = Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Box"));

        if ((Input.GetButtonDown("Jump") && grounded) || (Input.GetButtonDown("Jump") && boxGrounded))
        {
            jump = true;
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            isDigging = true;
        } else if (Input.GetKeyUp(KeyCode.Z))
            isDigging = false;

        Physics2D.queriesStartInColliders = false;
        RaycastHit2D boxHit = Physics2D.Raycast(transform.position, Vector2.left * transform.localScale.x, boxDistance, boxMask);

        if (boxHit.collider != null && boxHit.collider.gameObject.tag=="Crate" && Input.GetKeyDown(KeyCode.X))
        {
            isPushing = true;
            box = boxHit.collider.gameObject;
            box.GetComponent<FixedJoint2D>().enabled = true;
            box.GetComponent<BoxPull>().beingPushed = true;
            box.GetComponent<FixedJoint2D>().connectedBody = this.GetComponent<Rigidbody2D>();
        } else if (Input.GetKeyUp(KeyCode.X))
        {
            isPushing = false;
            box.GetComponent<FixedJoint2D>().enabled = false;
            box.GetComponent<BoxPull>().beingPushed = false;
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            RaycastHit2D barkHit = Physics2D.Raycast(transform.position, Vector2.left * transform.localScale.x, barkDistance, boxMask);

        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            digSpots = GameObject.FindGameObjectsWithTag("d_ground");

            Vector3 pos = new Vector3(0f, 3f, 0f);

            foreach (GameObject digSpot in digSpots)
            {

                Instantiate(spark, digSpot.transform.position + pos, digSpot.transform.rotation);
            }
        } else if (Input.GetKeyUp(KeyCode.V))
        {
            sparkItems = GameObject.FindGameObjectsWithTag("Spark");
            foreach (GameObject sparkItem in sparkItems)
            {
                Destroy(sparkItem);
            }
            
        }

        if (isDead == true)
        {

            restartText.SetActive(true);
            if (Input.GetKeyDown(KeyCode.R))
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

            return;

        }
    }

    void FixedUpdate()
    {
        float h = Input.GetAxis("Horizontal");

        anim.SetFloat("Speed", Mathf.Abs(h));

        if(h * rb2d.velocity.x < maxSpeed)
            rb2d.AddForce(Vector2.right * h * moveForce);

        if (Mathf.Abs(rb2d.velocity.x) > maxSpeed)
            rb2d.velocity = new Vector2(Mathf.Sign(rb2d.velocity.x) * maxSpeed, rb2d.velocity.y);

        if (h < 0 && !facingRight && !isPushing)
            Flip();
        else if (h > 0 && facingRight && !isPushing)
            Flip();

        if (jump)
        {
            anim.SetTrigger("Jump");
                rb2d.AddForce(new Vector2(0f, jumpForce));               
                jump = false;


        }

    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Enemy" && !isDead && lives <=1)
        {
            moveForce = 0f;
            jumpForce = 0f;
            isDead = true;

        } else if (collision.gameObject.tag == "Enemy" && lives > 1)
        {
            lives--;
            StartCoroutine("ImmuneTime");
            isImmune = true;
        }


        if (isDigging)
        {
            if (collision.collider.tag == "d_ground")
            {
                collision.collider.isTrigger = true;
            }
        }
    }

    IEnumerator ImmuneTime()
    {
        yield return new WaitForSeconds(immuneTime);
        isImmune = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawLine (transform.position, (Vector2)transform.position + Vector2.left * transform.localScale.x * barkDistance);

    }

}
