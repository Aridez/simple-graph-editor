using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaypointCluster : MonoBehaviour {

	[HideInInspector] 
	/// <summary>
	/// List of ALL waypoints inside the cluster
	/// </summary>
	public List<Waypoint> waypoints = new List<Waypoint>();
    [HideInInspector]
	/// <summary>
	/// Cluster object where the waypints will be added
	/// </summary>
    public GameObject cluster = null;
	/// <summary>
	/// Numeric id assigned to the waypoint
	/// </summary>
    private uint currentID = 0;

	/// <summary>
	/// Creates a new waypoint
	/// </summary>
	/// <param name="point"> The point where the waypoint will be created</param>
	/// <returns>Returns the created waypoint</returns>  
	public Waypoint CreateWaypoint(Vector3 point) {
		GameObject waypointAux = Resources.Load("Waypoint")  as GameObject;
		GameObject waypointInstance = Instantiate(waypointAux) as GameObject;
		waypointInstance.transform.position = point;
		waypointInstance.transform.parent = cluster.transform;
        waypointInstance.name = currentID.ToString();
        ++currentID;
		waypoints.Add(waypointInstance.GetComponent<Waypoint>());
		waypointInstance.GetComponent<Waypoint>().setParent(this);
		return waypointInstance.GetComponent<Waypoint>();
	}
	
}
