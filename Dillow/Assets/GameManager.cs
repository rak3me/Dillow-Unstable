using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Security;

public struct SaveData {
	//TODO: Fungus data

	public Vector3 playerSpawnLocation;
	public string currentScene;
	public string targetScene;

	public Dictionary<int, bool> obtainedCollectibles;
	public Dictionary<int, bool> abilities;
	public List<GameObject> inventory;

	public SaveData (Vector3 playerSpawnLocation, string currentScene, string targetScene, Dictionary<int, bool> obtainedCollectibles, Dictionary<int, bool> abilities, List<GameObject> inventory) {
		this.playerSpawnLocation = playerSpawnLocation;
		this.currentScene = currentScene;
		this.targetScene = targetScene;
		this.obtainedCollectibles = obtainedCollectibles;
		this.abilities = abilities;
		this.inventory = inventory;
	}
}

public class GameManager : MonoBehaviour {
	public static GameManager instance;

	private static string dataSubpath = "/StreamingAssets/data.json";

	public static GameObject player;

	public static SaveData saveData;

	private void Awake () {
		if (null == instance) {
			instance = this;
			DontDestroyOnLoad(gameObject);
		} else {
			Destroy(gameObject);
		}

		player = GameObject.FindWithTag("Player");
		Load();
	}


	public static void Save () {
		string jsonData = JsonUtility.ToJson(saveData);
		string filePath = Application.dataPath + dataSubpath;
		File.WriteAllText(filePath, jsonData);
	}

	public static void Load () {
		string filePath = Application.dataPath + dataSubpath;

		if (File.Exists(filePath)) {
			string jsonData = File.ReadAllText(filePath);
			saveData = JsonUtility.FromJson<SaveData>(jsonData);
		} else {
			saveData = new SaveData();
		}
	}
}
