using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InventoryItem : MonoBehaviour
{
	public WorldItem worldItem;
	public UnityEvent OnUse;
	public new string name = "NoName";
	public string description = "";
}
