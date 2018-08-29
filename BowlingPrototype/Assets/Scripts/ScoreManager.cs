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

	public List<Transform> pinPositionsList;

	private void Start()
	{
		instance = this;
		pinArray = GameObject.FindGameObjectsWithTag("Pin");
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
		ResetPins();
		// yield return new WaitForSeconds(waitingTime);
		// scoreText.text = "";
		// scoreValue = 0;
	}

	IEnumerator CheckPins()
	{
		Debug.Log("Check pins was called!");
		yield return new WaitForSeconds(5f);
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
	}

	private void ResetPins()
	{
		for(int i = 0; i < pinArray.Length; i++)
		{
			pinArray[i].transform.position = pinPositionsList[i].position;
			Debug.Log("Set Pin " + i + " to position " + i);
			pinArray[i].transform.rotation = Quaternion.identity;
			Debug.Log("Set Pin " + i + " to default rotation");
			pinArray[i].GetComponent<Rigidbody>().velocity = Vector3.zero;
			pinArray[i].GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
			Debug.Log("Set Pin " + i + " to zero velocity/angular velocity");
			
		}
	}

	private void GeneratePinPos()
	{
		var emptyGameObject = new GameObject();
		emptyGameObject.name = "template";
		
		for(int i = 0; i < pinArray.Length; i++)
		{
			var temp = Instantiate(emptyGameObject, pinArray[i].transform.position, Quaternion.identity);
			temp.name = "PinPosition" + i;
			temp.transform.localScale = pinArray[i].transform.localScale;
			pinPositionsList.Add(temp.transform);
		}
	}
}
