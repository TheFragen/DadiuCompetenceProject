using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class CreateGenericScriptableObject
{
	public static void CreateAsset<T>() where T : ScriptableObject
	{
		T asset = ScriptableObject.CreateInstance<T>();

		string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(
			"Assets/ScriptableObjects/" + typeof(T).ToString() + "_ScriptableObject.asset");

		AssetDatabase.CreateAsset(asset, assetPathAndName);

		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
		EditorUtility.FocusProjectWindow();
		Selection.activeObject = asset;
	}
}

public class CreateScriptableObject
{
	[MenuItem("Assets/Create/ScriptableObjects/Items")]
	public static void CreateItemScriptableObject()
	{
		CreateGenericScriptableObject.CreateAsset<Items>();
	}
}