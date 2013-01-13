using UnityEngine;
using System.Collections;

[RequireComponent(typeof(RetardPhysics))]
public class KeyMove : MonoBehaviour {
	
	
	public float MoveSpeed = 0.1f;
	public float JumpPower = 0.1f;
	
	RetardPhysics m_physics = null;
	
	// Use this for initialization
	void Start () 
	{
		m_physics = GetComponent<RetardPhysics>();
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		if(Input.GetKey(KeyCode.UpArrow))
		{
			//transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + (0.01f * MoveSpeed));
		}
		
		if(Input.GetKey(KeyCode.DownArrow))
		{
			//transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - (0.01f * MoveSpeed));
		}
		
		if(Input.GetKey(KeyCode.LeftArrow))
		{
			m_physics.AddVelocity(new Vector3(-MoveSpeed, 0.0f, 0.0f));
			//transform.position = new Vector3(transform.position.x - (0.01f * MoveSpeed), transform.position.y, transform.position.z);
		}
		
		if(Input.GetKey(KeyCode.RightArrow))
		{
			m_physics.AddVelocity(new Vector3(MoveSpeed, 0.0f, 0.0f));
			//transform.position = new Vector3(transform.position.x + (0.01f * MoveSpeed), transform.position.y, transform.position.z);
		}
		
		if(Input.GetKey(KeyCode.Space))
		{
			m_physics.AddVelocity(new Vector3(0.0f, JumpPower, 0.0f));
		}
	}
}
