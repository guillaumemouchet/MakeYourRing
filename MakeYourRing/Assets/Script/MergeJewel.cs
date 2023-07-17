using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Unity.XR.CoreUtils;
using UnityEngine;

/*=============================================================================
 |	    Project:  MakeYourRing Travail de Bachelor
 |
 |       Author:  Guillaume Mouchet - ISC3il-b
 |
 *===========================================================================*/

public class MergeJewel : MonoBehaviour
{
    /*=============================================================================
    |                               Attributes
    *===========================================================================*/
    public GameObject follower = null;
    public GameObject leader = null;

    private GameObject instance;
    private bool keepWordPosition = true;
    private List<GameObject> ignoredCollisions = new List<GameObject>();
    private Vector3 spherePosition = Vector3.zero;
    private float radius = 0.2f;
    private RigidbodyConstraints freezePosition =
        RigidbodyConstraints.FreezePositionX 
        | RigidbodyConstraints.FreezePositionY 
        | RigidbodyConstraints.FreezePositionZ;



    /*=============================================================================
    |                               Unity Public Functions
    *===========================================================================*/
    /// <summary>
    /// Checks if the sphere of this GameObject collide with another
    /// It separate them if they are too far
    /// It creates a follower to indicate if it's a leader or not
    /// </summary>
    void FixedUpdate()
    {
        calculateCenter();
        //Create an Follower to shoe the player where is the leader
        if (this.CompareTag("leader") && follower == null)
        {
            follower = Instantiate(instance, this.GetComponentInChildren<MeshRenderer>().bounds.center, new Quaternion(0, 0, 0, 0), null);
            EnableFollower();
        }
        else if (this.CompareTag("jewel"))
        {
            Destroy(follower);
            follower = null;
        }

        //Find the object in range of the sphere

        List<Collider> ObjectInRange = Physics.OverlapSphere(spherePosition, radius,
        LayerMask.GetMask("Jewel")).ToList<Collider>();

        //Skip the current gameObject
        ObjectInRange.Remove(ObjectInRange.SingleOrDefault(r => r.gameObject == this.gameObject));

        //Separate the item if they are not in range
        //Reassign tag depending on the number of children
        bool inRange = false;
        // We check if the leader is still in the range of this
        if (leader != null)
        {
            //We can't take the size, since we need only the item that are Leader or Jewel
            int totalLeader = 0;
            int totalThis = 0;

            //Check if we are stil in range 
            List<GameObject> listLeader = new List<GameObject>();
            leader.gameObject.GetChildGameObjects(listLeader);
            foreach (Collider collider in ObjectInRange)
            {
                foreach (GameObject go in listLeader)
                {
                    if (go == collider.gameObject) inRange = true; //The children of the leader are in range of this, we stay attached
                }
                if (collider.gameObject == leader) inRange = true; //The leader is still in range of this, we stay attached
            }

            if (!inRange && this.leader != GameObject.FindWithTag("handMenu").GetComponent<MainMenu>().lastItem) // don't separate if the moving object is his leader
            {

                Debug.Log("Separate in the gameobject " + this.name);

                //Separete the items
                this.transform.SetParent(null, keepWordPosition);

                //Check number of children of this
                List<GameObject> listThis = new List<GameObject>();
                this.gameObject.GetChildGameObjects(listThis);
                foreach (GameObject go in listThis)
                {
                    if (go.tag.Equals("jewel") || go.tag.Equals("leader"))
                    {
                        Debug.Log("This : " + go.name);
                        totalThis++;
                    }
                }

                //Checks the numbers of childrne of the leader
                foreach (GameObject go in listLeader)
                {
                    if (go.tag.Equals("jewel") || go.tag.Equals("leader"))
                    {
                        Debug.Log("Leader : " + go.name);
                        totalLeader++;
                    }
                }

                //need to recalculate with the removed element
                totalLeader--;

                //if they still have a child they are leaders
                this.tag = totalThis > 0 ? "leader" : "jewel";

                this.leader.tag = totalLeader > 0 ? "leader" : "jewel";

                //finish the separation
                this.leader = null;

            }

        }

        //Fuze the item if they are in range
        foreach (Collider collider in ObjectInRange)
        {
            Merge(collider);
        }

    }

