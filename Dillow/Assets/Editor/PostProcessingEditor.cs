using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessingEditor : MonoBehaviour {

	public static bool active = false;

	[MenuItem("Post-Processing/Toggle %#t")]
	public static void Toggle () {
		FindObjectOfType<PostProcessLayer>().enabled = active;

		active = !active;
	}
}
