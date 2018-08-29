using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private void Start()
    {
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
    }

    private void OnCollisionEnter(Collision myCollision)
    {
        if(myCollision.gameObject.tag == "Pin")
        {
            if(checkScoreOnce == true)
            {
                CallScoreCalculate();
            }
        }
    }

    private void ProccessInput()
    {
        if(detected == true)
        {
            if(flickOnce == true)
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
        myRigidBody.AddForce(direction * powerMultiplier, ForceMode.Impulse);
        Debug.Log("Added force: " + direction);

    }

    public void ResetBallAndScore()
    {
        ball.transform.position = ballSpawn.transform.position;
        ball.transform.rotation = Quaternion.identity;
        myRigidBody.velocity = Vector3.zero;
        myRigidBody.angularVelocity = Vector3.zero;
        myRigidBody.Sleep();
        flickOnce = true;
        ScoreManager.instance.ResetScore();
        checkScoreOnce = true;
    }

    public void ResetBallOnly()
    {
        ball.transform.position = ballSpawn.transform.position;
        ball.transform.rotation = Quaternion.identity;
        myRigidBody.velocity = Vector3.zero;
        myRigidBody.angularVelocity = Vector3.zero;
        myRigidBody.Sleep();
        flickOnce = true;
        checkScoreOnce = true;
    }
    
    private void OnDrawGizmos() 
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(ballSpawn.transform.position, gizmoSize);
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
