using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class ProtoDetectHandler : DefaultTrackableEventHandler
{
	[SerializeField]
	private SwipeBowlingBall myBowlingBall;

	protected override void Start()
	{
		mTrackableBehaviour = GetComponent<TrackableBehaviour>();
        if (mTrackableBehaviour)
            mTrackableBehaviour.RegisterTrackableEventHandler(this);

		var rigidBodyComponents = GetComponentsInChildren<Rigidbody>(true);

		foreach (var component in rigidBodyComponents)
			component.Sleep();
	}

	protected override void	 OnTrackingFound()
	{
		base.OnTrackingFound();

		var rigidBodyComponents = GetComponentsInChildren<Rigidbody>(true);

		foreach (var component in rigidBodyComponents)
			component.WakeUp();

		myBowlingBall.detected = true;
	}

	protected override void OnTrackingLost()
	{
		base.OnTrackingLost();

		//FindObjectOfType<SwipeBowlingBall>().ResetBallAndScore();

		var rigidBodyComponents = GetComponentsInChildren<Rigidbody>(true);

		foreach (var component in rigidBodyComponents)
			component.Sleep();

		myBowlingBall.detected = false;
	}
}
