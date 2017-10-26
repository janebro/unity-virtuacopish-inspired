using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

[System.Serializable]
public class SFB_ExportMaterialEditor : EditorWindow
{
	public static SFB_ExportMaterial exporterObject;															// The scriptable object that holds data
	public Vector2 scrolling;																					// Holds scrolling data for window
	public static int materialCounter = 1;																		// For keeping track of naming increments
	public static float progress = 0.0f;

	// This function will create a new scriptable object for users to set up and use.
	[MenuItem("Window/Substance Mass Exporter/Create Exporter Object")]											// Menu option
	public static void CreateExporterObject()
	{
		if(!Directory.Exists("Assets/SFBayStudios/Substance Mass Exporter/Export Texture Objects/")) {									// If the directory doesn't exist  
			Directory.CreateDirectory("Assets/SFBayStudios/Substance Mass Exporter/Export Texture Objects/");							// Create it
		}
		exporterObject = CreateObject ("Assets/SFBayStudios/Substance Mass Exporter/Export Texture Objects/SFB Texture Exporter");		// Create the object
		AssetDatabase.SaveAssets();																				// Save the asset
		Init ();																								// Run the initilization function
	}

	// Will create a new object file
	static SFB_ExportMaterial CreateObject(string newFile){
		SFB_ExportMaterial asset = ScriptableObject.CreateInstance<SFB_ExportMaterial>();						// create a new asset
		int x = 1;																								// Set counter to 1
		while (x < 999) {																						// Ensure no infinite loop
			// If the file (with counter number) doesn't exist...
			if (!File.Exists ("Assets/SFBayStudios/Substance Mass Exporter/Export Texture Objects/SFB Texture Exporter " + x + ".asset")) {
				// Create the asset file
				AssetDatabase.CreateAsset(asset, "Assets/SFBayStudios/Substance Mass Exporter/Export Texture Objects/SFB Texture Exporter " + x + ".asset");
				return asset;																					// return the file
			}
			x++;																								// Increase counter
		}
		// We went beyond 999, which means something is wrong since it's very very unlikely anyone will really have 999
		// objects of the same base name.
		Debug.LogError ("Oops!  You've made 999 objects with this name already...maybe you should rename some?  Or is something else going on??");
		return null;																							// Return null
	}


	// This doese the initizliation of the window
	[MenuItem("Window/Substance Mass Exporter/Load Texture Exporter Window")]
	static void Init()																							// On initialization
	{
		// Get existing open window or if none, make a new one:
		SFB_ExportMaterialEditor window = (SFB_ExportMaterialEditor)EditorWindow.GetWindow(typeof(SFB_ExportMaterialEditor));
		window.Show();																							// Show the window
	}

	void Update(){
		if (exporterObject) {
			for (int i = 0; i < exporterObject.materials.Count; i++) {
				if (!exporterObject.materials [i].loadingCacheClear) {
					Debug.Log(exporterObject.materials [i].materialName + " isn't cleared");
					ProceduralMaterial material = AssetDatabase.LoadAssetAtPath<ProceduralMaterial> (exporterObject.materials [i].substancePath);
					material.cacheSize = ProceduralCacheSize.None;
					material.ClearCache ();

					if (!material.isProcessing) {
						Debug.Log(exporterObject.materials [i].materialName + " Done!");
						exporterObject.materials [i].loadingCacheClear = true;
					} else {
						Debug.Log(exporterObject.materials [i].materialName + " processing...");
					}
				}
			}
		}

		if (exporterObject) {																					// If an object is attached
			EditorUtility.SetDirty (exporterObject);															// Set dirty
			EditorUtility.SetDirty (this);																		// Set dirty
		}
	}

