using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;


public class MergeJewel : MonoBehaviour
{
    private GameObject leader;
    private int ID;


    public float limite = 1f;
    /// <summary>
    /// Checks if they are still close, or do the need to be split
    /// </summary>

    void LateUpdate()
    {

        if (leader != null)
        {
            Vector3 localPosition = this.transform.position;
            Vector3 leaderPosition = leader.transform.position;

            float distance = Vector3.Distance(localPosition, leaderPosition);
            if (distance > limite)
            {
                Debug.Log("Separate");
                //Séparer les objets
                this.transform.SetParent(null);
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

        if (other.gameObject.CompareTag("jewel") || other.gameObject.CompareTag("leader"))
        {
            /* Transforme 2 objet en 1 nouveau */
            if (ID < other.gameObject.GetComponent<MergeJewel>().ID) { return; }
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

            //todo look in children if their is a leader???
        }

    }

}