    /// <summary>
    /// Add an actionListener to know what is the last selected item
    /// </summary>
    void Start()
    {
        instance = GameObject.Find("Leader_Follower");
        //Create a listener to know what is the last selected item
        var pointerHandler = this.gameObject.AddComponent<PointerHandler>();

        pointerHandler.OnPointerDragged.AddListener((e) => OnSelect());
        pointerHandler.OnPointerClicked.AddListener((e) => OnSelectEnd());

    }

    /*=============================================================================
    |                               Unity Private Functions
    *===========================================================================*/
    /// <summary>
    /// Calculate the center of the sphere and draw the collider of the object
    /// </summary>
    private void OnDrawGizmos()
    {
        calculateCenter();
        Gizmos.DrawWireSphere(spherePosition, radius);
    }

    /*=============================================================================
    |                               Public Functions
    *===========================================================================*/
    /// <summary>
    /// Set the follower activ and change his position to the new one
    /// </summary>
    public void EnableFollower()
    {
        if (this.CompareTag("leader") && follower != null) //update position of the follower
        {
            follower.GetComponent<DirectionalIndicatorModified>().enabled = true;

            follower.GetComponent<DirectionalIndicatorModified>().DirectionalTarget = this.GetComponentInChildren<MeshRenderer>().bounds.center;
        }
    }
    /*=============================================================================
    |                               Private Functions
    *===========================================================================*/


    private void calculateCenter()
    {
        Vector3 child = this.GetComponentInChildren<MeshRenderer>().bounds.center;
        MeshRenderer currentMesh = this.GetComponentInChildren<MeshRenderer>();
        this.TryGetComponent<MeshRenderer>(out currentMesh);
        spherePosition = currentMesh != null ? currentMesh.bounds.center : child;


        Renderer rend = this.GetComponentInChildren<MeshRenderer>();
        radius = (rend.bounds.size.x + rend.bounds.size.y + rend.bounds.size.z) / 3;
    }

    /// <summary>
    /// Check if two GameObjects are in the same hierarchy
    /// </summary>
    /// <param name="other">Object to check with this</param>
    /// <returns type="bool">true if they have the same root</returns>
    private bool IsRelated(GameObject other)
    {
        return other.transform.root == this.transform.root;
    }

