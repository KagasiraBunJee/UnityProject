using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum Typeu 
{
	Builder,
	Vehicle,
	Helis,
	Jets,
	Infantry,
	Ships,	
	Enhancement,
	MoneyHeli,
	MoneyVehicle,
	Misc
}

public enum aType 
{	
	General,
	Fire,
	Laser,
	Boom,
	Poison
}

public enum State 
{
	Idle,
	Building,
	GoToBuild,
	PrepareToBuild,
	Goto,
	Attacking,
	GoingToAttack,
	Capture,
	Freeze,
	GettingMoney,
	GoingToMoney,
	PreparingToGetMoney,
	GoingToCapture,
	GoingHome,
	MoneySend,
	GotoHeal,
	Healing,
	GotoRepair,
	Repairing,
	Misc
}

public enum cState 
{
	NotCreated,
	Creating,
	StartToCreate,
	Created,
	ReadyForUse
}

public class UnitInfo : MonoBehaviour {
	//Main Config
	public string Name;
	public float Health;
	public float currentHealth;
	public Texture icon;
	public string Hint;
	public float speed;
	public Typeu type;
	public int coalition;
	public int team;
	public int owner;
	
	
	
	
	public State sType;
	public cState stage;
	public aType attackType;
	public float shootSpeed;
	public Transform target;
	public Transform wayPoint;
	public GameObject self;
	public Vector3 destination;
	public bool idle;
	public  bool isAttacking;
	public bool isSelected;
	public bool rotating;
	public bool moving;
	public bool isOnTheWay = false;
	public bool AttackAble;
	
	public GameObject way;
	private bool isDraging;
	
	public Camera cam;
	
	private float startTime;
	
 	private Vector3 targetPoint;
	private Quaternion targetRotation;
	private RaycastHit hh;
	

	public Vector3 mouse_pos;
	public Vector3 mouse_posTemp;
	public Vector3 object_pos;
	public Vector3 object_posTemp;
	public float angle;
	
	//PVP Config
	public GameObject attackTarget;
	public bool attacking;
	private bool stopped;
	public bool forceAttack;
	public bool attackMoving;
	public bool canMoveAttack;
	
	//Build Config
	public int needMoney;
	public int needLvl;
	
	
	//Spawn Configs
	public double timer;
	public float createSpeed;
	public float createTime;
	public float currProgress;
	public GameObject inHangar;
	public GameObject whereToGoAfterSpawn;
	public bool rotateOnly;
	
	//Stats
	//Defence Bonus
	public float defence;
	public float laserDefence;
	public float fireDefence;
	public float boomDefence;
	public float poisonDefence;
	public float generalDefence;
	//Attack bonus
	public float laserMultiplier;
	public float fireMultiplier;
	public float boomMultiplier;
	public float poisonMultiplier;
	public float generalMultiplier;
	public bool sniperResist;
	public float attackDamage;
	public float attackDistance;
	public float rotateSpeed = 10;
	public float moveSpeed = 10;	
	public int lvl_point;
	public int lvl;
	//If MoneyGetter
	public float moneyGetDistance;
	public int moneyAmount;
	public int moneyAdd;
	public int currentMoneyGot;
	public bool endMoneyResource;
	public bool isGetting;
	public GameObject MyGoldPlace;
	public GameObject MyGoldHome;
	public GameObject MyGoldMainHome;
	private float gettingTimer;
	//if builder
	public float buildDistance;
	public GameObject currentBuildTarget;
	public bool goingtoBuild;
	//CaptureAbleUnits
	public float captureDistance;
	public bool canCapture;
	public float captureSpeed;
	private float captureTimer;
	//End of Stat
	
	//Battle timers for action like Healing/Repairing and etc
	public GameObject repairPl;
	public float repairSpeed;
	public float repairDistance;
	public bool goingToRepair;
	public GameObject healPl;
	public float healSpeed;
	public float healDistance;
	public bool goingtoHeal;
	//get BattleStatistic
	public List<GameObject> whosattackme;
	public GameObject lastAttackedBy;
	
	//get playerConfig
	
	TeamConfig tm;
	// Use this for initialization
	void Start () 
	{
		
		startTime = Time.time;
		self = transform.gameObject;
		sType = State.Idle;
		cam = Camera.main;
		tm = cam.transform.parent.GetComponent<TeamConfig>();
		attacking = false;
		forceAttack = false;
		//cc = 1;
	}
	
