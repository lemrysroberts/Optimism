using UnityEngine;
using System.Collections;

public class Camera : WorldObject 
{
	public float range = 2.0f;
	public float fov_degrees = 35.0f;
	public float rotation = 0.0f;
	public float rotationSpeed = 0.5f;
	public float maxRotation_degrees = 90.0f;
	
	Camera()
	{
			
	}
	
	protected override void OnStart()
	{
		transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotation);	
	}
	
	
	private bool right = false;
	void FixedUpdate()
	{
		//return;
		if(right)
		{
			if(rotation < maxRotation_degrees)
			{
				rotation += rotationSpeed;
			}
			else
			{
				right = !right;	
			}
		}
		else
		{
			if(rotation > -maxRotation_degrees)
			{
				rotation -= rotationSpeed;
			}
			else
			{
				right = !right;	
			}
		}
		
		transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotation);
	}
	
	void OnDrawGizmos()
	{
		Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);
		
		float halfWidth = range * Mathf.Tan((fov_degrees/ 2.0f) * Mathf.Deg2Rad) ;
		
		Gizmos.DrawLine(new Vector3(0.0f, 0.0f, 0.0f), new Vector3(-halfWidth, range, 0.0f));
		Gizmos.DrawLine(new Vector3(0.0f, 0.0f, 0.0f), new Vector3(halfWidth, range, 0.0f));
	}
}
