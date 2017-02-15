using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class BuildSelection : MonoBehaviour {
	
	public List<GameObject> allBuildings;
	public List<GameObject> myBuildings;
	public GameObject selectedBuild;
	public int myTeam;
	public int myCoal;
	public int myId;
	public float xy;
	//private WorldCamera
	private TeamConfig tConf;
	private RaycastHit hit;
	public Transform currentBuilding;
	
	//Building Build Config
	private GameObject buildingBuild;
	private GameObject builderBuild;
	
	public bool wantToBuild;
	public bool hasPlaced;
	private bool settingPosition;
	// Use this for initialization
	void Start () 
	{
		tConf = GetComponent<TeamConfig>();
		myTeam = tConf.getMyTeam();
		myCoal = tConf.getMyCoal();
		myId = tConf.getId();
		wantToBuild = false;
	}
	
	// Update is called once per frame
	void Update () 
	{
		Vector3 m = Input.mousePosition;
		Ray p = Camera.main.ScreenPointToRay(m);
		if(wantToBuild)
		{
			if(currentBuilding != null && !hasPlaced)
			{
				if(Physics.Raycast(p,out hit,200))
				{
					if(hit.transform.name == "Terrain")
					{
						//currentBuilding.GetComponent<BuildingList>().cam = GetComponent<WorldCamera>();
						//currentBuilding.rigidbody.position = hit.point;
						currentBuilding.transform.position = new Vector3(hit.point.x,hit.point.y+0.12f,hit.point.z);
					}
				}
				if(Input.GetMouseButtonDown(0))
				{
					hasPlaced = true;
				}
				if(Input.GetMouseButtonUp(1))
				{
					wantToBuild = false;
					Destroy(currentBuilding.gameObject);
					currentBuilding = null;
				}
			}
			if(Physics.Raycast(p,out hit,200))
			{
				if(hit.transform.tag == "building")
				{
					if(hasPlaced && currentBuilding != null)
					{
						if(currentBuilding.GetComponent<BuildingInfo>().bst == bState.ReadyToBuild)
						{
							GameObject[] bTarget = GameObject.FindGameObjectsWithTag("Unit");
							foreach(GameObject cuurObj in bTarget)
							{
								bool selected = cuurObj.GetComponent<UnitInfo>().isSelected;
								if(selected)
								{
									Typeu tUnit = cuurObj.GetComponent<UnitInfo>().type;
									if(tUnit == Typeu.Builder)
									{
										if(currentBuilding.GetComponent<BuildingInfo>().bst == bState.ReadyToBuild)
										{
											currentBuilding.GetComponent<BuildingInfo>().setBuildState(bState.PreperingToBuild);
											currentBuilding.position = new Vector3(currentBuilding.position.x,currentBuilding.position.y+0.12f,currentBuilding.position.z);
											this.GetComponent<UnitSelection>().setIsBuilding(true);
											cuurObj.GetComponent<UnitInfo>().currentBuildTarget = currentBuilding.gameObject;
											cuurObj.GetComponent<UnitInfo>().goToBuild(currentBuilding.gameObject);
											cuurObj.GetComponent<UnitInfo>().setUnitState(State.GoToBuild);
											buildingBuild = currentBuilding.gameObject;
											builderBuild = cuurObj;
											hasPlaced = false;
											currentBuilding = null;
											wantToBuild = false;
										}
									}
								}
							}
						}
					}
					/*if(Input.GetMouseButtonDown(0))
					{
						if(hit.transform.gameObject.GetComponent<BuildingInfo>())
						{
							selectedBuild = hit.transform.gameObject;
							BuildingInfo bInf = selectedBuild.GetComponent<BuildingInfo>();
							bInf.isSelected = true;
						}
					}*/
				}
				/*if(Input.GetMouseButtonDown(0))
				{
					if(hit.transform.tag != "building")
					{
						if(selectedBuild != null)
						{
							BuildingInfo bInf = selectedBuild.GetComponent<BuildingInfo>();
							if(bInf.isSelected)
							{
								bInf.isSelected = false;
							}
							selectedBuild = null;
						}					
					}
				}*/
			}
		}
		if(buildingBuild != null)
		{
			Vector3 buildPos = new Vector3(builderBuild.transform.position.x, 0 , builderBuild.transform.position.z);
			Vector3 constructpos = new Vector3(buildingBuild.transform.position.x, 0 ,buildingBuild.transform.position.z);
			
			if(Vector3.Distance(buildPos, constructpos) < 0.9f && buildingBuild.GetComponent<BuildingInfo>().bst == bState.PreperingToBuild && builderBuild.GetComponent<UnitInfo>().sType == State.GoToBuild)
			{
				
				builderBuild.GetComponent<UnitInfo>().stopMoving();
				builderBuild.GetComponent<UnitInfo>().setUnitState(State.Building);
				buildingBuild.GetComponent<BuildingInfo>().bst = bState.Building;
			}
		}
	}
	
	void OnGUI()
	{

	}
	
	public void SetSelectedBuilding(GameObject go)
	{
		hasPlaced = false;
		wantToBuild = true;
		currentBuilding = ((GameObject)Instantiate(go)).transform;
	}
	
	public void setSetting(bool setState)
	{
		settingPosition = setState;
	}
	
}
