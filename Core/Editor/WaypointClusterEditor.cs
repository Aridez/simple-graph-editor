/**
 * @file WaypointClusterEditor.cs
 * @author Eric Mourin
 * @date 12 Aug 2015
 * @brief Custom editor to handle waypoint placement.
 */
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(WaypointCluster))]
public class WaypointClusterEditor : Editor {

	/// <summary>
	/// Editing state, changed with buttons
	/// </summary>
	enum EditingState 	{
		/// <summary>
		/// Not currently editing
		/// </summary>
		None,
		/// <summary>
		/// Adding principal link arrows (red)
		/// </summary>
		Adding,
		/// <summary>
		/// Removing links between waypoints
		/// </summary>
        Removing
    };

	/// <summary>
	/// Editing state, changed with buttons
	/// </summary>
	private WaypointCluster clusterobject;			//Object being edited.
		/// <summary>
	/// Editing state, changed with buttons
	/// </summary>
	private bool dragging = false;					//True if the user is dragging the mouse.
	/// <summary>
	/// Editing state, changed with buttons
	/// </summary>
	private Waypoint waypointClicked;				//waypoint clicked, if any.
	/// <summary>
	/// Editing state, changed with buttons
	/// </summary>
	private Waypoint waypointDestiny;				//waypoint the mouse is over, if any.
	/// <summary>
	/// Editing state, changed with buttons
	/// </summary>
	private EditingState state = EditingState.None;	//Current editing state.
	/// <summary>
	/// Editing state, changed with buttons
	/// </summary>
	private string currentState = "Not editing.";	//Text shown in the inspector label.
	/// <summary>
	/// Editing state, changed with buttons
	/// </summary>
	private GameObject previewSphere;				//The small white preview sphere.
	/// <summary>
	/// Editing state, changed with buttons
	/// </summary>
	int hash = "ClusterControl".GetHashCode();		//hash used to keep the focus on the current object
	
	/// <summary>
	/// Editing state, changed with buttons
	/// </summary>
	void OnEnable() {
		clusterobject = (WaypointCluster)target;
	}

	/// <summary>
	/// GUI showed on the inspector, some buttons were added to change the editing state and a label to show the current state.
	/// </summary>
	public override void OnInspectorGUI(){

		DrawDefaultInspector ();
		if(GUILayout.Button("Add waypoints / edges")){
			state = EditingState.Adding;
			currentState = "Adding principal links.";
        } else if (GUILayout.Button("Remove edges"))
        {
            state = EditingState.Removing;
            currentState = "Removing links";
        }
        else if(GUILayout.Button("Turn off edit mode")){
			state = EditingState.None;
			currentState = "Not editing.";
		}
		EditorGUILayout.LabelField("Current state: " + currentState);
	}

	/// <summary>
	/// Scene event handling
	/// </summary>
	void OnSceneGUI() {
		Event current = Event.current;
		//note: current.button == 0 works only the frame you press it.
		if (state != EditingState.None) {
			if (dragging) movePreview();
			int ID = GUIUtility.GetControlID(hash, FocusType.Passive);

			switch(current.type) 
			{
				//For focus purposes
				case EventType.Layout:
					HandleUtility.AddDefaultControl(ID);
					break;

				//White sphere creation and dragging = true
				case EventType.MouseDown:
                    if (current.button == 1) //ignore right clicks, use them to cancel the dragging
                    {
                        dragging = false;
                        if(previewSphere != null) DestroyImmediate(previewSphere);
                    } else 	createPreview();
					break;

				//Point creation if necessary
				case EventType.MouseUp:
                    if (current.button == 1) break;
                    if (!dragging) break;
                    dragging = false;
					if (previewSphere != null) DestroyImmediate(previewSphere);
					Ray worldray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
					RaycastHit hitinfo;
					if (Physics.Raycast(worldray, out hitinfo)) {
                        if (state == EditingState.Adding) createLink(hitinfo);
                        else if (state == EditingState.Removing) removeLink(hitinfo);
					}
					waypointClicked = null;
					waypointDestiny = null;
					break;
			}
		}
	}

