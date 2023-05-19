using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

[RequireComponent(typeof(AudioSource))]
public class Inventory : MonoBehaviour
{
	[SerializeField] GameObject inventoryUI;
	[SerializeField] public GameObject menuUI;
	[SerializeField] int maxSlots;
	[SerializeField] Text slotsText;
	[SerializeField] Description description;
	AudioSource audioData;


	Player player;
	WorldItem item;
	int currentSlots;
	public InventoryItem[] allItems;


	private void Awake()
	{
		player = GetComponent<Player>();
	}

	private void Start()
	{
		player.playerInput.SwitchCurrentActionMap("Player");
		audioData = GetComponent<AudioSource>();
		RefreshSlots();
	}

	public void ToggleInventory(InputAction.CallbackContext ctx) //inventory on/off
	{
		if (menuUI.activeSelf && ctx.started)
		{
			InventoryOff();
		}
		else if (ctx.started)
		{
			InventoryOn();
		}
	}


	void OnTriggerEnter(Collider other)
	{
		item = other.gameObject.GetComponent<WorldItem>(); // get any close item
		if (item != null)
		{
			if (currentSlots >= maxSlots) player.hud.InteractMessage("No space in Inventory");
			else player.hud.InteractMessage("Pickup " + item.inventoryItem.name);
			player.otherInputs.OnCustomInteraction.RemoveAllListeners();
			player.otherInputs.OnCustomInteraction.AddListener(PickupItem);
		}

	}

	void OnTriggerExit(Collider other)
	{
		if (item != null)
		{
			player.hud.InteractMessage(); //disabling message

			item = null;
			player.otherInputs.OnCustomInteraction.RemoveListener(PickupItem);
		}
	}




	public void PickupItem()
	{
		if (item == null) return;
		RefreshSlots();
		if (currentSlots >= maxSlots)
		{
			Debug.Log("No space in inventory");
			return;
		}

		audioData.Play(0);
		AddToInventory(item.inventoryItem);
		Destroy(item.gameObject);
		player.hud.InteractMessage();
		player.otherInputs.OnCustomInteraction.RemoveListener(PickupItem);
	}

	void AddToInventory(InventoryItem item)
	{
		Instantiate(item, inventoryUI.transform); //spawning inventory variation of item in inventory
		RefreshSlots();
	}


	public void DropItem(InputAction.CallbackContext ctx)
	{
		if (ctx.started)
		{
			GameObject itemToDrop = EventSystem.current.currentSelectedGameObject;
			if (itemToDrop != null)
			{
				GameObject itemToSpawn = itemToDrop.GetComponent<InventoryItem>().worldItem.gameObject;
				Instantiate(itemToSpawn, transform.position, transform.rotation);
				Destroy(itemToDrop);

			}
		}
		ResetItemSelection();
		RefreshSlots();
	}

	public void UseItem(InputAction.CallbackContext ctx)
	{
		if (ctx.started)
		{
			InventoryItem inventoryItem = GetCurrentSelectedItem();
			if (inventoryItem != null) //check if is correct
			{
				inventoryItem.OnUse.Invoke();
			}
		}
		ResetItemSelection();
		RefreshSlots();
	}

	public void ToggleDescription(InputAction.CallbackContext ctx)
	{
		if (ctx.started)
		{
			InventoryItem item = GetCurrentSelectedItem();
			if (description.gameObject.activeSelf) HideDescription();
			else ShowDescription(item);
		}
	}

	void ShowDescription(InventoryItem item)
	{
		if (item == null) return;
		player.playerInput.SwitchCurrentActionMap("Description");
		description.gameObject.SetActive(true);
		description.SetNameText(item.name);
		description.SetDescriptionText(item.description);
	}
	void HideDescription()
	{
		description.gameObject.SetActive(false);
		player.playerInput.SwitchCurrentActionMap("UI");
	}

	void ResetItemSelection()
	{
		Button firstItem = inventoryUI.GetComponentInChildren<Button>();
		if (firstItem != null) firstItem.Select();
	}

	void RefreshSlots()
	{
		allItems = inventoryUI.GetComponentsInChildren<InventoryItem>(true);
		currentSlots = allItems.Length;
		slotsText.text = currentSlots + " / " + maxSlots;
	}

	public void LoadItems(string[] loadItemsNames)
	{
		for (int i = 0; i < allItems.Length; i++)
		{
			Destroy(allItems[i].gameObject);
		}
		InventoryItemsIndex itemsIndex = player.saveState.inventoryItemsIndex;
		foreach (string name in loadItemsNames)
		{
			AddToInventory(itemsIndex.GetWorldItemFromName(name).GetComponent<WorldItem>().inventoryItem);
		}
		RefreshSlots();
	}

	InventoryItem GetCurrentSelectedItem()
	{
		InventoryItem inventoryItem = null;
		GameObject currentGameObject = EventSystem.current.currentSelectedGameObject;
		if (currentGameObject != null) inventoryItem = currentGameObject.GetComponent<InventoryItem>();
		if (inventoryItem == null) Debug.Log("There is no selected Item");
		return inventoryItem;
	}

	public void InventoryOn()
	{
		player.hud.InteractMessage();
		menuUI.SetActive(true);
		ResetItemSelection();
		Time.timeScale = 0f;
		player.playerInput.SwitchCurrentActionMap("UI");
		RefreshSlots();
	}

	public void InventoryOff()
	{
		if (item != null) player.hud.InteractMessage("Pickup " + item.inventoryItem.name);
		menuUI.SetActive(false); //invrntory off
		Time.timeScale = 1f;          //resume game
		player.playerInput.SwitchCurrentActionMap("Player"); // player inputs off
	}

	public string[] GetItemsNames()
	{
		RefreshSlots();
		string[] names = new string[allItems.Length];
		for (int i = 0; i < allItems.Length; i++)
		{
			names[i] = allItems[i].name;
		}
		return names;
	}




}
