using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
	[HideInInspector] public Rigidbody rb;
	[Header("General")]
	[SerializeField] public float walkSpeed;
	public float defaultMovementSmoothing;
	[HideInInspector] public float movementSmoothing;
	[HideInInspector] public float speedModifier;
	Player player;
	float currentVel;

	[Header("Jump")]
	[SerializeField] Transform groundCheck;
	[SerializeField] LayerMask groundMask;
	[SerializeField] float gravity;
	[SerializeField] public float jumpForce;
	public int jumpsQty;
	int jumpsLeft;
	public float mSpeed;

	[Header("Sprint")]
	public bool canSprint;
	[SerializeField] public float sprintSpeed;
	public float maxStamina;
	[SerializeField] float staminaGain;
	[SerializeField] float staminaDrop;
	float stamina;
	public bool gravityEnabled = true;

	public Vector2 mDir;
	[HideInInspector] public float gravityVel; // current gravity force acceleration


	private void Start()
	{
		speedModifier = 1;
		player = GetComponent<Player>();
		mSpeed = walkSpeed;
		rb = GetComponent<Rigidbody>();
		gravityVel = 0;
		stamina = maxStamina;
	}

	private void Update()
	{
		Gravity();
		if (canSprint) Stamina();
		HudUpdate();
	}

	void FixedUpdate()
	{
		rb.velocity = new Vector3(Mathf.SmoothDamp(rb.velocity.x, mDir.x * mSpeed * speedModifier * Time.deltaTime, ref currentVel, movementSmoothing), rb.velocity.y + gravityVel, rb.velocity.z); // adding move to rb
	}

	//////////////////////////////
	////// INPUT METHODS//////////
	//////////////////////////////

	public void Move(InputAction.CallbackContext ctx)
	{
		mDir = ctx.ReadValue<Vector2>();
	}

	public void Jump(InputAction.CallbackContext ctx)
	{
		if (CanJump() && ctx.performed)
		{
			player.animations.JumpAnimationTrigger();
			gravityVel = 0;                  // reset gravity to make 2nd jump similar to 1st
			rb.velocity = new Vector3(rb.velocity.x, 0, 0);      /////////
			rb.AddForce(Vector3.up * jumpForce); // jumping
			jumpsLeft--;
		}
	}

	public void Sprint(InputAction.CallbackContext ctx)
	{

		if (ctx.performed && stamina > 0) mSpeed = sprintSpeed; //on button down
		if (ctx.canceled) mSpeed = walkSpeed; // on button up
	}


	//////////////////////////
	///// UPDATE METHODS /////
	//////////////////////////

	void Stamina() // stamina gain and drain
	{
		if (stamina <= 0) mSpeed = walkSpeed;
		if (stamina < maxStamina && (mSpeed == walkSpeed || rb.velocity.x == 0))
		{
			stamina += staminaGain * Time.deltaTime; // multiply by deltatime to achieve staminaGain per sec
		}
		else if (mSpeed == sprintSpeed && rb.velocity.x != 0)
		{
			stamina -= staminaDrop * Time.deltaTime;
		}
	}

	void Gravity() // custom gravity
	{
		if (isGrounded() || !gravityEnabled) gravityVel = 0;
		else gravityVel += gravity * Time.deltaTime;
	}

	void HudUpdate()
	{
		player.hud.SetStamina(stamina / maxStamina);
	}

	/////////////////////////
	///////// OTHER /////////
	/////////////////////////



	bool CanJump()
	{
		if (isGrounded())
		{
			jumpsLeft = jumpsQty;
		}


		if (jumpsLeft > 0) return true;
		else return false;
	}

	bool isGrounded()
	{
		return Physics.CheckSphere(groundCheck.position, 0.2f, groundMask); // check if playr touching ground
	}

	////////////////

	public void EnableDoubleJump() // method for unlocking ingame
	{
		jumpsQty = 2;
	}
}


