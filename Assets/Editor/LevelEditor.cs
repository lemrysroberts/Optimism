using UnityEditor;
using UnityEngine;
using System.IO;

[CustomEditor(typeof(Level))]
public class LevelEditor : Editor 
{
	/// <summary>
	/// Listing of the different ways to edit the level.
	/// </summary>
	private enum EditModes
	{
		MoveNodes,
		ConnectNodes,
		
		Max
	}
	
	/// <summary>
	/// Mode labels.
	/// </summary>
	private static string[] labels = 
	{
		"Move Nodes",
		"Connect Nodes"
	};
	
	public override void OnInspectorGUI() 
	{
		Level editorLevel = (Level)target;
		
		if(editorLevel != null &&  editorLevel.SelectedNode != null)
		{
			EditorGUILayout.BeginHorizontal();
			
			editorLevel.SelectedNode.m_position.x = EditorGUILayout.FloatField("x", editorLevel.SelectedNode.m_position.x);
			editorLevel.SelectedNode.m_position.y = EditorGUILayout.FloatField("y", editorLevel.SelectedNode.m_position.y);
			
			EditorGUILayout.EndHorizontal();
			
			GUILayout.Label("Selected item: " + editorLevel.SelectedNode.m_position.x + ", " + editorLevel.SelectedNode.m_position.y);
		}
		
		if(GUILayout.Button("Rebuild Level"))
		{
			LayoutObjectBuilder builder = new LayoutObjectBuilder();
			
			builder.BuildObjects(editorLevel);
		}
	}
	
	public void OnSceneGUI()
	{
		Level editorLevel = (Level) target;
		Event e = Event.current;
		
		if(e.type == EventType.KeyDown && e.keyCode == KeyCode.Delete)
		{
			if(editorLevel.SelectedNode != null)
			{
				editorLevel.RemoveNode(editorLevel.SelectedNode);
				e.Use();
				ClearDrag();
			}
		}
		
		if(e.type == EventType.MouseDown && e.button == 1)
		{
			ClearDrag();
			Debug.Log("Up");
		}
		
		int idCounter = 1;
		foreach(var node in editorLevel.Nodes)
		{
			Vector3 handlePos = new Vector3(node.m_position.x, node.m_position.y, 0.0f);
			
			if(m_mode == EditModes.MoveNodes)
			{
				Vector3 newPos =  Handles.Slider2D(idCounter, handlePos, new Vector3(0.0f, 0.0f, -1.0f), new Vector3(1.0f, 0.0f, 0.0f), new Vector3(0.0f, 1.0f, 0.0f), 0.2f, Handles.CubeCap, new Vector2(0.5f, 0.5f));
				
				node.m_position.x = newPos.x;
				node.m_position.y = newPos.y;
				
				ClearDrag();
			}
			else
			{
				if(Handles.Button(handlePos, Quaternion.identity, 0.2f, 0.2f, Handles.CubeCap))
				{
					if(!m_dragStarted)
					{
						Debug.Log("Click detected");
						m_connectionStart = node;
						m_dragStarted = true;
					
						editorLevel.SelectedNode = node;
					}
					else
					{
						if(node != m_connectionStart)
						{
							node.AddConnection(m_connectionStart);	
							ClearDrag();
						}
							
					}
				}
			}
			
			// Track whether the slider is selected.
			if(GUIUtility.hotControl == idCounter)
			{
				editorLevel.SelectedNode = node;	
			}
			
			// Draw connections
			foreach(var connection in node.ConnectedNodes)
			{
				if(connection.Target != null && connection.Source != null)
				{
					Vector3 connectionPos = new Vector3(connection.Target.m_position.x, connection.Target.m_position.y, 0.0f);
					Handles.DrawLine(handlePos, connectionPos);	
				}
			}
			
			idCounter++;
		}
		
		if(m_dragStarted)
		{
			
			Vector3 startPos = new Vector3(m_connectionStart.m_position.x, m_connectionStart.m_position.y, 0.0f);
			
			
			Plane zeroPlane = new Plane(new Vector3(0.0f, 0.0f, -1.0f), new Vector3(1.0f, 0.0f, 0.0f));

			Ray ray = HandleUtility.GUIPointToWorldRay(new Vector2(e.mousePosition.x, e.mousePosition.y));


			float hit;
			if(zeroPlane.Raycast(ray, out hit))
			{
					var hitLocation = ray.GetPoint(hit);
				
				Vector3 targetPos = new Vector3(hitLocation.x, hitLocation.y, 0.0f);
				
				Handles.DrawLine(startPos, targetPos);
				HandleUtility.Repaint();
			}
		}
		
		DrawGUILayout();
	}	
	
	private void ClearDrag()
	{
		m_connectionStart = null;
		m_dragStarted = false;
	}
	
	void OnMouseOver()
	{
		Debug.Log("TEST");	
	}
	
	private void DrawGUILayout()
	{
		Handles.BeginGUI();
		
		GUILayout.BeginArea(new Rect(10.0f, 10.0f, 150.0f, 400.0f));
		
		GUIStyle newStyle = GUI.skin.GetStyle("Button");
		newStyle.stretchWidth = true;
		newStyle.fixedWidth = 100.0f;
		
		m_mode = (EditModes) GUILayout.SelectionGrid((int)m_mode, labels, 1);
		
		GUILayout.EndArea();
		
		GUILayout.BeginArea(new Rect(10.0f, Screen.height - 100.0f, 150.0f, 90.0f));
		
		if(GUILayout.Button("Add Node"))
		{
			UnityEngine.Camera currentCamera = UnityEngine.Camera.current;
				
			Vector3 placePoint = currentCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 1.0f));
			
			Level editorLevel = (Level) target;
			WallLayoutNode newNode = ScriptableObject.CreateInstance<WallLayoutNode>();
			newNode.m_position.x = placePoint.x;
			newNode.m_position.y = placePoint.y;
				
			editorLevel.AddNode(newNode);
		}
		
		GUILayout.EndArea();
		
		Handles.EndGUI();
	}
	
	private EditModes m_mode = EditModes.MoveNodes;
	
	private LayoutNode m_connectionStart = null;
	
	private bool m_dragStarted = false;
}
