using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Level))]
public class LevelEditor : Editor 
{
	
	private bool m_blocked 	= false;
	private bool m_edit 	= false;
	
	public void OnEnable()
	{
		Level myTarget = (Level) target;
		EditorUtility.SetDirty (myTarget);
	}
	bool frameDelay = false;
	public override void OnInspectorGUI() 
	{
		Level myTarget = (Level) target;
		
		int oldWidth = myTarget.Width;
		int oldHeight = myTarget.Height;
		
		myTarget.Width = EditorGUILayout.IntSlider ("Width", myTarget.Width, 0, 100);
		myTarget.Height = EditorGUILayout.IntSlider ("Height", myTarget.Height, 0, 100);
		
		if(myTarget.Width != oldWidth || myTarget.Height != oldHeight || frameDelay)
		{
			myTarget.SetDirty(true);
			EditorUtility.SetDirty (target);
			
			// The renderer will also need to know about the change
			LevelRenderer renderer = myTarget.GetComponent<LevelRenderer>() as LevelRenderer;
			if(renderer != null)
			{
				EditorUtility.SetDirty (renderer);
				
				frameDelay = !frameDelay;
			}
		}
		
		m_blocked = GUILayout.Toggle(m_blocked, "Blocked");
		m_edit = GUILayout.Toggle(m_edit, "Edit Mode");
    }
	
	Vector3 hitLocation = new Vector3(0.0f, 0.0f, 0.0f);
	
	public void OnSceneGUI()
	{
	
		Level myTarget = (Level) target;
		Event e = Event.current;
		
		if(m_edit)
		{
			if ((e.type == EventType.MouseDrag || e.type == EventType.MouseDown) && e.button == 0)
		    {
		
				Plane zeroPlane = new Plane(new Vector3(0.0f, 0.0f, -1.0f), new Vector3(1.0f, 0.0f, 0.0f));
			
				Ray ray = HandleUtility.GUIPointToWorldRay(new Vector2(e.mousePosition.x, e.mousePosition.y));
			
				
				float hit;
				if(zeroPlane.Raycast(ray, out hit))
				{
					hitLocation = ray.GetPoint(hit);
					
				}
			
		    	e.Use();  //Eat the event so it doesn't propagate through the editor.
		
		  	}
			
			if ( e.type == EventType.Layout )
		    {
		       //somehow this allows e.Use() to actually function and block mouse input
		       HandleUtility.AddDefaultControl( GUIUtility.GetControlID( GetHashCode(), FocusType.Passive ) );
		    }
			
			EditorUtility.SetDirty (target);
			myTarget.SetTargetBlocked((int)(hitLocation.x + 0.5f), (int)(hitLocation.y + 0.5f), m_blocked);
			EditorUtility.SetDirty (target);
			
			
		}
		
		Handles.DrawLine(new Vector3(hitLocation.x, hitLocation.y, -10), new Vector3(hitLocation.x, hitLocation.y, -0.5f));
		Handles.DrawWireDisc(new Vector3(hitLocation.x, hitLocation.y, 0.0f), new Vector3(0.0f, 0.0f, -10), 0.2f);
		
		
			
		if (GUI.changed)
		{
       		
    	}
	}
	
}
