using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeBowlingBall : MonoBehaviour 
{
    //TODO Convert to mobile control also

    public GameObject ball;

    [SerializeField]
	private Vector3 startingPosition;
    private float startTime;
    [SerializeField]
    private float powerMultiplier;

    //Expected values for normalization
    [SerializeField]
    private float expectedMinimum = 50f;
    [SerializeField]
    private float expectedMaximum = 60f;
    [SerializeField]
    private float desiredMinimum = 15f;
    [SerializeField]
    private float desiredMaximum = 20f;

    private Rigidbody myRigidBody;

    private bool flickOnce;
    private bool checkScoreOnce;

    [SerializeField]
    private Transform ballSpawn;

    [SerializeField]
    private Vector3 gizmoSize;

    private void Start()
    {
        myRigidBody = ball.GetComponent<Rigidbody>();
        flickOnce = true;
        checkScoreOnce = true;
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

    private void SaveValues()
    {
        startingPosition = Input.mousePosition;
        startTime = Time.time;
    }

    private void ProccessInput()
    {
        if(flickOnce == true)
        {
            if(Input.GetMouseButtonDown(0))
            {
                SaveValues();
            }
            if(Input.GetMouseButtonUp(0))
            {
                CalculateForce();
                flickOnce = false;
            }
        }
        else if(flickOnce == false)
        {
            Debug.Log("Can't flick anymore!");
        }
    }

    private void CalculateForce()
    {
        Vector3 endingPosition = Input.mousePosition;
        float endTime = Time.time;

        startingPosition.z = 0.1f;
        endingPosition.z = 0.1f;

        startingPosition = Camera.main.ScreenToWorldPoint(startingPosition);
        endingPosition = Camera.main.ScreenToWorldPoint(endingPosition);

        float duration = endTime - startTime;

        Vector3 direction = endingPosition - startingPosition;

        float distance = direction.magnitude;

        float power = distance / duration;

        power -= expectedMinimum;
        power /= expectedMaximum - expectedMinimum;

        power = Mathf.Clamp01(power);

        power *= desiredMaximum - desiredMinimum;
        power += desiredMinimum;

        Vector3 velocity = (ball.transform.rotation * direction).normalized * power * powerMultiplier;

        velocity.y = 0f;

        Debug.Log(velocity);

        myRigidBody.AddForce(velocity, ForceMode.Impulse);
    }

    public void ResetBallAndScore()
    {
        ball.transform.position = ballSpawn.transform.position;
        ball.transform.rotation = Quaternion.identity;
        myRigidBody.velocity = Vector3.zero;
        myRigidBody.angularVelocity = Vector3.zero;
        flickOnce = true;
        ScoreManager.instance.ResetScore();
        checkScoreOnce = false;
    }
    
    private void OnDrawGizmos() 
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(ballSpawn.transform.position, gizmoSize);
    }

    private void CallScoreCalculate()
    {
        ScoreManager.instance.DisplayScore();
    }
}
