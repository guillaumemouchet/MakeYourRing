using Microsoft.MixedReality.Toolkit.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.XR.CoreUtils;
using UnityEngine;


public class MergeJewel : MonoBehaviour
{
    public GameObject leader = null;
    private int ID;

    public float limite = 1f;

    /// <summary>
    /// Checks if they are still close, or do the need to be split
    /// </summary>
    void LateUpdate()
    {
       
        if (leader != null)
        {
            if(this.CompareTag("leader"))//shouldn't be true
            {
                leader = null;
                
            }else
            {

                //Calculer la distance par rapport à tous les éléments de notre leader et si le plus petit est plus grand que notre limite alors on sépare???

                Vector3 localPosition = this.transform.position;
                List<float> listDistance = new List<float>
                {
                    Vector3.Distance(localPosition, leader.transform.position)
                };

                List<GameObject> listLeader = new List<GameObject>();
                leader.gameObject.GetChildGameObjects(listLeader);
                foreach (GameObject go in listLeader)
                {
                    if (go.tag.Equals("jewel") || go.tag.Equals("leader"))
                    {
                        listDistance.Add(Vector3.Distance(localPosition, go.transform.position));
                    }
                }


                var itemToRemove = listDistance.SingleOrDefault(r => r == 0);
                if (itemToRemove != null)
                    listDistance.Remove(itemToRemove);

                float distance = listDistance.Min();
                if (distance > limite)
                {

                    int totalLeader = 0;
                    int totalThis = 0;
                    Debug.Log("Separate");

                    //Separete the items
                    this.transform.SetParent(null);

                    //Check child of this
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
                    //if this still have child he is a leader
                    if (totalThis > 0) { this.tag = "leader"; } else { this.tag = "jewel"; }

                    //Check child of leader -> recalculate without the removed element
                    listLeader = new List<GameObject>();
                    leader.gameObject.GetChildGameObjects(listLeader);
                    foreach (GameObject go in listLeader)
                    {
                        if (go.tag.Equals("jewel") || go.tag.Equals("leader"))
                        {
                            Debug.Log("Leader : " + go.name);
                            totalLeader++;
                        }

                    }

                    if (totalLeader <= 0) //no child left he become a simple jewel
                        leader.gameObject.tag = "jewel";
                    leader = null;
                }
            }
        }

    }

    /// <summary>
    /// Get the ID of the object
    /// Add an actionListener to know what is the last selected item
    /// </summary>
    void Start()
    {
        ID = GetInstanceID();
        //Create a listener to know what is the last selected item
        var pointerHandler = this.gameObject.AddComponent<PointerHandler>();
        pointerHandler.OnPointerDragged.AddListener((e) => onSelect());
        pointerHandler.OnPointerClicked.AddListener((e) => onSelectEnd());

    }

    /// <summary>
    /// Gives the handmenu what is the last selected item
    /// Ignore collision between moving items
    /// </summary>
    public void onSelect()
    {
        Debug.Log("Dragging Selection remove features");

        GameObject.FindWithTag("handMenu").GetComponent<MainMenu>().lastItem = this.gameObject;


        //Deactivate the children???
        //Check child of this
        List<GameObject> listThis = new List<GameObject>();
        this.gameObject.GetChildGameObjects(listThis);
        foreach (GameObject go in listThis)
        {
            if (go.tag.Equals("jewel") || go.tag.Equals("leader"))
            {
                Debug.Log("Ignore collision" + go.name);
                Physics.IgnoreCollision(this.GetComponent<Collider>(), go.GetComponent<Collider>(), true);
            }
        }
    }

    /// <summary>
    /// Restore interaction between moving items
    /// </summary>
    public void onSelectEnd()
    {
        Debug.Log("End Selection restore features");


        //Deactivate the children???
        //Check child of this
        List<GameObject> listThis = new List<GameObject>();
        this.gameObject.GetChildGameObjects(listThis);
        foreach (GameObject go in listThis)
        {
            if (go.tag.Equals("jewel") || go.tag.Equals("leader"))
            {
                Debug.Log("Restore collision" + go.name);
                Physics.IgnoreCollision(this.GetComponent<Collider>(), go.GetComponent<Collider>(), false);
            }
        }
    }

