using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Create dungeon from 2D grid : int[,] map, 0=walkable, 1=blocked

// needs lots of cleanup.. c# version of the old perlin dungeon generator js : http://unitycoder.com/blog/2013/01/24/perlin-cave-dungeon-maker/

public class PerlinDungeonGenerator : MonoBehaviour 
{
	public Transform floorholder;
	public Transform roofholder;
	public Transform wallholder;
	
	public int mapsize = 64;
	public int wallHeight = 2;
	
	private int[,] map; // map grid
	
	public float noiseScale = 0.321f;
	
	// floor
	private List<Vector3> vertices = new List<Vector3>();
	private List<Vector4> tangents = new List<Vector4>();
	private List<Vector2> uvs = new List<Vector2>();
	private List<int> triangles = new List<int>();
	private int vertexIndex;
	
	// roof
	private List<Vector3> vertices2 = new List<Vector3>();
	private List<Vector4> tangents2 = new List<Vector4>();
	private List<Vector2> uvs2 = new List<Vector2>();
	private List<int> triangles2 = new List<int>();
	private int vertexIndex2;

	// walls
	private List<Vector3> vertices3 = new List<Vector3>();
//	private List<Vector4> tangents3 = new List<Vector4>();
	private List<Vector2> uvs3 = new List<Vector2>();
	private List<int> triangles3 = new List<int>();
	private int vertexIndex3;

	public Transform drawplane;

	int Walkable = 0;
	int Blocked = 1;


	void Start () {
		map = new int[mapsize,mapsize];
		createMap();
	}


	void createMap () 
	{
		
		var texture = new Texture2D (mapsize, mapsize);
		drawplane.GetComponent<Renderer>().material.mainTexture = texture;
		drawplane.GetComponent<Renderer>().material.mainTexture.filterMode = FilterMode.Point;

		for(int y=0;y<mapsize;y++)
		{
			for(int x=0;x<mapsize;x++)
			{
				// generate noise to create map
				var v = Mathf.PerlinNoise(x*noiseScale,y*noiseScale);

				// check from threshold, if are is walkable or blocked
				if (v>0.55f)
				{
					map[x,y] = Blocked;
				}else{
					map[x,y] = Walkable;
				}
				
				// block borders (for now)
				if (x==0||y==0||x==mapsize-1||y==mapsize-1) map[x,y] = Blocked;
				
				texture.SetPixel (x, y, new Color(map[x,y],map[x,y],map[x,y],1));

			}	
		}
		
		//texture.Apply(false);
		texture.Apply();
		
		// generate walls
		
		var sx=0; //start x
		var sy=0; //start y
		
		var started = false;
		var cell=0;

		// check, wheres floor
		for(int y=0;y<mapsize;y++)
		{
			for(int x=0;x<mapsize;x++)
			{
				cell = map[x,y];

				if (!started)
				{
					if (cell==Walkable)
					{
						sx=x;
						sy=y;
						started = true;
					}
					
				}else{ // we had started already
					
					if (cell==Blocked)
					{
						started = false;
						addFloorMesh(sx,sy,x-1,y); //-1 fix
						sx=0;sy=0;
					}
				}
			}	
		}
		
		started = false;
		var p1=0; //up
		var p2=0; // right
		var p3=0; // bottom
		var p4=0; // left
		
		// check, wheres floor
		for(int y=1;y<mapsize-1;y++)
		{
			for(int x=1;x<mapsize-1;x++)
			{
				cell = map[x,y];
				
				p1 = map[x,y+1];
				p2 = map[x+1,y];
				p3 = map[x,y-1];
				p4 = map[x-1,y];

				if (cell==0 && p1==1) // we hit empty area, with up wall
				{
					addTopWallMesh(x,y);
				}
				
				if (cell==0 && p2==1) // we hit empty area, with right wall
				{
					addRightWallMesh(x,y);
				}
				
				if (cell==0 && p3==1) // we hit empty area, with down wall
				{
					addBottomWallMesh(x,y);
				}
				
				if (cell==0 && p4==1) // we hit empty area, with left wall
				{
					addLeftWallMesh(x,y);
				}
			}	
		}
		
		
		// Build the floor Mesh:
		var mesh = floorholder.GetComponent<MeshFilter>().mesh;
		mesh.vertices = vertices.ToArray();
		mesh.triangles = triangles.ToArray();
		mesh.uv = uvs.ToArray();
		mesh.RecalculateNormals();
		mesh.tangents = tangents.ToArray();
		;
		floorholder.GetComponent<MeshCollider>().sharedMesh = null;
		floorholder.GetComponent<MeshCollider>().sharedMesh = mesh;
		
		var mesh2  = roofholder.GetComponent<MeshFilter>().mesh;
		mesh2.vertices = vertices2.ToArray();
		mesh2.triangles = triangles2.ToArray();
		mesh2.uv = uvs2.ToArray();
		mesh2.RecalculateNormals();
		mesh2.tangents = tangents.ToArray();
		;
		roofholder.GetComponent<MeshCollider>().sharedMesh = null;
		roofholder.GetComponent<MeshCollider>().sharedMesh = mesh2;
		
		var mesh3 = wallholder.GetComponent<MeshFilter>().mesh;
		mesh3.vertices = vertices3.ToArray();
		mesh3.triangles = triangles3.ToArray();
		mesh3.uv = uvs3.ToArray();
		mesh3.RecalculateNormals();
		;
		wallholder.GetComponent<MeshCollider>().sharedMesh = null;
		wallholder.GetComponent<MeshCollider>().sharedMesh = mesh3;
		
		
		
		
	}	

