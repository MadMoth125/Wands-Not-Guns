using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class EnemyTarget : MonoBehaviour
{
	public Transform target;
	
	public AIPath aiPath;
	
	// Start is called before the first frame update
	void Start()
	{
		
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		if (target != null) aiPath.destination = target.position;
	}
}
