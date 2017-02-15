using UnityEngine;
using System.Collections;
using Pathfinding;

public class NavMeshSCR : MonoBehaviour {
	
	public GameObject target;
	public NavMeshAgent me;
	NavMeshHit hit;
	Quaternion targetRotation;
	bool samplePosHit;
	public float angle;
	//Following unit
	public GameObject target2;
	public bool isFollow;
	private float distance_me = 0;
	// Use this for initialization
	void Start () 
	{

	}
	
	// Update is called once per frame
	void Update () 
	{
		if(target != null)
		{
			//transform.position = new Vector3(transform.position.x,Terrain.activeTerrain.SampleHeight(target2.position),transform.position.z);
			me.updateRotation = true;
			me.destination = target.transform.position;
			
			angle = Vector3.Angle(me.velocity.normalized, this.transform.forward);
			if (me.velocity.normalized.x < this.transform.forward.x)
			{
				angle *= -1;
			}
			angle = (angle + 180.0f) % 360.0f;
			
			angle = 180 - angle;
			//angle = 360 - angle;
			//print (angle);
			if(angle > 0)
			{
				//transform.LookAt(target2);
			}
			if(target2 != null)
			{
				UnitInfo me2 = target2.GetComponent<UnitInfo>();
				distance_me = Vector3.Distance(me2.transform.position, target.transform.position);
				if(me2.sType == State.Goto)
				{
					if(Vector3.Distance(transform.position,target.transform.position) < 0.5f)
					{
						Destroy(target);
						target = null;
						me.Stop();
						
						me2.sType = State.Idle;
					}
				}
				if(me2.stage == cState.Creating)
				{
					if(Vector3.Distance(transform.position,target.transform.position) < 0.5f)
					{
						me2.stage = cState.Created;
						target = null;
						me.Stop();
					}				
				}
				if(me2.type == Typeu.Infantry)
				{
					if(target != null)
					{
						//target2.animation.CrossFade("run");
					}
					else
					{
						//target2.animation.CrossFade("fire");
					}
				}
			}
			/*UnitInfo me2 = target2.GetComponent<UnitInfo>();
			//After Creating we end the move and send status of unit
			if(me2.stage == cState.Creating)
			{
				if(Vector3.Distance(transform.position,target.transform.position) < 0.5f)
				{
					me2.stage = cState.Created;
					target = null;
					me.Stop();
				}				
			}
			
			if(me2.sType == State.GoingToAttack)
			{
				if(Vector3.Distance(me2.transform.position, target.transform.position) <= me2.attackDistance)
				{
					target = null;
					me.Stop();
				}
			}
			else if(me2.sType == State.Goto)
			{
				if(Vector3.Distance(transform.position,target.transform.position) < 0.5f)
				{
					target = null;
					me.Stop();
					Destroy(target.transform);
				}
			}
			else if(me2.sType == State.GoToBuild)
			{
				if(Vector3.Distance(me2.transform.position, target.transform.position) <= me2.buildDistance)
				{
					target = null;
					me.Stop();
					me2.sType = State.PrepareToBuild;
				}
			}
			else if(me2.sType == State.GoingToCapture)
			{
				if(Vector3.Distance(me2.transform.position, target.transform.position) <= me2.captureDistance)
				{
					target = null;
					me.Stop();
					me2.sType = State.Capture;
				}
			}
			//for moneyGetters
			if(me2.type == Typeu.MoneyHeli || me2.type == Typeu.MoneyVehicle)
			{
				if(me2.sType == State.GoingToMoney)
				{
					if(Vector3.Distance(me2.transform.position, target.transform.position) <= me2.moneyGetDistance)
					{
						me2.sType = State.GettingMoney;
						target = null;
						me.Stop();						
					}
				}
				else if(me2.sType == State.GoingHome)
				{
					if(Vector3.Distance(me2.transform.position, target.transform.position) <= me2.moneyGetDistance)
					{
						me2.sType = State.MoneySend;
						target = null;
						me.Stop();						
					}					
				}
			}*/
		}
		
	}
	
	public float getDistance()
	{
		return distance_me;
	}
	
	public void StopMe()
	{
		
		me.Stop();
		if(target2 != null)
		{
			UnitInfo me2 = target2.GetComponent<UnitInfo>();
			if(target.GetComponent<UnitInfo>() == null && target.GetComponent<BuildingInfo>() == null)
			{
				//Destroy(target.transform);
			}
			me2.sType = State.Idle;
		}
		target = null;

	}
	
	void LateUpdate()
	{
		//transform.rotation = Quaternion.Euler(angle, 0, 0);
	}
	
}
