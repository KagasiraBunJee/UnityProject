using UnityEngine;
using System.Collections;

public class InfantryAttack : MonoBehaviour {
	
	public float rotateSpeed;
	
	public GameObject attackTarget;
	float attackSpeed;
	public bool attackOrder;
	public bool readyToAttack;
	public GameObject weaponPlace;
	public GameObject weapon;
	public GameObject bullet;
	public float bulletSpeed;
	//Turn 
	private Quaternion targetRotation;
	
	public float timer;
	GameObject weap;
	// Use this for initialization
	void Start ()
	{
		weap = Instantiate(weapon, new Vector3 (0, 0, 0), new Quaternion(0,0,0,0)) as GameObject;
		weap.transform.parent = weaponPlace.transform;
		weaponPlace.transform.GetChild(0).localPosition = Vector3.zero;
		weaponPlace.transform.GetChild(0).localRotation = new Quaternion(0,0,0,0);
	}
	
	// Update is called once per frame
	void Update ()
	{
		
		if(weaponPlace.transform.GetChild(0).localPosition != Vector3.zero)
		{
			weaponPlace.transform.GetChild(0).localPosition = Vector3.zero;
			weaponPlace.transform.GetChild(0).localRotation = new Quaternion(0,0,0,0);
		}
		
		timer += Time.deltaTime;
		if(timer > attackSpeed)
		{
			timer = attackSpeed;
			readyToAttack = true;
		}
		else
		{
			readyToAttack = false;
		}
		if(attackTarget != null)
		{
			if(readyToAttack && attackOrder)
			{
				Fire();
			}
		}
		attackSpeed = GetComponent<UnitInfo>().shootSpeed;
		if(attackTarget != null && Vector3.Distance(transform.position, attackTarget.transform.position) <= GetComponent<UnitInfo>().attackDistance)
		{
			TurningToTarget();
		}
	}
	
	public void Fire()
	{
		GameObject barrel = weap.transform.FindChild("shootFrom").gameObject;
		if(barrel != null)
		{
			Vector3 barrel_pos = barrel.transform.position;
			GameObject bull = ((GameObject)Instantiate(bullet)).transform.gameObject;
			bull.transform.position = barrel_pos;
			Vector3 angle = Quaternion.LookRotation(attackTarget.transform.position - barrel.transform.position).eulerAngles; 
			angle.x = bull.transform.eulerAngles.x; 
			angle.z = bull.transform.eulerAngles.z; 
			angle.y += 180.0f;
			bull.transform.eulerAngles = angle;
			bull.GetComponent<AmmoBehavior>().Fire(transform.gameObject,attackTarget,bulletSpeed);
		}

		
		
		print("Fire");
		timer = 0;
		attackOrder = false;
	}
	
	public void TurningToTarget()
	{
		
		Vector3 angle = Quaternion.LookRotation(attackTarget.transform.position - transform.position).eulerAngles; 
		angle.x = transform.eulerAngles.x; 
		angle.z = transform.eulerAngles.z; 
		//angle.y += 90.0f;
		transform.eulerAngles = angle;
	}
	
	public void orderToAttack(GameObject target, bool order)
	{
		attackTarget = target;
		attackOrder = order;
	}
}
