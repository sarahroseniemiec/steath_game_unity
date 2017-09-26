using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guard : MonoBehaviour {

	public Transform pathHolder;

	public float speed = 5;
	public float waitTime = 0.3f;
	public float turnSpeed = 90;
	//90 degrees per second

	// Use this for initialization
	void Start () {
		Vector3[] waypoints = new Vector3[pathHolder.childCount];

		for (int i = 0; i < waypoints.Length; i ++) {
			waypoints [i] = pathHolder.GetChild (i).position;
			waypoints [i] = new Vector3 (waypoints [i].x, transform.position.y, waypoints [i].z);
		}

		StartCoroutine (FollowPath (waypoints));

	

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
	}



	// Update is called once per frame
	void Update () {

	}

}


