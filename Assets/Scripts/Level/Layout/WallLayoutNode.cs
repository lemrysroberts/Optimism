using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class WallLayoutNode : LayoutNode 
{
	public override List<GameObject> BuildObject()
	{
		List<GameObject> newObjects = new List<GameObject>();
		
		foreach(var connection in m_connections)
		{
			if(connection.Built)
				continue; 
			
			GameObject newObject = new GameObject();
			newObject.tag = "Geometry";
			newObject.layer = LayerMask.NameToLayer("WorldCollision");
			
			
			newObject.AddComponent<MeshFilter>();
			newObject.AddComponent<MeshRenderer>();
			
			Material wallMaterial = (Material)AssetDatabase.LoadAssetAtPath("Assets/Materials/Wall.mat", typeof(Material));
			newObject.GetComponent<MeshRenderer>().sharedMaterial = wallMaterial;
			
			newObject.transform.position = new Vector3(m_position.x, m_position.y, 0.0f);
			
			LayoutNode other = connection.Source == this ? connection.Target : connection.Source;
			
			Vector2 directionToOther = other.m_position - m_position;
			float distance = directionToOther.magnitude;
			float rotation = Mathf.Atan2(directionToOther.x, directionToOther.y);
			
			newObject.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, (-rotation * Mathf.Rad2Deg));
			
			Mesh nodeMesh = new Mesh();
		
			Vector3[] 	vertices 	= new Vector3[4];
			Vector2[] 	uvs 		= new Vector2[4];
			int[] 		triangles 	= new int[6];
			
			vertices[0] = new Vector3(-0.2f, 0.0f, 0.0f);
			vertices[1] = new Vector3(-0.2f, distance, 0.0f);
			vertices[2] = new Vector3(0.2f, 0.0f, 0.0f);
			vertices[3] = new Vector3(0.2f, distance, 0.0f);
			
			uvs[0] = new Vector2(0.0f, 0.0f);
			uvs[1] = new Vector2(0.0f, distance);
			uvs[2] = new Vector2(1.0f, 0.0f);
			uvs[3] = new Vector2(1.0f, distance);
			
			
			triangles[0] = 0;
			triangles[1] = 1;
			triangles[2] = 2;
			
			triangles[3] = 2;
			triangles[4] = 1;
			triangles[5] = 3;
			
			
			nodeMesh.vertices 	= vertices;
			nodeMesh.uv 			= uvs;
			nodeMesh.triangles 	= triangles;
			
			newObject.GetComponent<MeshFilter>().mesh = nodeMesh;
			
			BuildCollision(newObject);
			
			newObjects.Add(newObject);
			
			connection.Built = true;
			
		}
		
		return newObjects;
	}
	
	private void BuildCollision(GameObject newObject)
	{
		newObject.AddComponent<Rigidbody>();
			
		Rigidbody rigidBody = newObject.GetComponent<Rigidbody>();
		rigidBody.useGravity = false;
		rigidBody.freezeRotation = true;
		
		
		rigidBody.constraints = RigidbodyConstraints.FreezeAll;
		
		newObject.AddComponent<BoxCollider>();
	}
	
	public void OnGUI()
	{
		
	}
}