	void OnGUI()
	{
		GUIStyle doNotSaveStyle = new GUIStyle(GUI.skin.label);													// Setup style
		doNotSaveStyle.fontStyle = FontStyle.Italic;															// Set Italics
		doNotSaveStyle.normal.textColor = Color.red;															// Set color
		GUIStyle saveStyle = new GUIStyle(GUI.skin.label);														// Setup style
		GUIStyle boldStyle = new GUIStyle(GUI.skin.label);														// Setup style
		boldStyle.fontStyle = FontStyle.Bold;																	// Set Bold

		if (!exporterObject) {
			EditorGUILayout.HelpBox ("Procedural Material Exporter Instructions\n\nSelect an \"Exporter Object\" from " +
				"your project using the field below, or create a new Object using the menu Window/SFBayStudios/Export " +
				"Textures/Create Exporter Object", MessageType.Info);
		} else {
			if (exporterObject.materials.Count == 0) {
				EditorGUILayout.HelpBox ("Procedural Material Exporter Instructions\n\nSelect at least one Procedural " +
				"Material (.sbsar) file.  You can drag and drop one into the \"Add Procedural Material\" field below, " +
				"or open the File dialog using that field and select all you'd like to include.  The field will reset " +
				"when you choose a file, so you can choose as many as you'd like without closing the dialog.  Close the " +
				"dialog window when you are done.\n\nAlternatively, select the .sbsar file(s) (either the Substance Archive [" +
				"rectangle] or the Material [circle] icon) you'd like to include, and use \"Window/SFBayStudios/Export " +
				"Textures/Include Selected Materials(s)\" to load all the materials you've selected.  The hot key command-shift-e" +
				" will accomplish the same thing.", MessageType.Info);
			} else {
				EditorGUILayout.HelpBox ("Procedural Material Exporter Instructions\n\nTurn on which textures you'd like " +
					"to keep.  If you have not turned on \"Generate All Outputs\", you may not see all options available. " +
					"Remember that non-standard textures are only available per-material if \"Generate All Outputs\" is " +
					"turned on, so you may wish to turn it on for all materials.\n\nWhen you are ready, click \"Export " +
					"Textures\" and the system will export all of your chosen textures and create Standard Shader " +
					"materials for you.", MessageType.Info);
			}
		}
		scrolling = GUILayout.BeginScrollView (scrolling);														// Save scrolling position
		// A field for users to choose or add an exporter Object
		exporterObject = EditorGUILayout.ObjectField ("Exporter Object:", exporterObject, typeof(SFB_ExportMaterial), false) as SFB_ExportMaterial;				

		if (exporterObject) {
			if (exporterObject.groupName == "") {																// if this is empty...
				exporterObject.groupName = "Group Name";														// Ensure this isn't empty
			}
			// Toggle for setting normal map mode
			exporterObject.setNormalMapMode = EditorGUILayout.Toggle ("Set Normal Map Mode?", exporterObject.setNormalMapMode);
			// Toggle for creating mateirals
			exporterObject.createMaterials = EditorGUILayout.Toggle ("Create Mateirals?", exporterObject.createMaterials);
			exporterObject.convertToPNG = EditorGUILayout.Toggle ("Convert to PNG?", exporterObject.convertToPNG);
			EditorGUILayout.Space ();																			// Add in some space

			if (GUILayout.Button("EditorUtility.UnloadUnusedAssets()")){
				for (int i = 0; i < exporterObject.materials.Count; i++) {
					ProceduralMaterial material = AssetDatabase.LoadAssetAtPath<ProceduralMaterial>(exporterObject.materials [i].substancePath);
					Debug.Log ("Before: " + material.cacheSize);
					material.cacheSize = ProceduralCacheSize.None;
					material.ClearCache();
					Debug.Log ("After: " + material.cacheSize);
				}
				EditorUtility.UnloadUnusedAssets ();
			}
			exporterObject.groupName = EditorGUILayout.TextField ("Group Name:", exporterObject.groupName);		// Name of the group
			EditorGUILayout.Space ();																			// Add in some space

			// Foldout for showing materials
			exporterObject.showMaterials = EditorGUILayout.Foldout(exporterObject.showMaterials, "Selected Materials (" + exporterObject.materials.Count + ")");
			if (exporterObject.showMaterials) {
				EditorGUI.indentLevel++;																		// Increase the indent level

				// Display an object field for new materials.
				exporterObject.newMaterial = EditorGUILayout.ObjectField ("Add Procedural Material: ", exporterObject.newMaterial, typeof(ProceduralMaterial), false) as ProceduralMaterial;
				if (exporterObject.newMaterial) {
					//Debug.Log ("exporterObject.newMaterial: " + exporterObject.newMaterial.cacheSize);

					string newPath = AssetDatabase.GetAssetPath(exporterObject.newMaterial.GetInstanceID());	// Get the path
					SubstanceImporter newImporter = AssetImporter.GetAtPath (newPath) as SubstanceImporter;		// Get substance Importer
					if (!IsInMaterialList (exporterObject.newMaterial as ProceduralMaterial)) {					// If it's not already in the list...
						// Create a new SFB_MaterialExports object
						bool generateAllOutputs = newImporter.GetGenerateAllOutputs(exporterObject.newMaterial as ProceduralMaterial);

						SFB_MaterialExports newMaterial = new SFB_MaterialExports(exporterObject.newMaterial.name, newPath, generateAllOutputs);
						exporterObject.materials.Add (newMaterial);												// Add it to the list

					}
					//exporterObject.newMaterial.ClearCache ();
					//ProceduralMaterial.StopRebuilds ();

					LoadTextureNames();
					exporterObject.newMaterial.cacheSize = ProceduralCacheSize.None;
					exporterObject.newMaterial.ClearCache();
					exporterObject.newMaterial = null;															// Set newMaterial to null, ready to accept more.
				}

				for (int i = 0; i < exporterObject.materials.Count; i++) {										// For each material in the list
					EditorGUILayout.BeginHorizontal ();															// Begin a horizontal Row
					EditorGUILayout.LabelField (exporterObject.materials [i].materialName);					// Print the name
					// Print a toggle with the "Generate All Outputs" option.  Will update w/ toggle.

					if (!exporterObject.materials [i].generateAllOutputs) {
						if (GUILayout.Button("Set Generate All Outputs")){
							SubstanceImporter newImporter = AssetImporter.GetAtPath (exporterObject.materials [i].substancePath) as SubstanceImporter;		// Get substance Importer

							ProceduralMaterial material = AssetDatabase.LoadAssetAtPath<ProceduralMaterial>(exporterObject.materials [i].substancePath);

							newImporter.SetGenerateAllOutputs (material, true);
							exporterObject.materials[i].generateAllOutputs = true;
							material.cacheSize = ProceduralCacheSize.None;
							material.ClearCache();

						}
					}

					if (GUILayout.Button ("Reload Outputs")) {
						LoadTextureNames();
					}
					if (GUILayout.Button ("Remove")) {															// Show a "Remove" button, if clicked...
						exporterObject.materials.RemoveAt (i);													// Remove that material
					}
					EditorGUILayout.EndHorizontal ();															// End the horizontal Row
				}
				if (!GenerateAllOutputs ()) {																	// If not all materials have generate all outputs set to true...
					EditorGUILayout.HelpBox ("Warning:  Not all materials have \"Generate All Outputs\" " +
						"selected.  This may reduce the texture options available, and some materials " +
						"may not export properly without this toggle selected.", MessageType.Warning);			// Show an instruction box
					if (GUILayout.Button("Set Generate All Outputs on All Materials")){							// If a button is clicked...
						SetGenerateAllOutputs();																// Run method
					}
				}
				EditorGUI.indentLevel--;																		// Reduce the indent level
			}

			if (exporterObject.materials.Count > 0) {															// If we have at least one material
				// Show a foldout for the "Texture Outputs" section
				exporterObject.showTextures = EditorGUILayout.Foldout (exporterObject.showTextures, "Texture Outputs (" + TextureOutputCount () + " of " + exporterObject.textureNames.Count + ")");
				if (exporterObject.showTextures) {																// If foldout is set to true...
					EditorGUI.indentLevel++;																	// Increase the indent level
					for (int t = 0; t < exporterObject.textureNames.Count; t++) {								// For each texture name...
						EditorGUILayout.BeginHorizontal ();														// Begin a horizontal row
						// display a toggle, and pass the value to the EditorPrefs
						EditorPrefs.SetBool (exporterObject.textureNames [t] + "_SaveOnExport", EditorGUILayout.Toggle(EditorPrefs.GetBool (exporterObject.textureNames [t] + "_SaveOnExport"), GUILayout.Width(100)));
						EditorGUILayout.LabelField (exporterObject.textureNames [t], saveStyle);				// Show the name in normal style
						EditorGUILayout.EndHorizontal ();														// End the horizontal Row
					}
					EditorGUI.indentLevel--;																	// Reduce the indent level
				}

				// Show a foldout for the "Reuse Textures" section
				exporterObject.showReuse = EditorGUILayout.Foldout (exporterObject.showReuse, "Reuse Textures (" + ReuseCount () + " of " + exporterObject.textureNames.Count + ")");

				if (exporterObject.showReuse) {																// If foldout is set to true...
					EditorGUI.indentLevel++;																	// Increase the indent level
					EditorGUILayout.HelpBox ("Any texture selected here will, if possible, use the first exported texture " +
						"from this material.  This is good for when you want to make multiple materials with " +
						"different diffuse (like color changes) textures, but you want to share the other maps, which " +
						"haven't changed.", MessageType.None);			// Show an instruction box
					for (int t = 0; t < exporterObject.textureNames.Count; t++) {								// For each texture name...
						EditorGUILayout.BeginHorizontal ();														// Begin a horizontal row
						// display a toggle, and pass the value to the EditorPrefs
						EditorPrefs.SetBool (exporterObject.textureNames [t] + "_ReuseFirst", EditorGUILayout.Toggle(EditorPrefs.GetBool (exporterObject.textureNames [t] + "_ReuseFirst"), GUILayout.Width(100)));
						EditorGUILayout.LabelField (exporterObject.textureNames [t], saveStyle);				// Show the name in normal style
						EditorGUILayout.EndHorizontal ();														// End the horizontal Row
					}
					EditorGUI.indentLevel--;																	// Reduce the indent level
				}

				if (TextureOutputCount () > 0) {																// If we are going to save at least one texture type
					if (GUILayout.Button ("Export Textures")) {													// If we press the button...
						ExportTextures ();																		// Run the function
					}
				}
			}
		}
		GUILayout.EndScrollView ();																				// End scroll view
	}

