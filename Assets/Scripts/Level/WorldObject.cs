
using UnityEngine;

public enum WorldView
{
	Agent,
	Admin
}

public abstract class WorldObject : MonoBehaviour
{
	public WorldView ViewType;
	void Start()
	{
		CreateViewObject(ViewType);
	}
	
	public void OnDestroy()
	{
		Destroy(m_gameObject);	
	}
	
	public void CreateViewObject(WorldView view)
	{
		switch(view)
		{
			case WorldView.Agent: 	m_gameObject = UnityEngine.GameObject.Instantiate(m_agentViewPrefab) as GameObject; break;
			case WorldView.Admin: 	m_gameObject = UnityEngine.GameObject.Instantiate(m_adminViewPrefab) as GameObject; break;
		}
		
		m_viewObject = m_gameObject.GetComponent<WorldViewObject>();
		
		if(m_viewObject == null)
		{
			Debug.LogError("Cannot add view of type: " + m_gameObject.name + " as the type does not contain a WorldViewObject script");
			m_gameObject = null;
			
			return;
		}
		
		m_viewObject.SetWorldObject(this);
		//m_gameObject.name = ;
		
		m_gameObject.transform.parent 	= transform;
		m_gameObject.transform.position = transform.position;
	}
	
	public GameObject m_agentViewPrefab;
	public GameObject m_adminViewPrefab;
	
	protected GameObject 		m_gameObject = null;
	protected WorldViewObject 	m_viewObject = null;
	
}
