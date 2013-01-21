using UnityEngine;
using System.Collections;

public class AgentDoor : WorldViewObject 
{
	private Door m_door = null;
	private GameObject m_planeChild = null;
	
	public void Start()
	{
		m_door = m_worldObject as Door;	
		m_planeChild = transform.FindChild("DoorPlane").gameObject;
	}
	
	public void OpenDoor()
	{
		if(m_door.State != Door.DoorState.Open)
		{
			Vector3 newTransform = m_planeChild.transform.position;
			newTransform.y += 1.0f;
			m_planeChild.transform.position = newTransform;
		}
		
		m_door.State = Door.DoorState.Open;
	}
	
	public void CloseDoor()
	{
		if(m_door.State == Door.DoorState.Open)
		{
			Vector3 newTransform = m_planeChild.transform.position;
			newTransform.y -= 1.0f;
			m_planeChild.transform.position = newTransform;
		}
		m_door.State = Door.DoorState.Closed;
	}
}
