using System;
using UnityEngine;
using System.Collections.Generic;



[Serializable]
public class LayoutNode : ScriptableObject
{
	public void AddConnection(LayoutNode other)
	{
		if(m_connections.Count < m_maxConnections)
		{
			LayoutConnection newConnection = ScriptableObject.CreateInstance<LayoutConnection>();
			newConnection.Source = this;
			newConnection.Target = other;
			
			m_connections.Add(newConnection);	
			other.m_connections.Add(newConnection);
		}
		else
		{
			Debug.Log("Maximum LayoutNode connections already made");
		}
	}
	
	public void RemoveConnection(LayoutNode other)
	{
		LayoutConnection foundConnection = null;
		foreach(var connection in m_connections)
		{
			if(connection.Source == other || connection.Target == other)
			{
				foundConnection = connection;
				break;
			}
		}
		if(foundConnection != null)
		{
			m_connections.Remove(foundConnection);	
			other.m_connections.Remove(foundConnection);	
		}
	}
	
	public void RemoveConnection(LayoutConnection connection)
	{
		m_connections.Remove(connection);
	}
	
	public void RemoveAllConnections()
	{
		while(m_connections.Count > 0)
		{
			RemoveConnection(m_connections[0]);	
		}
	}
	
	public List<LayoutConnection> ConnectedNodes
	{
		get { return m_connections; }	
	}
	
	public virtual List<GameObject> BuildObject()
	{
		Debug.Log("Building base-class");
		return null;
	}
	
	[SerializeField]
	public Vector2 m_position;
	
	[SerializeField]
	protected int m_maxConnections = 10;
	
	[SerializeField]
	protected List<LayoutConnection> m_connections = new List<LayoutConnection>();
}
