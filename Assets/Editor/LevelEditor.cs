using UnityEditor;
using UnityEngine;
using System.IO;

[CustomEditor(typeof(Level))]
public class LevelEditor : Editor 
{
	private bool m_blocked 	= false;
	private bool m_edit 	= false;
	
	public string loadedFile = string.Empty;
	
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
			AutoSave();
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
		/*
		GUILayout.Label("Loaded Level: " + myTarget.LoadedLevel);
		
		if(GUILayout.Button("Save..."))
		{
			var path = EditorUtility.SaveFilePanelInProject("Save Level...", "new_level", "xml", "bleurgh");
			
			myTarget.Serialise(path);
			AutoSave();
		}
		
		if(GUILayout.Button("Load..."))
		{
			var path = EditorUtility.OpenFilePanel("Load Level...", "levels", "xml");
			
			// Strip the data-path
			if(path != null)
			{
				int dataIndex = path.IndexOf(Application.dataPath);
				
				if(dataIndex != -1)
				{
					path = path.Remove(dataIndex, Application.dataPath.Length);
					
					if(path[0] == '\\' || path[0] == '/')
					{
						path = path.Remove(0, 1);	
					}
					
					myTarget.Deserialise(path);
				
					LevelRenderer renderer = myTarget.GetComponent<LevelRenderer>() as LevelRenderer;
					if(renderer != null)
					{
						EditorUtility.SetDirty (renderer);
					}
					EditorUtility.SetDirty (target);
				}
				else
				{
					Debug.LogError("Path \"" + path + "\" is not within the assets directory");
				}
			}
		}
		*/
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
					
					EditorUtility.SetDirty (target);
					myTarget.SetTargetBlocked((int)(hitLocation.x + 0.5f), (int)(hitLocation.y + 0.5f), m_blocked);
					EditorUtility.SetDirty (target);
					AutoSave();
					
				}
			
		    	e.Use();  //Eat the event so it doesn't propagate through the editor.
		
		  	}
			
			if ( e.type == EventType.Layout )
		    {
		       //somehow this allows e.Use() to actually function and block mouse input
		       HandleUtility.AddDefaultControl( GUIUtility.GetControlID( GetHashCode(), FocusType.Passive ) );
		    }
		}
		
		Handles.DrawLine(new Vector3(hitLocation.x, hitLocation.y, -10), new Vector3(hitLocation.x, hitLocation.y, -0.5f));
		Handles.DrawWireDisc(new Vector3(hitLocation.x, hitLocation.y, 0.0f), new Vector3(0.0f, 0.0f, -10), 0.2f);

	}
	
	private void AutoSave()
	{
		return;
		
	}
	
	private void OnDestroy()
	{
		Debug.Log("DESTROYING");	
	}
	
}
