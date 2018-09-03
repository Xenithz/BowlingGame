using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour 
{
	public SwipeBowlingBall myBall;

	public static ScoreManager instance;
	
	[SerializeField]
	private int scoreValue;
	[SerializeField]
	private float waitingTime;

	[SerializeField]
	private float checkThreshold = 0.6f;

	public Text scoreText;

	public GameObject[] pinArray;

	public List<Transform> pinPositionsList;

	

	private void Start()
	{
		instance = this;
		pinArray = GameObject.FindGameObjectsWithTag("Pin");
		myBall = GameObject.FindGameObjectWithTag("Player").GetComponent<SwipeBowlingBall>();
		GeneratePinPos();
	}

	private void Update()
	{
		if(Input.GetKeyDown(KeyCode.Q))
		{
			CheckPins();
			DisplayScore();
			StopAllCoroutines();
		}
	}

	public void DisplayScore()
	{
		StartCoroutine(CheckPins());
	}

	private void ScoreFade()
	{
		scoreText.text = scoreValue.ToString();
		StartCoroutine(EndStuff());
		// yield return new WaitForSeconds(waitingTime);
		// scoreText.text = "";
		// scoreValue = 0;
	}

	IEnumerator EndStuff()
	{
		Debug.Log("No more");
		yield return new WaitForSeconds(1f);
		myBall.ResetBallAndScore();
		myBall.killBox.SetActive(true);
	}

	IEnumerator CheckPins()
	{
		Debug.Log("Check pins was called!");
		yield return new WaitForSeconds(2f);
		for(int i = 0; i < pinArray.Length; i++)
		{
			Debug.Log("There are " + pinArray.Length + " pins!");
			if(UprightCheck(pinArray[i]))
			{
				Debug.Log("iteration " + i);
				scoreValue++;
				Debug.Log("Pin "+ i +" fell!");
				Debug.Log("current score is: " + scoreValue);
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
		ResetPins();
		myBall.ResetBallOnly();
	}

	private void ResetPins()
	{
		for(int i = 0; i < pinArray.Length; i++)
		{
			pinArray[i].transform.position = pinPositionsList[i].position;
			Debug.Log("Set Pin " + i + " to position " + i);
			pinArray[i].transform.rotation = pinPositionsList[i].rotation;
			Debug.Log("Set Pin " + i + " to default rotation");
			pinArray[i].GetComponent<Rigidbody>().velocity = Vector3.zero;
			pinArray[i].GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
			Debug.Log("Set Pin " + i + " to zero velocity/angular velocity");
			
		}
	}

	private void GeneratePinPos()
	{
		var emptyGameObject = new GameObject();
		emptyGameObject.name = "templatePin";
		
		for(int i = 0; i < pinArray.Length; i++)
		{
			var temp = Instantiate(emptyGameObject, pinArray[i].transform.position, Quaternion.Euler(-0f, 0f, 0f));
			temp.name = "PinPosition" + i;
			temp.transform.localScale = pinArray[i].transform.localScale;
			temp.transform.SetParent(pinArray[i].transform.parent);
			pinPositionsList.Add(temp.transform);
		}
	}
}
