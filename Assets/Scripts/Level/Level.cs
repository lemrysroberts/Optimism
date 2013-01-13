/// <summary>
/// High-level view of the level layout data.
/// </summary>

using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class Level : MonoBehaviour 
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
		m_layout = new bool[Width, Height];
	}
	
	public bool[,] GetData()
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
		
			bool[,] newLayout = new bool[Width, Height];
			
			for(int x = 0; x < Width ; x++)
			{
				for(int y = 0; y < Height; y++)
				{
					if(m_layout == null || x >= m_layout.GetLength(0) || y >= m_layout.GetLength(1))
					{
						newLayout[x,y] = false;	
					}
					else
					{
						newLayout[x,y] = m_layout[x,y];
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
		
		if(x >= 0 && y >= 0 && x < Width && y < Height)
		{
		//	Debug.Log("Set blocked");
			m_layout[x,y] = blocked;
			m_dirty = true;
			
			if(LevelChanged != null)
				LevelChanged(x, y);
		}
	}
#endif
	
	
	public bool GetBlocked(int x, int y)
	{
		if(x < m_layout.GetLength(0) && y < m_layout.GetLength(1))
		{
			return m_layout[x,y];	
		}
		
		return false;
	}
		
	// Tile array.
	// This is all for prototyping, so it can be replaced with less derpy constructs later.
	bool[,] m_layout;
	bool m_dirty;
}
