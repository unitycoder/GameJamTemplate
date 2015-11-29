using UnityEngine;
using System.Collections;

public class LoadLevel : MonoBehaviour 
{

	public bool useAnyKey = false; // if enabled, pressing anykey loads next level
	public bool erasePlayerPrefs = false; // if enabled, erases all player prefs from this game

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Application.Quit();
		}

		if (useAnyKey && Input.anyKeyDown) Load(Application.loadedLevel+1);
	}

	public void LoadNextLevel()
	{
		Load (Application.loadedLevel + 1);
	}

	public void Load(int level)
	{
		if (erasePlayerPrefs)
		{
			PlayerPrefs.DeleteAll();
			PlayerPrefs.Save();
		}

		// NOTE: we dont check if next level actually exists in list, could check with Application.levelCount
		Application.LoadLevel(level);
	}
}
