using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour ///Main Player Script all references to other PlayerScripts should go through this script.
{
	[HideInInspector] public PlayerMovement movement;
	[HideInInspector] public HudManager hud;
	[HideInInspector] public PlayerInput playerInput;
	[HideInInspector] public OtherInputs otherInputs;
	[HideInInspector] public Inventory inventory;
	[HideInInspector] public SaveState saveState;
	[HideInInspector] public PlayerAnimations animations;
	[SerializeField] float maxHp;

	[Space(2), Header("Water System")]
	[SerializeField] float maxWater;
	[SerializeField] float waterDrainSpeed;
	[SerializeField] float waterDamagePerSecond;

	float hp;
	float water;




	void Awake()
	{
		water = maxWater;
		hp = maxHp;
		playerInput = GetComponent<PlayerInput>();
		movement = GetComponent<PlayerMovement>();
		hud = GetComponent<HudManager>();
		otherInputs = GetComponent<OtherInputs>();
		inventory = GetComponent<Inventory>();
		saveState = FindAnyObjectByType<SaveState>();
		animations = GetComponent<PlayerAnimations>();
	}
	void Start()
	{
		LevelStartLoad();
	}



	void OnCollisionEnter(Collision other)
	{
		CheckForDamage(other);
	}

	void Update()
	{
		WaterDrain();
	}

	///////////////////////////
	//////// HP SYSTEM ////////
	///////////////////////////

	void CheckForDamage(Collision other)
	{
		DamagePlayerObject damageObject = other.gameObject.GetComponent<DamagePlayerObject>();
		if (damageObject != null)
		{
			TakeDamage(damageObject.damage);
		}
	}

	public void TakeDamage(float damage)
	{
		hp -= damage;
		hud.SetHealth(hp / maxHp);
		if (hp <= 0) Die();
	}

	public void Heal(float healedHp)
	{
		hp += healedHp;
		if (hp >= maxHp) hp = maxHp;
	}

	void Die()
	{
		movement.enabled = false;
		hud.ShowGameOverUI();
	}

	////////////////////////
	///// WATER SYSTEM /////
	////////////////////////

	void WaterDrain()
	{
		if (water > 0)
		{
			water -= waterDrainSpeed * Time.deltaTime;
			hud.SetWater(water / maxWater);
		}
		else
		{
			water = 0;
			TakeDamage(waterDamagePerSecond * Time.deltaTime);
			//slow down todo
		}
	}

	public void Drink(float amount)
	{
		water += amount;
		if (water > maxWater) water = maxWater;
	}

	/////////////////////////////
	///////// UPGRADES //////////
	/////////////////////////////


	/////////////////////////////
	///////// SAVING ////////////
	/////////////////////////////



	public void SaveState(InputAction.CallbackContext ctx)
	{
		if (ctx.started)
		{
			Debug.Log("Save");
			SavePlayerState();
			SaveWorldState();
			PlayerPrefs.Save();
		}
	}

	public void LoadState(InputAction.CallbackContext ctx)
	{
		if (ctx.started)
		{
			Debug.Log("Load");
			LoadPlayerState();
			LoadWorldState();
		}
	}

	public void ClearSaves(InputAction.CallbackContext ctx)
	{
		Debug.Log("ClearSaves");
		if (ctx.started)
		{
			Debug.Log("Saves Cleared");
			PlayerPrefs.DeleteAll();
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}
	}



	public void SavePlayerState(string key = null)
	{
		if (key == null) saveState.SavePlayerStateToPrefs(new SaveState.PlayerState(inventory.GetItemsNames(), maxHp, hp, maxWater, water, transform.position, movement.canSprint, movement.jumpsQty, movement.maxStamina, SceneManager.GetActiveScene().name));
		else saveState.SavePlayerStateToPrefs(new SaveState.PlayerState(inventory.GetItemsNames(), maxHp, hp, maxWater, water, transform.position, movement.canSprint, movement.jumpsQty, movement.maxStamina, SceneManager.GetActiveScene().name), key);
	}

	public void LoadPlayerState(string key = null)
	{
		SaveState.PlayerState playerState;
		if (key == null) playerState = saveState.LoadPlayerStateFromPrefs();
		else playerState = saveState.LoadPlayerStateFromPrefs(key);

		// if (playerState.currentSceneName != SceneManager.GetActiveScene().name)
		// 	SceneManager.LoadScene(playerState.currentSceneName);

		transform.position = playerState.position;
		hp = playerState.hp;
		maxHp = playerState.maxHp;
		water = playerState.water;
		maxWater = playerState.maxWater;
		movement.canSprint = playerState.canSprint;
		movement.jumpsQty = playerState.jumpsQty;
		movement.maxStamina = playerState.maxStamina;
		inventory.LoadItems(playerState.itemNames);
		TakeDamage(0); //update hud
	}

	public void SaveWorldState(string key = null)
	{
		if (key == null) saveState.SaveWorldStateToPrefs(saveState.GetWorldState());
		else saveState.SaveWorldStateToPrefs(saveState.GetWorldState(), key);
	}

	public void LoadWorldState(string key = null)
	{
		if (key == null) saveState.SetWorldState(saveState.LoadWorldStateFromPrefs());
		else saveState.SetWorldState(saveState.LoadWorldStateFromPrefs(key));
	}

	void LevelStartLoad()
	{
		LoadWorldState(SceneManager.GetActiveScene().name);
		LoadPlayerState();
		SetPos();

	}

	public Vector3 GetPositionOfDoorsToSpawn(int id)
	{
		ChangeSceneDoors[] allDoors = FindObjectsOfType<ChangeSceneDoors>(true);
		foreach (ChangeSceneDoors door in allDoors)
		{
			if (door.id == id) return new Vector3(door.transform.position.x, door.transform.position.y, transform.position.z);
		}
		Debug.LogError("There is no doors of given id");
		return transform.position;
	}

	void SetPos()
	{
		Vector3 posToSpawn = GetPositionOfDoorsToSpawn(saveState.doorsToSpawnAtId);
		transform.position = posToSpawn;
	}
}
