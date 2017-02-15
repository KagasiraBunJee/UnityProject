using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TeamConfig : MonoBehaviour {
	
	public int myTeam;
	public int myCoalition;
	public int myId;
	public int myLvl;
	public int myMoney;
	public int myEnergy;
	public int upgradePoints;
	public List<int> alliance;
	public GameObject way;
	
	public List<GameObject> enhancements;
	
	// Use this for initialization
	void Start () 
	{
		myMoney = 10000;
		myEnergy = 0;
		myLvl = 1;
		upgradePoints = 1;
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
	
	void OnGUI()
	{
		GUI.TextField(new Rect(100,100,100,30), myMoney.ToString());
	}
	
	//ID of coalition
	public int getMyCoal()
	{
		return myCoalition;
	}
	
	public void setMyCoal(int coal)
	{
		myCoalition = coal;
	}
	
	//id of my team
	public int getMyTeam()
	{
		return myTeam;
	}
	
	public void setMyTeam(int teamId)
	{
		myTeam = teamId;
	}
	
	//My Id
	public int getId()
	{
		return myId;
	}
	
	public void setId(int id)
	{
		myId = id;
	}
	
	//CurrentMoney
	public int getMoney()
	{
		return myMoney;
	}
	
	public void setMoney(int amount)
	{
		myMoney = amount;
	}

	public void addMoney(int amount)
	{
		myMoney += amount;
	}	
	
	//Current Energy
	public int getEnergy()
	{
		return myEnergy;
	}
	
	public void setEnergy(int energy)
	{
		myEnergy = energy;
	}
	
	//Current Level
	public int getLevel()
	{
		return myLvl;
	}
	
	public void setLevel(int lvl)
	{
		myLvl = lvl;
	}
	
	//Current upgrade Points
	public int getUP()
	{
		return upgradePoints;
	}
	
	public void setUP(int points)
	{
		upgradePoints = points;
	}
	
	public void BuyUnit(int price)
	{
		myMoney -= price;
	}
}
