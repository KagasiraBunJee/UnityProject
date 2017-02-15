using UnityEngine;
using System.Collections;

/* 
Class to control the camera within the game world.
Camera will move up, down, left and right when the users mouse hits the side of the screen in 2D space.
Camera will check desired location, will stop if over limits.
Camera can also be controlled by W,A,S,D keys, will call the same movement as the mouse events.
*/


public class WorldCamera : MonoBehaviour {
	
	#region structs
	
	//box limits Struct
	public struct BoxLimit {
		public float LeftLimit;
		public float RightLimit;
		public float TopLimit;
		public float BottomLimit;
		
	}
	//Настройка камеры
	public GameObject go;
    public float   sensitivity = 3f;
    private WorldCamera  goCamera;
    private Vector3  MousePos;
    private float   MyAngle = 0F;
	//Скролл камеры
	public float cameraScrollSense = 10.0f;
	public float maxHeightCameraScroll;
	public float minHeightCameraScroll;
	public float smooth_move;
	//Позиционирование и параметры области выбора
	public Texture2D border_select;
	private bool start_select = false;
	private float start_x;
	private float start_y;
	private bool started_selection = false;
	private int _depth = 1;
	
	
	#endregion
	
	
	#region class variables
	
	public static BoxLimit cameraLimits       = new BoxLimit();
	public static BoxLimit mouseScrollLimits  = new BoxLimit();
	public static WorldCamera Instance;

	private float cameraMoveSpeed = 60f;
	private float shiftBonus      = 45f;
	private float mouseBoundary   = 25f;
	public bool mouseDrag = false;
	
	#endregion
	
	
	void Awake()
	{
		Instance = this;
	}
	

