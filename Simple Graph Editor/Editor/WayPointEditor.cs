/**
 * @file WaypointEditor.cs
 * @author Eric Mourin
 * @date 12 Aug 2015
 * @brief Custom editor to handle waypoint removal and individual information.
 */
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WayPoint), true)]
[CanEditMultipleObjects]
class WaypointEditor : Editor {
	private WayPoint waypoint;					// The waypoint currently being edited.
	void OnEnable() {
		waypoint = (WayPoint)target;
	}

	/*	Button to set the same probabilites for all waypoints.
	*	Default inspector.
	*/
	public override void OnInspectorGUI(){
		if(GUILayout.Button("Same probabilities for all edges")) waypoint.setSame();
		DrawDefaultInspector ();

        if (Event.current.type == EventType.keyDown && Event.current.keyCode == KeyCode.Delete) handleDestruction();
    }

    void OnSceneGUI()
    {
        
        if (Event.current.type == EventType.keyDown && Event.current.keyCode == KeyCode.Delete) handleDestruction();
    }

    /*	Destruction handling code.
	*	For each linked waypoint in any way
	*		Its state is saved to be able to undo.
	*		The link is removed.
	*	The reference at the cluster it pertains is removed.
	*	Then the current waypoint is destroyed and won't leave null references.
	*/
    private void handleDestruction() {
		Event.current.Use();
		for (int i = 0; i < waypoint.outs.Count; ++i) {
			Undo.RecordObject(waypoint.outs[i].waypoint, "destroyed");
			waypoint.outs[i].waypoint.removeInWayPoint(waypoint);
		}
		for (int i = 0; i < waypoint.ins.Count; ++i) {
			Undo.RecordObject(waypoint.ins[i], "destroyed");
			waypoint.ins[i].removeOutWayPoint(waypoint);
		}
		Undo.RecordObject(waypoint.getParent(), "destroyed");
		waypoint.getParent().remove(waypoint);
		Undo.DestroyObjectImmediate(waypoint.gameObject);
	}
}

