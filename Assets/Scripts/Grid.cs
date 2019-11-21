using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum Direction
{
	Right,
	Left,
	Top,
	Bottom,
	BottomLeft,
	BottomRight,
	TopLeft,
	TopRight,  
}

public class Grid : MonoBehaviour {
		
	Vector2 Offset;
	
	public int Width;
	public int Height;
	
	public Node[,] Nodes;
	
	public int Left { get { return 0; } }
	public int Right { get { return Width; } }
	public int Bottom { get { return 0; } }
	public int Top { get { return Height; } }

	public const float UnitSize =10f;

	private LineRenderer LineRenderer;
	GameObject Player;
	GameObject monster;
	void Awake () 
	{	
		Player = GameObject.Find ("Player");
		monster = GameObject.FindGameObjectWithTag ("Enemy");
		LineRenderer = transform.GetComponent<LineRenderer>();

		//Get grid dimensions
		//Top left of grid object
		Offset = (Vector2)transform.position + new Vector2(transform.localScale.x *-1/2 ,transform.localScale.y/2 ) + Vector2.down*(int)UnitSize/2 ;
		Debug.Log (Offset);
		Width = ((int)transform.localScale.x) * 2 + (int)UnitSize;
		Height = ((int)transform.localScale.y) * 2 + (int)UnitSize;
		
		Nodes = new Node[Width, Height];

		//Initialize the grid nodes - UnitSize grid unit between each node
		//We render the grid in a diamond pattern
		for (int x = 0; x < Width/2; x+=(int)UnitSize)
		{
			for (int y = 0; y < Height; y+=(int)UnitSize)
			{
				float ptx = x;
				float pty = -(y/2) + (UnitSize/2f);
				int offsetx = 0;

				if (y % ((int)UnitSize*2) == 0)
				{
					ptx = x + (UnitSize/2f);
					offsetx = 1;
				}	
				else
				{
					if ((int)UnitSize % 2 == 1) {
						pty = -(y / 2);
					}
				}
							
				Vector2 pos = new Vector2(ptx, pty) + Offset;
				Node node = new Node((x*2/(int)UnitSize) + offsetx, (y/(int)UnitSize), pos, this);
				Nodes[(x*2/(int)UnitSize), (y/(int)UnitSize)] = node;
			}
		}
		
		//Create connections between each node
		for (int x = 0; x < (Width/((int)UnitSize))+1; x++)
		{
			for (int y = 0; y < (Height/((int)UnitSize))+1; y++)
			{
				
				if (Nodes[x,y] == null) continue;				
				Nodes[x, y].InitializeConnections(this);
			}
		}		

		//Pass 1, we removed the bad nodes, based on valid connections
		for (int x = 0; x < Width; x++)
		{
			for (int y = 0; y < Height; y++)
			{
				if (Nodes[x,y] == null) 
					continue;				

				Nodes[x, y].CheckConnectionsPass1 (this);
			}
		}		

		//Pass 2, remove bad connections based on bad nodes
		for (int x = 0; x < Width; x++)
		{
			for (int y = 0; y < Height; y++)
			{
				if (Nodes[x,y] == null) 
					continue;				

				Nodes[x, y].CheckConnectionsPass2 ();
				//Nodes[x, y].DrawConnections ();	//debug
			}
		}		
	}		
	

	public Point WorldToGrid(Vector2 worldPosition)
	{
		Vector2 gridPosition = worldPosition - Offset -  Vector2.down*(int)UnitSize/2 ;
		gridPosition.y = -gridPosition.y;
		Debug.Log (gridPosition);
		//adjust to our nearest co-ordinate
		float rx = gridPosition.x % (int)UnitSize;
		if (rx < (float)UnitSize/2)
			gridPosition.x = gridPosition.x - rx;
		else
			gridPosition.x = gridPosition.x + (1 - rx);
		
		float ry = gridPosition.y % (int)UnitSize;
		if (ry < (float)UnitSize/2)
			gridPosition.y = gridPosition.y - ry;
		else
			gridPosition.y = gridPosition.y + (1 - ry);


		int x = (int)gridPosition.x/5;
		int y = (int)gridPosition.y/5;
		Debug.Log (new Vector2(x,y));
		if (x < 0 || y < 0 || x > Width || y > Height)
			return null;

		Node node = Nodes [x, y];
		//We calculated a spot between nodes'
		//Find nearest neighbor
		if((node == null) ||  (x % 2 == 0 && y % 2 == 0) || (gridPosition.y % 2 == 1 && gridPosition.x % 2 == 1))
		{   
			float mag = 100;


			if (x < Width && !Nodes[x + 1, y].BadNode)
			{
				float mag1 = (Nodes[x + 1, y].Position - worldPosition).magnitude;
				if (mag1 < mag)
				{
					mag = mag1;
					node = Nodes[x + 1, y];
				}
			}
			if (y < Height - 1 && !Nodes[x, y + 1].BadNode)
			{
				float mag1 = (Nodes[x, y+ 1].Position - worldPosition).magnitude;
				if (mag1 < mag)
				{
					mag = mag1;
					node = Nodes[x, y + 1];
				}
			}
			if (x > 0 && !Nodes[x- 1, y].BadNode)
			{
				float mag1 = (Nodes[x - 1, y].Position - worldPosition).magnitude;
				if (mag1 < mag)
				{
					mag = mag1;
					node = Nodes[x-1, y];
				}
			}
			if (y > 0 && !Nodes[x, y - 1].BadNode)
			{
				float mag1 = (Nodes[x, y - 1].Position - worldPosition).magnitude;
				if (mag1 < mag)
				{
					mag = mag1;
					node = Nodes[x, y -1+ 1];
				}
			}
		}
		return new Point(node.X , node.Y);
	}

