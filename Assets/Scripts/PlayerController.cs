using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	[SerializeField] private float thrust, minTiltSmooth, maxTiltSmooth, hoverDistance, hoverSpeed;
	private bool start;
	private float timer, tiltSmooth, y;
	private Rigidbody2D playerRigid;
	private Quaternion downRotation, upRotation;
	[SerializeField] private SpriteRenderer playerSpriteRenderer;
	private static readonly int GrayScaleAmount = Shader.PropertyToID("_GrayscaleAmount");


	private bool up;
	private Vector3 pos;
	private bool alive;
	void Start () {
		tiltSmooth = maxTiltSmooth;
		playerRigid = GetComponent<Rigidbody2D> ();
		downRotation = Quaternion.Euler (0, 0, -90);
		upRotation = Quaternion.Euler (0, 0, 35);
		SetGrayScale(0);
		alive = true;

	}

	private float fallAmount = 3;
	
	void Update () {
		

		if (alive)
		{
			if (!start) {
				// Hover the player before starting the game
				timer += Time.deltaTime;
				y = hoverDistance * Mathf.Sin (hoverSpeed * timer);
				transform.localPosition = new Vector3 (0, y, 0);
			} else {
				// Rotate downward while falling
				transform.rotation = Quaternion.Lerp (transform.rotation, downRotation, tiltSmooth * Time.deltaTime);
			}
			// Limit the rotation that can occur to the player
			transform.rotation = new Quaternion (transform.rotation.x, transform.rotation.y, Mathf.Clamp (transform.rotation.z, downRotation.z, upRotation.z), transform.rotation.w);
			if (up)
			{
				pos = transform.position;
				pos.y += 10 * Time.deltaTime;
				transform.position = pos;
			}
			else
			{
				pos = transform.position;
				pos.y -= fallAmount * Time.deltaTime;
				transform.position = pos;
			}
		}
		
		
	}

	void LateUpdate () {
		if (GameManager.Instance.GameState ()) {
			if (Input.GetMouseButtonDown (0)) {
				if(!start){
					// This code checks the first tap. After first tap the tutorial image is removed and game starts
					start = true;
					GameManager.Instance.GetReady ();
					GetComponent<Animator>().speed = 2;
				}

				up = true;
				CancelInvoke("Falling");
				Invoke("Falling",0.15f);
				playerRigid.gravityScale = 1f;
				tiltSmooth = minTiltSmooth;
				transform.rotation = upRotation;
				playerRigid.velocity = Vector2.zero;
				// Push the player upwards
				playerRigid.AddForce (Vector2.up * thrust);
				SoundManager.Instance.PlayTheAudio("Flap");
			}
		}
		if (playerRigid.velocity.y < -1f) {
			// Increase gravity so that downward motion is faster than upward motion
			tiltSmooth = maxTiltSmooth;
			playerRigid.gravityScale = 2f;
		}
	}

	void Falling()
	{
		fallAmount = 3;
		up = false;
		CancelInvoke("FallingFast");
		Invoke("FallingFast",0.15f);
	}

	void FallingFast()
	{
		fallAmount = 6;
	}

	void OnTriggerEnter2D (Collider2D col) {
		if (alive)
		{
			if (col.transform.CompareTag ("Score")) {
				Destroy (col.gameObject);
				GameManager.Instance.UpdateScore ();
			} else if (col.transform.CompareTag ("Obstacle")) {
				// Destroy the Obstacles after they reach a certain area on the screen
				/*foreach (Transform child in col.transform.parent.transform) {
					child.gameObject.GetComponent<BoxCollider2D> ().enabled = false;
				}*/
				KillPlayer ();
				alive = false;
				CancelInvoke("Falling");
				CancelInvoke("FallingFast");
			}
		}
	}

	void OnCollisionEnter2D (Collision2D col) {
		if (col.transform.CompareTag ("Ground")) {
			playerRigid.simulated = false;
			KillPlayer ();
			CancelInvoke("Falling");
			CancelInvoke("FallingFast");
			transform.rotation = downRotation;
		}
	}

	public void KillPlayer ()
	{
		GameManager.Instance.EndGame();
		playerRigid.velocity = Vector2.zero;
		// Stop the flapping animation
		GetComponent<Animator> ().enabled = false;
		StartCoroutine(GrayScaleIenumerator(0.75f));
		
	}

	IEnumerator GrayScaleIenumerator(float duration, Action callback = null)
	{
		float time = 0;
		while (duration>time)
		{
			float ratito = time / duration;
			float amount = ratito;
			SetGrayScale(amount);
			time += Time.deltaTime;
			yield return null;
		}
		SetGrayScale(1);
		if (callback != null)
		{
			callback.Invoke();
		}
	}

	void SetGrayScale(float value)
	{
		playerSpriteRenderer.material.SetFloat(GrayScaleAmount,value);
	}

}