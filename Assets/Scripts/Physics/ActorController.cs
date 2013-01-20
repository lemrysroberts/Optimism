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
/// 		* Actual movement dynamics! 
/// 
/// </summary>


using UnityEngine;
using System.Collections;

public enum ActorControllerState
{
	Grounded,
	InAir,
	OnClimbable
}

public enum ActorControllerWallState
{
	None,
	Left,
	Right
}

[RequireComponent(typeof(Collider))]
public class ActorController : MonoBehaviour
{
	public int rayCount 			= 3;
	public float speed 				= 1.0f;
	public float gravity 			= 0.002f;
	public LayerMask collisionLayer = 0;
	
	private Vector3 m_velocity = new Vector3 (0.0f, 0.0f, 0.0f);
	private Vector3[] m_xRayOrigins;
	private Vector3[] m_yRayOrigins;
	private BoxCollider m_collider;
	private ActorControllerState m_state = ActorControllerState.Grounded;
	private ActorControllerWallState m_wallState = ActorControllerWallState.None;
	
	// I'm abusing properties to make these values visible to other classes, but not the inspector
	public ActorControllerState State
	{
		get { return m_state; }	
	}
	
	public ActorControllerWallState WallState
	{
		get { return m_wallState; }	
	}
	
	void OnEnable ()
	{
		m_collider = GetComponent<BoxCollider> ();
		
		if (m_collider != null) 
		{
			// Ensure that rays aren't cast at the extremes of the bounds.This basically just squeezes the rays in a smidgen.
			// Mathematically this shouldn't matter, but I don't know whether unity consider co-linear rays/bounds to be intersections. 
			// (I'm guessing it does, given the number of bugs I had)
			float tolerance = 0.7f;
			
			m_xRayOrigins = new Vector3[rayCount];
			m_yRayOrigins = new Vector3[rayCount];
			
			float xRaySeparation = ((float)m_collider.bounds.size.x * tolerance) / (float)(rayCount - 1);
			float yRaySeparation = ((float)m_collider.bounds.size.y * tolerance) / (float)(rayCount - 1);
			
			Vector3 position = transform.position;
			
			float startPosX = (m_collider.bounds.size.x - (m_collider.bounds.size.x * tolerance)) / 2.0f;
			float startPosY = (m_collider.bounds.size.y - (m_collider.bounds.size.y * tolerance)) / 2.0f;
								
			for (int rayIndex = 0; rayIndex < rayCount; rayIndex++) 
			{
				m_xRayOrigins [rayIndex] = new Vector3( position.x, startPosY + m_collider.bounds.min.y + ((float)rayIndex * yRaySeparation) - position.y, 0.0f);
				m_yRayOrigins [rayIndex] = new Vector3( startPosX + m_collider.bounds.min.x - position.x + ((float)rayIndex * xRaySeparation), position.y, 0.0f);
			}
			
			foreach (Vector3 vec in m_xRayOrigins) 
			{
				Debug.Log ("Created Left collision vectors: " + vec.x + ", " + vec.y + ", " + vec.z);
			}
			
			foreach (Vector3 vec in m_yRayOrigins) 
			{
				Debug.Log ("Created Up collision vectors: " + vec.x + ", " + vec.y + ", " + vec.z);
			}
		} 
		else 
		{
			Debug.LogError("Collider not found");	
		}
	}
	
	public void AddVelocity (Vector3 direction)
	{
		m_velocity += direction;	
	}
	