	public static Vector2 GridToWorld(Point gridPosition)
	{
		Vector2 world = new Vector2(gridPosition.X / 2f, -(gridPosition.Y / 2f - 0.5f));

		return world;
	}
	
	public bool ConnectionIsValid(Point point1, Point point2)
	{
		//comparing same point, return false
		if (point1.X == point2.X && point1.Y == point2.Y)
			return false;
		
		if (Nodes [point1.X, point1.Y] == null)
			return false;
		
		//determine direction from point1 to point2
		Direction direction = Direction.Bottom;

		if (point1.X == point2.X)
		{
			if (point1.Y < point2.Y)
				direction = Direction.Bottom;
			else if (point1.Y > point2.Y)
				direction = Direction.Top;
		}
		else if (point1.Y == point2.Y)
		{
			if (point1.X < point2.X)
				direction = Direction.Right;
			else if (point1.X > point2.X)
				direction = Direction.Left;
		}
		else if (point1.X < point2.X)
		{
			if (point1.Y > point2.Y)
				direction = Direction.TopRight;
			else if (point1.Y < point2.Y)
				direction = Direction.BottomRight;
		}
		else if (point1.X > point2.X)
		{
			if (point1.Y > point2.Y)
				direction = Direction.TopLeft;
			else if (point1.Y < point2.Y)
				direction = Direction.BottomLeft;
		}

		//check connection
		switch (direction)
		{
			case Direction.Bottom:
			if (Nodes[point1.X, point1.Y].Bottom != null)
				return Nodes[point1.X, point1.Y].Bottom.Valid;
			else
				return false;

			case Direction.Top:
			if (Nodes[point1.X, point1.Y].Top != null)
				return Nodes[point1.X, point1.Y].Top.Valid;
			else
				return false;
		
			case Direction.Right:
			if (Nodes[point1.X, point1.Y].Right != null)
				return Nodes[point1.X, point1.Y].Right.Valid;
			else
				return false;

			case Direction.Left:
			if (Nodes[point1.X, point1.Y].Left != null)
				return Nodes[point1.X, point1.Y].Left.Valid;
			else
				return false;
		
			case Direction.BottomLeft:
			if (Nodes[point1.X, point1.Y].BottomLeft != null)
				return Nodes[point1.X, point1.Y].BottomLeft.Valid;
			else
				return false;

			case Direction.BottomRight:
			if (Nodes[point1.X, point1.Y].BottomRight != null)
				return Nodes[point1.X, point1.Y].BottomRight.Valid;
			else
				return false;
		
			case Direction.TopLeft:
			if (Nodes[point1.X, point1.Y].TopLeft != null)
				return Nodes[point1.X, point1.Y].TopLeft.Valid;
			else
				return false;
		
			case Direction.TopRight:
			if (Nodes[point1.X, point1.Y].TopRight != null)
				return Nodes[point1.X, point1.Y].TopRight.Valid;
			else
				return false;
		
			default:
				return false;
		}		
	}


	void Update()
	{
		//Pathfinding demo
		if(Input.GetMouseButtonDown(0))
		{
			//Convert mouse click point to grid coordinates
			Vector2 worldPos = monster.transform.position;
			Point gridPos = WorldToGrid(worldPos);
			//Debug.Log (Input.mousePosition);
			Debug.Log (gridPos);
			if (gridPos != null) {						

				if (gridPos.X > 0 && gridPos.Y > 0 && gridPos.X < Width && gridPos.Y < Height) {

					//Convert player point to grid coordinates
					Point playerPos = WorldToGrid (Player.transform.position);					
					Nodes[playerPos.X, playerPos.Y].SetColor(Color.blue);

					//Find path from player to clicked position
					BreadCrumb bc = PathFinder.FindPath (this, playerPos, gridPos);

					int count = 0;		
					LineRenderer lr = Player.GetComponent<LineRenderer> ();
					lr.SetVertexCount(100);  //Need a higher number than 2, or crashes out
					lr.SetWidth(0.1f, 0.1f);
					lr.SetColors(Color.yellow, Color.yellow);

					//Draw out our path
					while (bc != null) {					
						lr.SetPosition(count, Grid.GridToWorld(bc.position));
						bc = bc.next;
						count += 1;
					}
					lr.SetVertexCount(count);					
				}				
			}
		}
	}

}



