using UnityEngine;
using System.Collections;

public class Door : WorldObject 
{
	public enum DoorState
	{
		Open,
		Closed
	}
	
	public DoorState State 
	{
		get; set;	
	}
	
	Door()
	{
		State = DoorState.Closed;	
	}
	
	void Update()
	{
		if(State == DoorState.Open)
			(m_viewObject as AgentDoor).OpenDoor();
		else if(State == DoorState.Closed)
			(m_viewObject as AgentDoor).CloseDoor();
	}
	
    void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) 
	{
		int state = 0;
        if (stream.isWriting) {
            state = (int)State;
            stream.Serialize(ref state);
        } else {
            stream.Serialize(ref state);
            State = (DoorState)state;
        }
    }
	
	void OnDrawGizmos()
	{
		
		Gizmos.DrawWireCube(transform.position, new Vector3(2.0f, 1.0f, 0.0f));
	}
}
