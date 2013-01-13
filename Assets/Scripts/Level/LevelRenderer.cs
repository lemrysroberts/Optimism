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
	
	// Use this for initialization
	void Start () 
	{
		
		
	}
	
	void OnEnable()
	{
		Debug.Log("Enabled...");	 
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
				m_layoutObjects = new GameObject[m_data.GetLength(0), m_data.GetLength(1)];
				
				List<GameObject> toDelete = new List<GameObject>();
				
				if(transform.childCount > 0)
				{
					foreach(Transform childTransform in transform)
					{
						GameObject child = childTransform.gameObject;
						
						Tile tile = child.GetComponent<Tile>();
						
						if(tile != null)
						{
							if(tile.X < m_data.GetLength(0) && tile.Y < m_data.GetLength(1))
							{
								m_layoutObjects[tile.X, tile.Y] = child;
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
			}
			
			m_level.SetDirty(false);
		}
		
	}
	
	private void BuildMesh()
	{
		if(m_data != null)
		{
			for(int x = 0; x < m_data.GetLength(0); x++)
			{
				for(int y = 0; y < m_data.GetLength(1); y++)
				{
					if(m_layoutObjects[x,y] != null)
						continue;
				
					bool blocked = m_level.GetBlocked(x, y);
					
					GameObject newTile = Instantiate(m_prefabTile) as GameObject;
					Tile tileComponent = newTile.GetComponent<Tile>();
					
					newTile.transform.parent = gameObject.transform;
					tileComponent.X = x;
					tileComponent.Y = y;
				
					newTile.transform.position = new Vector3(x, y, blocked ? 0.0f : 10.0f);
					
					// This should be set via a more meaningful script in the tileType itself
					MeshRenderer renderer = newTile.GetComponent<MeshRenderer>();
					if(renderer != null)
					{
						if(blocked)
						{
							renderer.sharedMaterial = blockedMaterial;
							
						}
						else
						{
							renderer.sharedMaterial = openMaterial;
						}
					}
					m_layoutObjects[x,y] = newTile;
				}
			}
			}
	}
	
	public void SetBlocked(int x, int y, bool blocked)
	{
		if(x < m_layoutObjects.GetLength(0) && y < m_layoutObjects.GetLength(1))
		{
			
			m_layoutObjects[x,y].transform.position = new Vector3(x, y, blocked ? 0.0f : 10.0f);
			
			MeshRenderer renderer = m_layoutObjects[x,y].GetComponent<MeshRenderer>();
			if(blocked)
			{
				renderer.material = blockedMaterial;
			}
			else
			{
				renderer.material = openMaterial;
			}
		}
	}
	
	public void OnLevelChanged(int x, int y)
	{
		bool blocked = m_level.GetData()[x, y];
		
		SetBlocked(x, y, blocked);
		
	}
	
	void OnDestroy()
	{
				
	}
	
	private bool[,] m_data;
	private GameObject[,] m_layoutObjects;
}
