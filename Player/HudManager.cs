using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HudManager : MonoBehaviour
{
	[SerializeField] Image staminaBar;
	[SerializeField] Image healthBar;
	[SerializeField] Image waterBar;
	[SerializeField] Image pickupMessage;
	[SerializeField] GameObject gameOverUI;
	Player player;

	private void Start()
	{
		player = GetComponent<Player>();
	}


	public void SetStamina(float fraction)
	{
		staminaBar.fillAmount = fraction;
	}
	public void SetHealth(float fraction)
	{
		healthBar.fillAmount = fraction;
	}

	public void SetWater(float fraction)
	{
		waterBar.fillAmount = fraction;
	}

	public void InteractMessage(string messageText = null)
	{
		if (messageText == null)
		{
			pickupMessage.gameObject.SetActive(false);
			return;
		}
		pickupMessage.GetComponentInChildren<Text>().text = messageText;
		pickupMessage.gameObject.SetActive(true);
	}

	public void ShowGameOverUI()
	{
		gameOverUI.SetActive(true);
		player.playerInput.SwitchCurrentActionMap("UI");

	}

	public void HideGameOverUI()
	{
		gameOverUI.SetActive(false);
		player.playerInput.SwitchCurrentActionMap("Player");
	}

}