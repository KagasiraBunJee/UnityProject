using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class BuilderList : MonoBehaviour {
	
	public List<GameObject> bList;
	public List<GameObject> canBuild;
	
	public Camera cam;
	private int myCoal;
	public bool isBuild;
	public float b = 1.00f;
	private UnitInfo uInfo;
	private double timer;
	private WorldCamera wcam;
	
	// Use this for initialization
	void Start () 
	{
		uInfo = transform.GetComponent<UnitInfo>();
		cam = Camera.main;
	}
	
	// Update is called once per frame
	void Update () 
	{
		foreach(GameObject b in bList)
		{
				//Requirements
			string bName = b.GetComponent<BuildingInfo>().getName();
			int bTeam = b.GetComponent<BuildingInfo>().getTeam();
			int bCoal = b.GetComponent<BuildingInfo>().getCoalition();
			int ownId = b.GetComponent<BuildingInfo>().getOwnerId();
			int needLvl = b.GetComponent<BuildingInfo>().needLevel();
			int needMoney = b.GetComponent<BuildingInfo>().needMoney();
			//currState
			wcam = cam.transform.parent.GetComponent<WorldCamera>();
			int currLvl = wcam.GetComponent<TeamConfig>().getLevel();
			int currMoney = wcam.GetComponent<TeamConfig>().getMoney();
			int currEnergy = wcam.GetComponent<TeamConfig>().getEnergy();
			int currTeam = wcam.GetComponent<TeamConfig>().getMyTeam();
			int currCoalition = wcam.GetComponent<TeamConfig>().getMyCoal();
			int currOwnerId = wcam.GetComponent<TeamConfig>().myId;
			if(currOwnerId == uInfo.owner)
			{
				if(currLvl >= needLvl)
				{
					if(currMoney >= needMoney)
					{
						if(!canBuild.Contains(b))
						{
							canBuild.Add(b);
						}
					}
					else
					{
						canBuild.Remove(b);
					}
				}
				else
				{
					canBuild.Remove(b);
				}
			}
			else
			{
				canBuild.Remove(b);
			}
		}
	}
	
	//GUI
	void OnGUI()
	{
		
		if(transform.gameObject.GetComponent<UnitInfo>().isSelect())
		{
			int i = 1;
			int ii = 0;
			foreach(GameObject b in canBuild)
			{
				string bName = b.GetComponent<BuildingInfo>().getName();
				
				float xDelta = 0.000f;
				xDelta = (float)Screen.width/1280;
				if(xDelta > 1)
				{
					xDelta = 1;
				}
				//print (Screen.width + "x" + Screen.height);
				if(ii < 5)
				{
					if (GUI.Button(new Rect(Screen.width/4  + Screen.width/12 * ii/xDelta,Screen.height - 180 ,60,60), bName)) 
					{
						wcam.GetComponent<UnitSelection>().setIsBuilding(true);
						wcam.GetComponent<BuildSelection>().SetSelectedBuilding(b);
						//b.GetComponent<BuildingList>().cam = cam.transform.parent.transform.GetComponent<WorldCamera>();
						wcam.GetComponent<BuildSelection>().setSetting(true);
						transform.gameObject.GetComponent<UnitInfo>().setUnitState(State.PrepareToBuild);
						wcam.GetComponent<TeamConfig>().BuyUnit(b.GetComponent<BuildingInfo>().needMoney());
						//buildingPlacement.SetItem(buildings[i]);
					}
				}
				else if(ii > 4)
				{
					if (GUI.Button(new Rect(Screen.width/4  + Screen.width/12 * (ii-5)/xDelta,Screen.height - 100 ,60,60), bName)) 
					{
						wcam.GetComponent<UnitSelection>().setIsBuilding(true);
						wcam.GetComponent<BuildSelection>().SetSelectedBuilding(b);
						//b.GetComponent<BuildingList>().cam = cam.transform.parent.transform.GetComponent<WorldCamera>();
						wcam.GetComponent<BuildSelection>().setSetting(true);
						transform.gameObject.GetComponent<UnitInfo>().setUnitState(State.PrepareToBuild);
						wcam.GetComponent<TeamConfig>().BuyUnit(b.GetComponent<BuildingInfo>().needMoney());
						//buildingPlacement.SetItem(buildings[i]);
					}							
				}
				ii++;
			}
		}
	}
	
	public void DesireOnCollision(GameObject obj)
	{
/*		if(cam.GetComponent<UnitSelection>().isBuilding)
		{
			obj.GetComponent<UnitInfo>().stopMoving();
			cam.GetComponent<UnitSelection>().setIsBuilding(false);
		}
		else obj.GetComponent<UnitInfo>().stopMoving();*/
	}
}
