using UnityEngine;
using UnityEditor;
using System.IO;

public class PuzzleGridEditorHelper : Editor {

	public TextAsset ConvertStringToTextAsset ( string text, string dataPath, string fileName ) {

         File.WriteAllText(dataPath, text);
         AssetDatabase.SaveAssets();
         AssetDatabase.Refresh();
         TextAsset textAsset = Resources.Load(fileName) as TextAsset;
         return textAsset;
    }
}
