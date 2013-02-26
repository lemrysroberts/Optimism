using UnityEngine;
using System;
using System.Collections;

[Serializable]
public class LayoutConnection : ScriptableObject
{
	public LayoutConnection()
	{
		Built = false;
	}
	
	public LayoutNode Source
	{
		get { return m_sourceNode; }
		set { m_sourceNode = value; }
	}
	
	public LayoutNode Target
	{
		get { return m_targetNode; }	
		set { m_targetNode = value; }
	}
	
	public bool Built
	{
		get; set;	
	}
	
	[SerializeField]
	private LayoutNode m_sourceNode;
	
	[SerializeField]
	private LayoutNode m_targetNode;
}
