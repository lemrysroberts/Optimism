using UnityEngine;
using System.Collections;

public class AgentDoor : WorldViewObject 
{
	private Door m_door = null;
	private GameObject m_planeChild = null;
	private bool m_opened = false;
		
	public void Start()
	{
		m_door = m_worldObject as Door;	
		m_planeChild = transform.FindChild("DoorPlane").gameObject;
	}
	
	public void OpenDoor()
	{
		if(!m_opened)
		{
			Vector3 newTransform = m_planeChild.transform.position;
			newTransform += m_planeChild.transform.TransformDirection(Vector3.up);
			m_planeChild.transform.position = newTransform;
		}
		
		m_opened =  true;
	}
	
	public void CloseDoor()
	{
		if(m_door != null)
		{
			if(m_opened)
			{
				Vector3 newTransform = m_planeChild.transform.position;
				newTransform -= m_planeChild.transform.TransformDirection(Vector3.up);
				m_planeChild.transform.position = newTransform;
			}
			m_opened = false;
		}
	}
	
	public void RequestOpen()
	{
		m_door.State = Door.DoorState.Open;	
		OpenDoor();
	}
	
	public void RequestClose()
	{
		m_door.State = Door.DoorState.Closed;
		CloseDoor();
	}
}
