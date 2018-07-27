using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[ExecuteInEditMode]
public class Waypoint : MonoBehaviour {
	[SerializeField]
	[HideInInspector] 
	/// <summary>
	/// Parent graph of the waypoint
	/// </summary>
	protected WaypointCluster parent;
	
	/// <summary>
	/// The outgoing list of edges
	/// </summary>
	public List<WaypointPercent> outs = new List<WaypointPercent>(); 
    
	[HideInInspector]
	/// <summary>
	/// Incoming list of edges, hidden in the inspector
	/// </summary>
    public List<Waypoint> ins = new List<Waypoint>();				

	/// <summary>
	/// Main node color
	/// </summary>
	Color color = Color.white;	

	public void setParent(WaypointCluster wc) { parent = wc; }

	public WaypointCluster getParent() { return parent;}

	/// <summary>
	/// Returns a random waypoint based on the probabilty defined in the WaypointPercent class
	/// </summary>
	/// <returns>Returns a random waypoint based on the chances set in the edges</returns>
	public Waypoint getNextWaypoint() {
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
	
	/// <summary>
	/// Sets the probabilities to choose each path to the same for each outgoing edge (100% in total).
	/// </summary>
	public void setSame() {
		int size = outs.Count;
		for (int i =0; i < size; ++i) {
			outs[i].probability = 100/size;
			if (i<100%size) outs[i].probability++;
		}
	}

	/// <summary>
	/// Links this waypoint (directionally) with the passed waypoint and sets the probabilities of all edges to the same
	/// </summary>
	/// <param name="node"> Node to be linked to</param>
	public void linkTo(Waypoint waypoint) {
		if (waypoint == this) {
			Debug.LogError("A waypoint cannot be linked to itself");
			return;
		}
		for (int i =0; i < outs.Count; ++i) if (waypoint == outs[i].waypoint) return;
		if (waypoint.ins.Contains(this)) return;
		outs.Add(new WaypointPercent(waypoint));
		waypoint.ins.Add(this);
		setSame();
		parent.CreateWaypoint(Vector3.zero);
	}

	/// <summary>
	/// Removes a link (directionally) between this waypoiny and the passed waypoint and sets the probabilities of all edges to the same
	/// </summary>
	/// <param name="node"> Node to remove the link from</param>
	public void unlinkFrom(Waypoint waypoint) {
		for (int i = 0; i < outs.Count; ++i) if (outs[i].waypoint == waypoint) outs.RemoveAt(i);
		waypoint.ins.Remove(this);
	}

	/// <summary>
	/// Changes the main node color
	/// </summary>
	/// <param name="color"> New color of the node</param>
	public void setColor(Color color) {
		this.color = color;
	}

	/// <summary>
	/// Draws the arrow from position "pos" in the direction "dir"
	/// </summary>
	/// <param name="pos"> Starting position of the arrow</param>
	/// <param name="dir"> Direction of the arrow</param>
	/// <param name="color"> Color of the arrow</param>
	/// <param name="arrowHeadLength"> Length of the arrow head line segments</param>
	/// <param name="arrowHeadAngle"> Angle of opening of the arrow head line segments</param>
	private static void ForGizmo(Vector3 pos, Vector3 direction, Color c, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f) {
		Gizmos.color = c;
		Gizmos.DrawRay(pos, direction);
		Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0,180+arrowHeadAngle,0) * new Vector3(0,0,1);
		Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0,180-arrowHeadAngle,0) * new Vector3(0,0,1);
		Gizmos.DrawRay(pos + direction, right * arrowHeadLength);
		Gizmos.DrawRay(pos + direction, left * arrowHeadLength);
	}

	/// <summary>
	/// Draws the white square representing the nodes and an arrow for each outgoing edge
	/// </summary>
	public virtual void OnDrawGizmos() {
		Gizmos.color = color;
		Gizmos.DrawCube(transform.position, new Vector3(1.5f,1.5f,1.5f));
		for (int i = 0; i < outs.Count; ++i) {
			Vector3 direction = outs[i].waypoint.transform.position -transform.position;
			ForGizmo(transform.position+direction.normalized, direction -direction.normalized*2f, Color.red, 2f);
		}
		
		if (color.Equals(Color.green) || color.Equals(Color.white)) color = Color.white;
	}

	/// <summary>
	/// Draws the yellow square representing the selected node and a magenta arrow for each outgoing edge
	/// </summary>
	public virtual void OnDrawGizmosSelected() {
		Gizmos.color = Color.yellow;
		Gizmos.DrawCube(transform.position, new Vector3(1.5f,1.5f,1.5f));
		for (int i = 0; i < outs.Count; ++i) {
			Vector3 direction = outs[i].waypoint.transform.position-transform.position;
			ForGizmo(transform.position+direction.normalized, direction -direction.normalized*2f, Color.magenta, 2f);
		}
	}

	
	[ExecuteInEditMode]
	/// <summary>
	/// Handles the destruction of the waypoint in editor and play modes
	/// </summary>
	private void OnDestroy() {
		if (parent == null) return;
		for (int i = 0; i < outs.Count; ++i) Undo.RegisterCompleteObjectUndo(outs[i].waypoint, "destroyed");
		for (int i = 0; i < ins.Count; ++i) Undo.RegisterCompleteObjectUndo(ins[i], "destroyed");
		Undo.RegisterCompleteObjectUndo(this.getParent(), "destroyed");
		for (int i = outs.Count-1; i >= 0; --i) this.unlinkFrom(outs[i].waypoint);
		for (int i = ins.Count-1; i >= 0; --i) ins[i].unlinkFrom(this);
		Undo.RegisterCompleteObjectUndo(this, "destroyed");
		this.getParent().waypoints.Remove(this);
	}

}

[System.Serializable]
/// <summary>
/// Small class to represent the probability of edges
/// </summary>
public class WaypointPercent {
	[Range(0, 100)]
	/// <summary>
	/// Probability of choosing this edge, from 0 to 100
	/// </summary>
	public int probability = 0;
	[ReadOnly] 
	/// <summary>
	/// The waypoint this edge is linked to
	/// </summary>
	public Waypoint waypoint;

	/// <summary>
	/// Class constructor
	/// </summary>
	/// <param name="waypoint"> The waypoint to be linked to</param>
	public WaypointPercent(Waypoint waypoint) {
		this.waypoint = waypoint;
	}

}