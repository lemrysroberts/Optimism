using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Because I. Love. FLOW.
/// </summary>
public class GameFlow
{
	public WorldView View = WorldView.Agent;
	
	
	private static GameFlow m_instance = null;	
	
	public List<GameObject> AgentStartupItems
	{
		get { return m_agentStartupObjects; }
		set { m_agentStartupObjects = value; }
	}
	
	public List<GameObject> AdminStartupItems
	{
		get { return m_adminStartupObjects; }
		set { m_adminStartupObjects = value; }
	}
	
	public static GameFlow Instance
	{
		get
		{
			if(m_instance == null)
			{
				m_instance = new GameFlow();
			}
			
			return m_instance;
		}
	}
	
	public void Begin()
	{
		if(View == WorldView.Agent)
		{
			BeginAgent();	
		}
		else if(View ==	WorldView.Admin)
		{
			BeginAdmin();	
		}
	
		
	}
	
	public void Update()
	{
		if(m_spawnRequired)
		{
			SpawnController.Instance.SpawnRandom("Player");
			m_spawnRequired = false;
		}
		
	}
	
	private void BeginAgent()
	{
		m_spawnRequired = true;	
	}
	
	private void BeginAdmin()
	{
		
	}
	
	private bool m_spawnRequired = false;
	
	private List<GameObject> m_agentStartupObjects = new List<GameObject>();
	private List<GameObject> m_adminStartupObjects = new List<GameObject>();
}
