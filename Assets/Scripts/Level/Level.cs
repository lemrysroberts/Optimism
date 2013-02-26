/// <summary>
/// High-level view of the level layout data.
/// 
/// Serialisation functions are contained within LevelSerialiser.cs
/// </summary>

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[ExecuteInEditMode]
public partial class Level : MonoBehaviour 
{
	void Start () 
	{
		/*
		WallLayoutNode newNode = ScriptableObject.CreateInstance<WallLayoutNode>();
		WallLayoutNode newNode2 = ScriptableObject.CreateInstance<WallLayoutNode>();
		
		newNode.m_position = new Vector2(3.0f, 6.5f);
		newNode2.m_position = new Vector2(3.0f, 3.5f);
		
		newNode.AddConnection(newNode2);
		
		m_nodes.Add(newNode);
		m_nodes.Add(newNode2);
		
		*/
	}
	
	void OnEnable()
	{
		
	}
	
	public List<LayoutNode> Nodes
	{
		get { return m_nodes; }	
	}
	
	public void AddNode(LayoutNode node)
	{
		m_nodes.Add(node);
		
	}
	
	public void RemoveNode(LayoutNode node)
	{
		node.RemoveAllConnections();
		m_nodes.Remove(node);
	}
	
	[SerializeField]
	private List<LayoutNode> m_nodes = new List<LayoutNode>();
	
	public LayoutNode SelectedNode = null;
	
}