	// This function exports all the textures.
	static void ExportTextures(){
		string pBarString = "Starting Texture Exporting Process";												// Set the progress bar text
		progress = 0.0f;																				// Set the progress bar percent
		EditorUtility.DisplayProgressBar("Texture Exporter", pBarString, progress);							// Start the progress bar at 0

		for (int i = 0; i < exporterObject.materials.Count; i++) {												// For each material in the list
			pBarString = "(Material " + (i + 1) + " of " + exporterObject.materials.Count + ") " +
				"Starting " + exporterObject.materials[i].materialName;										// Set String
			if (i != 0)
				progress		= (float)i / (float)exporterObject.materials.Count;								// Computer percent
			EditorUtility.DisplayProgressBar("Texture Exporter", pBarString, progress);					// Update progress bar
			string materialName = GetMaterialName(i);															// Set name
			ExportSubstance(i, materialName);																	// Export the material
			RenameAndRemove(i, materialName, exporterObject.materials[i].materialName);						// Rename textures & remove unwanted ones

			if (exporterObject.createMaterials) {																// If we are creating materials...
				CreateMaterials (i, materialName, exporterObject.materials [i].materialName);					// Create mateirials
			}
		}

		AssetDatabase.Refresh();																				// Refresh the asset database
		EditorUtility.ClearProgressBar();																		// Clear the progress bar
	}

