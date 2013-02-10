using UnityEngine;
using System.Collections;

public class GameFlowWrapper : MonoBehaviour 
{
	public string LevelToLoad
	{
		get { return m_levelToLoad; }
		set { m_levelToLoad = value; }
	}
	
	public GameObject LevelObject;
		
	[SerializeField]
	private string m_levelToLoad;

	// Use this for initialization
	void Start () 
	{
		GameFlow.Instance.LevelObject = LevelObject;
		GameFlow.Instance.CurrentLevel = m_levelToLoad;
		GameFlow.Instance.Begin();
	}
	
	// Update is called once per frame
	void Update () 
	{
		GameFlow.Instance.Update();
	}
}