	void Start () {
		
		//Declare camera limits
		cameraLimits.LeftLimit   = 10.0f;
		cameraLimits.RightLimit  = 240.0f;
		cameraLimits.TopLimit    = 204.0f;
		cameraLimits.BottomLimit = -20.0f;
		
		//Declare Mouse Scroll Limits
		mouseScrollLimits.LeftLimit   = mouseBoundary;
		mouseScrollLimits.RightLimit  = mouseBoundary;
		mouseScrollLimits.TopLimit    = mouseBoundary;
		mouseScrollLimits.BottomLimit = mouseBoundary;
		
		goCamera = this;
        go    = this.transform.FindChild("Main Camera").gameObject;
			
	}
	
	
	
	
	void Update () 
	{
	   //Скролим камеру вверх или вниз
       if (Input.GetAxis("Mouse ScrollWheel") < 0) // back
       {
       		//Camera.main.orthographicSize = Mathf.Max(Camera.main.orthographicSize-1, 1);
			if(goCamera.transform.position.y > minHeightCameraScroll )
			{
				goCamera.transform.Translate(new Vector3(0,-(cameraScrollSense),0));
			}
       }
       if (Input.GetAxis("Mouse ScrollWheel") > 0) // forward
       {
			//Camera.main.orthographicSize = Mathf.Min(Camera.main.orthographicSize-1, 6);
			if(goCamera.transform.position.y < maxHeightCameraScroll )
			{
				goCamera.transform.Translate(new Vector3(0,(cameraScrollSense),0));
			}
       }
		checkCameraHeightPosition(goCamera.transform.gameObject);
		//Старый скрипт перемещения возле границ экрана				
		/*if(CheckIfUserCameraInput())
		{
			Vector3 cameraDesiredMove = GetDesiredTranslation();
			
			if(!isDesiredPositionOverBoundaries(cameraDesiredMove))
			{
				
			}
			this.transform.Translate(cameraDesiredMove);
		}*/
		
		
		MousePos = Input.mousePosition;
		if(Input.GetMouseButtonDown(0))
		{
			started_selection = !started_selection;
			
		}
		if(Input.GetMouseButtonUp(0))
		{
			start_select = !start_select;
			started_selection = !started_selection;
		}
		if(Input.GetMouseButtonUp(1))
		{
			mouseDrag = false;
		}
		if(Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
		{
			if(Input.GetMouseButton(1))
			  mouseDrag = true;
		}
	}
	
	
	
	
	//Check if the user is inputting commands for the camera to move
	public bool CheckIfUserCameraInput()
	{
		bool keyboardMove;
		bool mouseMove;
		bool canMove;
		
		//check keyboard		
		if(WorldCamera.AreCameraKeyboardButtonsPressed()){
			keyboardMove = true;			
		} else {
			keyboardMove = false;
		}
		
		//check mouse position
		if(WorldCamera.IsMousePositionWithinBoundaries())
			mouseMove = true; else mouseMove = false;
		
		
		if(keyboardMove || mouseMove)
			canMove = true; else canMove = false;
		
		return canMove;
	}
	
	void LateUpdate()
	{

	}
	
    void FixedUpdate () 
	{
    	if(Input.GetMouseButton(1))
		{
    		MyAngle = 0;
    		  // расчитываем угол, как:
    		  // разница между позицией мышки и центром экрана, делённая на размер экрана
    		  //  (чем дальше от центра экрана тем сильнее поворот)
    		  // и умножаем угол на чуствительность из параметров
    		/*MyAngle = sensitivity*((MousePos.x-(Screen.width/2))/Screen.width);
    		goCamera.transform.RotateAround(go.transform.position, goCamera.transform.up, MyAngle);
    		MyAngle = sensitivity*((MousePos.y-(Screen.height/2))/Screen.height);
    		goCamera.transform.RotateAround(go.transform.position, goCamera.transform.right, -MyAngle);*/
			
			if(mouseDrag)
			{
				Terrain terra = Terrain.activeTerrain;
				Vector3 terraSize = terra.terrainData.size;
				Vector3 curr_camera_pos = goCamera.transform.position;
				float x1 = sensitivity*(MousePos.x-(Screen.width/2))/Screen.width;
				float z1 = sensitivity*(MousePos.y-(Screen.height/2))/Screen.height;
				
				//Проверка позиции по оси Х, если выходит за пределы дозволеного, останавливаем
				if((curr_camera_pos.x + x1+10) > terraSize.x || (curr_camera_pos.x + x1-10) < 0)
				{
					x1 = 0;
				}
				//Проверка позиции по оси Y, если выходит за пределы дозволеного, останавливаем
				else if((curr_camera_pos.z + z1+20) > terraSize.z || (curr_camera_pos.z + z1) < -20)
				{
					z1 = 0;
				}
				//Проверка если камера всё таки вышла за пределы, вернуть в рабочую позицию
				
				if(curr_camera_pos.z < -20 && curr_camera_pos.x < 100)
				{
					goCamera.transform.position = Vector3.Lerp(curr_camera_pos, new Vector3(10,curr_camera_pos.y,0), Time.deltaTime * smooth_move);
					
				}
				else if(curr_camera_pos.z < -20 && curr_camera_pos.x > 100)
				{
					goCamera.transform.position = Vector3.Lerp(curr_camera_pos, new Vector3(terraSize.x-10,curr_camera_pos.y,0), Time.deltaTime * smooth_move);
				}
				else if((curr_camera_pos.z+21) > terraSize.z && curr_camera_pos.x > 100)
				{
					goCamera.transform.position = Vector3.Lerp(curr_camera_pos, new Vector3(terraSize.x-10,curr_camera_pos.y,terraSize.z-22), Time.deltaTime * smooth_move);
					
				}
				else if((curr_camera_pos.z+21) > terraSize.z && curr_camera_pos.x < 100)
				{
					goCamera.transform.position = Vector3.Lerp(curr_camera_pos, new Vector3(10,curr_camera_pos.y,terraSize.z-22), Time.deltaTime * smooth_move);
				}
				//Конец проверки, если всё хорошо, просто перемещаем
				else
					goCamera.transform.Translate(new Vector3(x1, 0, z1));
				
				
			}
			
    	}
    }
	
	void OnGUI()
	{
		if(started_selection)
		{
			if(!start_select)
			{
				start_x = Event.current.mousePosition.x;
				start_y = Event.current.mousePosition.y;
				start_select = true;
			}
			//Рисуем на экране фигуру выбора юнитов
			GUI.DrawTexture(new Rect(start_x, start_y, Event.current.mousePosition.x-start_x,_depth), border_select);
			GUI.DrawTexture(new Rect(start_x, start_y, _depth,Event.current.mousePosition.y-start_y), border_select);
			GUI.DrawTexture(new Rect(start_x, start_y, _depth,_depth), border_select);
			
			GUI.DrawTexture(new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, -(Event.current.mousePosition.x-start_x),_depth), border_select);
			GUI.DrawTexture(new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, _depth,-(Event.current.mousePosition.y-start_y)), border_select);
			GUI.DrawTexture(new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, _depth,_depth), border_select);
		}
	}
	
	
	//Проверка высоты камеры
	public void checkCameraHeightPosition(GameObject cam)
	{
		if(cam.transform.position.y >= maxHeightCameraScroll)
		{
			cam.transform.position = Vector3.Lerp(cam.transform.position, new Vector3(cam.transform.position.x,maxHeightCameraScroll,cam.transform.position.z), Time.deltaTime * smooth_move);
		}
		else if(cam.transform.position.y <= minHeightCameraScroll)
		{
			cam.transform.position = Vector3.Lerp(cam.transform.position, new Vector3(cam.transform.position.x,minHeightCameraScroll,cam.transform.position.z), Time.deltaTime * smooth_move);
		}
	}
	
	//перемещение камеры когда курсор находится возле границы экрана
	public Vector3 GetDesiredTranslation()
	{
		float moveSpeed = 0f;
		float desiredX = 0f;
		float desiredZ = 0f;
		
		if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
			moveSpeed = (cameraMoveSpeed + shiftBonus) * Time.deltaTime;
		else
			moveSpeed = cameraMoveSpeed * Time.deltaTime;
		
		
		//move via keyboard
		if(Input.GetKey(KeyCode.W))
			desiredZ = moveSpeed;
		
				
		if(Input.GetKey(KeyCode.S))
			desiredZ = moveSpeed * -1;
		
		
		if(Input.GetKey(KeyCode.A))
			desiredX = moveSpeed * -1;
		
		
		if(Input.GetKey(KeyCode.D))
			desiredX = moveSpeed;

		//move via mouse
		if(Input.mousePosition.x < mouseScrollLimits.LeftLimit){
			desiredX = moveSpeed * -1;
		}
		
		if(Input.mousePosition.x > (Screen.width - mouseScrollLimits.RightLimit)){
			desiredX = moveSpeed;
		}
		
		if(Input.mousePosition.y < mouseScrollLimits.BottomLimit){
			desiredZ = moveSpeed * -1;
		}
		
		if(Input.mousePosition.y > (Screen.height - mouseScrollLimits.TopLimit)){
			desiredZ = moveSpeed;
		}
		//Debug.Log(desiredX+"x0x"+desiredZ);
		return new Vector3(desiredX, 0, desiredZ);
	}
	
	
	
	
	
	//checks if the desired position crosses boundaries
	public bool isDesiredPositionOverBoundaries(Vector3 desiredPosition)
	{
		bool overBoundaries = false;
		//check boundaries
		if((this.transform.position.x + desiredPosition.x) < cameraLimits.LeftLimit)
			overBoundaries = true;
		
		if((this.transform.position.x + desiredPosition.x) > cameraLimits.RightLimit)
			overBoundaries = true;
			
		if((this.transform.position.z + desiredPosition.z) > cameraLimits.TopLimit)
			overBoundaries = true;
			
		if((this.transform.position.z + desiredPosition.z) < cameraLimits.BottomLimit)
			overBoundaries = true;
		
		return overBoundaries;
	}
	
	
	
	
	
	#region Helper functions
		
	public static bool AreCameraKeyboardButtonsPressed()
	{
		if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
			return true; else return false;
	}
	
	public static bool IsMousePositionWithinBoundaries()
	{
		if(
			(Input.mousePosition.x < mouseScrollLimits.LeftLimit && Input.mousePosition.x > -5) ||
			(Input.mousePosition.x > (Screen.width - mouseScrollLimits.RightLimit) && Input.mousePosition.x < (Screen.width + 5)) ||
			(Input.mousePosition.y < mouseScrollLimits.BottomLimit && Input.mousePosition.y > -5) ||
			(Input.mousePosition.y > (Screen.height - mouseScrollLimits.TopLimit) && Input.mousePosition.y < (Screen.height + 5))
			)
				return true; else return false;
	}
	
	
	#endregion
}