	public static void SetTextureImporterFormat( Texture2D texture, bool isReadable)
	{
		if ( null == texture ) return;

		string assetPath = AssetDatabase.GetAssetPath( texture );
		var tImporter = AssetImporter.GetAtPath( assetPath ) as TextureImporter;
		if ( tImporter != null )
		{
			tImporter.textureType = TextureImporterType.Default;

			tImporter.isReadable = isReadable;

			AssetDatabase.ImportAsset( assetPath );
			//AssetDatabase.Refresh();
		}
	}

	/// <summary>
	///  Thanks to IMD @ the Unity Forums:  http://answers.unity3d.com/questions/432655/loading-texture-file-from-pngjpg-file-on-disk.html
	/// </summary>
	/// <param name="filepath">Filepath.</param>
	static void ConvertToPNG(string filepath){
		EditorUtility.DisplayProgressBar("Converting To PNG", filepath, progress);					// Update progress bar
		AssetDatabase.ImportAsset (filepath);
		Texture originalTexture = AssetDatabase.LoadAssetAtPath (filepath, typeof(Texture)) as Texture;
		string newFilePath = filepath.Replace (".tga", ".png");
		/*
		Texture2D tex = new Texture2D(originalTexture.width, originalTexture.height, TextureFormat.RGBA32, false);
		byte[] fileData = File.ReadAllBytes(filepath);
		//tex.LoadRawTextureData(fileData);
		tex.LoadImage (fileData);
	
		

		byte[] bytes = tex.EncodeToPNG ();
		Debug.Log ("Length: " + fileData.Length + " / " + bytes.Length);
*/


		Texture2D savedTexture = originalTexture as Texture2D;
		SetTextureImporterFormat (savedTexture, true);
		Texture2D newTexture = new Texture2D(savedTexture.width, savedTexture.height, TextureFormat.ARGB32, false);

		newTexture.SetPixels(0,0, savedTexture.width, savedTexture.height, savedTexture.GetPixels());
		//newTexture = FillInClear(newTexture);
		newTexture.Apply();
		byte[] bytes = newTexture.EncodeToPNG();




		File.WriteAllBytes (newFilePath, bytes);
		AssetDatabase.ImportAsset( newFilePath );
		AssetDatabase.DeleteAsset(filepath);

		//byte[] bytes = tex.EncodeToPNG ();
		//Texture2D originalTexture2D = originalTexture as Texture2D;
		//Color32[] pix = tex.GetPixels32();

		//tex.SetPixels32(pix);
		/*
		Texture2D tex = null;
		byte[] fileData;

		if (File.Exists(filepath)) {
			fileData = File.ReadAllBytes(filepath);
			Debug.Log ("fileData.Length: " + fileData.Length);
			tex = new Texture2D(2, 2);
			if (tex.LoadImage (fileData)) {
				byte[] bytes = tex.EncodeToPNG ();

				Debug.Log ("bytes.Length: " + bytes.Length);
				File.WriteAllBytes (newFilePath, bytes);
				System.IO.File.Delete (filepath);
			} else {
				Debug.Log ("Filepath: " + filepath + " could not be loaded");
			}
		}

*/

	}

