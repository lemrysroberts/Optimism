using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameFlowWrapper))]
public class GameFlowWrapperEditor : Editor 
{
	public override void OnInspectorGUI() 
	{
		GameFlowWrapper myTarget = (GameFlowWrapper) target;
		
		EditorGUILayout.BeginHorizontal();
		
		EditorGUILayout.LabelField("Level Object");
		
		var newLevelObject = EditorGUILayout.ObjectField(myTarget.LevelObject, typeof(GameObject), true, null) as GameObject;
		myTarget.LevelObject = newLevelObject;
		
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.LabelField("Level: " + (myTarget.LevelToLoad == null ? "None" : myTarget.LevelToLoad));
		if(GUILayout.Button("Load Level File"))
		{
			var path = EditorUtility.OpenFilePanel("Load Level...", "levels", "xml");
			
			int dataIndex = path.IndexOf(Application.dataPath);
				
			if(dataIndex != -1)
			{
					path = path.Remove(dataIndex, Application.dataPath.Length);
					
					if(path[0] == '\\' || path[0] == '/')
					{
						path = path.Remove(0, 1);	
					}
				myTarget.LevelToLoad = path;
			}
			else
			{
				Debug.LogError("Path \"" + path + "\" is not within the assets directory");
			}
			
		}
	}
}
