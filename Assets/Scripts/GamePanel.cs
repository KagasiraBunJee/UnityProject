using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GamePanel : MonoBehaviour {

//Private Variables
	private bool InventoryOn = false;
	private Vector2 scrollBarChopGrid = Vector2.zero;
	private int GridValue = -1;
	//GUI Pos/Size
	public Vector2 ClosePosition = new Vector2(312,5);
	public Vector2 CloseSize = new Vector2(35,35);
	public Vector2 GridPosition = new Vector2(10,2);
	public Vector2 GridSize = new Vector2(323,410);
	public Vector2 ScrollPosition = new Vector2(0,95);
	public Vector2 ScrollSize = new Vector2(353,257);
	public Vector2 WindowPosition = new Vector2(0,0);
	public Vector2 WindowSize = new Vector2(360,360);
	public float plusX = 0;
	//Textures
	public Texture2D InventoryWindow;
	public Texture2D CloseIcon;
	public Texture2D[] Grids;
	public List<Texture2D> img;
	public List<GameObject> imgObj;
	public BuildSelection bs;
	
	// Use this for initialization
	void Start () 
	{
		bs = GetComponent<BuildSelection>();
		

	}
	
	// Update is called once per frame
	void Update () 
	{
    	if (Input.GetKeyUp(KeyCode.I))
    	{
    	    InventoryOn = !InventoryOn;
    	}
	}
	
	void OnGUI()
	{
    	/*if (InventoryOn == true)
    	{
    	    //GUI.BeginGroup(new Rect(WindowPosition.x, WindowPosition.y, WindowSize.x, WindowSize.y), InventoryWindow);
    	    //Close Button
    	    if (GUI.Button(new Rect(ClosePosition.x, ClosePosition.y, CloseSize.x, CloseSize.y), CloseIcon))
    	    {
    	        InventoryOn = false;
    	    }
    	    //Scroll Bar
    	    //scrollBarChopGrid = GUI.BeginScrollView(new Rect (ScrollPosition.x, ScrollPosition.y, ScrollSize.x, ScrollSize.y), scrollBarChopGrid, new Rect(0,0,0,420));
    	        // Grid
    	        /*GridValue = GUI.SelectionGrid(new Rect(GridPosition.x, GridPosition.y, GridSize.x, GridSize.y), GridValue, Grids, 5);
			    if(GridValue == 0)
				{
					print ("food");
				}
			    
    	   // GUI.EndScrollView();
    	    //GUI.EndGroup();
			List<GameObject> allb = bs.getBuildings();
			foreach(GameObject ab in allb)
			{
				Texture2D bImg = ab.transform.FindChild("info").GetComponent<BuildingInfo>().getIcon();
				if(!imgIsExist(ab))
				{
					img.Add(bImg);
					imgObj.Add(ab);
				}
			}
			Grids = img.ToArray();
			foreach(Texture2D buttons in img)
			for(int i =0; i < Grids.Length; i++)
			{
				GUI.Button(new Rect(GridPosition.x + plusX, GridPosition.y, GridSize.x, GridSize.y), Grids[i]);
				plusX += 50;
			}
    	}*/
	}
	bool imgIsExist(GameObject im)
	{
		if(!imgObj.Contains(im))
		{
			return false;
		}
		return true;
	}
}
	