	// returns true if at least one texture output will be saved
	static int TextureOutputCount(){
		int count = 0;																						// Set Variable
		for (int t = 0; t < exporterObject.textureNames.Count; t++) {										// For each texture name...
			if (EditorPrefs.GetBool (exporterObject.textureNames[t] + "_SaveOnExport")) {					// If we are set to save this on export...
				count++;																					// Add to count
			}
		}
		return count;																						// Return count
	}

	// returns true if at least one texture output will be saved
	static int ReuseCount(){
		int count = 0;																						// Set Variable
		for (int t = 0; t < exporterObject.textureNames.Count; t++) {										// For each texture name...
			if (EditorPrefs.GetBool (exporterObject.textureNames[t] + "_ReuseFirst")) {						// If we are set to save this on export...
				count++;																					// Add to count
			}
		}
		return count;																						// Return count
	}

	// Will load all possible texture names from the mateirals included.
	static void LoadTextureNames(){
		//Debug.Log ("LoadTextureNames()");
		exporterObject.textureNames.Clear ();																// Clear the list
		for (int i = 0; i < exporterObject.materials.Count; i++) {											// For each material
			ProceduralMaterial material = AssetDatabase.LoadAssetAtPath<ProceduralMaterial>(exporterObject.materials [i].substancePath);
			Texture[] textures = material.GetGeneratedTextures ();				// Get array of all textures generated
			foreach (Texture texture in textures) {															// For each Texture...
				string[] nameArray = texture.name.Split("_"[0]);											// Split the name into strings
				string typeName = nameArray[nameArray.Length - 1];											// Get last array entry
				if (!IsInTextureList (typeName)) {															// If the name isn't already there...
					exporterObject.textureNames.Add (typeName);												// Add to the list
				}
			}
			material.cacheSize = ProceduralCacheSize.None;
			material.ClearCache();
		}
	}

	// Will return true if the textureName is already in the list
	static bool IsInTextureList(string textureName){
		for (int i = 0; i < exporterObject.textureNames.Count; i++){										// For each textureNames
			if (exporterObject.textureNames [i] == textureName) {											// If the name matches
				return true;																				// Return true
			}
		}
		return false;																						// Return false
	}


	// returns false if at least one output is not set to all
	static bool GenerateAllOutputs(){
		for (int i = 0; i < exporterObject.materials.Count; i++) {											// For each material
			// If the output is not set to all
			//SubstanceImporter newImporter = AssetImporter.GetAtPath (exporterObject.materials [i].substancePath) as SubstanceImporter;		// Get substance Importer

			//if (!newImporter.GetGenerateAllOutputs (exporterObject.materials [i].material)) {
			if (!exporterObject.materials[i].generateAllOutputs)
				return false;																				// return false
			//}
		}
		return true;																						// return true
	}

