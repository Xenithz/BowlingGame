using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour 
{
	public static ScoreManager instance;
	
	[SerializeField]
	private int scoreValue;
	[SerializeField]
	private int waitingTime = 2;

	[SerializeField]
	private float checkThreshold = 0.6f;

	public Text scoreText;

	public GameObject[] pinArray;

	private void Start()
	{
		instance = this;
	}

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
		StartCoroutine(CheckPins());
	}

	private void ScoreFade()
	{
		scoreText.text = scoreValue.ToString();
		// yield return new WaitForSeconds(waitingTime);
		// scoreText.text = "";
		// scoreValue = 0;
	}

	IEnumerator CheckPins()
	{
		yield return new WaitForSeconds(5f);
		for(int i = 0; i < pinArray.Length; i++)
		{
			if(UprightCheck(pinArray[i]))
			{
				scoreValue++;
				Debug.Log("Pin "+ i +" fell!");
				Debug.Log(scoreValue);
			}
			else
			{
				Debug.Log("Pin "+ i +" didn't fall!");
			}
		}
		ScoreFade();
	}

	private bool UprightCheck(GameObject gameObjectToCheck)
	{
		Debug.Log(gameObjectToCheck.transform.up.y);
		return gameObjectToCheck.transform.up.y < checkThreshold;
	}

	public void ResetScore()
	{
		scoreText.text = "";
		scoreValue = 0;
	}
}
