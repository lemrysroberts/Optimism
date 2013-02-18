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
	public int Width = 5;
	public int Height = 5;
	
	public delegate void LevelChangedEventHandler(int x, int y);
	public event LevelChangedEventHandler LevelChanged;
	
	
	// Use this for initialization
	void Start () 
	{
		m_dirty = true;
		
	}
	
	void OnEnable()
	{
		if(m_layout == null)
		{
			Layout newLayout = new Layout(Width, Height);
			
			m_layout = newLayout;
		}
	}
	
	public Layout GetData()
	{
		return m_layout;	
	}
	
	public bool Dirty()
	{
		return m_dirty;	
	}
	
	public void SetDirty(bool dirty)
	{
		m_dirty = dirty;	
		
		if(dirty)
		{
			Layout newLayout = new Layout(Width, Height);
			
			for(int x = 0; x < Width ; x++)
			{
				for(int y = 0; y < Height; y++)
				{
					if(m_layout == null || x >= m_layout.m_width || y >= m_layout.m_height)
					{
						newLayout.SetBlocked(x, y, false);	
					}
					else
					{
						newLayout.SetBlocked(x, y, m_layout.IsBlocked(x, y));
					}
				}
			}
			
			m_layout = newLayout;
			
		}
	}

#if UNITY_EDITOR	
	public void SetTargetBlocked(int x, int y, bool blocked)
	{
		if(m_layout == null)
		{
			SetDirty(true);	
		}
		
		m_layout.SetBlocked(x, y, blocked);
			
		if(LevelChanged != null)
			LevelChanged(x, y);
	}
#endif
	
	public bool GetBlocked(int x, int y)
	{
		return m_layout.IsBlocked(x, y);
	}
		
	// Tile array.
	// This is all for prototyping, so it can be replaced with less derpy constructs later.
	[SerializeField]
	public Layout m_layout = null;
	bool m_dirty;
	
	[System.Serializable]
	public class Layout
	{
		[SerializeField]
		public int m_width;
		
		[SerializeField]
		public int m_height;
		
		[SerializeField]
		public List<bool> m_layout = null;
		
		public Layout(int width, int height)
		{
			m_width = width;
			m_height = height;
			m_layout = new List<bool>(width * height);
			
			for(int i = 0; i < m_width * m_height; ++i)
			{
				m_layout.Add(false);	
			}
		}
		
		public bool IsBlocked(int x, int y)
		{
			if(x >= m_width || y >= m_height || x < 0 || y < 0)
			{
				return false;
			}
			
			return m_layout[y * (m_width - 1) + x];
		}
		
		public void SetBlocked(int x, int y, bool blocked)
		{
			if(x >= m_width || y >= m_height || x < 0 || y < 0)
			{
				return;
			}
			
			m_layout[y * (m_width - 1) + x] = blocked;
		}
				
	}
}
