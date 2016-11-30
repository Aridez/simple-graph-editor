using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaypointCluster : MonoBehaviour {

	[HideInInspector] public List<WayPoint> waypoints = new List<WayPoint>(); /**< List of ALL waypoints inside the cluster */
    [HideInInspector]
    public GameObject cluster = null;		 /**< Cluster object where the waypints will be added */
    private uint currentID = 0;              /**< Numeric id assigned to the waypoint */

	/** Removes all instances of a waypoint */
	public void remove(WayPoint w) {
		waypoints.Remove(w);
	}

	/*Creates a new waypoint*/
	public WayPoint CreateWaypoint(Vector3 point) {
		GameObject waypointAux = Resources.Load("Waypoint")  as GameObject;
		GameObject waypointInstance = Instantiate(waypointAux) as GameObject;
		waypointInstance.transform.position = point;
		waypointInstance.transform.parent = cluster.transform;
        waypointInstance.name = currentID.ToString();
        ++currentID;
		waypoints.Add(waypointInstance.GetComponent<WayPoint>());
		waypointInstance.GetComponent<WayPoint>().setParent(this);
		return waypointInstance.GetComponent<WayPoint>();
	}
	
	/* Creates a link between source and destiny */
	public static void link(WayPoint source, WayPoint destiny) {
		source.addOutWayPoint(destiny);
	}
}
