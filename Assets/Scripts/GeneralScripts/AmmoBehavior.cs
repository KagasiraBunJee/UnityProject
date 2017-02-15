using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum shType
{
	mainShell,
	atomicShell,
	Bullet,
	atomicBullet,
	missiles_Veh,
	AA_Missiles,
	AT_Missiles,
	AT_Missiles_Atomic,
	EM_AA_Missiles,
	EM_AT_Missiles,
	Anti_Ship_Missiles,
	EM_Anti_Ship_Missiles,
	Ballistic_Missiles,
	LRM,
	AtomicMainMissile,
	HellFire_Missiles,
	Atomic_missile_Inf,
	Artillery,
	Artillery_Atomic
}

public class AmmoBehavior : MonoBehaviour {

	private bool go;
	public shType aType;
	public float speed;
	private int owner;
	// for auto guiding missiles
	private GameObject followedTarget;
	public GameObject lookAtTarget;
	private GameObject sender;
	//for ballistic
	private Vector3 lastPosition;
	private Vector3 firstCoord;
	private Vector3 currentDir;
	public float BallisticKoef;
	private float timer;
	private bool targeted;
	private float currX;
	private float currZ;
	private float currY;
	public float countways;
	public int end;
	public float addY;
	public float damagePower;
	public float damageRadius;
	private List<GameObject> receivedDamage;
	public GameObject sparkEffect;
	public GameObject afterEffect;
	private bool destroyObjectAfterBool;
	private float destroyObjectAfter;
	private float lasttimehitted;
	private Vector3 lastcoords;
	// Use this for initialization
	void Start ()
	{
		targeted = false;
		
	}
	
