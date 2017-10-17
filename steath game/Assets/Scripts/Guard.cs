using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guard : MonoBehaviour {

	public static event System.Action onGuardHasSpottedPlayer;

	public Transform pathHolder;

	public float speed = 5;
	public float waitTime = 0.3f;
	public float turnSpeed = 90;
	//90 degrees per second
	public float timeToSpotPlayer = 0.5f;


	public Light spotlight;
	public float viewDistance;
	float viewAngle;
	float playerVisibleTimer;
	Color originalSpotlightColor;

	public LayerMask viewMask;

	GameObject player;
	Transform playerTransform;

	// Use this for initialization
	void Start () {
		
		player = GameObject.FindWithTag ("Player");
		playerTransform = player.transform;

		originalSpotlightColor = spotlight.color;
		viewAngle = spotlight.spotAngle;

		Vector3[] waypoints = new Vector3[pathHolder.childCount];

		for (int i = 0; i < waypoints.Length; i ++) {
			waypoints [i] = pathHolder.GetChild (i).position;
			waypoints [i] = new Vector3 (waypoints [i].x, transform.position.y, waypoints [i].z);
		}

		StartCoroutine (FollowPath (waypoints));

	

	}

	bool CanSeePlayer () {
		if (Vector3.Distance(transform.position, playerTransform.position) < viewDistance){
			Vector3 dirToPlayer = (playerTransform.position - transform.position).normalized;
			float angleBtwGuardAndPlayer = Vector3.Angle(transform.forward, dirToPlayer);
			if (angleBtwGuardAndPlayer < viewAngle / 2f) {
				if (!Physics.Linecast(transform.position, playerTransform.position, viewMask)) {
					return true;
				}
			}
		} 
		return false;
	}

	IEnumerator FollowPath(Vector3[] waypoints){
		transform.position = waypoints [0];
		int targetWaypointIndex = 1;
		Vector3 targetWaypointPosition = waypoints [targetWaypointIndex];
		transform.LookAt (targetWaypointPosition);
		while (true) {
			transform.position = Vector3.MoveTowards (transform.position, targetWaypointPosition, speed * Time.deltaTime);
			if (transform.position == targetWaypointPosition) {
				targetWaypointIndex = (targetWaypointIndex + 1) % waypoints.Length;
				targetWaypointPosition = waypoints [targetWaypointIndex];
				yield return new WaitForSeconds (waitTime);
				yield return StartCoroutine (TurnToFace (targetWaypointPosition));

			}
			yield return null;
		}
	}

	IEnumerator TurnToFace(Vector3 lookTarget) {
		Vector3 dirToLookTarget = (lookTarget - transform.position).normalized;
		float rotateAngle = 90 - Mathf.Atan2 (dirToLookTarget.z, dirToLookTarget.x) * Mathf.Rad2Deg;
		while (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, rotateAngle)) > 0.05f) {
			float angle = Mathf.MoveTowardsAngle (transform.eulerAngles.y, rotateAngle, turnSpeed * Time.deltaTime);
			transform.eulerAngles = Vector3.up * angle;
			yield return null;
		}



	}

	void OnDrawGizmos() {
		Vector3 startPosition = pathHolder.GetChild (0).position;
		Vector3 previousPosition = startPosition;
		foreach (Transform waypoint in pathHolder) {
			Gizmos.DrawSphere (waypoint.position, .3f);
			Gizmos.DrawLine(previousPosition, waypoint.position);
			previousPosition = waypoint.position;
		}
		Gizmos.DrawLine (previousPosition, startPosition);
		Gizmos.color = Color.blue;
		Gizmos.DrawLine (transform.position, transform.forward * viewDistance);

	}



	// Update is called once per frame
	void Update () {
		if (CanSeePlayer ()) {
			playerVisibleTimer += Time.deltaTime;
//			spotlight.color = Color.red;
		} else {
//			spotlight.color = originalSpotlightColor;
			playerVisibleTimer -= Time.deltaTime;
		}
		playerVisibleTimer = Mathf.Clamp (playerVisibleTimer, 0, timeToSpotPlayer);
		spotlight.color = Color.Lerp (originalSpotlightColor, Color.red, playerVisibleTimer/timeToSpotPlayer);
		if (playerVisibleTimer >= timeToSpotPlayer) {
			if (onGuardHasSpottedPlayer != null) {
				onGuardHasSpottedPlayer ();
			}
			
		}
	}






}