    /// <summary>
    /// Merge 2 "jewel" or "leader" when they enter in collision
    /// The merge depends on if they are already a leader or not
    /// </summary>
    /// <param name="other">the Collision Collider of the other GameObject it touched</param>
    private void OnCollisionEnter(Collision other)
    {

        if (other.gameObject.CompareTag("jewel") || other.gameObject.CompareTag("leader") && this.gameObject.CompareTag("jewel") || this.gameObject.CompareTag("leader"))
        {
            Debug.Log("-----------Triggered by " + this.gameObject + "-------------------------");
            Debug.Log("Other " + other.gameObject.name);
            Debug.Log("this " + this.gameObject.name);
            //Debug.Log(other.relativeVelocity);


            // JEWEL TRIGGER JEWEL
            if(other.gameObject.CompareTag("jewel") && this.gameObject.CompareTag("jewel")) //Toujours dire que c'est le other qui devient le leader sinon on sait pas comment les séparer
            {
                Debug.Log("Merge to Jewel");
                if(this.leader != null && other.gameObject.GetComponent<MergeJewel>().leader != null) //C'est un peu bourré, je suis pas convaincu
                {
                    Debug.Log(" TWO LEADER");
                    other.gameObject.GetComponent<MergeJewel>().leader.transform.SetParent(this.leader.transform);
                    other.gameObject.GetComponent<MergeJewel>().leader.GetComponent<MergeJewel>().leader = this.leader;
                    //other.gameObject.GetComponent<MergeJewel>().leader.GetComponent<MergeJewel>().leader.tag = "jewel";
                }
                else if (this.leader != null) // Si il a un leader alors on le donne à l'autre
                {
                    Debug.Log("THIS IS NOT NULL " + this.leader.name);

                    other.gameObject.GetComponent<MergeJewel>().leader = this.leader;
                    other.transform.SetParent(this.leader.transform);
                }else if(other.gameObject.GetComponent<MergeJewel>().leader != null)
                {
                    Debug.Log("OTHER IS NOT NULL" + other.gameObject.GetComponent<MergeJewel>().leader.name);
                    this.leader = other.gameObject.GetComponent<MergeJewel>().leader;
                    this.transform.SetParent(other.gameObject.GetComponent<MergeJewel>().leader.transform);
                }else //Si aucun des deux n'as de leader -> un devient le leader
                {
                    Debug.Log("NONE");
                    other.gameObject.tag = "leader";
                    this.gameObject.tag = "jewel";
                    leader = other.gameObject;
                    this.transform.SetParent(other.transform);
                }
                return;

            }
            
            //LEADER TRIGGER LEADER
            if(other.gameObject.CompareTag("leader") && this.gameObject.CompareTag("leader"))
            {
                Debug.Log("Merge to leader");
                other.gameObject.tag = "leader";
                this.gameObject.tag = "jewel";
                leader = other.gameObject;
                this.transform.SetParent(other.transform);
                return;
            }

            //LEADER TRIGGER JEWEL OR JEWEL TRIGGER LEADER
            if(other.gameObject.CompareTag("leader"))
            {
                leader = other.gameObject;
                this.transform.SetParent(other.transform);
                return;
            }else if(this.gameObject.CompareTag("leader"))
            {
                other.gameObject.GetComponent<MergeJewel>().leader = this.gameObject;
                other.transform.SetParent(this.transform);
                return;
            }
            /*Transforme 2 objet en 1 nouveau */
                /*

                Debug.Log("Merge this " + gameObject.name);
                Debug.Log("Merge other " + other.gameObject.name);

                if(other.gameObject.CompareTag("leader")) //other is already leader
                {
                    other.gameObject.tag = "leader";
                    leader = other.gameObject;
                    this.transform.SetParent(other.transform, true);
                }else if(this.gameObject.CompareTag("leader")) //this is the leader of the group
                {
                    leader = this.gameObject;
                    other.transform.SetParent(this.transform, true);
                }else if(other.gameObject.CompareTag("jewel")) //aucun des deux n'est leader
                {
                    if (leader == null)
                    {
                        other.gameObject.tag = "leader";
                        leader = other.gameObject;
                        this.transform.SetParent(other.transform, true);
                    }else
                    {
                        leader.tag = "leader";
                        this.transform.SetParent(leader.transform,true);
                        other.transform.SetParent(leader.transform, true);
                    }
                }
                */
                //todo look in children if their is a leader???
        }

    }

}
