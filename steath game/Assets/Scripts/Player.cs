using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	public event System.Action OnGameWin;

	public float moveSpeed = 7;
	public float smoothMoveTime = 0.1f;
	public float turnSpeed = 8;

	float angle;

	float smoothInputMagnitude;
	float smoothMoveVelocity;
	Vector3 velocity;

	new Rigidbody rigidbody;
	bool disabled;
	// Use this for initialization
	void Start () {
		rigidbody = GetComponent<Rigidbody>();
		Guard.OnGuardHasSpottedPlayer += Disable;
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 inputDirection = Vector3.zero;
		if (!disabled) {
			inputDirection = new Vector3 (Input.GetAxisRaw ("Horizontal"), 0, Input.GetAxisRaw ("Vertical")).normalized;
		}
		float inputMagnitude = inputDirection.magnitude;
		float targetAngle = Mathf.Atan2 (inputDirection.x, inputDirection.z) * Mathf.Rad2Deg;
		smoothInputMagnitude = Mathf.SmoothDamp (smoothInputMagnitude, inputMagnitude, ref smoothMoveVelocity, smoothMoveTime);
		angle = Mathf.LerpAngle (angle, targetAngle, Time.deltaTime * turnSpeed * inputMagnitude);

		velocity = transform.forward * moveSpeed * smoothInputMagnitude;
//		transform.eulerAngles = Vector3.up * angle;
//		transform.Translate (transform.forward * moveSpeed * Time.deltaTime * smoothInputMagnitude, Space.World);

	}

	void Disable() {
		disabled = true;
	}

	void FixedUpdate () {
		rigidbody.MoveRotation (Quaternion.Euler (Vector3.up * angle));
		rigidbody.MovePosition (rigidbody.position + velocity * Time.deltaTime);
	}

	void OnDestroy () {
		Guard.OnGuardHasSpottedPlayer -= Disable;
	}

	void OnTriggerEnter (Collider hitCollider) {
		if(hitCollider.tag == "Finish") {
			Disable ();
			if (OnGameWin != null) {
				OnGameWin ();
			}
		}
		
	}



}
