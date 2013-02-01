using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
public class AgentCamera : WorldViewObject 
{
	MeshFilter m_filter = null;
	Camera m_camera = null;
	
	public LayerMask collisionLayer = 0;
	
	private List<Collider> m_collidersInView = new List<Collider>();
	
	// Use this for initialization
	void Start () 
	{
		m_camera = m_worldObject as Camera;
		m_filter = GetComponent<MeshFilter>();
		
		RebuildMesh();
		
		MeshCollider collider = GetComponent<MeshCollider>();
		collider.sharedMesh = null;
		collider.sharedMesh = m_filter.mesh;
	}
	
	public void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.tag == "Player")
		{
			m_collidersInView.Add(other);
		}
	}
	
	void OnTriggerExit(Collider other)
	{
		if(other.gameObject.tag == "Player")
		{
			m_collidersInView.Remove(other);
		}
	}
	
	void Update()
	{
		RebuildMesh();
	}
	
	void FixedUpdate()
	{
		// If we can see any verts, that's a spot
		foreach(Collider collider in m_collidersInView)
		{
			
				Vector3 direction = collider.transform.position - transform.position;
				
				
				
				if(!Physics.CapsuleCast(transform.position, collider.transform.position, collider.bounds.size.x / 8.0f ,direction))
				{
					Debug.Log("Spotted!");
				}
			
		}
	}
	
	private void RebuildMesh()
	{
		Mesh newMesh = new Mesh();
		
		Vector3[] 	vertices 	= new Vector3[5];
		Vector2[] 	uvs 		= new Vector2[5];
		int[] 		triangles 	= new int[6 * 3];
		
		float halfWidth = m_camera.range * Mathf.Tan((m_camera.fov_degrees / 2.0f) * Mathf.Deg2Rad);
		
		vertices[0] = new Vector3(0.0f, 0.0f, 0.0f);
		vertices[1] = new Vector3(-halfWidth, m_camera.range, 0.5f);
		vertices[2] = new Vector3(halfWidth, m_camera.range, 0.5f);
		vertices[3] = new Vector3(-halfWidth, m_camera.range, -0.5f);
		vertices[4] = new Vector3(halfWidth, m_camera.range, -0.5f);
		
		triangles[0] = 0;
		triangles[1] = 1;
		triangles[2] = 2;
		
		triangles[3] = 0;
		triangles[4] = 4;
		triangles[5] = 3;
		
		triangles[6] = 0;
		triangles[7] = 3;
		triangles[8] = 1;
		
		triangles[9] = 0;
		triangles[10] = 2;
		triangles[11] = 4;
		
		triangles[12] = 2;
		triangles[13] = 1;
		triangles[14] = 4;
		
		triangles[15] = 1;
		triangles[16] = 3;
		triangles[17] = 4;
		
		newMesh.vertices 	= vertices;
		newMesh.uv 			= uvs;
		newMesh.triangles 	= triangles;
		
		m_filter.mesh = newMesh;
	}
}
