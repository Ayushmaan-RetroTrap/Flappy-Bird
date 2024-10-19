using UnityEngine;
using System.Collections;
using DG.Tweening;
using System;

public class BirdControl : MonoBehaviour
{
    public float upSpeed = 10f;  // Speed at which the bird moves up
    public int rotateRate = 10;
    public GameObject scoreMgr;

    public AudioClip jumpUp;
    public AudioClip hit;
    public AudioClip score;

    public bool inGame = false;
    private bool dead = false;
    private bool landed = false;

    private Sequence birdSequence;
    private Rigidbody2D rb2D;

    // Use this for initialization
    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();  // Get the Rigidbody2D component
        float birdOffset = 0.05f;
        float birdTime = 0.3f;
        float birdStartY = transform.position.y;

        birdSequence = DOTween.Sequence();

        birdSequence.Append(transform.DOMoveY(birdStartY + birdOffset, birdTime).SetEase(Ease.Linear))
            .Append(transform.DOMoveY(birdStartY - 2 * birdOffset, 2 * birdTime).SetEase(Ease.Linear))
            .Append(transform.DOMoveY(birdStartY, birdTime).SetEase(Ease.Linear))
            .SetLoops(-1);
    }

    // Update is called once per frame
    void Update()
    {
        if (!inGame)
        {
            return;
        }

        birdSequence.Kill();

        if (!dead)
        {
            FollowMouse();  // Follow the mouse cursor's vertical position
        }

        if (!landed)
        {
            float v = rb2D.linearVelocity.y;  // Use linearVelocity to calculate rotation

            float rotate = Mathf.Min(Mathf.Max(-90, v * rotateRate + 60), 30);
            transform.rotation = Quaternion.Euler(0f, 0f, rotate);
        }
        else
        {
            rb2D.rotation = -90;
        }
    }

    // Move the bird towards the mouse cursor's Y position, maintaining its X position
    void FollowMouse()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);  // Get the mouse position in world space
        mousePos.z = 0;  // Set z position to 0 for 2D

        // Set the bird's Y position to the mouse Y position, keeping the X position unchanged
        rb2D.position = new Vector2(rb2D.position.x, mousePos.y);

        // Optionally, play jump sound (or another sound) when the Y position changes
        // Uncomment if you want to play sound when the bird moves vertically
        // AudioSource.PlayClipAtPoint(jumpUp, Vector3.zero);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.name == "land" || other.name == "pipe_up" || other.name == "pipe_down")
        {
            if (!dead)
            {
                GameObject[] objs = GameObject.FindGameObjectsWithTag("movable");
                foreach (GameObject g in objs)
                {
                    g.BroadcastMessage("GameOver");
                }

                GetComponent<Animator>().SetTrigger("die");
                AudioSource.PlayClipAtPoint(hit, Vector3.zero);
            }

            if (other.name == "land")
            {
                rb2D.gravityScale = 0;
                rb2D.linearVelocity = Vector2.zero;

                landed = true;
            }
        }

        if (other.name == "pass_trigger")
        {
            scoreMgr.GetComponent<ScoreMgr>().AddScore();
            AudioSource.PlayClipAtPoint(score, Vector3.zero);
        }
    }

    public void GameOver()
    {
        dead = true;
    }

    internal void JumpUp()
    {
        throw new NotImplementedException();
    }
}
    