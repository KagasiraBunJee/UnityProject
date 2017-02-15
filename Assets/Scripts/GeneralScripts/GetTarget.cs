using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GetTarget : MonoBehaviour {
	
	public float radius;
	public GameObject selectedTarget;
	private float lastDistance;
	public bool firstStart;
	TeamConfig tc;
	
	UnitInfo uInfo;
	// Use this for initialization
	void Start () 
	{
		firstStart = true;
	}
	
	// Update is called once per frame
	void Update () 
	{
		radius = GetComponent<UnitInfo>().attackDistance;
		if(tc == null)
		{
			tc = Camera.main.transform.parent.GetComponent<TeamConfig>();
		}
		if(GetComponent<UnitInfo>().attackTarget == null)
		{
        	Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);
        	if(hitColliders.Length > 1)
			{
        		foreach(Collider target in hitColliders)
				{
					GameObject unit = target.transform.gameObject;
					if(unit.tag == "Unit")
					{
						UnitInfo ut = unit.GetComponent<UnitInfo>();
						if(tc.getId() != ut.owner)
						{
							if(!tc.alliance.Contains(ut.owner))
							{
								if(firstStart)
								{
									selectedTarget = unit;
									firstStart = false;
								}
								else if(lastDistance > Vector3.Distance(transform.position, unit.transform.position))
								{
									selectedTarget = unit;
								}
								lastDistance = Vector3.Distance(transform.position ,unit.transform.position);
							}
						}
					}
					else if(unit.tag == "building")
					{
						BuildingInfo bt = unit.GetComponent<BuildingInfo>();
						if(tc.getId() != bt.ownerId)
						{
							if(!tc.alliance.Contains(bt.ownerId))
							{
								if(firstStart)
								{
									selectedTarget = unit;
									firstStart = false;
								}
								else if(lastDistance > Vector3.Distance(transform.position, unit.transform.position))
								{
									selectedTarget = unit;
								}
								lastDistance = Vector3.Distance(transform.position ,unit.transform.position);
							}
						}		
					}
				}
			}
			if(selectedTarget != null)
			{
				GetComponent<UnitInfo>().attackTarget = selectedTarget;
				GetComponent<UnitInfo>().GoToAttack(selectedTarget.transform.position);
			}
		}
	}
	
	
	void resetDistanceCalculation()
	{
		
	}
	
	void CheckThisBuilding(GameObject build)
	{
		BuildingInfo bi = build.GetComponent<BuildingInfo>();
		if(bi.ownerId != tc.getId())
		{
			if(!tc.alliance.Contains(bi.ownerId))
			{
				
			}
			else
			{
				selectedTarget = null;
			}
		}
		else
		{
			selectedTarget = null;
		}
	}
}
