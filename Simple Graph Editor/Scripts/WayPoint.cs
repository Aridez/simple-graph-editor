using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]

public class WayPoint : MonoBehaviour {
	[SerializeField][HideInInspector] protected WaypointCluster parent;
	
	public List<WayPointPercent> outs = new List<WayPointPercent>();    //References to waypoints that are principal choices.
    [HideInInspector]
    public List<WayPoint> ins = new List<WayPoint>();					//Refeences to waypoints that have this one as a principal choice.

		/**************************FUNCTIONS***********************************/

	/*	Function assigns  the next waypoint on demand to cars.
	*	The current waypoint will have one less car incoming.
	*	The waypoint selected will have one more car coming.
	*/
	public WayPoint getNextWaypoint(bool substract = true) {
		int prob = Random.Range(0, 100);
		int sum = 0;
		for (int i = 0; i < outs.Count; ++i) {
			sum += outs[i].probability;
			if (prob <= sum) {
				return outs[i].waypoint;
			}
		}
		Debug.LogError("Last waypoint was returned on waypoint " + this.name + ". Check that the probabilities correctly sum at least 100 on that waypoint.");
		return outs[outs.Count-1].waypoint;
	}

	/**************************EDITOR FUNCTIONS***********************************/

	/** Sets the WaypointCluster object this waypoint pertains to */
	public void setParent(WaypointCluster wc) { parent = wc; }

	/** Returns the WaypointCluster object this waypoint pertains to */
	public WaypointCluster getParent() { return parent;}

	/*	Sets the probabilities to choose each path to the same for each (100% in total).
	*	Called every time a waypoint is added/removed to keep it consistent.
	*/
	public void setSame() {
		int size = outs.Count;
		for (int i =0; i < size; ++i) {
			outs[i].probability = 100/size;
			if (i<100%size) outs[i].probability++;
		}
	}

	/**ADD/REMOVAL OF LINKS FUNCTIONS**/

	/* 	Adds an "out" (red line) waypoint reference.
	*	Adds the correspondent "in" reference to the target wapyoint.
	*	Sets the remaining wayoints to the same probability.
	*/
	public void addOutWayPoint(WayPoint waypoint) {
		for (int i =0; i < outs.Count; ++i) if (waypoint == outs[i].waypoint || waypoint == this) return;
		outs.Add(new WayPointPercent(waypoint, 0));
		
		setSame();
	}
	/* 	Remmoves target waypoint from the "outs" list.
	*	Sets the ramaining waypoints to the same probability.
	*/
	public void removeOutWayPoint(WayPoint waypoint) {
		for (int i = 0; i < outs.Count; ++i) if (outs[i].waypoint == waypoint) outs.RemoveAt(i);
		setSame();
	}
	/** Adds the waypoint to the "ins" list if it does not exist already*/
	public void addInWayPoint(WayPoint waypoint) {
		if (!ins.Contains(waypoint) && waypoint != this) ins.Add(waypoint);
	}
	/** Removes the waypoint from the "ins" list */
	public void removeInWayPoint(WayPoint waypoint) {

		ins.Remove(waypoint);
	}
	/* 	Adds an "alternativesOut" (blue line) waypoint reference.
	*	Adds the correspondent "alternativesIn" reference to the target wapyoint.
	*/

	//GUI SHOWED IN EDITOR MODE
	Color color = Color.white;
	public virtual Color mainColor() { return Color.white; }
	public void setColor(Color color) {
		this.color = color;
	}

	private static void ForGizmo(Vector3 pos, Vector3 direction, Color c, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f) {
		Gizmos.color = c;
		Gizmos.DrawRay(pos, direction);
		Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0,180+arrowHeadAngle,0) * new Vector3(0,0,1);
		Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0,180-arrowHeadAngle,0) * new Vector3(0,0,1);
		Gizmos.DrawRay(pos + direction, right * arrowHeadLength);
		Gizmos.DrawRay(pos + direction, left * arrowHeadLength);
	}

	public virtual void OnDrawGizmos() {
		
		Gizmos.color = color;
		Gizmos.DrawCube(transform.position, new Vector3(1.5f,1.5f,1.5f));
		for (int i = 0; i < outs.Count; ++i) {
			Vector3 direction = outs[i].waypoint.transform.position -transform.position;
			ForGizmo(transform.position+direction.normalized, direction -direction.normalized*2f, Color.red, 2f);
		}
		
		if (color.Equals(Color.green) || color.Equals(Color.white)) color = mainColor();
	}

	public virtual void OnDrawGizmosSelected() {
		Gizmos.color = Color.yellow;
		Gizmos.DrawCube(transform.position, new Vector3(1.5f,1.5f,1.5f));
		for (int i = 0; i < outs.Count; ++i) {
			Vector3 direction = outs[i].waypoint.transform.position-transform.position;
			ForGizmo(transform.position+direction.normalized, direction -direction.normalized*2f, Color.magenta, 2f);
		}
		
		//Gizmos.color = Color.green;
		//Gizmos.DrawWireSphere(transform.position, actionDistance);
	}

}

[System.Serializable]
public class WayPointPercent {
	[Range(0, 100)]
	public int probability;
	[ReadOnly] public WayPoint waypoint;

	public WayPointPercent(WayPoint waypoint, int percent) {
		this.waypoint = waypoint;
		this.probability = percent;
	}

}