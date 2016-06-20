using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
	public float speed = 4;
	public float ladderSpeed = 2;
	public float gravity = 16;
	public float jumpforce = 8;

	private Vector2 velocity = new Vector2 (0.0F, 0.0F);
	private bool facingRight = true;
	private bool grounded = false;
	private bool onLadder = false;
	private int platformMask = 1 << 9;
	private int ladderMask = 1 << 10;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		//Check if grounded
		this.grounded = Grounded ();

		//Jumping and gravity
		if (this.grounded) {
			if (Input.GetButton ("Jump")) {
				this.velocity [1] = this.jumpforce;
			}
		} else {
			this.velocity [1] -= this.gravity*Time.deltaTime;
		}

		//Horizontal walking
		float x_axis = Input.GetAxis("Horizontal");
		this.velocity [0] = speed * x_axis;
		if (x_axis > 0 && !facingRight){
			Flip();
		} else if (x_axis < 0 && facingRight){
			Flip();
		}

		//Ladder snap
		int canLadder = LadderCheck ();
		float y_axis = Input.GetAxis ("Vertical");
		if (canLadder != 0 && !this.onLadder) {
			if (Mathf.Abs (y_axis) > 0.5f) {
				if (canLadder == -1) {
					RaycastHit2D hit = Physics2D.Raycast (transform.position, Vector2.left, 0.6f, this.ladderMask);
					transform.position = new Vector2 (hit.transform.position.x, transform.position.y);
				} else {
					RaycastHit2D hit = Physics2D.Raycast (transform.position, Vector2.right, 0.6f, this.ladderMask);
					transform.position = new Vector2 (hit.transform.position.x, transform.position.y);
				}
				this.onLadder = true;
				Physics2D.IgnoreLayerCollision (8, 9, true);
			}

		//Ladder movement (up, down, get off)
		} else if (this.onLadder) {
			if (Input.GetButton ("Jump")) {
				this.onLadder = false;
				Physics2D.IgnoreLayerCollision (8, 9, false);
				velocity [0] = 0;
				velocity [1] = 0;
			} else {
				velocity [0] = 0;
				velocity [1] = y_axis * ladderSpeed;
			}
		}

		Move(this.velocity*Time.deltaTime);

	}

	void Flip () {
		transform.localScale *= -1;
		facingRight = !facingRight;
	}

	void Move (Vector2 velocity) {
		transform.Translate (velocity);
	}

	bool Grounded () {
		int layermask = platformMask;
		return Physics2D.Raycast (transform.position, Vector2.down, 0.6f, layermask);
	}

	//return -1 for ladder left;
	//return 0 for no ladder;
	//return 1 for ladder right;
	int LadderCheck () {
		int layermask = ladderMask;
		if (Physics2D.Raycast (transform.position, Vector2.left, 0.6f, layermask))
			return -1;
		if (Physics2D.Raycast (transform.position, Vector2.right, 0.6f, layermask))
			return 1;
		return 0;
	}

	void OnCollisionEnter2D(Collision2D collision){
		Debug.Log ("collision enter");
		if (collision.gameObject.layer == 9) {
			this.velocity [1] = 0;
		}
	}

	void OnCollisionExit2D(Collision2D collision){
		Debug.Log ("collision exit");
	}

//	void OnTriggerEnter2D(Collider2D collider){
//		Debug.Log ("trigger enter");
//		if (collider.gameObject.layer == 9) {
//			this.grounded = true;
//		}
//	}
//
//	void OnTriggerExit2D(Collider2D collider){
//		Debug.Log ("trigger exit");
//		if (collider.gameObject.layer == 9) {
//			this.grounded = false;
//		}
//	}
}
