﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BallState
{
    dontReset,
    resetMe
}
public class SwipeBowlingBall : MonoBehaviour 
{
    public GameObject ball;

    private int frameTrack;

    [SerializeField]
    private int maxFrames = 5;

    [SerializeField]
	private Vector3 startingPosition;
    private Vector3 direction;
    private float startTime;
    [SerializeField]
    private float powerMultiplier;

    private Rigidbody myRigidBody;

    private bool flickOnce;
    private bool checkScoreOnce;

    [SerializeField]
    private GameObject ballSpawn;

    [SerializeField]
    private Vector3 gizmoSize;

    [SerializeField]
    private List<Vector2> deltaList;

    private Plane groundPlane;

    private Vector3 startPosition;

    private Vector3 endPosition;

    private const float scaleConst = 1000f;

    public bool detected;

    [SerializeField]
    private float velocityClamp;

    public GameObject killBox;

    public GameObject normalBox;

    public bool shouldTrackTouch;

    public float myTimer;

    public float targetTime;

    private void Start()
    {
        myTimer = 0f;
        shouldTrackTouch = true;
        deltaList = new List<Vector2>();
        myRigidBody = ball.GetComponent<Rigidbody>();
        flickOnce = true;
        checkScoreOnce = true;
        groundPlane = new Plane(Vector3.up, 0f);
        powerMultiplier *= Screen.width / scaleConst;
        GenerateBallSpawn();
        myRigidBody.Sleep();
        detected = false;
    }

    private void Update()
    {
        ProccessInput();

        if(Input.GetKeyDown(KeyCode.Q))
        {
            ResetBallAndScore();    
        }
        if(Input.GetKeyDown(KeyCode.E))
        {
            ResetBallOnly();
        }

        // if(flickOnce == false)
        // {
        //     if(myRigidBody.velocity.magnitude <= 0.25f)
        //     {
        //         CallScoreCalculate();
        //     }
        // }

        if(myRigidBody.velocity.magnitude <= 0.25f && flickOnce == false)
        {
            SetTime();
        }

        if(myTimer >= targetTime)
        {
            ResetBallAndScore();            
            flickOnce = true;
            myTimer = 0;
        }
    }

    private void SetTime()
    {
        myTimer += Time.deltaTime;
    }

    private void OnCollisionEnter(Collision myCollision)
    {
        if(myCollision.gameObject.tag == "Pin")
        {
            if(checkScoreOnce == true)
            {
                killBox.SetActive(false);
                CallScoreCalculate();
            }
        }
        if(myCollision.gameObject.tag == "KillBox")
        {
            CallScoreCalculate();
        }
    }

    private void ProccessInput()
    {
        if(detected == true)
        {
            if(flickOnce == true && shouldTrackTouch == true)
            {
                if(Input.touchCount > 0)
                {
                    Touch touch = Input.GetTouch(0);

                    if(touch.phase == TouchPhase.Began)
                    {
                        Debug.Log("Started touching");
                        myRigidBody.WakeUp();
                        deltaList.Clear();
                        frameTrack=0;
                    }
                    if(touch.phase == TouchPhase.Moved)
                    {
                        frameTrack++;
                    
                        if(frameTrack <= maxFrames)
                        {
                            SaveDeltaPosition(touch);

                            if(frameTrack == 1)
                            {
                                startingPosition = SendRay(touch);
                            }
                        }
                    }
                    if(touch.phase == TouchPhase.Ended||frameTrack>=maxFrames)
                    {
                        endPosition = SendRay(touch);
                        CalculateFlick();
                        flickOnce = false;
                    }
                }
            }
            else if(flickOnce == false)
            {
                Debug.Log("Can't flick anymore!");
            }
        }
    }

    private Vector3 SendRay(Touch touchToPass)
    {
        Ray myRay = Camera.main.ScreenPointToRay(touchToPass.position);
        float enter = 0f;
        if(groundPlane.Raycast(myRay, out enter))
        {
            return myRay.GetPoint(enter);
        }
        else
        {
            return Vector3.zero;
        }
    }

    private void SaveDeltaPosition(Touch myTouch)
    {
        Vector2 deltaToAdd = myTouch.deltaPosition / Time.deltaTime;
        deltaList.Add(deltaToAdd);
    }

    private void CalculateFlick()
    {
        Vector2 average = Vector2.zero;
        for(int i = 0; i < deltaList.Count; i++)
        {
            average += deltaList[i];
        }

        average /= deltaList.Count;

        Debug.Log("Average velocity is: " + average);

        Vector3 direction = (endPosition - startingPosition).normalized;
        direction *= average.magnitude;
        direction *= powerMultiplier;
        direction = Vector3.ClampMagnitude(direction, velocityClamp);
        myRigidBody.AddForce(direction, ForceMode.Impulse);
        Debug.Log("Added force: " + direction);

    }

    public void ResetBallAndScore()
    {
        myRigidBody.Sleep();
        myRigidBody.velocity = Vector3.zero;
        myRigidBody.angularVelocity = Vector3.zero;
        ball.transform.position = ballSpawn.transform.position;
        ball.transform.rotation = Quaternion.identity;
        flickOnce = true;
        ScoreManager.instance.ResetScore();
        checkScoreOnce = true;
    }

    public void ResetBallOnly()
    {
        myRigidBody.isKinematic = true;
        myRigidBody.velocity = Vector3.zero;
        myRigidBody.angularVelocity = Vector3.zero;
        myRigidBody.Sleep();
        ball.transform.position = ballSpawn.transform.position;
        ball.transform.rotation = Quaternion.Euler(Vector3.zero);
        flickOnce = true;
        checkScoreOnce = true;
        myRigidBody.isKinematic = false;
    }

    private void CallScoreCalculate()
    {
        checkScoreOnce = false;
        ScoreManager.instance.DisplayScore();
    }

    private void GenerateBallSpawn()
    {
        var emptyGameObject = new GameObject();
		emptyGameObject.name = "templateBall";

        var temp = Instantiate(emptyGameObject, ball.transform.position, Quaternion.identity);
		temp.name = "BallPosition";
		temp.transform.localScale = ball.transform.localScale;
		temp.transform.SetParent(ball.transform.parent);

        ballSpawn = temp;
    }
}
