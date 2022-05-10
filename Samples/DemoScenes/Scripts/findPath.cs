using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class findPath : MonoBehaviour {

    public List<Waypoint> route;

    public Waypoint start;
    public Waypoint end;

    public float speed = 1f;
    private Transform t;

    /* Initialization */
    void Start ()
    {
        route = findRouteTo(start, end);
        t = this.gameObject.transform;
    }

    /* Move the square */
    int i = 0;
	void Update () {
        //leave in case we reached our destination
        if (i >= route.Count) return;
        //Distance between the square and the current target waypoint
        float distance = Vector3.Distance(t.position, route[i].transform.position);
        //Move the square towards the current target
        t.position = Vector3.Lerp(t.position, route[i].transform.position, speed * Time.deltaTime / distance);
        //In case the square arrived to the target waypoint (very small distance)
        if (distance < 0.1)
            //Change the current target to the next defined waypoint
            i++;
	}

    public List<Waypoint> findRouteTo(Waypoint Start, Waypoint End) {
        //We will store the path through a dictionary to keep track of where we came from to that point
        //and to keep track of the visited waypoints
        Dictionary<Waypoint, Waypoint> d= new Dictionary<Waypoint, Waypoint>();
        //first we save the root as visited in our dictionary
        d.Add(Start, null);

        //BFS to find the path with least nodes in between to our target
        Queue<Waypoint> q = new Queue<Waypoint>();
        q.Enqueue(Start);
        while (q.Count > 0) {
            Waypoint current = q.Dequeue();
            if (current == null)
                continue;
            foreach(WaypointPercent w in current.outs) {
                if (!d.ContainsKey(w.waypoint)) {
                    q.Enqueue(w.waypoint);
                    d.Add(w.waypoint, current);
                }
            }
        }

        //Now we have to translate from dictionary to list
        List<Waypoint> result = new List<Waypoint>();
        //start retrieving the path from the end
        Waypoint i = End;
        //until we find a node without origin (the starting node)
        while (i != null) {
            //insert at the beggining of the list
            result.Insert(0, i);
            //go to the next node
            i = d[i];
        }

        return result;
    }

}