	// Update is called once per frame
	void Update () 
	{
		
		if(currentBuildTarget != null)
		{
			//print (Vector3.Distance(transform.position, currentBuildTarget.transform.position));
		}
		
		if(tm == null)
		{
			tm  = cam.transform.parent.GetComponent<TeamConfig>();
		}
		if(way != null)
		{
			destination = way.transform.position;
		}
		else
		{
			//way = ((GameObject)Instantiate(tm.way)).transform.gameObject;
			//way.transform.position = new Vector3(transform.position.x, Terrain.activeTerrain.SampleHeight(transform.position) , transform.position.z);			
		}
		if(way != null)
		{
			
			if(Vector3.Distance(way.transform.position,transform.position) > 0.00f)
			{
				NewMovingScript();
			}
			else
			{
				if(!stopped)
				{
					stopped = true;
					sType = State.Idle;
				}
			}
		}
		if(goingtoHeal)
		{
			Healme(healPl,Time.deltaTime);
		}
		if(goingToRepair)
		{
			RepairMe(repairPl,Time.deltaTime);
		}
		
		if(AttackAble)
		{
			if(attacking)
			{
				if(attackTarget != null)
				{
					GoToAttack(attackTarget.transform.position,forceAttack);
				}
			}			
		}
		if(type == Typeu.Builder)
		{
			if(sType == State.GoToBuild || sType == State.PrepareToBuild)
			{
				Collider[] hitColliders = Physics.OverlapSphere(transform.position, buildDistance);
				if(hitColliders.Length > 0)
				{
					foreach(Collider col_b in hitColliders)
					{
						GameObject building = col_b.transform.gameObject;
						if(building.GetComponent<BuildingInfo>() != null)
						{
							BuildingInfo bu = building.GetComponent<BuildingInfo>();
							if(bu.bst == bState.Building || bu.bst == bState.PreperingToBuild || bu.bst == bState.ReadyToBuild)
							{
								if(building == currentBuildTarget)
								{
									bu.addToBuilderMe(transform.gameObject);
									bu.StartToBuild();
									stopMoving();
								}
							}
						}
					}
				}
			}
		}
		
		if(type == Typeu.Infantry)
		{
			if(sType == State.GoingToAttack ||
			   sType == State.GoingHome ||
			   sType == State.GoingToCapture ||
			   sType == State.GoingToMoney ||
			   sType == State.Goto ||
			   sType == State.GoToBuild ||
			   sType == State.GotoHeal ||
			   sType == State.GotoRepair)
			{
				animation.CrossFade("run");
			}
			else if(sType == State.Idle)
			{
				
				animation.CrossFade("fire");
				//animation.Stop();
			}
		}
		
		if(sType == State.Idle && attacking)
		{
			sType = State.Attacking;
		}
		
		/*if(stage == cState.StartToCreate)
		{
			timer = 0;
			stage = cState.Creating;
		}
		else if(stage == cState.Creating)
		{
			timer += Time.deltaTime;
			if(timer > createTime && stage == cState.Creating)
			{
				stage = cState.Created;
			}
		}
		else if(stage == cState.Created)
		{
			stage = cState.ReadyForUse;
			transform.position = inHangar.transform.position;
		}*/
		
		if(stage == cState.Created)
		{

			//StartMove(way.transform.position,false);
			stage = cState.ReadyForUse;
			if(type == Typeu.MoneyHeli || type == Typeu.MoneyVehicle)
			{
				isGetting = true;
			}
		}
		if((type == Typeu.MoneyHeli || type == Typeu.MoneyVehicle) && isGetting)
		{
			GettingResources(MyGoldHome, Time.deltaTime);
		}
    	if(rotating) TurnToTarget();
    	else if(moving) MakeMove();
		
		//this.transform.position += Vector3.forward * 10 * Time.deltaTime;
		//if(Vector3.Distance(self.transform.position,target.position) >= 6)
		//{
		/*Vector3 checkForward = transform.position + transform.forward;
		Ray rayForward = new Ray(transform.position);
		RaycastHit hh;
		if(Physics.Raycast(rayForward,out hh,Mathf.Infinity))
		{
			if(hh.collider.tag != "Respawn")
			{
				goToMark();
			}
			else print ("ok");
		}*/
		
		//}
		//self.transform.position += Vector3.forward * Time.deltaTime*10;
		//transform.Translate (0, 0, 20*Time.deltaTime, Space.Self);
		if(Input.GetMouseButtonUp(0))
		{
		
			
		}
		if(Input.GetMouseButtonDown(1))
		{
			
		}
		if(isOnTheWay)
		{
			//print (Vector3.Distance(transform.position,way.transform.position));
			/*if(Vector3.Distance(way.transform.position,transform.position) <= 3.5f)
			{
				wayPoint = null;
				Destroy(way);
				isOnTheWay = false;
				moving = false;
			}*/
			//else transform.Translate (0, 0, 20*Time.deltaTime, Space.Self);
		}
		if(transform.FindChild("selected") != null)
		{
			transform.FindChild("selected").gameObject.SetActive(isSelected);
		}
		//Stat update
		if(currentHealth <= 0)
		{
			doDie();
		}
		
		
	}
	
