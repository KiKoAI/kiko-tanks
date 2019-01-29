using System;
using System.Collections;
using System.Collections.Generic;
using Complete;
using UnityEngine;
using UnityEngine.UI;
using MLAgents;

public class Tank1Agent : Agent
{
	public bool IsWinner = false;
	public bool HasAttacked = false;
	public float AttackPoint = 0;
	
	private GameManager _gameManager;
	private TankMovement _tankMovement;
	private TankHealth _tankHealth;
	private TankShooting _tankShooting;

	private int _playerNumber;
	private Vector3 _enemyPosition;
	private float _tankHealthValue;
	private bool _hasColided;
	private bool _getDamaged;
	private float _damageValue;
	private bool _isDead;
	private float _distanceToEnemy;
	private float _tempDistanceToEnemy;
	private GameObject _enemy;
	private Tank1Agent _enemyAgent;
	
	// Use this for initialization
	void Start ()
	{
		_tankMovement = GetComponent<TankMovement>();
		_tankHealth = GetComponent<TankHealth>();
		_tankShooting = GetComponent<TankShooting>();
		_playerNumber = _tankMovement.m_PlayerNumber;

		_gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
		_enemy = _gameManager.GetTankManager(_playerNumber == 1 ? 2 : 1).m_Movement.gameObject;
		_enemyPosition = _enemy.transform.position;
		_enemyAgent = _enemy.GetComponent<Tank1Agent>();
		
		
//		_enemyPosition = _gameManager.GetTankPosition(_playerNumber == 1 ? 2 : 1);
		_distanceToEnemy = Vector3.Distance(gameObject.transform.position, _enemyPosition);
		_tankHealthValue = _tankHealth.GetHealth();
		_isDead = false;
	}
	
	void OnCollisionEnter(Collision collision)
	{
		_hasColided = true;
	}

	public override void AgentReset()
	{
		
	}

	public override void CollectObservations()
	{
		AddVectorObs(Mathf.FloorToInt(gameObject.transform.position.x) / 50);
		AddVectorObs(Mathf.FloorToInt(gameObject.transform.position.z) / 50);
		
//		AddVectorObs(Mathf.FloorToInt(gameObject.transform.rotation.eulerAngles.y) / 180 - 1);
		
		AddVectorObs(Mathf.FloorToInt(_enemyPosition.x) / 50);
		AddVectorObs(Mathf.FloorToInt(_enemyPosition.z) / 50);
		
//		AddVectorObs(Mathf.FloorToInt(_tankHealth.GetHealth()) / 100);
	}
	
	public override void AgentAction(float[] vectorAction, string textAction)
	{
		Monitor.Log("Actions", vectorAction);
		Monitor.Log("Damaged", (_getDamaged ? 1 : 0));

		_tankMovement.Move(vectorAction[0]);
		_tankMovement.Turn(vectorAction[1]);
		_tankShooting._launchForce = vectorAction[2] * 15 + 15;
		
		_enemyPosition = _enemy.transform.position;
		
		if (Math.Abs(_tankHealthValue - _tankHealth.GetHealth()) > 0.01)
		{
			_getDamaged = true;
			_damageValue = Mathf.Abs(_tankHealthValue - _tankHealth.GetHealth());
//			Debug.Log("damage : " + _damageValue);
			_tankHealthValue = _tankHealth.GetHealth();
		}

		if (_tankHealth.m_Dead && !_isDead)
		{
			_isDead = true;
		}
		
		var distance = Vector3.Distance(gameObject.transform.position, _enemyPosition);
		
		if (Math.Abs(distance - _distanceToEnemy) > 0.001)
		{
			if (distance > _tempDistanceToEnemy)
			{
//				Debug.Log(-(Mathf.Abs(distance / 100)));
				AddReward(-(Mathf.Abs(distance / 100)));
			}
			else
			{
				if (distance < _distanceToEnemy)
				{
//					Debug.Log("+5");
					AddReward(1.0f);
					_distanceToEnemy = distance;
				}
				else
				{
//					Debug.Log("0.1");
					AddReward(0.1f);	
				}
			}
			_tempDistanceToEnemy = distance;
		}
		
		
		if (_hasColided)
		{
			AddReward(-0.5f);
			_hasColided = false;
		}

		if (HasAttacked)
		{
			AddReward(2 * (AttackPoint / 100));
			HasAttacked = false;
			AttackPoint = 0;
		}
		
		if (_getDamaged)
		{
			AddReward(-(_damageValue / 100));
			_enemyAgent.HasAttacked = true;
			_enemyAgent.AttackPoint = _damageValue;
			_damageValue = 0;
			_getDamaged = false;
		}

		if (IsWinner)
		{
			AddReward(2.0f);
			IsWinner = false;
			Done();
		}
		
		if (_isDead)
		{
//			_enemyAgent.IsWinner = true;
			Done();
			_isDead = false;
			_tankHealthValue = 100;
			AddReward(-1.0f);
			gameObject.SetActive (false);
		}
	}
		
	public override void AgentOnDone()
	{
//		Debug.Log("agentOnDone");
		AgentReset();
	}
}
