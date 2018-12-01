using System.Collections;
using System.Collections.Generic;
using Graphene.DisconnectionDungeon;
using UnityEngine;

public class ForceInit : MonoBehaviour {
	private DDManager _manager;

	void Start ()
	{
		_manager = DDManager.Instance;
	}
}
