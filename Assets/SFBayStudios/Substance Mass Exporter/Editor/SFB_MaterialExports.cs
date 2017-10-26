using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class SFB_MaterialExports
{
	//public ProceduralMaterial material;
	public string materialName;
	public string substancePath;
	//public SubstanceImporter substanceImporter;
	public bool generateAllOutputs;
	public bool loadingCacheClear = false;

	public SFB_MaterialExports(string newMaterialName, string newSubstancePath, bool newGenerateAllOutputs)
	{
		//material = newMaterial;
		materialName = newMaterialName;
		substancePath = newSubstancePath;
		generateAllOutputs = newGenerateAllOutputs;
		//substanceImporter = newSubstanceImporter;
	}


}