	void addFloorMesh(float x1,float y1,float x2,float y2)
	{
		// TODO: 1 quad or 1 triangle?
		
		addRoofMesh(x1,y1,x2,y2);
		
		
		// floor
		
		x1-=0.5f;
		y1-=0.5f;
		x2+=0.5f;
		y2+=0.5f;

		// 1 face = 2 tris
		vertices.Add(new Vector3(x1, 0, y1));
		vertices.Add(new Vector3(x2, 0, y1));
		vertices.Add(new Vector3(x1, 0, y2));
		vertices.Add(new Vector3(x2, 0, y2));
		// first triangle for the top
		triangles.Add(vertexIndex+2);
		triangles.Add(vertexIndex+1);
		triangles.Add(vertexIndex);
		// second triangle for the bottom
		triangles.Add(vertexIndex+3);
		triangles.Add(vertexIndex+1);
		triangles.Add(vertexIndex+2);
		// add UV
		
		// calc tiling
		var tiling = Mathf.Abs(x1-x2);
		
		uvs.Add(new Vector2 (0, 1));
		uvs.Add(new Vector2 (1*tiling, 1));
		uvs.Add(new Vector2 (0, 0));
		uvs.Add(new Vector2 (1*tiling, 0));
		
		// tangents
		tangents.Add(new Vector4(1,0,0,1));
		tangents.Add(new Vector4(1,0,0,1));
		tangents.Add(new Vector4(1,0,0,1));
		tangents.Add(new Vector4(1,0,0,1));

		vertexIndex+=4;
		
		
	}

	void addRoofMesh(float x1,float y1,float x2, float y2)
	{
		
		x1-=0.5f;
		y1-=0.5f;
		x2+=0.5f;
		y2+=0.5f;
		
		// add roof (same coords, y+height, inverted UV)
		vertices2.Add(new Vector3(x1, wallHeight, y1));
		vertices2.Add(new Vector3(x2, wallHeight, y1));
		vertices2.Add(new Vector3(x1, wallHeight, y2));
		vertices2.Add(new Vector3(x2, wallHeight, y2));
		// first triangle for the top
		triangles2.Add(vertexIndex2);
		triangles2.Add(vertexIndex2+1);
		triangles2.Add(vertexIndex2+2);
		// second triangle for the bottom
		triangles2.Add(vertexIndex2+2);
		triangles2.Add(vertexIndex2+1);
		triangles2.Add(vertexIndex2+3);
		// add UV
		
		// calc tiling
		var tiling = Mathf.Abs(x1-x2);

		uvs2.Add(new Vector2 (0, 0));
		uvs2.Add(new Vector2 (1*tiling, 0));
		uvs2.Add(new Vector2 (0, 1));
		uvs2.Add(new Vector2 (1*tiling, 1));
		
		tangents2.Add(new Vector4(1,0,0,-1));
		tangents2.Add(new Vector4(1,0,0,-1));
		tangents2.Add(new Vector4(1,0,0,-1));
		tangents2.Add(new Vector4(1,0,0,-1));
		
		
		vertexIndex2+=4;
	}


