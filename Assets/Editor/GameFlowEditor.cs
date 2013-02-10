using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class GameFlowEditor : EditorWindow 
{
	[MenuItem("Optimism/Startup")]
	public static void ShowWindow()
	{
		//Show existing window instance. If one doesn't exist, make one.
		EditorWindow.GetWindow(typeof(GameFlowEditor));
	}

	void OnGUI()
	{
		GameFlow flow = GameFlow.Instance;
		
		GUILayout.Label ("Agent Instantiation Objects", EditorStyles.boldLabel);
		
		EditorGUILayout.BeginVertical();
		
		List<GameObject> newItems = new List<GameObject>();
		
		foreach(GameObject item in flow.AgentStartupItems)
		{
			EditorGUILayout.BeginHorizontal();
			GameObject newObject = EditorGUILayout.ObjectField(item, typeof(GameObject), false) as GameObject;
			EditorGUILayout.EndHorizontal();
			
			if(newObject != null)
			{
				newItems.Add(newObject);	
			}
		}
		
		EditorGUILayout.BeginHorizontal();
		GameObject blankObject = EditorGUILayout.ObjectField(null, typeof(GameObject), false) as GameObject;
		EditorGUILayout.EndHorizontal();
		
		if(blankObject != null)
		{
			newItems.Add(blankObject);	
		}
		
		flow.AgentStartupItems = newItems;
		
		// Admin
		
		
		GUILayout.Label ("Admin Instantiation Objects", EditorStyles.boldLabel);
		
		newItems = new List<GameObject>();
		
		foreach(GameObject item in flow.AdminStartupItems)
		{
			EditorGUILayout.BeginHorizontal();
			GameObject newObject = EditorGUILayout.ObjectField(item, typeof(GameObject), false) as GameObject;
			EditorGUILayout.EndHorizontal();
			
			if(newObject != null)
			{
				newItems.Add(newObject);	
			}
		}
		
		EditorGUILayout.BeginHorizontal();
		blankObject = EditorGUILayout.ObjectField(null, typeof(GameObject), false) as GameObject;
		EditorGUILayout.EndHorizontal();
		
		if(blankObject != null)
		{
			newItems.Add(blankObject);	
		}
		
		flow.AdminStartupItems = newItems;
		
		EditorGUILayout.EndVertical();
		
		GUILayout.Button("Add type");
		
		
	}
}
