using UnityEngine;
using System.Collections;
using System.Collections.Generic;

	public enum bType
	{
		Airfield,
		Warehouse,
		VehicleFactory,
		InfanryBarracks,
		TechnologyCenter,
		WaterStation,
		EnergyFactory,
		Turrets,
		OilrigGeneral,
		AirfieldGeneral,
		HospitalGeneral,
		RepairCenterGeneral,
		OilContributeGeneral,
		MoneyHolderGeneral,
		Other
	}

	public enum bState
	{
		ReadyToBuild,
		Builded,
		Building,
		PreperingToBuild,
		Deleted,
		Empty,
		Misc
	}

public class BuildingInfo: MonoBehaviour {

	public string Name;
	public bType uType;
	public bState bst;
	public string type;
	public string Hint;
	public float bTime;
	public int addEnergy;
	public int addGold;
	public GameObject[] units;
	public float Health;
	public float currentHealth;
	public int team;
	public int coalition;
	public int ownerId;
	public float buildProgress;
	public float buildTime;
	public Texture2D icon;
	public Camera cam;
	public bool isEnterable;
	
	//Export func
	public bool isSelected;
	
	//Требования
	public int requiredLevel;
	public int requiredEnergy;
	public int requiredMoney;
	public List<GameObject> neededBuild;
	
	
	//Build Control
	private bool bFreezed;
	private List<GameObject> getingMoney;
	private GameObject buildingMe;
	
	//myInfo Initialize
	private TeamConfig tConfig;
	
	
	//Stats
	public float defence;
	public float laserDefence;
	public float fireDefence;
	public float boomDefence;
	public float generalDefence;
	public float poisonDefence;
	//if turrets
	public float attackPower;
	public float laserPower;
	public float firePower;
	public float boomPower;	
	public float poisonPower;
	public float generalPower;
	//type attack of turrents
	public aType abType; 
	//ResourceStats
	public int allMoneyIn;
	private bool available;
	//CurrentStates
	private float currTime;
	
	//Places
	public GameObject repairPlace;
	public GameObject healPlace;
	private GameObject lastAttackedBy;
	