	// This will allow users to use a hot key or menu item to include selected materials, multiple as well
	// to the list.  Will work on the main Substnace Archive (rectangle icon) and material (circle icon)
	[MenuItem("Window/Substance Mass Exporter/Include Selected Material(s) %#e")]						// Add menu to the Window menu w/ Hotkey
	static void SelectProceduralMaterials(){
		if (exporterObject){
			foreach (Object selectedObject in Selection.objects) {											// For each selected object...
				bool generateAllOutputs = false;
				string newPath = "";																		// Set variable
				SubstanceImporter newImporter;																// Set variable
				if (selectedObject.GetType () == typeof(SubstanceArchive)) {								// If it's a substance Archive...
					newPath = AssetDatabase.GetAssetPath(selectedObject.GetInstanceID());					// Get path
					newImporter = AssetImporter.GetAtPath (newPath) as SubstanceImporter;					// Get substance Importer
					ProceduralMaterial[] newMaterials = newImporter.GetMaterials ();						// Array of mateirals
					for (int i = 0; i < newMaterials.Length; i++) {											// For each material
						if (!IsInMaterialList (newMaterials[i])) {											// If it's not already in the list...
							Debug.Log("CacheSize: " + newMaterials[i].cacheSize);

							// Create a new SFB_MaterialExports object
							generateAllOutputs = newImporter.GetGenerateAllOutputs(newMaterials[i]);
							SFB_MaterialExports newMaterial = new SFB_MaterialExports(newMaterials[i].name, newPath, generateAllOutputs);
							exporterObject.materials.Add (newMaterial);										// Add it to the list
							//newMaterials[i].ClearCache ();
							newMaterials[i].cacheSize = ProceduralCacheSize.None;
						}
						newMaterials[i].cacheSize = ProceduralCacheSize.None;
						newMaterials[i].ClearCache();
					}
				}
				if (selectedObject.GetType () == typeof(ProceduralMaterial)) {								// If it's a procedural material...
					newPath = AssetDatabase.GetAssetPath(selectedObject.GetInstanceID());					// Get the path
					newImporter = AssetImporter.GetAtPath (newPath) as SubstanceImporter;					// Get substance Importer
					if (!IsInMaterialList (selectedObject as ProceduralMaterial)) {							// If it's not already in the list...
						// Create a new SFB_MaterialExports object
						generateAllOutputs = newImporter.GetGenerateAllOutputs(selectedObject as ProceduralMaterial);
						SFB_MaterialExports newMaterial = new SFB_MaterialExports(selectedObject.name, newPath, generateAllOutputs);
						exporterObject.materials.Add (newMaterial);											// Add it to the list
						ProceduralMaterial pObject = selectedObject as ProceduralMaterial;
						pObject.cacheSize = ProceduralCacheSize.None;
						pObject.ClearCache();
					}

				}
				//exporterObject.materials[exporterObject.materials.Count - 1].material.ClearCache ();
			}
			LoadTextureNames ();																			// Load all texture names
			Init ();																						// Initialize window
			EditorUtility.UnloadUnusedAssets ();

		}
	}

	// This will check to see if the value material is already in our materials list.
	static bool IsInMaterialList(ProceduralMaterial value){	
		for (int i = 0; i < exporterObject.materials.Count; i++) {											// For each material in our list
			if (exporterObject.materials [i].materialName == value.name) {											// if the value matches
				return true;																				// return true
			}
		}
		return false;																						// return false
	}

	// This function sets all materaisl to "Generate All Outputs" = true.  It takes a while, so a progress bar is shown.
	static void SetGenerateAllOutputs(){
		for (int i = 0; i < exporterObject.materials.Count; i++) {											// For each material
			progress = ((float)i / (float)exporterObject.materials.Count);							// Compute progress %
			// Display a progress bar
			EditorUtility.DisplayProgressBar("Setting Generate All Outputs...", "Setting Value for " + exporterObject.materials[i].materialName, progress);
			SubstanceImporter newImporter = AssetImporter.GetAtPath (exporterObject.materials [i].substancePath) as SubstanceImporter;		// Get substance Importer
			// If the output is not set to all

			ProceduralMaterial material = AssetDatabase.LoadAssetAtPath<ProceduralMaterial>(exporterObject.materials [i].substancePath);


			if (!newImporter.GetGenerateAllOutputs (material)) {
				// Set to true
				newImporter.SetGenerateAllOutputs (material, true);
			}
			material.cacheSize = ProceduralCacheSize.None;
			material.ClearCache();
			EditorUtility.UnloadUnusedAssets ();
		}
		LoadTextureNames ();																				// Load all texture names
		EditorUtility.ClearProgressBar ();																	// Clear the progress bar
	}

	// This will get the material name, adding a # value at the end if needed
	static string GetMaterialName(int id){
		if(!Directory.Exists("Assets/SFBayStudios/Exported Materials/" + exporterObject.groupName)) {		// If the directory doesn't exist  
			Directory.CreateDirectory("Assets/SFBayStudios/Exported Materials/" + exporterObject.groupName);// Create it
		}

		ProceduralMaterial material = AssetDatabase.LoadAssetAtPath<ProceduralMaterial>(exporterObject.materials [id].substancePath);

		string materialName = material.name;									// Set initial name
		int x = 1;																							// Set counter
		while (x < 999) {																					// loop
			materialCounter = x;																			// Keep track of this
			string checkName = materialName;																// Set variable
			if (x > 1){
				checkName = materialName + "_" + x;															// Add number
			}
			// If the texture directory for this name doesn't exist...
			if(!Directory.Exists("Assets/SFBayStudios/Exported Materials/" + exporterObject.groupName + "/tex_" + checkName)) {
				// Create the directory for textures
				Directory.CreateDirectory("Assets/SFBayStudios/Exported Materials/" + exporterObject.groupName + "/tex_" + checkName);
				return checkName;																			// Return the checked name
			}
			x++;																							// add one
		}

		return materialName;																				// Return material name
	}

