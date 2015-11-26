// ::: C Y B E R M A G E :::

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public enum FacingDirection
{
	Up,
	Right,
	Down,
	Left
}

public class PlayerController : MonoBehaviour 
{

	public string horizontalAxis = "Horizontal";
	public string verticalAxis = "Vertical";
	public string fireButton = "Fire1";

	public float walkSpeed = 1;


	Animator anim;
	Vector2 movementVelocity;
	Vector3 movement;

	bool isAlive = true;

	public FacingDirection facingDirection;

	Rigidbody2D rb;
	SpriteRenderer sr;
	CircleCollider2D col;


	void Awake () 
	{
		anim = GetComponent<Animator>();
		rb = GetComponent<Rigidbody2D>();
		sr =  GetComponent<SpriteRenderer>();
		col = GetComponent<CircleCollider2D>();
	}


	void Start()
	{

	}

	
	// MAIN LOOP
	void Update()
	{
		if (!isAlive) return;

		CheckMovement();
	}
	
	
	void CheckMovement ()
	{
		float h = Input.GetAxisRaw(horizontalAxis);
		float v = Input.GetAxisRaw(verticalAxis);


		if (v!=0 || h!=0)
		{
			movement.Set(h,v,0);

			// fix diagonal from moving too fast
			movement.Normalize();

			movement*=walkSpeed*Time.deltaTime;

			anim.SetBool("Walk",true);
			anim.SetFloat("SpeedHorizontal",h);
			anim.SetFloat("SpeedVertical",v);
		}else{ // not moving
			movement.Set(0,0,0);
			anim.SetBool("Walk",false);
		}
	} 


	// physics loop
	void FixedUpdate()
	{
		rb.MovePosition (transform.position + movement);
	}



}
