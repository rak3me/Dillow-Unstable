using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.PostProcessing;

public class PostProcessingEditor : MonoBehaviour {

	public static bool active = false;

	[MenuItem("Post-Processing/Toggle %#t")]
	public static void TogglePostProcessing () {
		foreach (PostProcessingBehaviour behavior in FindObjectsOfType<PostProcessingBehaviour>()) {
			behavior.enabled = active;
		}

		active = !active;
	}
}
