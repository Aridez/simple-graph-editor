using UnityEngine;
using System.Collections;

public class randomPath : MonoBehaviour {

    public WayPoint target = null;
    public float speed = 1f;
    private Transform t;

    /* Initialization */
    void Start ()
    {
        t = this.gameObject.transform;
    }

    /* Move the square */
	void Update () {
        //Distance between the square and the current target waypoint
        float distance = Vector3.Distance(t.position, target.transform.position);
        //Move the square towards the current target
        t.position = Vector3.Lerp(t.position, target.transform.position, speed * Time.deltaTime / distance);
        //In case the square arrived to the target waypoint (very small distance)
        if (distance < 0.1)
            //Change the current target to the next waypoint
            //The getNextWaypoint function will automatically return the next waypoint to go to
            //according to the probabilities defined
            target = target.getNextWaypoint();
	}
}