	// This is the heavy lifting for exporting the substnace textures
	static void ExportSubstance(int id, string materialName){
		// Set full path for textures
		SubstanceImporter newImporter = AssetImporter.GetAtPath (exporterObject.materials [id].substancePath) as SubstanceImporter;		// Get substance Importer
		string path = "Assets/SFBayStudios/Exported Materials/" + exporterObject.groupName + "/tex_" + materialName + "/";
		ProceduralMaterial material = AssetDatabase.LoadAssetAtPath<ProceduralMaterial>(exporterObject.materials [id].substancePath);
		material.cacheSize = ProceduralCacheSize.None;
		material.ClearCache();
		// Export the bitmaps
		newImporter.ExportBitmaps(material, path, true);	

	}

	// This will remove all unwanted files and rename those we are keeping
	static void RenameAndRemove(int id, string materialName, string originalName){
		// Set full path for textures
		string path = "Assets/SFBayStudios/Exported Materials/" + exporterObject.groupName + "/tex_" + materialName + "/";
		ProceduralMaterial material = AssetDatabase.LoadAssetAtPath<ProceduralMaterial>(exporterObject.materials [id].substancePath);

		Texture[] textures = material.GetGeneratedTextures ();				// Get array of all textures generated
		foreach (Texture texture in textures) {															// For each Texture...
			string[] nameArray = texture.name.Split("_"[0]);											// Split the name into strings
			string typeName = nameArray[nameArray.Length - 1];											// Get last array entry
			string newTexturePath = path + "" + texture.name + ".tga";									// set variable

			string pngPath = newTexturePath.Replace (".tga", ".png");

			bool canRemove = false;																		// set variable
			if (EditorPrefs.GetBool (typeName + "_ReuseFirst")) {										// if we want to reuse the first version...
				// Compute original path
				string originalPath = "Assets/SFBayStudios/Exported Materials/" + exporterObject.groupName + "/tex_" + originalName + "/";
				if (File.Exists (originalPath + "" + texture.name + ".tga") && materialCounter != 1) {	// If we found the first file...
					canRemove = true;																	// Set true
				}
			}
			bool deleted = false;
			if (!EditorPrefs.GetBool (typeName + "_SaveOnExport") || canRemove) {						// If we are not saving this type...
				System.IO.File.Delete(path + "" + texture.name + ".tga");								// Delete it
				deleted = true;
			} else if (materialCounter > 1){															// Else if materialCounter is greater than 1...
				string newName = texture.name + "_" + materialCounter;									// Compute new name
				System.IO.File.Move(path + "" + texture.name + ".tga", path + "" + newName + ".tga");
				newTexturePath = path + "" + newName + ".tga";											// set new path

			}
			if (!deleted && exporterObject.convertToPNG) {
				ConvertToPNG (newTexturePath);														// Convert this file to png
			}
			if (exporterObject.setNormalMapMode && typeName == "normal")								// If we are setting the normal map mode
			{
				string newPngPath = newTexturePath.Replace (".tga", ".png");
				if (File.Exists (newTexturePath)) {
					EditorUtility.DisplayProgressBar("Texture Exporter", "Setting Normal Map Mode", progress);	
					AssetDatabase.ImportAsset (newTexturePath);												// import asset
					ProceduralTexture proceduralTexture = texture as ProceduralTexture;						// Get procedural texture
					// If this is a normal map...
					if (proceduralTexture.GetProceduralOutputType () == ProceduralOutputType.Normal) {
						// Get textureImporter
						TextureImporter textureImporter = AssetImporter.GetAtPath (newTexturePath) as TextureImporter;
						textureImporter.textureType = TextureImporterType.NormalMap;						// Set normal map mode
						AssetDatabase.ImportAsset (newTexturePath);											// Reimport asset
					}
				}
				else {
					AssetDatabase.ImportAsset( newPngPath );
					if (File.Exists (newPngPath)) {
						EditorUtility.DisplayProgressBar("Texture Exporter", "Setting Normal Map Mode", progress);	
						AssetDatabase.ImportAsset (newPngPath);												// import asset
						ProceduralTexture proceduralTexture = texture as ProceduralTexture;						// Get procedural texture
						// If this is a normal map...
						if (proceduralTexture.GetProceduralOutputType () == ProceduralOutputType.Normal) {
							// Get textureImporter
							TextureImporter textureImporter = AssetImporter.GetAtPath (newPngPath) as TextureImporter;
							textureImporter.textureType = TextureImporterType.NormalMap;						// Set normal map mode
							AssetDatabase.ImportAsset (newPngPath);											// Reimport asset
						}
					}
				}
			}
		}
		material.cacheSize = ProceduralCacheSize.None;
		material.ClearCache();
	}

