using UnityEngine;
using System.Collections;
using UnityEditor;

public class SFB_SetTextureSlots : MonoBehaviour {

	[MenuItem ("Window/SFBayStudios/Clear Progress Bar")]
	static void ClearProgressBar() {
		EditorUtility.ClearProgressBar ();
	}

	[MenuItem ("Window/SFBayStudios/Populate Exported Material Textures")]
	static void PopulateExportedMaterialTextures() {
		float i = 0.0f;
		EditorUtility.DisplayProgressBar ("Populating Materials", "Starting Population", i);
		foreach (Object selectedObject in Selection.objects){
			i++;
			float progress = i / Selection.objects.Length;
			EditorUtility.DisplayProgressBar ("Populating Materials", "Working on " + selectedObject.name, progress);
			if (selectedObject.GetType () == typeof(Material)) {
				Material selectedMaterial = selectedObject as Material;

				string path = AssetDatabase.GetAssetPath (selectedObject);
				path = path.Replace (selectedObject.name + ".mat", "");
				path = path + "tex_" + selectedObject.name;

				string alebedoOpacity = path + "/" + selectedObject.name + "_albedoOpacity.png";
				string ambientOcclusion = path + "/" + selectedObject.name + "_ambientOcclusion.png";
				string metallicRoughness = path + "/" + selectedObject.name + "_metallicRoughness.png";
				string height = path + "/" + selectedObject.name + "_height.png";
				string normal = path + "/" + selectedObject.name + "_normal.png";

				Object[] alebedoOpacityObj = AssetDatabase.LoadAllAssetsAtPath( alebedoOpacity );
				Object[] normalObj = AssetDatabase.LoadAllAssetsAtPath( normal );
				Object[] metallicRoughnessObj = AssetDatabase.LoadAllAssetsAtPath( metallicRoughness );
				Object[] ambientOcclusionObj = AssetDatabase.LoadAllAssetsAtPath( ambientOcclusion );
				Object[] heightObj = AssetDatabase.LoadAllAssetsAtPath( height );

				if (alebedoOpacityObj.Length > 0) {
					Texture tex_albedoOpacity = alebedoOpacityObj [0] as Texture;
					selectedMaterial.SetTexture ("_MainTex", tex_albedoOpacity);
				}
				if (normalObj.Length > 0) {
					Texture tex_normal = normalObj [0] as Texture;
					selectedMaterial.SetTexture ("_BumpMap", tex_normal);
					TextureImporter textureImporter = AssetImporter.GetAtPath (normal) as TextureImporter;
					textureImporter.textureType = TextureImporterType.NormalMap;
					AssetDatabase.ImportAsset (normal);
				}
				if (metallicRoughnessObj.Length > 0) {
					Texture tex_metallicRoughness = metallicRoughnessObj [0] as Texture;
					selectedMaterial.SetTexture ("_MetallicGlossMap", tex_metallicRoughness);
				}
				if (ambientOcclusionObj.Length > 0) {
					Texture tex_ambientOcclusion = ambientOcclusionObj [0] as Texture;
					selectedMaterial.SetTexture ("_OcclusionMap", tex_ambientOcclusion);
				}
				if (heightObj.Length > 0) {
					Texture tex_height = heightObj [0] as Texture;
					selectedMaterial.SetTexture ("_ParallaxMap", tex_height);
				}

				AssetDatabase.ImportAsset (AssetDatabase.GetAssetPath (selectedObject));
			}
		}
		EditorUtility.ClearProgressBar ();
	}
}