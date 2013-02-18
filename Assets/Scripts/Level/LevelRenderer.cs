using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
[RequireComponent(typeof(Level))]
public class LevelRenderer : MonoBehaviour 
{
	private Level m_level;
	public GameObject m_prefabTile;
	public Material blockedMaterial;
	public Material openMaterial;
	
	public Material blockedMaterialAlt;
	public Material openMaterialAlt;
	
	void OnEnable()
	{
		m_level = GetComponent<Level>();
		m_level.LevelChanged += new Level.LevelChangedEventHandler(OnLevelChanged);
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(m_level.Dirty())
		{
			m_data = m_level.GetData();
			
			if(m_data != null)
			{
				m_layoutObjects = new List<GameObject>(m_data.m_width * m_data.m_height);
				for(int i = 0; i < m_data.m_width * m_data.m_height; ++i)
				{
					m_layoutObjects.Add(null);	
				}
								
				List<GameObject> toDelete = new List<GameObject>();
				
				if(transform.childCount > 0)
				{
					foreach(Transform childTransform in transform)
					{
						GameObject child = childTransform.gameObject;
						
						Tile tile = child.GetComponent<Tile>();
						
						if(tile != null)
						{
							if(tile.X < m_data.m_width && tile.Y < m_data.m_height)
							{
								int index = tile.Y * (m_data.m_width - 1) + tile.X;
								if(index < m_layoutObjects.Count)
									m_layoutObjects[index] = child;
								else
									toDelete.Add(child);
							}
							else
							{
								toDelete.Add(child);
							}
						}
					}
					
					// Flush out any dead tiles
					foreach(GameObject gameObject in toDelete)
					{
						DestroyImmediate(gameObject);	
					}
				}
				BuildMesh();
				m_level.SetDirty(false);
			}
			
			
		}
		
	}
	
	private void BuildMesh()
	{
		if(m_data != null)
		{
			for(int y = 0; y < m_data.m_height; y++)
			{
				for(int x = 0; x < m_data.m_width; x++)
				{
				
					int index = y * (m_data.m_width - 1) + x;
					if(index >= m_layoutObjects.Count ||  m_layoutObjects[index] == null)
					{
						Debug.Log("Creating new tile...");
						GameObject newTile = Instantiate(m_prefabTile) as GameObject;
						Tile tileComponent = newTile.GetComponent<Tile>();
						
						newTile.transform.parent = gameObject.transform;
						tileComponent.X = x;
						tileComponent.Y = y;
						m_layoutObjects.Insert(index, newTile);
					}
				
					bool blocked = m_level.GetBlocked(x, y);
					
					
					SetBlocked(x, y, blocked);
				}
			}
		}
	}
	
	public void SetBlocked(int x, int y, bool blocked)
	{
		if(x < m_data.m_width && y < m_data.m_height && x >= 0 && y >= 0)
		{
			
			m_layoutObjects[y * (m_data.m_width - 1) + x].transform.position = new Vector3(x, y, blocked ? 0.0f : 10.0f);
			
			MeshRenderer renderer = m_layoutObjects[y * (m_data.m_width - 1) + x].GetComponent<MeshRenderer>();
			if(blocked)
			{
				renderer.sharedMaterial = GameFlow.Instance.View == WorldView.Agent ?  blockedMaterial : blockedMaterialAlt;
			}
			else
			{
				renderer.sharedMaterial = GameFlow.Instance.View == WorldView.Agent ?  openMaterial : openMaterialAlt;
			}
		} 
	}
	
	public void OnLevelChanged(int x, int y)
	{
		bool blocked = m_level.GetData().IsBlocked(x, y);
		
		SetBlocked(x, y, blocked);
		
	}
	
	[SerializeField]
	private Level.Layout m_data;
	
	[SerializeField]
	private List<GameObject> m_layoutObjects = null;
	
}
