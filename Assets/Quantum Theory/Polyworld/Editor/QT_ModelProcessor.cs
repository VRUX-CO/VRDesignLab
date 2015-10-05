using UnityEngine;
using System.Collections;
using UnityEditor;

public class QT_ModelProcessor : AssetPostprocessor 
{

	public static bool isFacetedMesh=false;

	void OnPreprocessModel()
	{
		ModelImporter im = (ModelImporter)assetImporter;
		//if(isFacetedMesh)
        if(im.assetPath.Contains("-Source"))  //this thing is flakey...
    		{

			im.normalImportMode = ModelImporterTangentSpaceMode.Calculate;
			im.normalSmoothingAngle = 0;
			im.animationType = ModelImporterAnimationType.None;
	    	}
		}


}