	void addTopWallMesh(float x1,float y1)
	{
		x1-=0.5f;
		y1+=0.5f;
		var x2=x1+1;
		var y2=y1;
		
		// 1 face = 2 tris
		vertices3.Add(new Vector3(x1, 0, y1));
		vertices3.Add(new Vector3(x2, 0, y1));
		vertices3.Add(new Vector3(x1, wallHeight, y2));
		vertices3.Add(new Vector3(x2, wallHeight, y2));
		// first triangle for the top
		triangles3.Add(vertexIndex3+2);
		triangles3.Add(vertexIndex3+1);
		triangles3.Add(vertexIndex3);
		// second triangle for the bottom
		triangles3.Add(vertexIndex3+3);
		triangles3.Add(vertexIndex3+1);
		triangles3.Add(vertexIndex3+2);
		// add UV
		// calc tiling
		var tiling = Mathf.Abs(x1-x2);
		
		uvs3.Add(new Vector2 (0, 0));
		uvs3.Add(new Vector2 (1*tiling, 0));
		uvs3.Add(new Vector2 (0, 1));
		uvs3.Add(new Vector2 (1*tiling, 1));
		vertexIndex3+=4;
	}

	void addRightWallMesh(float x1,float y1)
	{
		x1+=0.5f;
		y1+=0.5f;
		var x2=x1;
		var y2=y1-1;
		
		// 1 face = 2 tris
		vertices3.Add(new Vector3(x1, 0, y1));
		vertices3.Add(new Vector3(x1, wallHeight, y1));
		vertices3.Add(new Vector3(x2, 0, y2));
		vertices3.Add(new Vector3(x2, wallHeight, y2));
		// first triangle for the top
		triangles3.Add(vertexIndex3);
		triangles3.Add(vertexIndex3+1);
		triangles3.Add(vertexIndex3+2);
		// second triangle for the bottom
		triangles3.Add(vertexIndex3+2);
		triangles3.Add(vertexIndex3+1);
		triangles3.Add(vertexIndex3+3);

		uvs3.Add(new Vector2 (0, 0));
		uvs3.Add(new Vector2 (0, 1));
		uvs3.Add(new Vector2 (1, 0));
		uvs3.Add(new Vector2 (1, 1));
		
		
		vertexIndex3+=4;
	}

	void addLeftWallMesh(float x1,float y1)
	{

		x1-=0.5f;
		y1+=0.5f;
		var x2=x1;
		var y2=y1-1;
		
		// 1 face = 2 tris
		vertices3.Add(new Vector3(x1, 0, y1));
		vertices3.Add(new Vector3(x1, wallHeight, y1));
		vertices3.Add(new Vector3(x2, 0, y2));
		vertices3.Add(new Vector3(x2, wallHeight, y2));
		// first triangle for the top
		triangles3.Add(vertexIndex3+2);
		triangles3.Add(vertexIndex3+1);
		triangles3.Add(vertexIndex3);
		// second triangle for the bottom
		triangles3.Add(vertexIndex3+3);
		triangles3.Add(vertexIndex3+1);
		triangles3.Add(vertexIndex3+2);
		// add UV
		// calc tiling (should calc height also..?)
		var tiling = Mathf.Abs(y1-y2);
		
		uvs3.Add(new Vector2 (0, 0));
		uvs3.Add(new Vector2 (0, 1));
		uvs3.Add(new Vector2 (1*tiling, 0));
		uvs3.Add(new Vector2 (1*tiling, 1));
		
		vertexIndex3+=4;
	}
	
	void addBottomWallMesh(float x1,float y1)
	{
		x1-=0.5f;
		y1-=0.5f;
		var x2=x1+1;
		var y2=y1;
		
		// 1 face = 2 tris
		vertices3.Add(new Vector3(x1, 0, y1));
		vertices3.Add(new Vector3(x2, 0, y1));
		vertices3.Add(new Vector3(x1, wallHeight, y2));
		vertices3.Add(new Vector3(x2, wallHeight, y2));
		// first triangle for the top
		triangles3.Add(vertexIndex3);
		triangles3.Add(vertexIndex3+1);
		triangles3.Add(vertexIndex3+2);
		// second triangle for the bottom
		triangles3.Add(vertexIndex3+2);
		triangles3.Add(vertexIndex3+1);
		triangles3.Add(vertexIndex3+3);
		// add UV
		// calc tiling
		var tiling = Mathf.Abs(x1-x2);
		
		uvs3.Add(new Vector2 (0, 0));
		uvs3.Add(new Vector2 (1*tiling, 0));
		uvs3.Add(new Vector2 (0, 1));
		uvs3.Add(new Vector2 (1*tiling, 1));
		
		vertexIndex3+=4;
	}


}



