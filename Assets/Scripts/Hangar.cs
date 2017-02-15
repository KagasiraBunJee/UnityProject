using UnityEngine;
using System.Collections;

public class Hangar : MonoBehaviour {
	
	public GameObject unit;
	
	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
	
	public void toggleFree(GameObject u)
	{
		unit = u;
	}
	
	public bool isFree()
	{
		if(unit != null) return false;
		return true;
	}
	
	public void freeHangar()
	{
		unit = null;
	}
	
}
