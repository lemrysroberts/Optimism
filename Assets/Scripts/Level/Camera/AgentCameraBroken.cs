using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// DON'T LOOK AT ME, I'M HIDEOUS.
/// 
/// This thing is Dr Wright's fetish material. 
/// </summary>

[RequireComponent(typeof(MeshFilter))]
public class AgentCameraBroken : WorldViewObject 
{
	MeshFilter m_filter = null;
	Camera m_camera = null;
	
	private List<Collider> m_collidersInView = new List<Collider>();
	public LayerMask collisionLayer = 0;
	
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
		if(other.tag == "Geometry")
			m_collidersInView.Add(other);
	}
	
	void OnTriggerExit(Collider other)
	{
		//Debug.Log("Colliders count: " + m_collidersInView.Count);
		m_collidersInView.Remove(other);
		
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		
		RebuildMesh();
	}
	
	private class VectorOffsetPair
	{
		public Vector3 vec;
		public Vector3 offsetVec;
	}
	
	private class OccluderVector
	{
		public Vector3 vec;
		public float angle;
	}
	
	private void RebuildMesh()
	{
		float nudgeMagnitude = 0.001f;
		
		Mesh newMesh = new Mesh();
		newMesh.name = "CameraView";
		
		Vector3 cameraDirection = m_camera.transform.rotation * Vector3.up;
		
		List<VectorOffsetPair> verts = new List<VectorOffsetPair>();
		
		foreach(Collider viewCollider in m_collidersInView)
		{
			MeshFilter mesh = viewCollider.GetComponent<MeshFilter>();
			if(mesh != null)
			{
				foreach(Vector3 vert in mesh.mesh.vertices)
				{
					Vector3 vertWorldPosition = viewCollider.transform.TransformPoint(vert );
					
					float cosTheta = Vector3.Dot(cameraDirection, Vector3.Normalize( vertWorldPosition - transform.position));
					float angle = Mathf.Acos(cosTheta);
					float mag = (vertWorldPosition - transform.position).magnitude;
					
					float thing = mag * cosTheta;
					
					if(thing < m_camera.range && angle < Mathf.Deg2Rad * m_camera.fov_degrees / 2.0f)
					{
						VectorOffsetPair newPair = new VectorOffsetPair();
						newPair.vec = 	vertWorldPosition;
					
						Vector3 offsetDirection = newPair.vec - (mesh.mesh.bounds.center + mesh.gameObject.transform.position);
						newPair.offsetVec = newPair.vec + (offsetDirection * nudgeMagnitude);
						newPair.offsetVec.z = newPair.vec.z;
					
						//Debug.DrawRay (newPair.vec  + new Vector3(0.0f, 0.0f, -5.0f), (offsetDirection * nudgeMagnitude) , Color.white);
					
						verts.Add(newPair);	
					}
				}
				
			}
			
			
		}
		
		
		
		
	
		List<Vector3> validVerts = new List<Vector3>();
		validVerts.Clear();
		
		//Debug.Log("Verts count: " + verts.Count);
		RaycastHit hitInfo;
		
		
		// Manually cast the edges
		Vector3 right = Quaternion.Euler(0.0f, 0.0f, -((m_camera.fov_degrees / 2.0f))) * cameraDirection;
		Vector3 left = Quaternion.Euler(0.0f, 0.0f, ((m_camera.fov_degrees / 2.0f))) * cameraDirection;
		
		Vector3 leftDirection = left;
		
		float edgeCosTheta = Vector3.Dot(cameraDirection, Vector3.Normalize(right));
		float edgeMaxMagnitude = m_camera.range / edgeCosTheta;
		
		right = Vector3.Normalize(right) * edgeMaxMagnitude;
		left = Vector3.Normalize(left) * edgeMaxMagnitude;
		
		
		
		//Debug.DrawRay(transform.position, left , Color.white);
		//Debug.DrawRay(transform.position, right , Color.white);
		
		if(!Physics.Raycast(this.transform.position, right, out hitInfo, edgeMaxMagnitude, collisionLayer))
		{
			validVerts.Add(transform.position + right);
		}
		else
		{
			validVerts.Add(hitInfo.point);	
		}
		
		if(!Physics.Raycast(this.transform.position, left, out hitInfo, edgeMaxMagnitude, collisionLayer))
		{
			validVerts.Add(transform.position + left);
		}
		else
		{
			validVerts.Add(hitInfo.point);	
		}
		
		right += transform.position;
		left += transform.position;
		
		// Iterate through the initial verts and add both valid verts and projected offset vert intersections
		foreach(VectorOffsetPair vert in verts)
		{
			
			Vector3 directionToVert = vert.vec - this.transform.position;
			
			directionToVert = vert.offsetVec - this.transform.position;
			float magnitude = directionToVert.magnitude * 0.98f;
			
			//Debug.DrawRay (this.transform.position  + new Vector3(0.0f, 0.0f, -5.0f), directionToVert , Color.red);
			
			
			// Check to see if the original vertex is occluded
			if (!Physics.Raycast (this.transform.position, directionToVert, out hitInfo, magnitude, collisionLayer)) 
			{
				// Not occluded. The vert itself is fine. Next we check the projected ray...
				validVerts.Add(vert.vec);
				
				float cosTheta = Vector3.Dot(cameraDirection, Vector3.Normalize(directionToVert));
				float maxMagnitude = m_camera.range / cosTheta;
				
				Vector3 maxPosition = transform.position + (Vector3.Normalize(directionToVert) * maxMagnitude);
				Vector3 newDirection = Vector3.Normalize(directionToVert) * maxMagnitude ;
				
				if(!Physics.Raycast(this.transform.position, newDirection, out hitInfo, newDirection.magnitude, collisionLayer))
				{
					validVerts.Add(maxPosition);
				}
				else
				{
					// Check to see how close this is to the non-offset version, as they're likely to mash together.
					float proximity = (vert.vec - hitInfo.point).magnitude;
					if(proximity > 0.5f)
					{

						
						validVerts.Add (hitInfo.point);	
					}
				}
			}
		}
		
		
		
		
		
		
		// Ray-cast along the extent of the wedge
		RaycastHit[] hits;
		RaycastHit[] hitsReverse;
		Vector3 direction = right - left;
		Vector3 directionReverse = -direction;
		
		Debug.DrawRay (right + new Vector3(0.0f, 0.0f, -5.0f), directionReverse , Color.cyan);
		
		hits = Physics.RaycastAll(left, direction, direction.magnitude, collisionLayer);
		hitsReverse = Physics.RaycastAll(right, directionReverse, direction.magnitude, collisionLayer);
		
		List<OccluderVector> occluders = new List<OccluderVector>();
		
		
		foreach(RaycastHit hit in hits)
		{
			Vector3 directionToVert = hit.point - this.transform.position;
			//Debug.DrawRay (this.transform.position  + new Vector3(0.0f, 0.0f, -5.0f), directionToVert , Color.magenta);
			if (!Physics.Raycast (this.transform.position, directionToVert, out hitInfo, directionToVert.magnitude * 0.98f, collisionLayer)) 
			{
				OccluderVector newOccluder = new OccluderVector();
			
				
				float angle = Mathf.Acos(Vector3.Dot(leftDirection, Vector3.Normalize(directionToVert)));
				if(float.IsNaN(angle))
				{
					int test = 5;	
				}
				
				newOccluder.angle = angle;
				
				newOccluder.vec = hit.point;
				
				occluders.Add(newOccluder);
					
				//validVerts.Add(hit.point);
			}
		}
		
		foreach(RaycastHit hit in hitsReverse)
		{
			Vector3 directionToVert = hit.point - this.transform.position;
			//Debug.DrawRay (this.transform.position  + new Vector3(0.0f, 0.0f, -5.0f), directionToVert , Color.magenta);
			if (!Physics.Raycast (this.transform.position, directionToVert, out hitInfo, directionToVert.magnitude * 0.98f, collisionLayer)) 
			{
				
				OccluderVector newOccluder = new OccluderVector();
				
				float angle = Mathf.Acos(Vector3.Dot(leftDirection, Vector3.Normalize(directionToVert)));
				newOccluder.angle = angle;
				
				if(float.IsNaN(angle))
				{
					int test = 5;	
				}
				
				newOccluder.vec = hit.point;
				occluders.Add(newOccluder);
				//validVerts.Add(hit.point);
			}
		}
		
		
		
		// Output all results
		foreach(Vector3 vert in validVerts)
		{
			Vector3 directionToVert = vert - this.transform.position;
			
			
			Vector3 cross = Vector3.Cross(cameraDirection, Vector3.Normalize(directionToVert));
			float sign = -(cross.z / Mathf.Abs(cross.z));
			
			OccluderVector newOccluder = new OccluderVector();
			newOccluder.vec = vert;
			Vector3 normalDirection = Vector3.Normalize(directionToVert);
			
			float angle = Mathf.Acos(Vector3.Dot(leftDirection, normalDirection));
			if(leftDirection == normalDirection)
			{
				angle = 0.0f;	
			}
			
			
			newOccluder.angle = angle;
			if(float.IsNaN(angle))
				{
					int test = 5;	
				}
			
			
			occluders.Add(newOccluder);
			
			
		}
		
		occluders.Sort(OccluderComparison);
		
		List<OccluderVector> dupes = new List<OccluderVector>();
		
		foreach(OccluderVector vert in occluders)
		{
			Vector3 directionToVert = vert.vec - this.transform.position;
			//Debug.Log("Offset angle: " + vert.offsetX + " | " + vert.vec.x + ", " + vert.vec.y + ", " + vert.vec.z);
			Debug.DrawRay (this.transform.position  + new Vector3(0.0f, 0.0f, -5.0f), directionToVert , Color.green);
			
			foreach(OccluderVector vert2 in occluders)
			{
				if((vert2.vec - vert.vec).magnitude < 0.1f && !dupes.Contains(vert2) && vert2 != vert)
				{
					dupes.Add(vert2);	
				}
				
			}
		}
		
		foreach(OccluderVector vert in dupes)
		{
		//	occluders.Remove(vert);	
		}
		
		
		
		//Debug.Log("Valid points: " + validVerts.Count);
		
		
		// Sort the points relative to the camera direction
		
		
		//Debug.Log("Valid verts: " + validVerts.Count);
		int indexCount = ((occluders.Count) - 1) * 3;
		
		Vector3[] 	vertices 	= new Vector3[occluders.Count + 1];
		Vector2[] 	uvs 		= new Vector2[occluders.Count + 1];
		int[] 		indices 	= new int[indexCount];
		
		vertices[0] = new Vector3(0.0f, 0.0f, 0.0f);
		
		//Debug.Log("Start");
		
		int index = 1;
		foreach(OccluderVector vert in occluders)
		{
			//Debug.Log("Angle: " + vert.angle);
			vertices[index] = Quaternion.Inverse(transform.rotation) *  (vert.vec - transform.position);
			uvs[index] = new Vector2(0.0f, 0.0f);
			
			index++;
		}
		
		for(int i = 1; i < occluders.Count; i++)
		{
			indices[(i - 1) * 3] = 0;
			indices[(i - 1) * 3 + 1] = i;
			indices[(i - 1) * 3 + 2] = i + 1;
		}
		
		
		//newMesh.SetIndices(indices, MeshTopology.Triangles, 0);
		newMesh.vertices = vertices;
		newMesh.uv = uvs;
		newMesh.triangles = indices;
		
		m_filter.mesh = newMesh;
	//	newMesh.RecalculateBounds();
		
	}
			
	private static int OccluderComparison(OccluderVector v1, OccluderVector v2)
	{
		if(v1.angle > v2.angle)
		{
			return 1;	
		}
		
		if(v1.angle == v2.angle)
		{
			//Debug.Log("Identical: " + v1.vec.x + ", " + v1.vec.y + " | " + v2.vec.x + ", " + v2.vec.y);
			return 0;
		}
		
		return -1;
	}
		
}
