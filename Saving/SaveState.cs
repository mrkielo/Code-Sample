using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.SceneManagement;

public class SaveState : MonoBehaviour
{
	public const string saveKey = "save";
	public const string cachePlayerState = "cachePlayerState";
	public const string cacheWorldState = "cacheWorldState";
	public const string doorsKey = "doorsKey";

	public class PlayerState
	{
		public string[] itemNames;
		public float maxHp;
		public float hp;
		public float maxWater;
		public float water;
		public Vector3 position;
		//movement
		public bool canSprint;
		public int jumpsQty;
		public float maxStamina;
		public string currentSceneName;

		public PlayerState(
			string[] itemNames,
			float maxHp,
			float hp,
			float maxWater,
			float water,
			Vector3 position,
			bool canSprint,
			int jumpsQty,
			float maxStamina,
			string currentSceneName
			)
		{
			this.itemNames = itemNames;
			this.maxHp = maxHp;
			this.hp = hp;
			this.maxWater = maxWater;
			this.water = water;
			this.position = position;
			this.canSprint = canSprint;
			this.jumpsQty = jumpsQty;
			this.maxStamina = maxStamina;
			this.currentSceneName = currentSceneName;
		}
	}
	public class WorldState
	{
		public string[] itemNames;
		public Vector3[] itemsPositions;

		public WorldState(string[] itemNames, Vector3[] itemsPositions)
		{
			this.itemNames = itemNames;
			this.itemsPositions = itemsPositions;
		}
	}


	public InventoryItemsIndex inventoryItemsIndex;

	public PlayerState playerState;

	[HideInInspector] public int doorsToSpawnAtId;

	private void Awake()
	{
		inventoryItemsIndex = GetComponent<InventoryItemsIndex>();
		//switch port code
	}
	/////////////////////////////
	/////// PLAYER STATE ///////
	/////////////////////////////

	private void Start()
	{
		//switch port code
		SkipScene();
	}

	public void SavePlayerStateToPrefs(PlayerState playerState, string key = cachePlayerState)
	{
		string jsonSave;
		jsonSave = JsonUtility.ToJson(playerState);
		PlayerPrefs.SetString(key, jsonSave);
		//PlayerPrefs.SetInt(doorsKey, doorsToSpawnAtId);
	}

	public PlayerState LoadPlayerStateFromPrefs(string key = cachePlayerState)
	{
		//doorsToSpawnAtId = PlayerPrefs.GetInt(doorsKey);
		if (!PlayerPrefs.HasKey(key)) return null;
		return JsonUtility.FromJson<PlayerState>(PlayerPrefs.GetString(key));
	}

	/////////////////////////////
	//////// WORLD STATE ////////
	/////////////////////////////


	public void SaveWorldStateToPrefs(WorldState worldState, string key = cacheWorldState)
	{
		string json;
		json = JsonUtility.ToJson(worldState);
		PlayerPrefs.SetString(key, json);
	}
	public WorldState LoadWorldStateFromPrefs(string key = cacheWorldState)
	{
		if (!PlayerPrefs.HasKey(key)) return null;
		return JsonUtility.FromJson<WorldState>(PlayerPrefs.GetString(key));
	}

	/////////////////////////////
	///// GETTING & SETTING /////
	/////   WORLD STATE     /////
	/////////////////////////////


	public WorldState GetWorldState()
	{
		WorldItem[] items = FindObjectsOfType<WorldItem>();

		string[] itemsNames = new string[items.Length];
		Vector3[] itemsPositions = new Vector3[items.Length];

		for (int i = 0; i < items.Length; i++)
		{
			itemsNames[i] = items[i].inventoryItem.name;
			itemsPositions[i] = items[i].transform.position;
		}
		return new WorldState(itemsNames, itemsPositions);
	}

	public void SetWorldState(WorldState worldState)
	{
		if (worldState == null) return;
		WorldItem[] items = FindObjectsOfType<WorldItem>();
		foreach (WorldItem item in items)
		{
			Destroy(item.gameObject);
		}

		for (int i = 0; i < worldState.itemNames.Length; i++)
		{
			Instantiate(inventoryItemsIndex.GetWorldItemFromName(worldState.itemNames[i]), worldState.itemsPositions[i], transform.rotation);
		}
	}

	public void SkipScene()
	{
		if (SceneManager.GetActiveScene().buildIndex == 0)
		{
			Debug.Log("Start Scene");
			SceneManager.LoadScene(1);
		}
	}
}
