using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent (typeof (MeshFilter))]
public class GeometryFactory : MonoBehaviour 
{
	
	public enum GeometryType
	{
		Plane,	
		Triangle
	}
	
	public GeometryType geometry_type = GeometryType.Plane;
		
	
	// Use this for initialization
	void Start () 
	{
		MeshFilter meshFilterComponent = GetComponent<MeshFilter>();
		
		Mesh newMesh = null;
		
		switch(geometry_type)
		{
			case GeometryType.Plane: newMesh = CreatePlane(); break;
		}
		
		meshFilterComponent.mesh = newMesh;
	}
	
	public static Mesh CreatePlane()
	{
		Mesh newMesh = new Mesh();
		
		newMesh.name = "GeometryFactory:Plane";
		
		Vector3[] 	vertices 	= new Vector3[4];
		Vector2[] 	uvs 		= new Vector2[4];
		int[] 		triangles 	= new int[6];
		
		vertices[0] = new Vector3(-0.5f, -0.5f, 0.0f);
		vertices[1] = new Vector3(0.5f, -0.5f, 0.0f);
		vertices[2] = new Vector3(-0.5f, 0.5f, 0.0f);
		vertices[3] = new Vector3(0.5f, 0.5f, 0.0f);
		
		uvs[0] = new Vector2(0.0f, 0.0f);
		uvs[1] = new Vector2(1.0f, 0.0f);
		uvs[2] = new Vector2(0.0f, 1.0f);
		uvs[3] = new Vector2(1.0f, 1.0f);
		
		triangles[0] = 0;
		triangles[1] = 2;
		triangles[2] = 1;
		triangles[3] = 1;
		triangles[4] = 2;
		triangles[5] = 3;
		
		newMesh.vertices = vertices;
		newMesh.uv = uvs;
		newMesh.triangles = triangles;
		
		return newMesh;
	}
}
