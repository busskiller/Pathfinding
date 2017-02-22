using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Steering : MonoBehaviour {

    public Transform goal;
    public float moveSpeed = 5f;

    public float pursuitWeight = 0.001f;
    public float seperationWeight = 1000.0f;
    public float cohesionWeight = 0.001f;
    public float alignmentWeight = 0.0001f;
    public float avoidanceWeight = 1000.0f;

    private Vector3 averageNeighbourPosition;
    private Vector3 averageNeighbourDirection;
    private int numberOfNearbyFriendlies;
    private Collider[] nearbyObjects;
    private RaycastHit hitInfo;

	
	void FixedUpdate () {
        SteeringBehaviour();

    }

    private void SteeringBehaviour()
    {

        //Setting up a start move vector, this is the one we will be adding to along the lines.
        Vector3 moveDir = Vector3.zero;

        // We add the seek vector to the combined steering vector.
        if (goal != null)
        {
            moveDir = ((goal.position + goal.forward) - transform.position).normalized * pursuitWeight;
        }


        //Collision detection
        nearbyObjects = Physics.OverlapSphere(transform.position, 15);

        averageNeighbourPosition = Vector3.zero;
        averageNeighbourDirection = Vector3.zero;
        numberOfNearbyFriendlies = 0;

        for (int i = 0; i < nearbyObjects.Length; i++)
        {
            if (nearbyObjects[i].gameObject.tag == "Friendlies" && nearbyObjects[i] != GetComponent<Collider>())
            {
                //We add up all the positions of nearby Friendlies to do proper cohesion.
                averageNeighbourPosition += nearbyObjects[i].transform.position;
                numberOfNearbyFriendlies++;

                //We add up all the directions of nearby Friendliess to do proper alignment.
                averageNeighbourDirection += nearbyObjects[i].transform.forward;

                //We add a vector moving away from each nearby neighbour.
                Vector3 offSet = nearbyObjects[i].transform.position - transform.position;
                moveDir += (offSet / -offSet.sqrMagnitude) * seperationWeight;
            }

        }

        //We divide by total number of Friendlies to get average position and direction.
        if (numberOfNearbyFriendlies > 0)
        {
            //We divide by total number of Face Happies to get average position and direction.
            averageNeighbourPosition /= numberOfNearbyFriendlies;
            averageNeighbourDirection /= numberOfNearbyFriendlies;

            moveDir += (averageNeighbourPosition - transform.position).normalized * cohesionWeight;
            moveDir += averageNeighbourDirection.normalized * alignmentWeight;
        }

        //We check 5 length units in front of us for collisions, if one occur we take the necessary steps.
        if (Physics.SphereCast(new Ray(transform.position + moveDir.normalized * 1.6f, moveDir), 1f, out hitInfo, 3))
        {
            if (hitInfo.transform.gameObject.name.Contains("KillSphere"))
            {
                Vector3 vectorToCenterOfObstacle = hitInfo.transform.position - transform.position;
                moveDir -= Vector3.Project(vectorToCenterOfObstacle, transform.right).normalized * (1f / vectorToCenterOfObstacle.magnitude) * avoidanceWeight;
            }
        }

        //We cap the movement vector at moveSpeed, to avoid unnaturally fast Friendliess
        moveDir = moveDir.normalized * moveSpeed;

        //Time.fixedDeltaTime is multiplied with the movement to make the movement frame dependant
        //Tis means  that cases with higher frame rate won't experience faster agents.
        transform.position += moveDir * Time.fixedDeltaTime;

        //Make sure he faces the right way.
        if (moveDir != Vector3.zero) {
            transform.forward = moveDir;
        }

        //Safety to ensure that they don't leave earth, this is needed due to small calculation-mistakes' tendency to add up.
        transform.position = new Vector3(transform.position.x, transform.position.y, 0 );
    }

}
