using UnityEngine;
using System.Collections;

public class GameFlowWrapper : MonoBehaviour {

	// Use this for initialization
	void Start () 
	{
		GameFlow.Instance.Begin();
	}
	
	// Update is called once per frame
	void Update () 
	{
		GameFlow.Instance.Update();
	}
}
