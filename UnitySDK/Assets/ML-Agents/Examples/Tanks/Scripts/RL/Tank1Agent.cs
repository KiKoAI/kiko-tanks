using System;
using System.Collections;
using System.Collections.Generic;
using Complete;
using UnityEngine;
using MLAgents;

public class Tank1Agent : Agent
{

	public Vector3 Target;
	public TankMovement TankMovement; 
	
	private TankMovement _tankMovement;
	private TankHealth _tankHealth;
	private TankShooting _tankShooting;
	
	// Use this for initialization
	void Start ()
	{
		_tankMovement = GetComponent<TankMovement>();
		_tankHealth = GetComponent<TankHealth>();
		_tankShooting = GetComponent<TankShooting>();
	}

//	private void Update()
//	{
//		Debug.Log(gameObject.transform.position);
//	}

	public override void AgentReset()
	{
		if (gameObject.transform.position == Target)
		{
			Vector3 position = new Vector3(-3, 0, 30);
			gameObject.transform.position = position;
		}
	}

	public override void CollectObservations()
	{
		AddVectorObs(gameObject.transform.position);
		AddVectorObs(_tankHealth.GetHealth());
		AddVectorObs(Target);
	}
	
	public override void AgentAction(float[] vectorAction, string textAction)
	{
		Monitor.Log("Actions", vectorAction);
		
		_tankMovement.Move(vectorAction[0]);
		_tankMovement.Turn(vectorAction[1]);
		_tankShooting.Fire(vectorAction[2] * 30);
		
		// Rewards
		float distanceToTarget = Vector3.Distance(gameObject.transform.position,
			Target);

		// Reached target
		if (distanceToTarget < 1.42f)
		{
			Monitor.Log("Target", distanceToTarget);
			SetReward(1.0f);
			Done();
		}
	}
}
