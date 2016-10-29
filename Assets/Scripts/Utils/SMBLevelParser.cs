using UnityEngine;
using System.Collections;

public class SMBLevelParser {

	public static int[,] Parse(string filename) {

		int width = 0;
		int height = 0;

		TextAsset levelfile = Resources.Load<TextAsset> (filename);

		if (!levelfile) {
			Debug.LogError ("Could not read file");
			return null;
		}
			
		string text = levelfile.text.Remove (levelfile.text.Length - 1);
		string[] lines = text.Split ('\n');

		width = lines [0].Length;
		height = lines.Length;

		foreach (string line in lines) {

			if (line.Length != width) {
				Debug.LogError ("Level row has different width, they all have to have the same amount of tiles!");
				return null;
			}
		}
			
		int[,] tileMap = new int[height, width] ;

		for (int i = 0; i < height; i++) {

			char[] row = lines [i].ToCharArray ();

			for (int j = 0; j < width-1; j++) {

				string tileValue = row[j].ToString();
				tileMap[i,j] = int.Parse(tileValue);
			}
		}

		return tileMap;
	}
}


