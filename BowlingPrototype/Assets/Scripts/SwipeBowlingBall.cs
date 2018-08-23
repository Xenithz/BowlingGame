using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeBowlingBall : MonoBehaviour 
{
    public GameObject ball;

    [SerializeField]
	private Vector3 startingPosition;
    private float startTime;

    //Expected values for normalization
    [SerializeField]
    private float expectedMinimum = 50f;
    [SerializeField]
    private float expectedMaximum = 60f;
    [SerializeField]
    private float desiredMinimum = 15f;
    [SerializeField]
    private float desiredMaximum = 20f;

    private void Update()
    {
        ProccessInput();
    }

    private void SaveValues()
    {
        startingPosition = Input.mousePosition;
        startTime = Time.time;
    }

    private void ProccessInput()
    {
        if(Input.GetMouseButtonDown(0))
        {
            SaveValues();
        }
        if(Input.GetMouseButtonUp(0))
        {
            CalculateForce();
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

        Vector3 velocity = (ball.transform.rotation * direction).normalized * power;


    }
}
