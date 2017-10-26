using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class SFB_ExportMaterial : ScriptableObject
{
	public string groupName = "Group Name";																// Group name, i.e. Group Name/Material Name
	public List<SFB_MaterialExports> materials = new List<SFB_MaterialExports> ();						// .sbsar materials we have selected
	public List<string> textureNames = new List<string>();												// Names of textures available

	public bool showMaterials = true;																	// Are we showing materials?
	public bool showTextures = true;																	// Are we showing the textures?
	public bool showReuse = true;																		// Are we showing the reuse area?
	public bool setNormalMapMode = true;																// Are we setting the normal map mode?
	public bool createMaterials = true;																	// Are we creating materials too?
	public bool convertToPNG = true;																	// Should we convert these files to .png?

	public ProceduralMaterial newMaterial;																// For dragging mateirals into the list
}