	public void OnGUI()
	{
		if(isSelected)
		{
        	Vector3 screenPos = cam.WorldToScreenPoint(transform.position);
			float y = Mathf.Abs(screenPos.y - cam.pixelHeight)-10;
			GUI.color = Color.red;
			if(screenPos.y - cam.pixelHeight < 0)	GUI.Label(new Rect(screenPos.x,y,100,30),currentHealth+"/"+Health);
		}
	}
	
	public void stopMoving()
	{
		wayPoint = null;
		//Destroy(way);
		isOnTheWay = false;
		moving = false;
		rotating = false;
		sType = State.Idle;
		way.GetComponent<NavMeshSCR>().StopMe();
		if(currentBuildTarget != null && goingtoBuild)
		{
			sType = State.Building;
			goingtoBuild = false;
		}
	}
	
	public void goToMark()
	{
		//float dist = Vector3.Distance(transform.position,way.transform.position);
        //float distCovered = (Time.time - startTime) * 2 * Time.deltaTime;
        //float fracJourney = distCovered / dist;
		//self.transform.rotation = Quaternion.Slerp(this.transform.rotation,Quaternion.LookRotation(new Vector3(target.position.x - this.transform.position.x, 0 , target.position.z- this.transform.position.z)),10*Time.deltaTime);
		//Debug.DrawLine(target.position,this.transform.position,Color.green);
		//Debug.DrawLine(transform.position,transform.FindChild("back").position,Color.red);
		//float angle = Vector3.Angle(way.position, transform.position);
		//print (angle);

		//Vector3 targetPoint = way.transform.position;
		
		//transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(transform.position - targetPoint , new Vector3(0,0,0)), fracJourney); 
		//self.transform.position = Vector3.Lerp(this.transform.position,way.transform.position, fracJourney);
		//self.transform.position += Vector3.forward * Time.deltaTime*10;

		//print (mouse_pos);
        //mouse_pos = mouse_posTemp;
		//mouse_pos.z = 5.23f;
		
		//The distance between the camera and object
		//object_pos = object_posTemp;
		//mouse_pos.x = mouse_pos.x - object_posTemp.x;
		//mouse_pos.y = mouse_pos.y - object_posTemp.y;
		//angle = Mathf.Atan2( mouse_pos.x,mouse_pos.y) * Mathf.Rad2Deg;
		//transform.rotation = Quaternion.Euler(new Vector3(0, angle, 0));
		//print (angle);

		target = transform;
		if(wayPoint != null)
		{
			
			if(wayPoint.gameObject.GetComponent<BuildingInfo>() == null && wayPoint.gameObject.GetComponent<UnitInfo>() == null)
			{
				//Destroy(wayPoint.gameObject);
			}
		}
		
		if(sType == State.Attacking || sType == State.GoingToAttack)
		{
			//return;
		}
		if(type == Typeu.Builder)
		{
			if(!goingtoBuild)
			{
				if(currentBuildTarget != null)
				{
					currentBuildTarget.GetComponent<BuildingInfo>().FreezeBuild();
					currentBuildTarget = null;
				}
			}
		}
		isGetting = false;
		mouse_pos = Input.mousePosition;
		mouse_posTemp = Input.mousePosition;
		//print (mouse_pos);
		mouse_pos.z = 5.23f;
		//The distance between the camera and object
		object_pos = Camera.main.WorldToScreenPoint(target.position);
		object_posTemp = Camera.main.WorldToScreenPoint(target.position);
		mouse_pos.x = mouse_pos.x - object_pos.x;
		mouse_pos.y = mouse_pos.y - object_pos.y;
		float pi = 3.14159265f;
		angle = Mathf.Atan2(mouse_pos.x,mouse_pos.y) * (180/pi);
		//transform.rotation = Quaternion.Euler(new Vector3(0, angle, 0));
		//
		
		//print (angle);
		Ray rayForward = Camera.main.ScreenPointToRay(Input.mousePosition);
		
		if(Physics.Raycast(rayForward,out hh,Mathf.Infinity))
		{
			if(hh.collider.name == "Terrain")
			{
				/*way = new GameObject("way");
				way.transform.position = hh.point;
				StartMove(hh.point);
				wayPoint = way.transform;*/
				if(wayPoint != null)
				{
					if(wayPoint.GetComponent<UnitInfo>() == null && wayPoint.GetComponent<BuildingInfo>() == null)
					{
						Destroy(wayPoint.gameObject);
					}
				}
				GameObject way_1 = new GameObject("way_1");
				wayPoint = way_1.transform;
				way_1.transform.position = hh.point;
				way.GetComponent<NavMeshSCR>().target = way_1;
			}
		}		
		goingtoBuild = false;
		isOnTheWay = true;

		//print (dist);
		/*Vector3 velocity = rigidbody.velocity;
		Vector3 tp = target.position;
		Vector3 md = tp-transform.position;
		if(md.magnitude < 1)
		{
			velocity = Vector3.zero;
		}
		else
		{
			velocity = md.normalized*10;
		}
		rigidbody.velocity = new Vector3(velocity.x,velocity.y-1,velocity.z);*/
	}
	