	void Awake()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(go)
		{
			if(damagePower == 0)
			{
				damagePower = sender.GetComponent<UnitInfo>().attackDamage;
			}
			if(lookAtTarget != null)
			{
				lastcoords = lookAtTarget.transform.position;
			}
			if(aType == shType.Bullet || aType == shType.atomicBullet || aType == shType.mainShell || aType == shType.atomicShell)
			{
				//transform.Translate(0,0,-speed/300*Time.deltaTime);
				//transform.LookAt(lookAtTarget.transform);
				
				
	    		float distance = Vector3.Distance(lastcoords , transform.position);
	    		transform.forward = Vector3.Lerp(transform.forward, transform.position - lastcoords , Time.deltaTime * (10 / distance));
				//transform.Translate(transform.up * speed * Time.deltaTime);
				//Up
				//transform.Translate(0,speed/2.3f * Time.deltaTime,0);
				//forward
	    		transform.Translate(0,0,-speed * Time.deltaTime);
				
			}
			else if(aType == shType.AT_Missiles || aType == shType.AT_Missiles_Atomic)
			{
				//transform.Translate(0,0,-speed/300*Time.deltaTime);
				//transform.LookAt(lookAtTarget.transform);
				
				
	    		float distance = Vector3.Distance(lastcoords , transform.position);
	    		transform.forward = Vector3.Lerp(transform.forward, transform.position - lastcoords , Time.deltaTime * (10 / distance));
				//transform.Translate(transform.up * speed * Time.deltaTime);
				//Up
				//transform.Translate(0,speed/2.3f * Time.deltaTime,0);
				//forward
	    		transform.Translate(0,0,-speed * Time.deltaTime);
				
			}
			else if(aType == shType.Ballistic_Missiles)
			{
				if(!targeted)
				{
					lastPosition = lastcoords;
					currX = transform.position.x;
					currZ = transform.position.z;
					currY = transform.position.y;
					firstCoord = transform.position;
					targeted = true;
				}
				//Vector3 start = lookAtTarget.transform.position;
				//timer += Time.deltaTime;
				

				float delimX = Mathf.Abs((firstCoord.x-lastPosition.x)/(countways+1));
				
				//float delimX = (firstCoord.x+secondCoord.x)/countways;
				float delimZ = Mathf.Abs((firstCoord.z-lastPosition.z)/(countways+1));
				float distance_1 = Vector3.Distance(transform.position, currentDir);
				if(currentDir == Vector3.zero)
				{
					currentDir = new Vector3(currX,currY,currZ);
				}
				print (distance_1);
				if(distance_1 < 8f)
				{
					if(end < countways)
					{
						if(firstCoord.x > lastPosition.x)
						{
							currX -= delimX;
						}
						else
						{
							currX += delimX;
						}
						
						if(firstCoord.z > lastPosition.z)
						{
							currZ -= delimZ;
						}
						else
						{
							currZ += delimZ;
						}
						if(end > (Mathf.Floor((countways/2))+1))
						{
							//currY -= addY;
						}
						else
						{
							currY += addY;
						}
						end++;
					}
					if(end <= (Mathf.Floor((countways/2))+1))
					{
						currentDir = new Vector3(currX,currY,currZ);
					}
					else if(end == (Mathf.Floor((countways/2))+2))
					{
						currentDir = new Vector3(currX,currY,currZ);
					}
					else
					{
						currentDir = lastPosition;
					}
				}
	    		/*float distance = Vector3.Distance(currentDir , transform.position);
				Quaternion rotat = Quaternion.LookRotation(transform.forward, transform.position - currentDir);
	    		//transform.forward -= Vector3.Lerp(transform.forward, currentDir - transform.position, Time.deltaTime * (BallisticKoef / distance));
				transform.rotation = Quaternion.Slerp(transform.rotation, rotat, Time.time * speed);
					//ransform.Translate(transform.up * speed * Time.deltaTime);
					//p
				//transform.Translate(0,speed/5f * Time.deltaTime,0);
					//forward
	    		transform.Translate(0,0,-speed * Time.deltaTime);*/
				Quaternion targetRotation = Quaternion.LookRotation(transform.position - currentDir);
				Quaternion inverseTargetRotation = new Quaternion(-targetRotation.x, -targetRotation.y, -targetRotation.z, -targetRotation.w);
				if(transform.rotation != targetRotation || transform.rotation != inverseTargetRotation)
				{
					transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 1);
			
				}
				transform.position = Vector3.MoveTowards(transform.position, currentDir, Time.deltaTime * speed);
				//transform.Translate(0,0,-speed * Time.deltaTime);
			}
			else
			{
				transform.Translate(0,0,-speed * Time.deltaTime);
				timer += Time.deltaTime;
				if(timer > 1)
				{
					transform.LookAt(lookAtTarget.transform);
				}
			}
		}
		else
		{
			
		}
		if(destroyObjectAfterBool)
		{
			destroyObjectAfter += Time.deltaTime;
			if(destroyObjectAfter > 1.2f)
			{
				destroyObjectAfter = 0;
				destroyObjectAfterBool = false;
				Destroy(transform.gameObject);
			}
		}
	}
	
	void OnTriggerEnter (Collider col)
	{
		if(col.tag == "Terrain" || col.tag == "Unit" || col.tag == "building")
		{
			SendDamage();
		}
	}
	
	public void Fire(GameObject owner, GameObject target, float bSpeed)
	{
		followedTarget = target;
		lookAtTarget = target;
		sender = owner;
		speed = bSpeed;
		go = true;
	}
	
	public void SendDamage()
	{
		//Damage it if it unit/build
		if(lookAtTarget != null)
		{
			//if(Vector3.Distance(transform.position, lookAtTarget.transform.position) < 0.1f)
			//{
				//Send Damage to Unit
				if(lookAtTarget.GetComponent<UnitInfo>() != null)
				{
					lookAtTarget.GetComponent<UnitInfo>().ReceiveDamage(damagePower,sender.GetComponent<UnitInfo>().attackType,sender);
				}
				//Send Damage to Build
				else if(lookAtTarget.GetComponent<BuildingInfo>() != null)
				{
					lookAtTarget.GetComponent<BuildingInfo>().ReceiveDamage(damagePower,sender.GetComponent<UnitInfo>().attackType,sender);
				}
				if(sparkEffect != null)
				{
					GameObject spark = (GameObject)Instantiate(sparkEffect,lookAtTarget.transform.position, Quaternion.identity);
				}
			//}
		}
		//Get other Object which are in damage zone
		Collider[] hitColliders = Physics.OverlapSphere(transform.position, damageRadius);
		foreach(Collider receiver in hitColliders)
		{
			if(receiver.transform.gameObject != sender)
			{
				if(receiver.transform.gameObject.GetComponent<UnitInfo>() != null && lookAtTarget != receiver.transform.gameObject)
				{
					receivedDamage.Add(receiver.transform.gameObject);
				}
				else if(receiver.transform.gameObject.GetComponent<BuildingInfo>() != null && lookAtTarget != receiver.transform.gameObject)
				{
					receivedDamage.Add(receiver.transform.gameObject);
				}
			}
		}
		
		//Remove already damaged unit
		if(receivedDamage != null)
		{
			if(receivedDamage.Count > 0)
			{
				if(lookAtTarget != null)
				{
					if(receivedDamage.Contains(lookAtTarget))
					{
						receivedDamage.Remove(lookAtTarget);
					}
				}
			}
			//Send damage to other object/creating DamageZone
			foreach(GameObject getts in receivedDamage)
			{
				
			}
		}
		DestroyAfterCollision();
	}
	
	public void DestroyAfterCollision()
	{
		destroyObjectAfterBool = true;
		speed = 0;
		if(afterEffect != null)
		{
			GameObject blow = (GameObject)Instantiate(afterEffect,transform.position, Quaternion.identity);
		}
	}
}