	// This will create mateirals
	static void CreateMaterials(int id, string materialName, string originalName){
		
		// Set full path for textures
		string path = "Assets/SFBayStudios/Exported Materials/" + exporterObject.groupName + "/" + materialName + ".mat";
		ProceduralMaterial material = AssetDatabase.LoadAssetAtPath<ProceduralMaterial>(exporterObject.materials [id].substancePath);

		Material newMaterial = new Material (material.shader);				// Create a new material of the same shader as the source
		newMaterial.CopyPropertiesFromMaterial(material);					// Copy properties from source material
		AssetDatabase.CreateAsset(newMaterial, path);													// Create the asset
		AssetDatabase.Refresh();																		// Refresh asset list

		int propertyCount = ShaderUtil.GetPropertyCount (newMaterial.shader);							// Get property count of shader
		for (int i = 0; i < propertyCount; i++) {														// For each property
			if (ShaderUtil.GetPropertyType (newMaterial.shader, i) == ShaderUtil.ShaderPropertyType.TexEnv) {
				string propertyName = ShaderUtil.GetPropertyName (newMaterial.shader, i);
				if (newMaterial.GetTexture (propertyName) != null) {									// If the texture had this map populated...
					string[] nameArray = newMaterial.GetTexture (propertyName).name.Split ("_" [0]);		// Split the name into strings
					string typeName = nameArray [nameArray.Length - 1];									// Get last array entry
					bool foundTexture = false;															// Set variable
					if (EditorPrefs.GetBool (typeName + "_ReuseFirst") && materialCounter != 1) {		// If we are reusing the texture & it's not the first one...
						// Compute original path
						string originalPath = "Assets/SFBayStudios/Exported Materials/" + exporterObject.groupName + "/tex_" + originalName + "/";
						string fileName = originalPath + "" + originalName + "_" + typeName + ".tga";	// Start w/ .tga
						if (!File.Exists (fileName)) {													// If we DID NOT find a .tga file...
							fileName = originalPath + "" + originalName + "_" + typeName + ".png";		// Try .png then...
						}
						if (File.Exists (fileName)) {													// If we found the first file...
							Texture originalTexture = AssetDatabase.LoadAssetAtPath (fileName, typeof(Texture)) as Texture;
							newMaterial.SetTexture (propertyName, originalTexture);						// Set Texture
							foundTexture = true;														// Mark as true
						}
					}
					if (!foundTexture) {																// If we haven't found the texture yet...
						string texturePath = "Assets/SFBayStudios/Exported Materials/" + exporterObject.groupName + "/tex_" + materialName + "/";
						// If we found the latest version of the file...

						string filePath = texturePath + "" + originalName + "_" + typeName + ".tga";	// Set default to .tga
						if (materialCounter > 1) {														// if this isn't the first
							filePath = texturePath + "" + originalName + "_" + typeName + "_" + materialCounter + ".tga";
							if (!File.Exists (filePath)) {												// If we can't find the .tga file
								// try .png
								filePath = texturePath + "" + originalName + "_" + typeName + "_" + materialCounter + ".png";
							}
						} else {																		// If it's the first
							if (!File.Exists (filePath)) {												// If we can't find the .tga file
								filePath = texturePath + "" + originalName + "_" + typeName + ".png";	// we will try .png
							}
						}
						if (File.Exists (filePath)) {	
							Texture newTexture = AssetDatabase.LoadAssetAtPath (filePath, typeof(Texture)) as Texture;
							newMaterial.SetTexture (propertyName, newTexture);							// Set Texture
							foundTexture = true;														// Mark as true
						}
					}
					if (!foundTexture) {
						newMaterial.SetTexture (propertyName, null);									// Set Texture as empty
						if (propertyName == "_EmissionMap") {											// If it's the emission color	
							newMaterial.SetColor ("_EmissionColor", Color.black);						// Set to black
						}
					}
				}
			}
		}
		material.cacheSize = ProceduralCacheSize.None;
		material.ClearCache();
	}
}

