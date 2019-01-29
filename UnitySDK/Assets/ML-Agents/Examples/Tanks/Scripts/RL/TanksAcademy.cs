using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class TanksAcademy : Academy {
	
	public override void InitializeAcademy()
	{
		Monitor.SetActive(true);
		Debug.Log("Academy initialized");
	}

	public override void AcademyReset()
	{
		Debug.Log("Academy reset");
	}

	public override void AcademyStep()
	{
//		Debug.Log("Academy step");
	}
}
