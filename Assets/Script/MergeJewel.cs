using Microsoft.MixedReality.Toolkit.Input;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.UIElements;


public class MergeJewel : MonoBehaviour
{
    public GameObject leader = null;
    private int ID;


    public float limite = 1f;
    public bool isTriggered = false;
    /// <summary>
    /// Checks if they are still close, or do the need to be split
    /// </summary>

    void LateUpdate()
    {

        if (leader != null)
        {
            Vector3 localPosition = this.transform.position;
            Vector3 leaderPosition = leader.transform.position;

            int totalLeader = 0;
            int totalThis = 0;

            float distance = Vector3.Distance(localPosition, leaderPosition);
            if (distance > limite)
            {
                Debug.Log("Separate");

                //Séparer les objets
                this.transform.SetParent(null);
                //Check child of this
                List<GameObject> listThis = new List<GameObject>();
                leader.gameObject.GetChildGameObjects(listThis);
                foreach (GameObject go in listThis)
                {
                    if (go.tag.Equals("jewel") || go.tag.Equals("leader"))
                    {
                        Debug.Log(go.name);
                        totalThis++;
                    }
                }
                if(totalThis > 0) { this.tag = "leader"; } else { this.tag = "jewel"; }

                //Check child of leader
                List<GameObject> listLeader = new List<GameObject>();
                leader.gameObject.GetChildGameObjects(listLeader);
                foreach(GameObject go in listLeader)
                {
                    if(go.tag.Equals("jewel") || go.tag.Equals("leader"))
                    {
                        Debug.Log(go.name);
                        totalLeader++;
                    }

                }

                if(totalLeader <= 0) //no child left he become a simple jewel
                    leader.gameObject.tag = "jewel";
                leader = null;
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
        var pointerHandler = this.gameObject.AddComponent<PointerHandler>();
        pointerHandler.OnPointerClicked.AddListener((e) => onItemClick());
    }

    /// <summary>
    /// Gives the handmenu what is the last selected item
    /// </summary>
    public void onItemClick()
    {
        Debug.Log("OnItemClick, find selected Item");

        GameObject.FindWithTag("handMenu").GetComponent<MainMenu>().lastItem = this.gameObject;
    }

    /// <summary>
    /// Merge 2 "jewel" or "leader" when they enter in collision
    /// The merge depends on if they are already a leader or not
    /// </summary>
    /// <param name="other">the Collision Collider of the other GameObject it touched</param>
    // TODO ameliorer la fusion
    private void OnCollisionEnter(Collision other)
    {

        if (other.gameObject.CompareTag("jewel") || other.gameObject.CompareTag("leader") && this.gameObject.CompareTag("jewel") || this.gameObject.CompareTag("leader"))
        {
            Debug.Log("-----------Triggered by " + this.gameObject + "-------------------------");
            Debug.Log("Other " + other.gameObject.name);
            Debug.Log("this " + this.gameObject.name);
            Debug.Log(other.relativeVelocity);

            if(other.gameObject.CompareTag("jewel") && this.gameObject.CompareTag("jewel")) //Toujours dire que c'est le other qui devient le leader sinon on sait pas comment les séparer
            {
                Debug.Log("Merge to Jewel");
                if(this.leader != null && other.gameObject.GetComponent<MergeJewel>().leader != null) //C'est un peu bourré, je suis pas convaincu
                {
                    other.gameObject.GetComponent<MergeJewel>().leader.transform.SetParent(this.leader.transform);
                    other.gameObject.GetComponent<MergeJewel>().leader.GetComponent<MergeJewel>().leader = this.leader;
                    other.gameObject.GetComponent<MergeJewel>().leader.GetComponent<MergeJewel>().leader.tag = "jewel";
                }
                else if (this.leader != null) // Si il a un leader alors on le donne à l'autre
                {
                    other.transform.SetParent(this.leader.transform);
                }else if(other.gameObject.GetComponent<MergeJewel>().leader != null)
                {
                    this.transform.SetParent(other.gameObject.GetComponent<MergeJewel>().leader.transform);
                }else //Si aucun des deux n'as de leader -> un devient le leader
                {
                    other.gameObject.tag = "leader";
                    this.gameObject.tag = "jewel";
                    leader = other.gameObject;
                    this.transform.SetParent(other.transform);
                }
                return;

            }

            if(other.gameObject.CompareTag("leader"))
            {
                Debug.Log("Merge to leader");
                other.gameObject.tag = "leader";
                this.gameObject.tag = "jewel";
                leader = other.gameObject;
                this.transform.SetParent(other.transform);
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
