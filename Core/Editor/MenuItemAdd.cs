using UnityEditor;
using UnityEngine;
public class MenuTest : MonoBehaviour {
	[MenuItem ("GameObject/Place Waypoints")]
	static void CreateCluster (MenuCommand menuCommand) {
		GameObject go = new GameObject("Waypoint Editor");
		GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
		go.AddComponent<WaypointCluster>();
		Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
		Selection.activeObject = go;
	}
}