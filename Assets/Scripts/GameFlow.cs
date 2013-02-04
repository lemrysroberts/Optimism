using UnityEngine;
using System.Collections;

public class GameFlow : MonoBehaviour 
{
	public WorldView View = WorldView.Agent;
	
	
	private static GameFlow m_instance = null;	
	
	public static GameFlow Instance
	{
		get
		{
			if(m_instance == null)
			{
				Debug.LogError("GameFlow accessed prior to instantiation");
			}
			
			return m_instance;
		}
	}
	
	// Use this for initialization
	void Start () 
	{
		m_instance = this;
	}
	
	private GameFlow()
	{
		m_instance = this;
	}
}
