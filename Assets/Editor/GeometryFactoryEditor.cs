using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(GeometryFactory))]
public class GeometryFactoryEditor : Editor 
{
	
	
	public override void OnInspectorGUI() 
	{
		GeometryFactory myTarget = (GeometryFactory)target;
		
		GeometryFactory.GeometryType newType  = (GeometryFactory.GeometryType)EditorGUILayout.EnumPopup("Geometry type", myTarget.geometryType);
		bool newScaleValue = EditorGUILayout.Toggle("Scaled World-Space UVs", myTarget.ScaleUVs);
		
		float NewUVScale = myTarget.UVScale;
		if(newScaleValue)
		{
			NewUVScale = EditorGUILayout.FloatField(myTarget.UVScale);	
		}
		
		if(newType != myTarget.geometryType || newScaleValue != myTarget.ScaleUVs || NewUVScale != myTarget.UVScale)
		{
			myTarget.geometryType = newType;	
			myTarget.ScaleUVs = newScaleValue;
			myTarget.UVScale = NewUVScale;
			
			myTarget.RebuildMesh();
		}
		
		
		EditorUtility.SetDirty (myTarget);
	}
	
	
}