	void FixedUpdate ()
	{
		// Splat some gravity on there
		if(m_state != ActorControllerState.OnClimbable)
		{
			m_velocity.y -= gravity;
		}
		
			
		Vector3 delta = m_velocity;
		delta.x *= speed;
		
		Vector3 newPosition = transform.position + delta;
		
		if (Mathf.Abs (delta.x) > m_collider.bounds.size.x / 2.0f || Mathf.Abs (delta.y) > m_collider.bounds.size.y / 2.0f) 
		{
			Debug.Log ("Speed too high. Collider could tunnel.");
		}
		
		Vector3 xDirectionVec = new Vector3(m_collider.bounds.size.x / 2.0f, 0.0f, 0.0f);
		Vector3 yDirectionVec = new Vector3(0.0f, m_collider.bounds.size.y / 2.0f, 0.0f);
		
		if(delta.x <= 0)
		{
			xDirectionVec.x = -xDirectionVec.x;
		}
		
		if(delta.y <= 0)
		{
			yDirectionVec.y = -yDirectionVec.y;
		}
	
		bool xCollided = false;
		bool yCollided = false;
		
		// This is a bit of a hack to mitigate a bug I've yet to sort.
		// Basically, if you've accelerated a quarter of the way through a block before correcting the position, the orthogonal ray-cast can mis-fire.
		// This weights the checks to try and ensure that the most significant delta is checked first, but it's not really a solution.
		//
		// Bugs caused this attempts to fix: Getting stuck on the ceiling during a jump.
		// TODO: Find a solution!
		if(Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
		{
			xCollided = CollideX( ref newPosition, xDirectionVec );
			yCollided = CollideY( ref newPosition, yDirectionVec );
		}
		else
		{
			yCollided = CollideY (ref newPosition, yDirectionVec);
			xCollided = CollideX (ref newPosition, xDirectionVec);
		}
		
		UpdateStates(delta, xCollided, yCollided);
		
		transform.position = newPosition;
		m_velocity.x = 0.0f;
		
		if(m_state == ActorControllerState.OnClimbable)
		{
			m_velocity.y = 0.0f;	
		}
	}
	
	private bool CollideX(ref Vector3 position, Vector3 directionVec)
	{
		bool collided = false;
		
		RaycastHit hitInfo;
		foreach (Vector3 vector in m_xRayOrigins) 
		{
			Vector3 vecSource = position;
			vecSource.y += vector.y;
				
			Debug.DrawRay (vecSource, directionVec , Color.yellow);
			Debug.DrawRay (vecSource, -directionVec , Color.red);
			
			// Okay, so this should really loop over all results and find the closest intersection, but hey, they're the same size as the dude right now.
			if (Physics.Raycast (vecSource, directionVec, out hitInfo, directionVec.magnitude, collisionLayer)) 
			{
				position.x -= directionVec.x - (directionVec.x * (hitInfo.distance / directionVec.magnitude));
				m_velocity.x = 0.0f;
				collided = true;
				//break;
			}
		}
		return collided;
	}
	
	private bool CollideY(ref Vector3 position, Vector3 directionVec)
	{
		bool collided = false;
		
		RaycastHit hitInfo;
		foreach (Vector3 vector in m_yRayOrigins) 
		{
			Vector3 vecSource = position;
			vecSource.x += vector.x;
				
			Debug.DrawRay (vecSource, directionVec, Color.cyan);
			Debug.DrawRay (vecSource, new Vector3(directionVec.x, -directionVec.y, 0.0f), Color.gray);
			
			// Okay, so this should really loop over all results and find the closest intersection, but hey, they're the same size as the dude right now.
			if (Physics.Raycast (vecSource, directionVec, out hitInfo, directionVec.magnitude, collisionLayer)) 
			{
				position.y -= directionVec.y - (directionVec.y * (hitInfo.distance / directionVec.magnitude));
				m_velocity.y = 0.0f;
				collided = true;
				//break;
			}
		}
		return collided;
	}
	
	private void UpdateStates(Vector3 delta, bool collidedX, bool collidedY)
	{
		// 
		if(delta.y <= 0.0f && collidedY)
		{
			m_state = ActorControllerState.Grounded;	
		}
		else if(m_state != ActorControllerState.OnClimbable)
		{
			m_state = ActorControllerState.InAir;	
		}
		
		if(delta.x < 0.0f && collidedX)
		{
			m_wallState = ActorControllerWallState.Left;	
		}
		else if(delta.x < 0.0f)
		{
			m_wallState = ActorControllerWallState.None;
		}
		
		if(delta.x > 0.0f && collidedX)
		{
			m_wallState = ActorControllerWallState.Right;	
		}
		else
		{
				
		}
		
	}
	
	#region Triggers
	
	public void OnTriggerEnter(Collider other)
	{
		if(other.tag == "Climbable")
		{
			Debug.Log("Climbable entered");	
			m_state = ActorControllerState.OnClimbable;
		}
	}
	
	public void OnTriggerStay(Collider other)
	{
		if(other.tag == "Climbable")
		{
			m_state = ActorControllerState.OnClimbable;
		}
	}
	
	public void OnTriggerExit(Collider other)
	{
		m_state = ActorControllerState.InAir;
	}
	
	#endregion
	
	#region GUI
	
	public void OnGUI()
	{
		string output = "Actor State: \t" + System.Enum.GetName(typeof(ActorControllerState), m_state);
		output += "\nActorWall State: \t" + System.Enum.GetName(typeof(ActorControllerWallState), m_wallState);
		
		GUI.TextArea(new Rect(Screen.width - 200, Screen.height - 100, 180, 80), output);
	}
	
	#endregion
}

