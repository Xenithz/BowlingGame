using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour 
{
	private int scoreValue;
	[SerializeField]
	private int waitingTime = 2;

	[SerializeField]
	private float checkThreshold = 0.6f;

	public Text scoreText;

	public GameObject[] pinArray;

	private void Update()
	{
		if(Input.GetKeyDown(KeyCode.Q))
		{
			CheckPins();
			DisplayScore();
		}
	}

	public void DisplayScore()
	{
		StartCoroutine(ScoreFade());
	}

	IEnumerator ScoreFade()
	{
		scoreText.text = scoreValue.ToString();
		yield return new WaitForSeconds(waitingTime);
		scoreText.text = "";
		scoreValue = 0;
	}

	public void CheckPins()
	{
		for(int i = 0; i < pinArray.Length; i++)
		{
			if(UprightCheck(pinArray[i]))
			{
				Debug.Log("Pin "+ i +" fell!");
				scoreValue++;
			}
			else
			{
				Debug.Log("Pin "+ i +" didn't fall!");
			}
		}
	}

	private bool UprightCheck(GameObject gameObjectToCheck)
	{
		Debug.Log(gameObjectToCheck.transform.up.y);
		return gameObjectToCheck.transform.up.y < checkThreshold;
	}
}
