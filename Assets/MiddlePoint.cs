using UnityEngine;
using System.Collections;

public class MiddlePoint : MonoBehaviour {
	
	public GameObject first;
	public GameObject second;
	Vector3 currentDir;
	public float countways;
	public int end;
	public float addY;
	//public float maxCurrH;
	float currX;
	float currZ;
	float currY;
	// Use this for initialization
	void Start ()
	{
		currX = first.transform.position.x;
		currZ = first.transform.position.z;
		currY = first.transform.position.y;
	}
	
	// Update is called once per frame
	void Update ()
	{
		Vector3 firstCoord = first.transform.position;
		Vector3 secondCoord = second.transform.position;
		
		
		float delimX = Mathf.Abs((firstCoord.x-secondCoord.x)/(countways+1));
		//print ((firstCoord.x-secondCoord.x)/countways);
		//float delimX = (firstCoord.x+secondCoord.x)/countways;
		float delimZ = Mathf.Abs((firstCoord.z-secondCoord.z)/(countways+1));
		float distance_1 = Vector3.Distance(transform.position, currentDir);
		if(distance_1 < 10f)
		{
			if(end < countways)
			{
				if(firstCoord.x > secondCoord.x)
				{
					currX -= delimX;
				}
				else
				{
					currX += delimX;
				}
				
				if(firstCoord.z > secondCoord.z)
				{
					currZ -= delimZ;
				}
				else
				{
					currZ += delimZ;
				}
				if(end > Mathf.Floor((countways/2)))
				{
					currY -= addY;
				}
				else
				{
					currY += addY;
				}
				end++;
			}
			if(end <= Mathf.Floor((countways/2)))
			{
				currentDir = new Vector3(currX,currY,currZ);
			}
			else
			{
				currentDir = new Vector3(currX,currY,currZ);
			}
		}
	}
}
