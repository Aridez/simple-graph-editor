/**
 * @file WaypointEditor.cs
 * @author Eric Mourin
 * @date 12 Aug 2015
 * @brief Custom editor to handle waypoint removal and individual information.
 */
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Waypoint), true)]
[CanEditMultipleObjects]
class WaypointEditor : Editor {

	/// <summary>
	/// The waypoint currently being edited
	/// </summary>
	private Waypoint waypoint;

	/// <summary>
	/// Initialization of the waypoint variable
	/// </summary>
	void OnEnable() {
		waypoint = (Waypoint)target;
	}

	/// <summary>
	/// Adds a button to the waypoint inspector to set the weights of all edges to the same value
	/// </summary>
	public override void OnInspectorGUI(){
		if(GUILayout.Button("Same probabilities for all edges")) waypoint.setSame();
		DrawDefaultInspector ();
    }

}

