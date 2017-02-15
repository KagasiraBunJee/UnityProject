using UnityEngine;
using System.Collections;

public class AngleScript : MonoBehaviour {

public float BallisticKoef = 5f;  // это подбираем руками
public Transform targetPosition; // сюда передаем координаты цели
public float speed;
public Vector3 start;    
	
	void Start()
	{
		start = targetPosition.position;
	}
	
	void Update () 
	{
	    float distance = Vector3.Distance(start , transform.position);
	    transform.forward = Vector3.Lerp(transform.forward, start - transform.position, Time.deltaTime * (BallisticKoef / distance));
		//transform.Translate(transform.up * speed * Time.deltaTime);
		//Up
		transform.Translate(0,speed/2.3f * Time.deltaTime,0);
		//forward
	    transform.Translate(0,0,speed * Time.deltaTime);
	}
	
}