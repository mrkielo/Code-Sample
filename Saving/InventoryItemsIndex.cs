using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItemsIndex : MonoBehaviour
{
	[SerializeField] GameObject[] allProjectItems;

	public GameObject GetWorldItemFromName(string name)
	{

		foreach (GameObject item in allProjectItems)
		{
			if (item.GetComponent<WorldItem>().inventoryItem == null) return null;
			if (item.GetComponent<WorldItem>().inventoryItem.name == name) return item;
		}
		return null;
	}




}
