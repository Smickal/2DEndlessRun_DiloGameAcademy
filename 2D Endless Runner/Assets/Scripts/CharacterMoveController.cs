using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMoveController : MonoBehaviour {

    [Header ("Movement")]
    public float moveAccel;
    public float maxSpeed;

    [Header("jump")]
    public float JumpAccel;


    [Header("Ground Raycast")]
    public float groundRaycastDistance;
    public LayerMask groundLayerMask;

    private Rigidbody2D rig;
    private Animator anim;
    private CharacterSoundController sound;
    private bool isJumping;
    private bool isOnGround;


    [Header("Scoring")]
    public ScoreController score;
    public float scoringRatio;
    private float lastPositionX;


    [Header("GameOver")]
    public GameObject gameOverScreen;
    public float fallPositionY;

    [Header("Camera")]
    public CameraMoveController gameCamera;

    private void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sound = GetComponent<CharacterSoundController>();
    }

    private void Update()
    {
        //jumping char
        if (Input.GetMouseButtonDown(0))
        {
            if (isOnGround)
            {
                isJumping = true;
                sound.PlayJump();
                
            }
            
        }

        
        
        //change animation
        anim.SetBool("isOnGround", isOnGround);

        //calculate Score
        int distancePassed = Mathf.FloorToInt(transform.position.x - lastPositionX);
        int scoreIncrement = Mathf.FloorToInt(distancePassed / scoringRatio);

        if(scoreIncrement > 0)
        {
            score.IncreaseCurrentScore(scoreIncrement);
            lastPositionX += distancePassed;
        }

        if(transform.position.y < fallPositionY)
        {
            GameOver();
        }
    }
    
    private void GameOver()
    {
        score.FinishScoring();

        gameCamera.enabled = false;

        gameOverScreen.SetActive(true);

        this.enabled = false;
    }

    private void FixedUpdate()
    {

        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, groundRaycastDistance, groundLayerMask);
        if (hit)
        {
            if (!isOnGround && rig.velocity.y == 0)
            {
                isOnGround = true;
            }
        }
        else
        {
            isOnGround = false;
        }

        //calculate velocity vector
        Vector2 velocityVector = rig.velocity;

        if (isJumping)
        {
            velocityVector.y += JumpAccel;
            isJumping = false;
        }

        velocityVector.x = Mathf.Clamp(velocityVector.x + moveAccel * Time.deltaTime, 0.0f, maxSpeed);


        rig.velocity = velocityVector;

        

    }

   

    private void OnDrawGizmos()
    {
        Debug.DrawLine(transform.position, transform.position + (Vector3.down * groundRaycastDistance), Color.white);
    }
}

    
