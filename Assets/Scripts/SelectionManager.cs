using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SelectionManager : MonoBehaviour {
	
	
	private RaycastHit hit;
	public bool mouseDrag = false;
	public GameObject selectedBuilding;
	public List<GameObject> selectedUnits;
	public Vector3 mouseDownPos;
	public bool mouseSelection;
	public bool shiftPressed;
	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Input.GetMouseButtonUp(0))
		{
			if(MouseInNormalPosition(Input.mousePosition))
			{
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				if(Physics.Raycast(ray,out hit,500))
				{
					if(!mouseSelection)
					{
						
						if(hit.transform.tag == "Unit")
						{
							GameObject unit = hit.collider.gameObject;
							if(!shiftPressed)
							{
								selectOneUnit(unit);
							}
							else if(shiftPressed)
							{
								selectSomeUnits(unit);
							}
							removeSelectedBuilding(selectedBuilding);
						}
						else if(hit.collider.tag == "building")
						{
							GameObject building = hit.transform.gameObject;
							if(selectedBuilding != null)
							{
								selectedBuilding.GetComponent<BuildingInfo>().isSelected = false;
							}
							if(!GetComponent<BuildSelection>().wantToBuild)
							{
								selectedBuilding = building;
								UnSelectAllUnits();
							}
						}
						else if(hit.collider.name == "Misc")
						{
							
						}
						else
						{
							removeSelectedBuilding(selectedBuilding);
							UnSelectAllUnits();
						}
						addSelectedUnit(selectedUnits);
						addSelectedBuilding(selectedBuilding);
					}
					else
					{
						removeSelectedBuilding(selectedBuilding);
						UnSelectAllUnits();
						SelectUnitsInThisArea(mouseDownPos,hit.point);
					}
				}
			}
			mouseSelection = false;
		}
		if(Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
		{
			shiftPressed = true;
		}
		if(Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift))
		{
			shiftPressed = false;
		}
		if(Input.GetMouseButtonDown(0))
		{
			//Mouse Drag
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if(Physics.Raycast(ray,out hit,500))
			{
				mouseDownPos = hit.point;
			}
		}
		if(Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
		{
			mouseDrag = true;
			if(Input.GetMouseButton(0))
			{
				mouseSelection = true;
			}
		}
		else
		{
			mouseDrag = false;
		}
		updateStatus();
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

	private void SelectUnitsInThisArea(Vector3 p1, Vector3 p2)
	{
		Vector3 p3;
		Vector3 p4;
		Vector3 temp;
		if(!shiftPressed)
			if(selectedUnits != null)
				selectedUnits.Clear();
		if(p1.x > p2.x)
		{
			temp = p1;
			p1 = new Vector3(p2.x,p1.y,p1.z);
			p2 = new Vector3(temp.x, p2.y, p2.z);
		}
		if(p1.z > p2.z)
		{
			temp = p1;
			p1 = new Vector3(p1.x,p1.y,p2.z);
			p2 = new Vector3(p2.x, p2.y, temp.z);
		}
		p3 = new Vector3(p1.x,p1.y,p2.z);
		p4 = new Vector3(p2.x,p1.y,p1.z);
		
		List<GameObject> allUnits = new List<GameObject>(GameObject.FindGameObjectsWithTag("Unit"));
		
		foreach(GameObject m in allUnits)
		{
			if(m != null)
			{
				
				UnitInfo u = m.GetComponent<UnitInfo>();
				int teamId = u.team;
				int ownerId = u.owner;
				TeamConfig tc = transform.gameObject.GetComponent<TeamConfig>();
				int myTeamId = tc.getMyTeam();
				int myId = tc.getId();
				Vector3 upos = m.transform.position;
				if(upos.x > p1.x && upos.x < p2.x && upos.z > p1.z && upos.z < p2.z)
				{
					if(selectedUnits != null)
					{
						if(!selectedUnits.Contains(m))
						{
							if(myId == ownerId && teamId == myTeamId)
							{
								selectedUnits.Add(m);
								m.GetComponent<UnitInfo>().selectCurr(true);
							}
						}
					}
				}
			}
		}
		addSelectedUnit(selectedUnits);
	}
	
	public void selectOneUnit(GameObject unit)
	{
		UnSelectAllUnits();
		selectedUnits.Add(unit);
		unit.GetComponent<UnitInfo>().selectCurr(true);
	}
	
	public void selectSomeUnits(GameObject unit)
	{
		if(!selectedUnits.Contains(unit))
		{
			selectedUnits.Add(unit);
			unit.GetComponent<UnitInfo>().selectCurr(true);
		}		
	}
	
	public void removeOneUnit(GameObject unit)
	{
		selectedUnits.Remove(unit);
		unit.GetComponent<UnitInfo>().selectCurr(false);		
	}
	
	public void addSelectedUnit(List<GameObject> units)
	{
		transform.GetComponent<UnitSelection>().selectedUnits = units;
	}
	
	public void updateStatus()
	{
		if(selectedUnits != null)
		{
			if(selectedUnits.Count > 0)
			{
				foreach(GameObject unit in selectedUnits)
				{
					if(unit != null)
					{
						unit.GetComponent<UnitInfo>().selectCurr(true);
					}
				}
			}
		}
	}
	
	public void addSelectedBuilding(GameObject build)
	{
		
		GetComponent<BuildSelection>().selectedBuild = build;
		if(build != null)
		{
			build.GetComponent<BuildingInfo>().isSelected = true;
		}
	}
	
	public void removeSelectedBuilding(GameObject build)
	{
		if(build != null)
		{
			build.GetComponent<BuildingInfo>().isSelected = false;
		}
		selectedBuilding = null;
		GetComponent<BuildSelection>().selectedBuild = null;	
	}
	
	public void UnSelectAllUnits()
	{
		if(selectedUnits.Count > 0)
		{
			foreach(GameObject unit in selectedUnits)
			{
				if(unit != null)
				{
					unit.GetComponent<UnitInfo>().selectCurr(false);
				}
			}
			selectedUnits.Clear();
		}
	}
}
