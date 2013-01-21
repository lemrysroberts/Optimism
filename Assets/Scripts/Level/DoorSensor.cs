using UnityEngine;
using System.Collections;

public class DoorSensor : MonoBehaviour 
{
	public string TriggerTag = "Player";
	
	private AgentDoor m_door;
	private int m_detectedEntities = 0;
	
	// Use this for initialization
	void Start () 
	{
		m_door = transform.parent.gameObject.GetComponent<AgentDoor>();
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
	
	void OnTriggerEnter(Collider other)
	{
		Debug.Log(other.tag);
		if(other.gameObject.tag == TriggerTag)
		{
			m_detectedEntities++;
			m_door.OpenDoor();
			Debug.Log("Opening door");
		}
	}
	
	void OnTriggerExit(Collider other)
	{
		if(other.gameObject.tag == TriggerTag)
		{
			m_detectedEntities--;	
			
			Debug.Log("Entity count: " + m_detectedEntities);
			if(m_detectedEntities == 0)
			{
				Debug.Log("Closing door");
				m_door.CloseDoor();	
			}
		}
	}
}