    /// <summary>
    /// Merge 2 "jewel" or "leader" when they enter in collision
    /// The merge depends on their tag
    /// </summary>
    /// <param name="other">the Collision Collider of the other GameObject it touched</param>
    private void Merge(Collider other)
    {

        if (IsRelated(other.gameObject)) { /*Debug.Log("They are Related ");*/ return; };
        {
            // JEWEL TRIGGER JEWEL
            if (other.gameObject.CompareTag("jewel") && this.gameObject.CompareTag("jewel"))
            {
                //They both have a leader
                if (this.leader != null && other.gameObject.GetComponent<MergeJewel>().leader != null)
                {
                    other.gameObject.GetComponent<MergeJewel>().leader.GetComponent<Rigidbody>().constraints = freezePosition;

                    //One leader become the leader of the other
                    other.gameObject.GetComponent<MergeJewel>().leader.GetComponent<MergeJewel>().leader = this.leader;
                    other.gameObject.GetComponent<MergeJewel>().leader.transform.SetParent(this.leader.transform, keepWordPosition);
                    other.gameObject.GetComponent<MergeJewel>().leader.tag = "jewel";
                    StartCoroutine(RemoveConstraints(other.gameObject.GetComponent<MergeJewel>().leader.GetComponent<Rigidbody>()));

                    return;
                }
                else if (this.leader != null) //Only "this" have a leader -> other become the children
                {

                    other.GetComponent<Rigidbody>().constraints = freezePosition;

                    other.gameObject.GetComponent<MergeJewel>().leader = this.leader;
                    other.transform.SetParent(this.leader.transform, keepWordPosition);

                    StartCoroutine(RemoveConstraints(other.GetComponent<Rigidbody>()));


                    return;

                }
                else if (other.gameObject.GetComponent<MergeJewel>().leader != null) //Only "other" have a leader -> "this" become the children
                {
                    this.GetComponent<Rigidbody>().constraints = freezePosition;

                    this.leader = other.gameObject.GetComponent<MergeJewel>().leader;
                    this.transform.SetParent(other.gameObject.GetComponent<MergeJewel>().leader.transform, keepWordPosition);

                    StartCoroutine(RemoveConstraints(this.GetComponent<Rigidbody>()));

                    return;


                }
                else //None of them have leaders -> other become the leader
                {
                    this.GetComponent<Rigidbody>().constraints = freezePosition;

                    other.gameObject.tag = "leader";
                    this.gameObject.tag = "jewel";
                    leader = other.gameObject;
                    this.transform.SetParent(other.transform, keepWordPosition);

                    StartCoroutine(RemoveConstraints(this.GetComponent<Rigidbody>()));

                    return;

                }

            }

            //LEADER TRIGGER LEADER
            if (other.gameObject.CompareTag("leader") && this.gameObject.CompareTag("leader")) //Both of them are leaders -> "this" becomes the child
            {
                this.GetComponent<Rigidbody>().constraints = freezePosition;

                other.gameObject.tag = "leader";
                this.gameObject.tag = "jewel";
                leader = other.gameObject;

                this.transform.SetParent(other.transform, keepWordPosition);

                StartCoroutine(RemoveConstraints(this.GetComponent<Rigidbody>()));

                return;
            }

            //LEADER TRIGGER JEWEL OR JEWEL TRIGGER LEADER
            if (other.gameObject.CompareTag("leader")) // Other is already the leader and "this" is already a child
            {
                this.GetComponent<Rigidbody>().constraints = freezePosition;

                this.leader = other.gameObject;

                this.transform.SetParent(other.transform, keepWordPosition);

                StartCoroutine(RemoveConstraints(this.GetComponent<Rigidbody>()));

                return;
            }
            else if (this.gameObject.CompareTag("leader"))// this is already the leader and other is already a child
            {
                other.GetComponent<Rigidbody>().constraints = freezePosition;

                other.gameObject.GetComponent<MergeJewel>().leader = this.gameObject;

                other.transform.SetParent(this.transform, keepWordPosition);

                StartCoroutine(RemoveConstraints(other.GetComponent<Rigidbody>()));

                return;
            }
        }
    }

    /// <summary>
    /// Coroutine that remove the Constraints on our object after 0.7 seconds to allow it to move 
    /// </summary>
    /// <param name="obj">RigidBody of the child in the merge</param>
    private IEnumerator RemoveConstraints(Rigidbody obj)
    {
        // Wait a certain time
        yield return new WaitForSeconds(0.7f);

        // Remove the constraints to allow movements
        obj.constraints = RigidbodyConstraints.None;
    }

    /// <summary>
    /// Gives the handmenu the new last selected item
    /// Remove collision to avoid contact between objects in the same hierarchy
    /// </summary>
    private void OnSelect()
    {
        GameObject.FindWithTag("handMenu").GetComponent<MainMenu>().lastItem = this.gameObject;
        if (this.CompareTag("leader") && follower != null)
        {
            follower.GetComponent<DirectionalIndicatorModified>().enabled = false;
        }
        //Check child of this
        List<GameObject> listThis = new List<GameObject>();
        this.gameObject.GetChildGameObjects(listThis);
        foreach (GameObject child in listThis)
        {
            if (child.tag.Equals("jewel"))
            {
                if (ignoredCollisions.Contains(child)) { continue; }

                Physics.IgnoreCollision(this.GetComponent<Collider>(),
                    child.GetComponent<Collider>(), true);

                ignoredCollisions.Add(child);
            }
        }

        

    }

    /// <summary>
    /// Restore interaction between moving items
    /// Renable the follower with the new position
    /// </summary>
    private void OnSelectEnd()
    {
        //Check child of this
        foreach (GameObject child in ignoredCollisions)
        {
            if (child.tag.Equals("jewel"))
            {
                Physics.IgnoreCollision(this.GetComponent<Collider>(), child.GetComponent<Collider>(), false);
            }
        }
        ignoredCollisions.Clear();

        EnableFollower();

    }

}
