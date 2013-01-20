/// <summary>
/// Actor controller.
/// 
/// Overview: 	This is just a ghetto box ray-cast. It looks at the next position of the object and checks for ray-intersections with any colliders.
/// 			The motivation I have for using this over box-colliders is that it's trivial to find the intersection point and halt movement there.
/// 			
/// 
/// Notes: 	
/// 		* Collides with itself if its own box-collider is enabled. How odd.
/// 		* Due to the simplistic ray-casting, this will tunnel at sufficiently high speeds. A warning is issued, at least.
/// 
/// TODO:
/// 		* Sort out the self-collision problem. No other objects will be able to test for intersection with the player until this is done, as the collider is disabled.
/// 		* This class is an optimisation wet-dream, as I've done the bare minimum to ensure collisions occur correctly. Most of the involved data could be pre-processed.
/// 
/// </summary>


using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class ActorController : MonoBehaviour
{
	private Vector3 m_velocity = new Vector3 (0.0f, 0.0f, 0.0f);
	
	private Vector3[] m_leftVectors;
	private Vector3[] m_rightVectors;
	private Vector3[] m_upVectors;
	private Vector3[] m_downVectors;
	public int rayCount = 3;
	public float speed = 1.0f;
	public float gravity = 0.002f;
	private BoxCollider m_collider;
	
	// Use this for initialization
	void Start ()
	{
		m_collider = GetComponent<BoxCollider> ();
	}
	
	void OnEnable ()
	{
		m_collider = GetComponent<BoxCollider> ();
		
		if (m_collider != null) 
		{
			float tolerance = 0.98f;
			
			m_leftVectors = new Vector3[rayCount];
			m_rightVectors = new Vector3[rayCount];
			m_upVectors = new Vector3[rayCount];
			m_downVectors = new Vector3[rayCount];
			
			float xRaySeparation = ((float)m_collider.bounds.size.x * tolerance) / (float)(rayCount - 1);
			float yRaySeparation = ((float)m_collider.bounds.size.y * tolerance) / (float)(rayCount - 1);
			
			Vector3 position = transform.position;
			
			float startPosX = (m_collider.bounds.size.x - (m_collider.bounds.size.x * tolerance)) / 2.0f;
			float startPosY = (m_collider.bounds.size.y - (m_collider.bounds.size.y * tolerance)) / 2.0f;
								
			for (int rayIndex = 0; rayIndex < rayCount; rayIndex++) 
			{
				m_leftVectors [rayIndex] = new Vector3 (m_collider.bounds.min.x - position.x, startPosY + m_collider.bounds.min.y + ((float)rayIndex * yRaySeparation) - position.y, 0.0f);
				m_rightVectors [rayIndex] = new Vector3 (m_collider.bounds.max.x - position.x, startPosY + m_collider.bounds.min.y + ((float)rayIndex * yRaySeparation) - position.y, 0.0f);
				m_upVectors [rayIndex] = new Vector3 (startPosX + m_collider.bounds.min.x - position.x + ((float)rayIndex * xRaySeparation), m_collider.bounds.max.y - position.y, 0.0f);
				m_downVectors [rayIndex] = new Vector3 (startPosX + m_collider.bounds.min.x - position.x + ((float)rayIndex * xRaySeparation), m_collider.bounds.min.y - position.y, 0.0f);
			}
			
			foreach (Vector3 vec in m_leftVectors) 
			{
				Debug.Log ("Created Left collision vectors: " + vec.x + ", " + vec.y + ", " + vec.z);
			}
			foreach (Vector3 vec in m_rightVectors) 
			{
				Debug.Log ("Created Right collision vectors: " + vec.x + ", " + vec.y + ", " + vec.z);
			}
			foreach (Vector3 vec in m_upVectors) 
			{
				Debug.Log ("Created Up collision vectors: " + vec.x + ", " + vec.y + ", " + vec.z);
			}
			foreach (Vector3 vec in m_downVectors) 
			{
				Debug.Log ("Created Down collision vectors: " + vec.x + ", " + vec.y + ", " + vec.z);
			}
				
		} 
		else 
		{
			Debug.Log ("Collider not found");	
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
	}
	
	public void AddVelocity (Vector3 direction)
	{
		m_velocity += direction;	
	}
	
	void OnGui ()
	{
		Debug.Log ("test");
	}
	
	void FixedUpdate ()
	{
		// Three ray-cast approach
		
		m_velocity.y -= gravity;

			
		Vector3 delta = m_velocity;
		delta.x *= speed;
		//delta.y -= gravity;
		Vector3 newPosition = transform.position + delta;
		Debug.Log ("Delta: " + delta.x + ", " + delta.y);
		
		if (Mathf.Abs (delta.x) > m_collider.bounds.size.x / 2.0f || Mathf.Abs (delta.y) > m_collider.bounds.size.y / 2.0f) 
		{
			Debug.Log ("Speed too high. Collider could tunnel.");
		}
		
		// TODO: Remove redundant rays at corners
		float length = 1.0f;
		
		if (delta.x > 0) 
		{ // right
			RaycastHit hitInfo;
			foreach (Vector3 vector in m_rightVectors) 
			{
				Vector3 newDir = new Vector3 (0.5f, 0.0f, 0.0f);
				Vector3 vecSource = newPosition;
				vecSource -= newDir;
				vecSource.y = newPosition.y;
					
				Debug.DrawRay (vecSource, newDir * length, Color.yellow);
				if (Physics.Raycast (vecSource, newDir, out hitInfo, length)) 
				{
					newPosition.x -= newDir.x - (newDir.x * hitInfo.distance);
					m_velocity.x = 0.0f;
					break;
				}
			}
		} 
		else if (delta.x < 0) 
		{ // left
			RaycastHit hitInfo;
			foreach (Vector3 vector in m_rightVectors) 
			{
				Vector3 newDir = new Vector3 (-0.5f, 0.0f, 0.0f);
				Vector3 vecSource = newPosition;
				vecSource -= newDir;
				vecSource.y = newPosition.y;
					
				Debug.DrawRay (vecSource, newDir * length, Color.cyan);
				if (Physics.Raycast (vecSource, newDir, out hitInfo, length)) 
				{
					newPosition.x -= newDir.x - (newDir.x * hitInfo.distance);
					m_velocity.x = 0.0f;
					break;
				}
			}
		}
		
		if (delta.y < 0) 
		{
			RaycastHit hitInfo;
			foreach (Vector3 vector in m_downVectors) 
			{
				Vector3 newDir = new Vector3 (-0.0f, -0.5f, 0.0f);
				Vector3 vecSource = newPosition;
				vecSource -= newDir;
				vecSource.x += vector.x;
					
				Debug.DrawRay (vecSource, newDir * length, Color.cyan);
				if (Physics.Raycast (vecSource, newDir, out hitInfo, length)) 
				{
					float change = newDir.y - (newDir.y * hitInfo.distance);
					newPosition.y -= change;
					m_velocity.y = 0.0f;
					//Debug.Log ("Resetting y. Change " + change);
					Debug.Log ("New y: " + newPosition.y);
					//break;
					
					if(newPosition.y > 1.3f)
					{
						int test = 0;	
					}
				}
			}
		}
		
		if (delta.y > 0) 
		{
			RaycastHit hitInfo;
			foreach (Vector3 vector in m_downVectors) 
			{
				Vector3 newDir = new Vector3 (-0.0f, 0.5f, 0.0f);
				Vector3 vecSource = newPosition;
				vecSource -= newDir;
				vecSource.x += vector.x;
					
				Debug.DrawRay (vecSource, newDir * length, Color.cyan);
				if (Physics.Raycast (vecSource, newDir, out hitInfo, length)) 
				{
					float change = newDir.y - (newDir.y * hitInfo.distance);
					newPosition.y -= change;
					m_velocity.y = 0.0f;
					Debug.Log ("New y: " + newPosition.y);
					
				}
			}
		}
		
		transform.position = newPosition;
	}
}