	// Use this for initialization
	void Start ()
	{
		cam = Camera.main;
		tConfig = cam.transform.parent.GetComponent<TeamConfig>();
		if(allMoneyIn > 0)
		{
			available = true;
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(bst == bState.Building && !bFreezed)
		{
			currTime += Time.deltaTime;
			buildProgress = Mathf.Floor(100*(currTime/buildTime));
			if(currTime > buildTime)
			{
				EndBuild();
			}
		}
		if(available && allMoneyIn < 1)
		{
			sendStatusEmptyToGetters();
		}
		if(currentHealth <= 0)
		{
			doDie();
		}
	}
	
	void OnCollisionEnter(Collision collision) 
	{
		Typeu info = collision.transform.gameObject.GetComponent<UnitInfo>().type;
		if(info == Typeu.Builder)
		{
			collision.transform.gameObject.GetComponent<BuilderList>().DesireOnCollision(collision.transform.gameObject);
		}
		print (collision.transform.name);
	}
	
	void OnTriggerEnter(Collider other)
	{
		print(other.transform.name);
	}
	
	void OnGUI()
	{
		if(bst != bState.Builded)
		{
        	Vector3 screenPos = cam.WorldToScreenPoint(transform.position);
			float y = Mathf.Abs(screenPos.y - cam.pixelHeight)-10;
			GUI.color = Color.red;
			if(screenPos.y - cam.pixelHeight < 0)	GUI.Label(new Rect(screenPos.x,y,100,30), Mathf.Floor(buildProgress)+"%");			
		}
		if(isSelected && bst == bState.Builded)
		{
        	Vector3 screenPos = cam.WorldToScreenPoint(transform.position);
			float y = Mathf.Abs(screenPos.y - cam.pixelHeight)-10;
			GUI.color = Color.red;
			if(screenPos.y - cam.pixelHeight < 0)	GUI.Label(new Rect(screenPos.x,y,100,30), currentHealth+"/"+Health);				
		}
	}
	
	/*
	 Export functions of buiding 
	 */
	//Name of building
	public string getName()
	{
		return Name;
	}
	
	//whole helath of building
	public float fullHealth()
	{
		return Health;
	}
	
	//current health of building
	public float getCurrentHealth()
	{
		return currentHealth;
	}
	
	//get team id
	public int getTeam()
	{
		return team;
	}
	
	//get coalition
	public int getCoalition()
	{
		return coalition;
	}
	
	//owner id
	public int getOwnerId()
	{
		return ownerId;
	}
	
	//Building Progress
	public float getBuidProgress()
	{
		return buildProgress;
	}
	//Building Icon
	public Texture2D getIcon()
	{
		return icon;
	}
	
	//Requirements
	public int needMoney()
	{
		return requiredMoney;
	}
	
	public int needEnergy()
	{
		return requiredEnergy;
	}
	
	public int needLevel()
	{
		return requiredLevel;
	}
	
	public List<GameObject> needBuild()
	{
		return neededBuild;
	}
	
	public void setBuildState(bState st)
	{
		bst = st;
	}
	
	//Build Control
	public void FreezeBuild()
	{
		bFreezed = true;
		bst = bState.PreperingToBuild;
		removeBuilderMe();
	}
	
	public bool isFreezed()
	{
		return bFreezed;
	}
	
	public void ContinueBuilding()
	{
		if(bFreezed)
		{
			bFreezed = false;
		}
	}
	
	public void StartToBuild()
	{
		setBuildState(bState.Building);
		if(isFreezed())
		{
			ContinueBuilding();
		}
	}
	
	public void EndBuild()
	{
		setBuildState(bState.Builded);
		buildProgress = 100;
		if(buildingMe != null)
		{
			buildingMe.GetComponent<UnitInfo>().currentBuildTarget = null;
			buildingMe.GetComponent<UnitInfo>().sType = State.Idle;
			removeBuilderMe();
		}
	}
	
	public void DeleteBuild()
	{
		setBuildState(bState.Deleted);
		Destroy(transform.gameObject);
	}
	
	public void addToGettersMoney(GameObject getter)
	{
		if(!getingMoney.Contains(getter))
		{
			getingMoney.Add(getter);
		}
	}
	
	public void removeGettersMoney(GameObject getter)
	{
		if(getingMoney.Contains(getter))
		{
			getingMoney.Remove(getter);
		}
	}
	
	public void sendStatusEmptyToGetters()
	{
		if(getingMoney != null)
		{
			if(getingMoney.Count > 0)
			{
				foreach(GameObject getter in getingMoney)
				{
					getter.GetComponent<UnitInfo>().endMoneyResource = true;
					removeGettersMoney(getter);
				}
				
			}
		}
		available = false;
	}
	
	public bool isAvailebleForMoney()
	{
		return available;
	}
	
	public void addToBuilderMe(GameObject builder)
	{
		if(buildingMe == null)
		{
			buildingMe = builder;
		}
	}
	
	public void removeBuilderMe()
	{
		buildingMe = null;
	}
	
	public bool HaveBuilder()
	{
		if(buildingMe != null)
		{
			return true;
		}
		else return false;
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
	}
	
	//if attackable
	public float SendDamage()
	{
		float diff = 100.0f;
		//Calculate resists to all types of attack
		if(abType == aType.Boom)
		{
			diff += boomPower;
		}
		else if(abType == aType.Fire)
		{
			diff += firePower;
		}
		else if(abType == aType.Laser)
		{
			diff += laserPower;
		}
		else if(abType == aType.Poison)
		{
			diff += poisonPower;
		}
		
		diff += generalPower;
		
		if(diff < 0)
		{
			diff = 0.0f;
		}
		
		return attackPower*(diff/100);
	}
	
	public void doDie()
	{
		Destroy (transform.gameObject);
	}
	
}
