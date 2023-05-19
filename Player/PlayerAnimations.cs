using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAnimations : MonoBehaviour
{
	Animator animator;
	Player player;
	Rigidbody rb;
	float xVel;
	bool yVel;
	float baseXScale;

	void Start()
	{
		baseXScale = transform.localScale.x;
		player = GetComponent<Player>();
		animator = GetComponentInChildren<Animator>();
		rb = GetComponent<Rigidbody>();
	}

	private void Update()
	{
		xVelUpdate();
		yVelUpdate();
		FlipUpdate();
		animator.SetFloat("xVel", xVel);
		animator.SetBool("yVel", yVel);
	}

	void xVelUpdate()
	{
		if (player.movement.mSpeed == player.movement.walkSpeed && (rb.velocity.x < -0.1 || rb.velocity.x > 0.1)) xVel = 1;
		else if (player.movement.mSpeed == player.movement.sprintSpeed && rb.velocity.x != 0) xVel = 2;
		else if (rb.velocity.x == 0) xVel = 0;
		else xVel = 0;
	}

	void yVelUpdate()
	{
		if (rb.velocity.y == 0) yVel = false;
		else yVel = true;
	}

	void FlipUpdate()
	{
		if (player.movement.mDir.x > 0) transform.localScale = new Vector3(baseXScale, transform.localScale.y, transform.localScale.z);
		if (player.movement.mDir.x < 0) transform.localScale = new Vector3(-baseXScale, transform.localScale.y, transform.localScale.z);
	}

	public void JumpAnimationTrigger()
	{
		animator.SetTrigger("Jump");
	}
}
