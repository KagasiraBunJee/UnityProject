using UnityEngine;
using System.Collections;

public class Fans : MonoBehaviour {
	
	public float rotateSpeed;
	private float start = 0;
	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		//transform.rotation = Quaternion.RotateTowards(transform.rotation, transform.rotation, rotateSpeed*Time.deltaTime);
		//transform.RotateAround (transform.position, Vector3.zero, Mathf.SmoothStep (0, 90, Time.deltaTime * rotateSpeed) );
		start += rotateSpeed*2*Time.deltaTime;
		if(start > 360)
		{
			start = 0;
		}
		transform.rotation = Quaternion.Euler(new Vector3(270,start,0));
	}
}
