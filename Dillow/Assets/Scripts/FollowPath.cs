using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FollowPath : MonoBehaviour {

	public float speed = 20f;

	public int currentIndex;
	private int lastIndex;
	private Vector3 target;
	private Vector3 dampVel; //Smooth damp reference
	private Rigidbody rb;

	public Vector3[] pathNodes;

	public bool traversing = false;

	private Transform guide;

	public void SetPath (Vector3[] nodes) {
		currentIndex = 1;
		pathNodes = nodes;
		lastIndex = nodes.Length;
		target = nodes[0];
		rb = GetComponent<Rigidbody>();
	}

	public void SetPath (Vector3[] nodes, float speed) {
		currentIndex = 1;
		this.speed = speed;
		pathNodes = nodes;
		lastIndex = nodes.Length;
		target = nodes[0];
		rb = GetComponent<Rigidbody>();
	}
	public void SetPath (Vector3[] nodes, float speed, bool autoTraverse) {
		currentIndex = 1;
		this.speed = speed;
		pathNodes = nodes;
		lastIndex = nodes.Length;
		target = nodes[0];
		rb = GetComponent<Rigidbody>();

		if (true == autoTraverse && null != rb) {
			TraversePath();
		} else {
			print("No rigidbody!");
		}
	}

	private void FixedUpdate () {
		if (false == traversing) return;

		if (pathNodes.Length > 0) {
			float t = 0f;
			if (currentIndex < lastIndex) {
				target = pathNodes[currentIndex];
				//transform.up = Vector3.SmoothDamp(transform.up, (target - transform.position).normalized, ref dampVel, 0.3f);

				guide.position += (target - pathNodes[currentIndex - 1]).normalized * speed * Time.fixedDeltaTime;

				if ((guide.position - target).magnitude < 2 * speed * Time.fixedDeltaTime) {
					guide.position = target;
					currentIndex++;
				}
			} else {
				guide.position += (pathNodes[lastIndex-1] - guide.position).normalized * speed * Time.fixedDeltaTime;

				if ((guide.position - transform.position).sqrMagnitude < float.Epsilon * float.Epsilon) {
					print("DONE DONE DONE DONE");
					rb.useGravity = true;
					traversing = false;
					Destroy(guide.gameObject);
					Destroy(this);
				}
			}
		}

		rb.position = Vector3.SmoothDamp(rb.position, guide.position, ref dampVel, 0.2f);
	}

	public void TraversePath () {
		rb.useGravity = false;
		traversing = true;

		GameObject guideGO = new GameObject("Guide");
		guide = guideGO.transform;
		guide.position = transform.position;
	}
}
