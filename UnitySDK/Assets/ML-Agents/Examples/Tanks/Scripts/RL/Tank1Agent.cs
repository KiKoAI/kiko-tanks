using System;
using System.Collections;
using System.Collections.Generic;
using Complete;
using UnityEngine;
using UnityEngine.UI;
using MLAgents;

public class Tank1Agent : Agent
{

	public Vector3 Target;
	
	private TankMovement _tankMovement;
	private TankHealth _tankHealth;
	private TankShooting _tankShooting;

	public bool isDead;
	
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
		var health = _tankHealth.GetHealth();
		AddVectorObs(gameObject.transform.position);
		AddVectorObs(health);
		AddVectorObs(Target);
	}
	
	public override void AgentAction(float[] vectorAction, string textAction)
	{
		Monitor.Log("Actions", vectorAction);
		
		_tankMovement.Move(vectorAction[0]);
		_tankMovement.Turn(vectorAction[1]);
		_tankShooting._launchForce = vectorAction[2] * 20;
		
		float distanceToTarget = Vector3.Distance(gameObject.transform.position,
			Target);

		if (distanceToTarget < 1.42f)
		{
			Monitor.Log("Target", distanceToTarget);
			SetReward(10.0f);
			Done();
		}

		if (_tankHealth.m_Dead)
		{
			SetReward(-10);
		}
	}
}
