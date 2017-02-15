using UnityEngine;
using System.Collections;

public class TankAttack : MonoBehaviour {
	
	public GameObject attackTarget;
	private bool doAttack;
	public float shellSpeed;
	public GameObject bullet;
	public GameObject shootFrom;
	public float cooldown;
	private float currentTime;
	public GameObject rotateBarrelHoriz;
	public GameObject rotateBarrelVert;
	Quaternion targetRotation;
	public GameObject fired;
	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(GetComponent<UnitInfo>().attackTarget != null)
		{
			attackTarget = GetComponent<UnitInfo>().attackTarget;
		}
		if(doAttack && attackTarget != null)
		{
			currentTime += Time.deltaTime;
			if(currentTime >= cooldown)
			{
				currentTime = 0;
				
				MakeAttack(attackTarget.transform.position);
			}
		}
		else if(attackTarget == null && doAttack)
		{
			currentTime = 0;
			doAttack = false;
		}
		if(attackTarget != null)
		{
			TargetingEnemy();
		}
		//
	}
	
	public void MakeAttack(Vector3 pos)
	{
		GameObject cShell = ((GameObject)Instantiate(bullet)).transform.gameObject;
		cShell.transform.position = shootFrom.transform.position;
		if(fired != null)
		{
			GameObject fireAnim = ((GameObject)Instantiate(fired)).transform.gameObject;
			fireAnim.transform.position = shootFrom.transform.position;
		}
		Vector3 angle = Quaternion.LookRotation(attackTarget.transform.position - cShell.transform.position).eulerAngles; 
		angle.x = cShell.transform.eulerAngles.x; 
		angle.z = cShell.transform.eulerAngles.z; 
		angle.y += 180.0f;
		cShell.transform.eulerAngles = angle;
		cShell.GetComponent<AmmoBehavior>().Fire(transform.gameObject,attackTarget,shellSpeed);
		doAttack = false;
		currentTime = 0;
		
	}
	
	public void orderToAttack(bool attack = false)
	{
		doAttack = attack;
	}
	
	public void TargetingEnemy()
	{
		Vector3 angle = Quaternion.LookRotation(attackTarget.transform.position - rotateBarrelHoriz.transform.position).eulerAngles; 
		angle.x = rotateBarrelHoriz.transform.eulerAngles.x; 
		angle.z = rotateBarrelHoriz.transform.eulerAngles.z; 
		angle.y += 90.0f;
		rotateBarrelHoriz.transform.eulerAngles = angle;
		//rotateBarrelHoriz.transform.rotation = Quaternion.AngleAxis(Time.deltaTime * angle, Vector3.up) * rotateBarrelHoriz.transform.rotation;
		
		Vector3 angle1 = Quaternion.LookRotation(attackTarget.transform.position - rotateBarrelVert.transform.position).eulerAngles; 
		angle1.y += 90.0f; 
		angle1.z = angle1.x; 
		angle1.x = rotateBarrelVert.transform.eulerAngles.x; 
		rotateBarrelVert.transform.eulerAngles = angle1;
	}
}