	void LateUpdate()
	{

	}
	
	private void TurnToTarget()
	{
	    transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotateSpeed);
	    //sometimes it gets stuck exactly 180 degrees out in the calculation and does nothing, this check fixes that
	    Quaternion inverseTargetRotation = new Quaternion(-targetRotation.x, -targetRotation.y, -targetRotation.z, -targetRotation.w);
	    if(transform.rotation == targetRotation || transform.rotation == inverseTargetRotation) 
		{
			
	        rotating = false;
			if(!rotateOnly)
			{
	        	moving = true;
			}
			else 
			{
				stopMoving();
				rotateOnly = false;
			}
	    }
	}
	
	public void StartMove(Vector3 destination, bool build = false, bool onlyRotate = false)
	{
		sType = State.Goto;
		if(build)
		{
			sType = State.PrepareToBuild;
		}
	    this.destination = destination;
	    targetRotation = Quaternion.LookRotation (destination - transform.position);
	    rotating = true;
	    moving = false;
	}	
	
	public void NewMovingScript()
	{
		
		targetRotation = Quaternion.LookRotation (destination - transform.position);
		Quaternion inverseTargetRotation = new Quaternion(-targetRotation.x, -targetRotation.y, -targetRotation.z, -targetRotation.w);
		if(transform.rotation != targetRotation || transform.rotation != inverseTargetRotation)
		{
			transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotateSpeed);
			
		}
		if(sType == State.PrepareToBuild)
		{
			sType = State.GoToBuild;
		}
		transform.position = new Vector3(transform.position.x,Terrain.activeTerrain.SampleHeight(transform.position),transform.position.z);
	    transform.position = Vector3.MoveTowards(transform.position, destination, Time.deltaTime * moveSpeed);
		if(sType == State.Idle)
		{
			sType = State.Goto;
		}
		if(type == Typeu.Infantry)
		{
			animation.CrossFade("run");
		}
		stopped = false;
	    /*if(transform.position == destination)
		{
			moving = false;
			sType = State.Idle;
		}	*/	
	}
	
	private void MakeMove()
	{
		/*if(moving)
		{
			transform.Translate (0, 0, moveSpeed*Time.deltaTime, Space.Self);
		}
		else moving = false;*/
		
		if(sType == State.PrepareToBuild)
		{
			sType = State.GoToBuild;
		}
	    transform.position = Vector3.MoveTowards(transform.position, destination, Time.deltaTime * moveSpeed);
	    if(transform.position == destination)
		{
			moving = false;
			sType = State.Idle;
		}
	}	
	
	public bool isSelect()
	{
		return isSelected;
	}
	
	public void selectCurr(bool state)
	{
		isSelected = state;
	}
	
	public void insertIntoHangar(GameObject hang)
	{
		inHangar = hang;
	}
	
	public int getNeedMoney()
	{
		return needMoney;
	}
	
	public int needLevel()
	{
		return needLvl;
	}
	
	public void setUnitState(State sIType)
	{
		sType = sIType;
	}
	
	public void readyForCreate()
	{
		
	}
	
	public void setCreateStage(cState st)
	{
		stage = st;
	}
	
	public void CreateWay()
	{
		way = ((GameObject)Instantiate(Camera.main.transform.parent.GetComponent<TeamConfig>().way)).transform.gameObject;
		if(type == Typeu.Vehicle || type == Typeu.MoneyVehicle)
		{
			way.GetComponent<NavMeshAgent>().radius = 8;
		}
		else if(type == Typeu.Infantry)
		{
			way.GetComponent<NavMeshAgent>().radius = 1;
		}
		else if(type == Typeu.Ships)
		{
			
		}
		way.transform.position = new Vector3(whereToGoAfterSpawn.transform.position.x, Terrain.activeTerrain.SampleHeight(transform.position) , whereToGoAfterSpawn.transform.position.z);
		way.GetComponent<NavMeshSCR>().target2 = transform.gameObject;
		GameObject afterresp = new GameObject("way_1");
		afterresp.transform.position = whereToGoAfterSpawn.transform.position;
		wayPoint = afterresp.transform;
		wayPoint.position = afterresp.transform.position;
		way.GetComponent<NavMeshSCR>().target = afterresp;
	}
	
	public void CancelAttack()
	{
		//sType = State.Goto;
		if(attackTarget != null)
		{
			attackTarget = null;
		}
	}
	
	public void doAttack(Vector3 targetPos)
	{
		//Infantry
		if(type == Typeu.Infantry)
		{
			//Attacking and attack animation
			GetComponent<InfantryAttack>().orderToAttack(attackTarget,true);
		}
		//Jets
		else if(type == Typeu.Jets)
		{
			//Attacking and attack animation
		}
		//Helicopters
		else if(type == Typeu.Helis)
		{
			//Attacking and attack animation
		}
		//Ships
		else if(type == Typeu.Ships)
		{
			//Attacking and attack animation
		}
		//Vehicles
		else if(type == Typeu.Vehicle)
		{
			//Attacking and attack animation
			GetComponent<TankAttack>().orderToAttack(true);
		}
	}
	
	public void GoToAttack(Vector3 targetPos, bool forcedAttack = false, bool _new = false)
	{
		if(AttackAble)
		{
			if(attackTarget == null || _new)
			{
				
				if(!attacking)
				{
					attacking = true;
				}
				if(forcedAttack && !forceAttack)
				{
					forceAttack = true;
				}
				if(sType != State.GoingToAttack)
				{
					sType = State.GoingToAttack;
				}
				if(attackTarget != null && ( attackTarget.name == "dummyAttack" || attackTarget.name == "dummyAttack(Clone)"))
				{
					Destroy(attackTarget);
					attackTarget = null;
				}
				if(attackTarget == null)
				{
					
					GameObject _dummyAttack = new GameObject("dummyAttack");
					_dummyAttack.transform.position = targetPos;
					//GameObject dummyAttack = (GameObject)Instantiate(_dummyAttack,targetPos, Quaternion.identity);
					attackTarget = _dummyAttack;
				}
				//wayPoint = attackTarget.transform;
				if(!attackMoving)
				{
					way.GetComponent<NavMeshSCR>().target = attackTarget;
				}
				isOnTheWay = true;
			}
			else if(Vector3.Distance(transform.position, attackTarget.transform.position) <= attackDistance && sType == State.GoingToAttack)
			{
				
				if(!attackMoving)
				{
					way.GetComponent<NavMeshSCR>().StopMe();
				}
				sType = State.Attacking;
			}
			else if(Vector3.Distance(transform.position, attackTarget.transform.position) > attackDistance && sType == State.Attacking)
			{
				sType = State.GoingToAttack;
				wayPoint = attackTarget.transform;
				if(!attackMoving)
				{
					way.GetComponent<NavMeshSCR>().target = attackTarget;
				}
				isOnTheWay = true;
			}
			else if(sType == State.Attacking)
			{
				
				if(attackTarget != null)
				{
					if(attackTarget.tag == "Unit")
					{
						UnitInfo ub = attackTarget.GetComponent<UnitInfo>();
						if(ub.owner == owner && !forcedAttack)
						{
							StopAttack();
						}
						else if(tm.alliance.Contains(ub.owner) && !forcedAttack)
						{
							StopAttack();
						}
						else
						{
							doAttack(attackTarget.transform.position);
						}
					}
					else if(attackTarget.tag == "building")
					{
						BuildingInfo ub = attackTarget.GetComponent<BuildingInfo>();
						if(ub.ownerId == owner && !forcedAttack)
						{
							StopAttack();
						}
						else if(tm.alliance.Contains(ub.ownerId) && !forcedAttack)
						{
							StopAttack();
						}
						else
						{
							doAttack(attackTarget.transform.position);
						}
					}
					else
					{
						doAttack(attackTarget.transform.position);
					}
				}
				else
				{
					StopAttack();
				}
			}
			if(!forcedAttack)
			{
				CheckAttackedTarget(attackTarget);
			}
		}
	}
	
	public void CheckAttackedTarget(GameObject attacked)
	{
		
		if(attacked != null)
		{
			if(attacked.tag == "Unit")
			{
				UnitInfo ub = attacked.GetComponent<UnitInfo>();
				if(ub.owner == owner)
				{
					StopAttack();
				}
				else if(tm.alliance.Contains(ub.owner))
				{
					StopAttack();
				}
			}
			else if(attacked.tag == "building")
			{
				BuildingInfo ub = attacked.GetComponent<BuildingInfo>();
				if(ub.ownerId == owner)
				{
					StopAttack();
				}
				else if(tm.alliance.Contains(ub.ownerId))
				{
					StopAttack();
				}
			}
		}
		else
		{
			StopAttack();
		}
	}
	
	public void StopAttack()
	{
		attackTarget = null;
		sType = State.Idle;
		attacking = false;
		forceAttack = false;
		if(type == Typeu.Infantry)
		{
			//Attacking and attack animation
			GetComponent<InfantryAttack>().orderToAttack(attackTarget,false);
		}
		//Jets
		else if(type == Typeu.Jets)
		{
			//Attacking and attack animation
		}
		//Helicopters
		else if(type == Typeu.Helis)
		{
			//Attacking and attack animation
		}
		//Ships
		else if(type == Typeu.Ships)
		{
			//Attacking and attack animation
		}
		//Vehicles
		else if(type == Typeu.Vehicle)
		{
			//Attacking and attack animation
			GetComponent<TankAttack>().orderToAttack(attacking);
		}
		
	}
	
	public void AttackMoving()
	{
		if(canMoveAttack)
		{
			attackMoving = true;
		}
		else
		{
			StopAttack();
		}
	}	
	
	public void doDie()
	{
		Destroy(transform.gameObject);
	}
	
	public void doKill()	
	{
		attackTarget.GetComponent<UnitInfo>().doDie();
	}
	
	//Money Automatic geting Resouce
	public void GettingResources(GameObject build,float deltaTime)
	{
		if(MyGoldPlace == null)
		{
			GameObject[] resourcePlace = GameObject.FindGameObjectsWithTag("building");
			float distance = 0;
			bool firstSearch = true;
			foreach(GameObject resource in resourcePlace)
			{
				if(resource.GetComponent<BuildingInfo>().uType == bType.MoneyHolderGeneral && resource.GetComponent<BuildingInfo>().isAvailebleForMoney())
				{
					if(firstSearch)
					{
						firstSearch = false;
						distance = Vector3.Distance(transform.position,resource.transform.position);
						MyGoldPlace = resource;
					}
					else if(distance > Vector3.Distance(transform.position, resource.transform.position))
					{
						MyGoldPlace = resource;
					}
					distance = Vector3.Distance(transform.position,resource.transform.position);
				}
			}
		}
		else
		{
			if(type == Typeu.MoneyVehicle)
			{
				if(MyGoldPlace.GetComponent<BuildingInfo>().isAvailebleForMoney())
				{
					if(currentMoneyGot == 0 && (sType == State.Idle || sType == State.MoneySend))
					{
						sType = State.GoingToMoney;
						wayPoint = MyGoldPlace.transform;
						way.GetComponent<NavMeshSCR>().target = MyGoldPlace;
					}
					else if(sType == State.GoingToMoney)
					{
						if(way.GetComponent<NavMeshSCR>().getDistance() <= moneyGetDistance)
						{
							sType = State.GettingMoney;
						}
					}
					else if(sType == State.GettingMoney && currentMoneyGot < moneyAmount)
					{
						gettingTimer += deltaTime;
						if(gettingTimer > 1)
						{
							gettingTimer = 0.00f;
							currentMoneyGot += moneyAdd;
							MyGoldPlace.GetComponent<BuildingInfo>().allMoneyIn -= moneyAdd;
						}
					}
					else if(sType == State.GettingMoney && (currentMoneyGot >= moneyAmount || endMoneyResource))
					{
						if(currentMoneyGot > moneyAmount)
						{
							currentMoneyGot = moneyAmount;
						}
						sType = State.GoingHome;
						wayPoint = MyGoldHome.transform;
						way.GetComponent<NavMeshSCR>().target = MyGoldHome;
					}
					else if(sType == State.GoingHome)
					{
						if(way.GetComponent<NavMeshSCR>().getDistance() <= moneyGetDistance)
						{
							sType = State.MoneySend;
						}
					}
					else if(sType == State.MoneySend && currentMoneyGot >= moneyAmount)
					{
						tm.addMoney(currentMoneyGot);
						currentMoneyGot = 0;
						if(endMoneyResource)
						{
							endMoneyResource = false;
							MyGoldPlace = null;
							sType = State.Idle;
						}
						else
						{
							wayPoint = MyGoldHome.transform;
							way.GetComponent<NavMeshSCR>().target = MyGoldHome;
						}
					}
				}
				else
				{
					MyGoldPlace = null;
					sType = State.MoneySend;
				}
			}
			else if(type == Typeu.MoneyHeli)
			{
				
			}
		}
		if(MyGoldHome == null && MyGoldMainHome != null)
		{
			MyGoldHome = MyGoldMainHome;
		}
		else if(MyGoldHome == null && MyGoldMainHome == null)
		{
			GameObject[] resourcePlace = GameObject.FindGameObjectsWithTag("building");
			float distance = 0;
			bool firstSearch = true;
			foreach(GameObject resource in resourcePlace)
			{
				if(resource.GetComponent<BuildingInfo>().uType == bType.Warehouse && resource.GetComponent<BuildingInfo>().ownerId == tm.getId())
				{
					if(firstSearch)
					{
						firstSearch = false;
						distance = Vector3.Distance(transform.position,resource.transform.position);
						MyGoldHome = resource;
					}
					else if(distance > Vector3.Distance(transform.position, resource.transform.position))
					{
						MyGoldHome = resource;
					}
					distance = Vector3.Distance(transform.position,resource.transform.position);
				}
			}
			if(MyGoldMainHome == null)
			{
				MyGoldMainHome = MyGoldHome;
			}
		}
	}
	
	public void RepairMe(GameObject build,float deltaTime)
	{
		if(currentHealth < Health)
		{
			if(!goingToRepair)
			{
				goingToRepair = true;
			}
			//if money getter
			if(isGetting)
			{
				isGetting = false;
			}
			if(sType != State.GotoRepair)
			{
				sType = State.GotoRepair;
				wayPoint = build.GetComponent<BuildingInfo>().repairPlace.transform;
				way.GetComponent<NavMeshSCR>().target = build.GetComponent<BuildingInfo>().repairPlace;
			}
			else if(sType == State.GotoRepair)
			{
				if(way.GetComponent<NavMeshSCR>().getDistance() <= repairDistance)
				{
					sType = State.Repairing;
				}
			}
			else if(sType == State.Repairing)
			{
				currentHealth += repairSpeed*deltaTime;
				if(currentHealth >= Health)
				{
					currentHealth = Health;
					goingToRepair = false;
					sType = State.Misc;
					wayPoint = build.GetComponent<BuildingInfo>().repairPlace.transform.FindChild("goto");
					way.GetComponent<NavMeshSCR>().target = build.GetComponent<BuildingInfo>().repairPlace.transform.FindChild("goto").gameObject;
				}
			}
		}
	}
	
	public void Healme(GameObject build,float deltaTime)
	{
		if(currentHealth < Health)
		{
			if(!goingtoHeal)
			{
				goingtoHeal = true;
			}
			//if money getter
			if(isGetting)
			{
				isGetting = false;
			}
			if(sType != State.GotoHeal)
			{
				sType = State.GotoHeal;
				wayPoint = build.GetComponent<BuildingInfo>().healPlace.transform;
				way.GetComponent<NavMeshSCR>().target = build.GetComponent<BuildingInfo>().healPlace;
			}
			else if(sType == State.GotoHeal)
			{
				if(way.GetComponent<NavMeshSCR>().getDistance() <= healDistance)
				{
					sType = State.Repairing;
				}
			}
			else if(sType == State.Healing)
			{
				currentHealth += healSpeed*deltaTime;
				if(currentHealth >= Health)
				{
					currentHealth = Health;
					goingtoHeal = false;
					sType = State.Misc;
					wayPoint = build.GetComponent<BuildingInfo>().healPlace.transform.FindChild("goto");
					way.GetComponent<NavMeshSCR>().target = build.GetComponent<BuildingInfo>().healPlace.transform.FindChild("goto").gameObject;
				}
			}
		}		
	}
	
	public void goToBuild(GameObject build)
	{
		if(build.GetComponent<BuildingInfo>() != null)
		{
			BuildingInfo bu = build.GetComponent<BuildingInfo>();
			currentBuildTarget = build;
			bu.addToBuilderMe(transform.gameObject);
			way.GetComponent<NavMeshSCR>().target = build;
			sType = State.GoToBuild;
			bu.bst = bState.PreperingToBuild;
			goingtoBuild = true;
			
		}
	}
	
	void OnCollisionEnter(Collision collision) 
	{
		print (collision.transform.name);
	}
	
	public void ReceiveDamage(float damage, aType type, GameObject attacker)
	{
		float diff = 100.0f;
		//Calculate resists to all types of attack
		if(type == aType.Boom)
		{
			diff -= boomDefence;
		}
		else if(type == aType.Fire)
		{
			diff -= fireDefence;
		}
		else if(type == aType.Laser)
		{
			diff -=  laserDefence;
		}
		else if(type == aType.Poison)
		{
			diff -= poisonDefence;
		}
		//Calculate overall defence
		diff -= generalDefence;
		
		if(diff < 0)
		{
			diff = 0.0f;
		}
		
		damage = defence - damage*(diff/100);
		if(damage >= currentHealth)
		{
			damage = currentHealth;
		}
		
		currentHealth -= Mathf.Abs(damage);
		lastAttackedBy = attacker;
		if(!whosattackme.Contains(attacker))
		{
			whosattackme.Add(attacker);
		}
	}
	
	public float SendDamage()
	{
		float diff = 100.0f;
		//Calculate resists to all types of attack
		if(attackType == aType.Boom)
		{
			diff += boomMultiplier;
		}
		else if(attackType == aType.Fire)
		{
			diff += fireMultiplier;
		}
		else if(attackType == aType.Laser)
		{
			diff += laserMultiplier;
		}
		else if(attackType == aType.Poison)
		{
			diff += poisonMultiplier;
		}
		
		diff += generalMultiplier;
		
		if(diff < 0)
		{
			diff = 0.0f;
		}
		
		return attackDamage*(diff/100);
	}
	
	private void NotifyDeath()
	{
		if(whosattackme != null)
		{
			if(whosattackme.Count > 0)
			{
				foreach(GameObject attacker in whosattackme)
				{
					if(attacker.GetComponent<UnitInfo>().attackTarget == transform.gameObject)
					{
						attacker.GetComponent<UnitInfo>().CancelAttack();
					}
				}
			}
		}
	}
}
