using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour {
	
	
	
	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
	
	
	
	public void OpenDoor()
	{
		Debug.Log("Opening door..");
		Transform panelTransform = transform.FindChild("DoorPanel");
		
		if(panelTransform != null)
		{
			Vector3 newPosition = panelTransform.position;
			newPosition.y += 0.8f;
			
			panelTransform.position = newPosition;
		}
	}
	
	public void CloseDoor()
	{
		Debug.Log("Closing door..");
		Transform panelTransform = transform.FindChild("DoorPanel");
		
		if(panelTransform != null)
		{
			Vector3 newPosition = panelTransform.position;
			newPosition.y -= 0.8f;
			
			panelTransform.position = newPosition;
		}
	}
}
