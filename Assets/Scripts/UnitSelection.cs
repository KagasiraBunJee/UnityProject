using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitSelection : MonoBehaviour {
	
	
	//Глобальные переменные
	private Rect objRect;
	private Vector2 MousePos;
	//move it away from the mouse cursor
	public Vector3 offset = new Vector3 (16, 16, 0);
	
	public List<GameObject> allUnits;
	
	public List<GameObject> selectedUnits;
	
	public bool isBuilding;
	
	public Camera cam;
	
	public GameObject tart;
	
	private RaycastHit hit;
	
	TeamConfig tc;
	
	//public List<GameObject> squad1,squad2,squad3,squad4,squad5,squad6,squad7,squad8,squad9,squad0;

	// Use this for initialization
	void Start ()
	{
		objRect = new Rect(0, 0, 200, 35);
		MousePos = new Vector2(0, 0);
		tc = Camera.main.transform.parent.GetComponent<TeamConfig>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(Input.GetMouseButtonUp(1))
		{
			if(MouseInNormalPosition(Input.mousePosition))
			{
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				if(Physics.Raycast(ray,out hit,500))
				{
					if(hit.transform.tag == "Unit")
					{
						GameObject this_unit = hit.transform.gameObject;
						int ownerId = this_unit.GetComponent<UnitInfo>().owner;
						if(ownerId == tc.getId())
						{
							if(Input.GetKey(KeyCode.LeftControl))
							{
								if(selectedUnits.Count > 0)
								{
									foreach(GameObject v in selectedUnits)
									{
										v.GetComponent<UnitInfo>().attackTarget = this_unit;
										v.GetComponent<UnitInfo>().GoToAttack(hit.point,true,true);
										v.GetComponent<UnitInfo>().attackMoving = false;
									}
								}
							}
						}
						else
						{
							int _team = this_unit.GetComponent<UnitInfo>().team;
							//if unit is empty/npc/misc
							if(_team == 0)
							{
								if(Input.GetKey(KeyCode.LeftControl))
								{
									if(selectedUnits.Count > 0)
									{
										foreach(GameObject v in selectedUnits)
										{
											v.GetComponent<UnitInfo>().attackTarget = this_unit;
											v.GetComponent<UnitInfo>().GoToAttack(hit.point,true,true);
											v.GetComponent<UnitInfo>().attackMoving = false;
										}
									}									
								}
								//Here must be
								//enter to empty vehicle
							}
							//if unit is from my team
							else if(tc.alliance.Contains(_team))
							{
								//Nothing to do
								if(Input.GetKey(KeyCode.LeftControl))
								{
									if(selectedUnits.Count > 0)
									{
										foreach(GameObject v in selectedUnits)
										{
											v.GetComponent<UnitInfo>().attackTarget = this_unit;
											v.GetComponent<UnitInfo>().GoToAttack(hit.point,true,true);
											v.GetComponent<UnitInfo>().attackMoving = false;
										}
									}									
								}
							}
							//Enemy
							else
							{
								if(selectedUnits.Count > 0)
								{
									foreach(GameObject v in selectedUnits)
									{
										
										v.GetComponent<UnitInfo>().attackTarget = this_unit;
										v.GetComponent<UnitInfo>().GoToAttack(hit.point,false,true);
									}
								}
							}
						}
					}
					else if(hit.transform.tag == "building")
					{
						GameObject this_build = hit.transform.gameObject;
						int ownerId = this_build.GetComponent<BuildingInfo>().ownerId;
						if(ownerId == tc.getId())
						{
							if(Input.GetKey(KeyCode.LeftControl))
							{
								if(selectedUnits.Count > 0)
								{
									foreach(GameObject v in selectedUnits)
									{
										v.GetComponent<UnitInfo>().attackTarget = this_build;
										v.GetComponent<UnitInfo>().GoToAttack(hit.point,true,true);
										v.GetComponent<UnitInfo>().attackMoving = false;
									}
								}
							}
							else
							{
								BuildingInfo bu = this_build.GetComponent<BuildingInfo>();
								if(bu.bst == bState.PreperingToBuild || bu.bst == bState.ReadyToBuild)
								{
									if(!bu.HaveBuilder())
									{
										
										if(selectedUnits.Count > 0)
										{
											foreach(GameObject v in selectedUnits)
											{
												if(v.GetComponent<UnitInfo>().type == Typeu.Builder)
												{
													v.GetComponent<UnitInfo>().goToBuild(this_build);
												}
											}
										}										
									}
								}
								else if(bu.bst == bState.Building)
								{
									
								}
								else
								{
									if(bu.uType == bType.RepairCenterGeneral || bu.uType == bType.VehicleFactory)
									{
										//GoToRepair
									}
									else if(bu.uType == bType.Warehouse)
									{
										//Go to warehouse and givemoney
										if(selectedUnits.Count > 0)
										{
											foreach(GameObject v in selectedUnits)
											{
												UnitInfo uu = v.GetComponent<UnitInfo>();
												if(uu.type == Typeu.MoneyVehicle || uu.type == Typeu.MoneyHeli)
												{
													uu.isGetting = true;
													if(uu.currentMoneyGot > 0)
													{
														uu.sType = State.GettingMoney;
													}
													else
													{
														uu.sType = State.Idle;
													}
												}
											}
										}									
									}
									else if(bu.uType == bType.InfanryBarracks || bu.uType == bType.HospitalGeneral)
									{
										//GotoHealInfantry
									}
									else if(bu.uType == bType.Airfield)
									{
										//Goto reapir air units
									}
									else if(bu.uType == bType.WaterStation)
									{
										//Goto repair ships
									}
								}
							}
						}
						else
						{
							int _team = this_build.GetComponent<BuildingInfo>().team;
							//if unit is empty/npc/misc
							if(_team == 0)
							{
								if(Input.GetKey(KeyCode.LeftControl))
								{
									if(selectedUnits.Count > 0)
									{
										foreach(GameObject v in selectedUnits)
										{
											v.GetComponent<UnitInfo>().attackTarget = this_build;
											v.GetComponent<UnitInfo>().GoToAttack(hit.point,true,true);
											v.GetComponent<UnitInfo>().attackMoving = false;
										}
									}									
								}
								//Here must be
								//capture empty building/GasStation/OilFactory/RepairStation/Airborn/Hospital/Bunkers
								else
								{
									
								}
							}
							//if unit is from my team
							else if(tc.alliance.Contains(_team))
							{
								//Nothing to do
								if(Input.GetKey(KeyCode.LeftControl))
								{
									if(selectedUnits.Count > 0)
									{
										foreach(GameObject v in selectedUnits)
										{
											v.GetComponent<UnitInfo>().attackTarget = this_build;
											v.GetComponent<UnitInfo>().GoToAttack(hit.point,true,true);
											v.GetComponent<UnitInfo>().attackMoving = false;
										}
									}									
								}
							}
							//Enemy
							else
							{
								if(selectedUnits.Count > 0)
								{
									foreach(GameObject v in selectedUnits)
									{
										v.GetComponent<UnitInfo>().attackTarget = this_build;
										v.GetComponent<UnitInfo>().GoToAttack(hit.point,false,true);
										v.GetComponent<UnitInfo>().attackMoving = false;
									}
								}
							}							
						}
					}
					else if(hit.transform.tag == "Terrain")
					{
						if(Input.GetKey(KeyCode.LeftControl))
						{
							if(selectedUnits.Count > 0)
							{
								foreach(GameObject v in selectedUnits)
								{
									//v.GetComponent<UnitInfo>().attackTarget = null;
									v.GetComponent<UnitInfo>().GoToAttack(hit.point,true,true);
									v.GetComponent<UnitInfo>().attackMoving = false;
								}
							}							
						}
						//Cancel Attack
						else
						{
							if(selectedUnits.Count > 0)
							{
								foreach(GameObject v in selectedUnits)
								{
									if(v.GetComponent<UnitInfo>().attackTarget != null)
									{
										v.GetComponent<UnitInfo>().AttackMoving();
									}
								}
							}
						}
					}
					//Cancel attack
					else
					{
						if(selectedUnits.Count > 0)
						{
							foreach(GameObject v in selectedUnits)
							{
								v.GetComponent<UnitInfo>().CancelAttack();
								
							}
						}
					}
					SendAllSelectedUnitsTo();
				}
			}
		}
	}
	
	void OnGUI()
	{

	}
	
	void SendAllSelectedUnitsTo()
	{
		if(selectedUnits.Count > 0)
		{
			foreach(GameObject v in selectedUnits)
			{
				if(v != null)
				{
					v.GetComponent<UnitInfo>().goToMark();
				}
			}
		}
	}
	
	public bool isSelected(GameObject unit)
	{
		if(!selectedUnits.Contains(unit))
		{
			return false;
		}
		
		return true;
	}
	
	public void setIsBuilding(bool state)
	{
		isBuilding = state;
	}
	
	private bool MouseInNormalPosition(Vector3 pos)
	{
		if(pos.y < 200)
		{
			return false;
		}
		else return true;
		//Vector2 screenPos = Event.current.mousePosition;
		//if(screenPos.y < Screen.height)
		//{
		//	return false;
		//}
	}
}
