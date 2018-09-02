using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonFunctions : MonoBehaviour 
{
	private SwipeBowlingBall myBowlingBall;

	private void Start()
	{
		myBowlingBall = FindObjectOfType<SwipeBowlingBall>();
	}
	
	public void CompleteReset()
	{
		myBowlingBall.ResetBallAndScore();
	}

	public void BallReset()
	{
		myBowlingBall.ResetBallOnly();
	}

	public void ToggleTouchTrack()
	{
		StartCoroutine(TouchToggle());
	}

	IEnumerator TouchToggle()
	{
		yield return new WaitForSeconds(1f);
		myBowlingBall.shouldTrackTouch = !myBowlingBall.shouldTrackTouch;
	}
}
