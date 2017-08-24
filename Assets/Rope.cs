using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public Vector3 currentPosition;
    public Vector3 lastPosition;
    public bool isFrozen;
}

public class Rope : MonoBehaviour {

    [SerializeField]

    public int numberOfNodes;
    public List<Node> nodes = new List<Node>();

    public float dampen;
    public float distance;

    public LineRenderer rope;

    public Vector3 acceleration;

    // Use this for initialization
    void Start ()
    {
        for (int i = 0; i < numberOfNodes; i++)
        {
            nodes.Add(new Node());
            nodes[i].currentPosition = transform.position + Vector3.down * numberOfNodes * i;
            nodes[i].lastPosition = transform.position + Vector3.down * numberOfNodes * i;
        }

        rope = GetComponent<LineRenderer>();

        nodes[0].isFrozen = true;

        rope.positionCount = nodes.Count;
    }

    void FixedUpdate()
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            if (i != 0)
            {
                Restraint(nodes[i], nodes[i - 1]);
            }
        }
        Verlet(acceleration);
    }

    void Restraint(Node node1, Node node2)
    {
        float differenceBetweenX = node1.currentPosition.x - node2.currentPosition.x;
        float differenceBetweenY = node1.currentPosition.y - node2.currentPosition.y;
        float differenceBetweenZ = node1.currentPosition.z - node2.currentPosition.z;

        Vector3 direction = (node1.currentPosition - node2.currentPosition).normalized;

        float distanceBetween = Vector3.Distance(node1.currentPosition, node2.currentPosition);
        float difference = ((distance - distanceBetween) / distanceBetween);

        float translateNodeX = differenceBetweenX * 0.5f * difference;
        float translateNodeY = differenceBetweenY * 0.5f * difference;
        float translateNodeZ = differenceBetweenZ * 0.5f * difference;

        node1.currentPosition = new Vector3(node1.currentPosition.x + translateNodeX, node1.currentPosition.y + translateNodeY, node1.currentPosition.z + translateNodeZ);
        node2.currentPosition = new Vector3(node2.currentPosition.x + translateNodeX, node2.currentPosition.y + translateNodeY, node2.currentPosition.z + translateNodeZ);
    }

    void Verlet(Vector3 ropeAcceleration)
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            if (!nodes[i].isFrozen)
            {
                Node currentNode = nodes[i];

                Vector3 currentVelocity = (currentNode.currentPosition - currentNode.lastPosition) * dampen;

                currentNode.lastPosition = currentNode.currentPosition;
                currentNode.currentPosition += (currentVelocity + ropeAcceleration * (Time.fixedDeltaTime * Time.fixedDeltaTime));

                rope.SetPosition(i, nodes[i - 1].currentPosition);
            }
        }
    }
}
