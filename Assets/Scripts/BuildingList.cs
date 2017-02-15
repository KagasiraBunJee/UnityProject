using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BuildingList : MonoBehaviour {
	
	public Camera cam;
	private int myCoal;
	public List<GameObject> uList;
	public List<GameObject> spawnList;

	//Unit building
	public List<GameObject> queneCount;
	public List<GameObject> unitBuild;
	public List<bool> hangars;
	
	//For optimization, we create constant for available for creating units
	public List<GameObject> canBuild;
	private UnitInfo uInfo;
	private TeamConfig tConf;
	private int ownerId;
	private TeamConfig tConfig;
	private GameObject spawnIn;
	private GameObject instant;
	//Now Creating
	private GameObject nowCreatingTemp;
	private GameObject nowCreating;
	private bool isReady;
	private bool firstTimeCreate;
	private double timer;
	// Use this for initialization
	void Start ()
	{
		cam = Camera.main;
		tConfig = cam.transform.parent.GetComponent<TeamConfig>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		ownerId = tConfig.getId();
		if(transform.gameObject.GetComponent<BuildingInfo>().isSelected)
		{
			if(unitBuild.Count > 0)
			{
				foreach(GameObject uBuild in unitBuild)
				{
					if(uBuild != null)
					{
						uInfo = uBuild.GetComponent<UnitInfo>();
						
						if(uInfo.needLevel() <= tConfig.getLevel())
						{
							if(uInfo.getNeedMoney() <= tConfig.getMoney())
							{
								if(uBuild != null)
								{
									if(!canBuild.Contains(uBuild))
									{
										canBuild.Add(uBuild);
									}
								}
							}
							else removeFromAble(uBuild);
						}
						else removeFromAble(uBuild);
					}
				}
			}
		}
		if(spawnList.Count == 1)
		{
			foreach(GameObject spp in spawnList)
			{
				if(spp != null)
				{
					spawnIn = spp;
				}
			}
		}
		if(queneCount.Count > 0)
		{
			foreach(GameObject go in queneCount)
			{
				if(nowCreating == null)
				{
					
					nowCreatingTemp = go;
					nowCreating = go;
				}
			}
		}
		if(nowCreating != null)
		{
			timer += Time.deltaTime;
			double endTime = nowCreating.GetComponent<UnitInfo>().createTime;
			if(nowCreating.GetComponent<UnitInfo>().stage != cState.Creating)
			{
				nowCreating.GetComponent<UnitInfo>().stage = cState.Creating;
			}
			if(timer > endTime)
			{
				queneCount.Remove(nowCreatingTemp);
				
				
				
				
				nowCreating.transform.position = spawnIn.transform.position;
				nowCreating = ((GameObject)Instantiate(nowCreatingTemp)).transform.gameObject;
				if(nowCreating.GetComponent<UnitInfo>().type == Typeu.Jets )
				{
					nowCreating.GetComponent<UnitInfo>().rotateOnly = true;
				}
				else if(nowCreating.GetComponent<UnitInfo>().type == Typeu.MoneyHeli || nowCreating.GetComponent<UnitInfo>().type == Typeu.MoneyVehicle)
				{
					nowCreating.GetComponent<UnitInfo>().MyGoldMainHome = transform.gameObject;
				}
				nowCreating.GetComponent<UnitInfo>().whereToGoAfterSpawn = spawnIn.transform.FindChild("goto").gameObject;
				nowCreating.GetComponent<UnitInfo>().CreateWay();
				//nowCreating.GetComponent<UnitInfo>().setCreateStage(cState.Created);
				nowCreating = null;
				timer = 0;
			}
		}
	}
	
	void OnGUI()
	{
		if(transform.GetComponent<BuildingInfo>().isSelected && transform.GetComponent<BuildingInfo>().bst == bState.Builded)
		{
			int ii = 0;
			int ii1 = 0;
			int currOwner = tConfig.getId();
			if(ownerId == currOwner)
			{
				if(unitBuild.Count > 0)
				{
					foreach(GameObject uBuild in canBuild)
					{
						if(uBuild != null)
						{
							
							UnitInfo uInfo = uBuild.GetComponent<UnitInfo>();
							//Needed Money
							string uName = uInfo.Name;
							float xDelta = 0.000f;
							xDelta = (float)Screen.width/1280;
							if(xDelta > 1)
							{
								xDelta = 1;
							}
							if(ii < 5)
							{
								if (GUI.Button(new Rect(Screen.width/4  + Screen.width/12 * ii/xDelta,Screen.height - 180 ,60,60), uName)) 
								{
									addToBuild(uBuild);
								}
							}
							else if(ii > 4)
							{
								if (GUI.Button(new Rect(Screen.width/4  + Screen.width/12 * (ii-5)/xDelta,Screen.height - 100 ,60,60), uName)) 
								{
									addToBuild(uBuild);
								}
							}
							ii++;
						}
					}
				}
				if(queneCount.Count > 0)
				{
					foreach(GameObject unitT in queneCount)
					{
						if(unitT != null)
						{
							UnitInfo uInfo = unitT.GetComponent<UnitInfo>();
							string uName = uInfo.Name;
							float xDelta = 0.000f;
							xDelta = (float)Screen.width/1280;
							if(xDelta > 1)
							{
								xDelta = 1;
							}
							if(ii < 4)
							{
								
								if (GUI.Button(new Rect(Screen.width/30  + Screen.width/29 * ii/xDelta,Screen.height - 180 ,45,45), uName)) 
								{
									removeFromBuild(unitT);
								}
							}
							else if(ii > 3 && ii < 7)
							{
								if (GUI.Button(new Rect(Screen.width/30  + Screen.width/29 * (ii-3)/xDelta,Screen.height - 135 ,45,45), uName)) 
								{
									removeFromBuild(unitT);
								}
							}
							
							else if(ii > 6 && ii < 10)
							{
								if (GUI.Button(new Rect(Screen.width/30  + Screen.width/29 * (ii-6)/xDelta,Screen.height - 90 ,45,45), uName)) 
								{
									removeFromBuild(unitT);
								}							
							}
							ii++;
						}
					}
				}
			}
		}
	}
	
	public void addToBuild(GameObject unit)
	{
		if(queneCount.Count < 9)
		{
			queneCount.Add(unit);
			tConfig.BuyUnit(unit.GetComponent<UnitInfo>().needMoney);
		}
	}
	
	public void removeFromBuild(GameObject unit)
	{
		if(queneCount.Contains(unit))
		{
			queneCount.Remove(unit);
			if(unit == nowCreating)
			{
				nowCreating = null;
				nowCreatingTemp = null;
			}
		}
	}
	
	//spawn unit on spawn point if it's free
	public void SpawnUnit(GameObject spawn)
	{
		if(spawnList.Count > 0)
		{
			foreach(GameObject sp in spawnList)
			{
				if(isSpawnFree(sp))
				{
					spawn.transform.position = sp.transform.position;
				}
			}
		}
	}
	
	private void removeFromAble(GameObject rem)
	{
		canBuild.Remove(rem);
	}
	
	//current hangar is free?
	public bool isSpawnFree(GameObject sPoint)
	{
		return true;
	}
	
	//is one of all hangars is free
	public bool isAirForJetsFree()
	{
		int freeCount = 0;
		if(spawnList.Count > 0)
		{
			foreach(GameObject sp in spawnList)
			{
				if(sp.GetComponent<Hangar>().isFree())
				{
					freeCount++;
				}
			}
		}
		if(freeCount > 0)
		{
			return true;
		}
		return false;
	}
	
	//Building quene
	public List<GameObject> getBuildQuene()
	{
		return queneCount;
	}
	
	public void addToQuene(GameObject build)
	{
		queneCount.Add(build);
	}
}
