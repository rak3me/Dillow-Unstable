using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CannonPath : MonoBehaviour {

	[Header("Node Info")]
    public Transform start;
    public Transform mid;
    public Transform end;
	public int resolution = 20;

	[Header("Automation Info")]
	public bool autoConfigureMidpoint = false;
	[Tooltip("Midpoint will be double this value")] 
	public float curveHeight = 200f;

	[Header("Debug Info")]
	public bool drawNodes = true;
	public float nodeRadius = 10f;
	public bool drawPath = true;
	public float pathRadius = 10f;

	public Vector3[] path;

    private void Update () {
		if (resolution < 3) resolution = 3;

		if (null == start) {
			start = transform.Find("Start");

			if (null == start) {
				start = transform.GetChild(0);
			}
		}
		if (null == mid) {
			mid = transform.Find("Mid");

			if (null == mid) {
				mid = transform.GetChild(1);
			}
		}
		if (null == end) {
			end = transform.Find("End");

			if (null == end) {
				end = transform.GetChild(2);
			}
		}

		if (false == Application.isPlaying) {
			if (true == autoConfigureMidpoint) {
				Vector3 targetPosition = (start.position + end.position) / 2;
				targetPosition.y = curveHeight * 2f;
				mid.position = targetPosition;
			}

			UpdatePath();
		}
	}

	public void UpdatePath () {
        if (start && mid && end) {
            path = GetPath (start.position, mid.position, end.position, resolution);
        }
    }

    Vector3 GetPoint (Vector3 p0, Vector3 p1, Vector3 p2, float t) {
        return Vector3.Lerp (Vector3.Lerp (p0, p1, t), Vector3.Lerp (p1, p2, t), t);
    }

    Vector3[] GetPath (Vector3 p0, Vector3 p1, Vector3 p2, int resolution = 20) {
        Vector3[] path = new Vector3[resolution];
        for (int i = 0; i < resolution; i++) {
            path[i] = GetPoint (p0, p1, p2, (float)i / (resolution-1));
        }
        return path;
    }

    private void OnDrawGizmos () {
		if (true == drawNodes) {
			if (true == start) {
				Gizmos.color = Color.green;
				Gizmos.DrawSphere(start.position, nodeRadius);
			}
			if (true == mid) {
				Gizmos.color = Color.blue;
				Gizmos.DrawSphere(mid.position, nodeRadius);
			}
			if (true == end) {
				Gizmos.color = Color.red;
				Gizmos.DrawSphere(end.position, nodeRadius);
			}
		}

        if (true == drawPath && path.Length > 1) {
            Gizmos.color = Color.cyan;
            for (int i = 0; i < path.Length - 1; i++) {
                DrawLine (path[i], path[i + 1], 10f);
            }
        }
    }

	public void DrawLine (Vector3 p1, Vector3 p2, float width) {
		int count = Mathf.CeilToInt(width); // how many lines are needed.
		if (count == 1)
			Gizmos.DrawLine(p1, p2);
		else {
			Camera c = Camera.current;
			if (c == null) {
				Debug.LogError("Camera.current is null");
				return;
			}
			Vector3 v1 = (p2 - p1).normalized; // line direction
			Vector3 v2 = (c.transform.position - p1).normalized; // direction to camera
			Vector3 n = Vector3.Cross(v1, v2); // normal vector
			for (int i = 0; i < count; i++) {
				Vector3 o = n * width * ((float)i / (count - 1) - 0.5f);
				Gizmos.DrawLine(p1 + o, p2 + o);
			}
		}
	}
}