	/// <summary>
	/// Finds the cluster object
	/// Adds the desired waypoint, depending on the current state
	/// Places it in the hitpoint, as a cluster object child
	/// Gives it a unique number
	/// Adds it to the cluster object lists
	/// If it is source/sink is added to the pertinent special list too
	/// The cluster object is referenced in the waypoint
	/// </summary>
	private Waypoint createPoint(RaycastHit hitinfo) {
		if (clusterobject.cluster == null) clusterobject.cluster = new GameObject("Waypoint Cluster");
		GameObject waypointAux;
		Undo.RecordObject(clusterobject, "Created waypoint");
		waypointAux = Resources.Load("Waypoint")  as GameObject;
		GameObject waypointInstance = Instantiate(waypointAux) as GameObject;
		waypointInstance.transform.position = hitinfo.point;
		waypointInstance.transform.parent = clusterobject.cluster.transform;
		waypointInstance.name = (clusterobject.waypoints.Count + 1).ToString();
		clusterobject.waypoints.Add(waypointInstance.GetComponent<Waypoint>());
		waypointInstance.GetComponent<Waypoint>().setParent(clusterobject);
		Undo.RegisterCreatedObjectUndo (waypointInstance, "Created waypoint");
		return waypointInstance.GetComponent<Waypoint>();
	}
	
	/// <summary>
	/// a)Creates a link between two existing waypoints.
	/// b)Creates and links a new waypoint to an existing waypoint.
	/// c)Creates a new waypoiny with no links.
	/// </summary>
	private void createLink(RaycastHit hitinfo) {
		if (waypointClicked != null && waypointDestiny != null && waypointClicked != waypointDestiny) link (waypointClicked, waypointDestiny);
		else if (waypointClicked != null && waypointDestiny == null) link (waypointClicked, createPoint(hitinfo));
		else if (waypointClicked == null && waypointDestiny == null) createPoint(hitinfo);
	}

	/// <summary>
	/// a)Removes a link between two existing waypoints.
	/// </summary>
    private void removeLink(RaycastHit hitinfo)
    {
        if (waypointClicked != null && waypointDestiny != null && waypointClicked != waypointDestiny) unLink(waypointClicked, waypointDestiny);
    }

	/// <summary>
	/// Creates a link between source and destiny
	/// </summary>
    private void link(Waypoint source, Waypoint destiny) {
		Undo.RecordObject(source, "waypointadd");
		Undo.RecordObject(destiny, "waypointadd");
		source.linkTo(destiny);
	}

	/// <summary>
	/// Removes a link between source and destiny
	/// </summary>
    private void unLink(Waypoint source, Waypoint destiny)
    {
        Undo.RecordObject(source, "waypointremove");
        Undo.RecordObject(destiny, "waypointremove");
        source.unlinkFrom(destiny);
    }

	/// <summary>
	/// Creates the white sphere preview.
	/// If a waypoint is clicked then waypointClicked is set.
	/// </summary>
    private void createPreview() {
		Ray worldRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
		RaycastHit hitInfo;
		if (Physics.Raycast(worldRay, out hitInfo)) {
			dragging = true;
			previewSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			DestroyImmediate(previewSphere.GetComponent<SphereCollider>());
			previewSphere.transform.position = hitInfo.point;
			waypointClicked = hitInfo.transform.gameObject.GetComponent<Waypoint>();
		}
	}

	/// <summary>
	/// Moves the white sphere preview.
	/// If the previewsphere is null it is created again.
	/// If a waypointClicked is set then an arrow is created.
	/// If there is hovering over another wayopint waypointDestiny is set and pointed with the preview arrow.
	/// If there was no object hitted then the white sphete preview is destroyed.
	/// </summary>
	private void movePreview() {
		Ray worldRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
		RaycastHit hitInfo;
		if (Physics.Raycast(worldRay, out hitInfo)) {
			if (previewSphere == null) {
				previewSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
				DestroyImmediate(previewSphere.GetComponent<SphereCollider>());
			}
			previewSphere.transform.position = hitInfo.point;
			waypointDestiny = hitInfo.transform.gameObject.GetComponent<Waypoint>();
			if (waypointDestiny != null) waypointDestiny.setColor(Color.green);
			if (waypointClicked != null && waypointClicked != waypointDestiny) {
				if (waypointDestiny != null) DrawArrow.ForDebug(waypointClicked.transform.position, waypointDestiny.transform.position - waypointClicked.transform.position, (state == EditingState.Adding ? Color.red : Color.blue));
				else  if (previewSphere != null && waypointClicked != null) DrawArrow.ForDebug(waypointClicked.transform.position, previewSphere.transform.position - waypointClicked.transform.position, (state == EditingState.Adding ? Color.red : Color.blue)); 
			}
		} else if (previewSphere != null) DestroyImmediate(previewSphere);

	}
}
