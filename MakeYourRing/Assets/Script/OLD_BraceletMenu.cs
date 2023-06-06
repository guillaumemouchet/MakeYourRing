using Dummiesman;
using Microsoft.MixedReality.Toolkit.UI;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class OLD_BraceletMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject braceletMenu;

    [SerializeField]
    private GameObject settingsMenu;


    public List<string> prefabString = new List<string>();

    private List<GameObject> buttonList = new List<GameObject>();

    void OnDisable()
    {
        // prefabString.Clear();

    }

    //Vu qu'il n'est pas possible (autre qu'en mode Editeur) d'ajouter des actionsListener sur les boutons,
    //Alors une 20 aine de boutons seront fait et auront un listener sur un nomber
    //Ils seront affiché selon la taille de la liste d'objet
    //TODO : Voir le chemin d'accès parce qu'il ne risque pas d'être global à tous les utilisateurs
    // Genre demander le chemin au début du projet
    /*
     * Solution pas optimbal mais à préciser dans les limiatations
     */
    void OnEnable()
    {
        //prefabString.Clear();


        ////Créer des boutons pour chaque prefab dans notre resources bracelet
        //string localPath = "/Resources/Bracelet/";
        //var globalPath = Application.dataPath + localPath;
        //Debug.Log(globalPath);
        //var info = new DirectoryInfo(globalPath);
        //var fileInfo = info.GetFiles("*.prefab").ToList<FileInfo>();
        //float i = 0;
        //float j = 0;
        //foreach (FileInfo obj in fileInfo)
        //{
        //    string name = Path.GetFileNameWithoutExtension(obj.Name);
        //    Debug.Log("onEnable " + name);
        //    prefabString.Add(name);

        //    if (i % 3 == 0 && i != 0)
        //    {
        //        j++;
        //    }
        //    //+0.02
        //    Vector3 targetPosition = prefab.transform.position;
        //    targetPosition.x += 0.04f * (i - 3 * j);
        //    targetPosition.y -= 0.04f * j;
        //    ////    //// Keep our y position unchanged.
        //    //targetPosition.y += 0.04f*i;
        //    GameObject new_obj = Instantiate(prefab, targetPosition, Quaternion.identity, parent.transform);

        //    new_obj.GetComponent<ButtonConfigHelper>().MainLabelText = name;
        //    new_obj.SetActive(true);
        //    buttonList.Add(new_obj);

        //    /* MARCHE POUR TOUT LES FUNCTIONS SANS LES PARAMETRES

        //    Interactable inter = new_obj.GetComponent<Interactable>();
        //    Debug.Log(inter);


        //    var go = new GameObject();

        //    UnityAction<GameObject> action = new UnityAction<GameObject>(delegate { OnButtonClick(); });

        //    UnityEventTools.AddObjectPersistentListener(inter.OnClick, action, go);
        //    */

        //    Interactable inter = new_obj.GetComponent<Interactable>();
        //    Debug.Log(inter);

        //    UnityAction<int> action = new UnityAction<int>(delegate { onItemClick((int)i); });

        //UnityEditor.Events.UnityEventTools.AddIntPersistentListener(inter.OnClick, onItemClick, (int)i);


        //inter.OnClick.AddListener(() => { onItemClick(i); });
        //var targetinfo = UnityEvent.GetValidMethodInfo(script,
        //"OnButtonClick", new Type[] { typeof(GameObject) });

        //UnityAction<GameObject> action = Delegate.CreateDelegate(typeof(UnityAction<GameObject>), script, targetinfo, false) as UnityAction<GameObject>;

        //UnityEventTools.AddObjectPersistentListener<GameObject>(script.OnClick, action, go);
        //new_obj.GetComponent<ButtonConfigHelper>().OnClick.AddListener(delegate { onItemClick(i); });
        //Button button = myButton.getComponent<Button>();
        //next, any of these will work:
        // button.onClick += myMethod;
        // button.onClick.AddListener(myMethod);UnityEventTools.AddPersistentListener(myBtn.onClick, new UnityAction(methodName));


        //UnityEventTools.AddPersistentListener(myBtn.onClick, new UnityAction(methodName));
        ////button.OnClick.AddListener(delegate { onItemClick(i); });

        //ButtonConfigHelper btn = new_obj.GetComponent<ButtonConfigHelper>();
        //btn.OnClick.AddListener(delegate { onItemClick(i); });


        //UnityAction methodDelegate = (UnityAction)System.Delegate.CreateDelegate(typeof(UnityAction), braceletMenu, onItemClick);
        //UnityEventTools.AddPersistentListener(button.OnClick, methodDelegate);
        //new_obj.transform.position = targetPosition;

        //    i++;

        //}



    }
    public void OnButtonClick()
    {
        Debug.Log("TETS");
    }

    public void onItemClick(int i)
    {
        // Debug.Log("onItemClick"+ prefabString[i]);
        //GameObject obj = Resources.Load<GameObject>("Bracelet/" + prefabString[i]);
        ////load les meshFilter et meshRenderer enregistrer au lieu d'avoir des dict??

        ////CA MARCHE VITE FAIT, il faut sauvegarder les mesh aussi dans les fichiers pour les réattribuer
        //MeshFilter[] meshfilters = obj.GetComponentsInChildren<MeshFilter>();
        //SettingMenu settings = settingsMenu.GetComponent<SettingMenu>();
        //int j = 0;

        //NE MARCHE PAS SI L'objet de base est supprimé
        //NE MARCHE PAS ENTRE LES EXECUTIONS
        //foreach (MeshFilter filter in meshfilters)
        //{
        //    settings.filterDict.TryGetValue(obj.name+j, out var mesh);

        //    Debug.Log(mesh);
        //    j++;
        //    filter.mesh = mesh.mesh;
        //}
        //MeshRenderer[] meshRenderer = obj.GetComponentsInChildren<MeshRenderer>();

        //j = 0;
        //foreach (MeshRenderer renderer in meshRenderer)
        //{
        //    settings.rendererDict.TryGetValue(obj.name + j, out var rend);

        //    Debug.Log(rend);
        //    j++;
        //    renderer.material = rend.material;
        //}
        //obj.GetComponentsInChildren<MeshRenderer>().; 
        //Set it in scene
        //Instantiate(obj);
    }
    public void onCloseClick()
    {
        foreach (GameObject btn in buttonList)
        {
            Debug.Log("DESTOY");
            Destroy(btn);
        }
        braceletMenu.SetActive(false);

    }

}

/*

using Dummiesman;
using Microsoft.MixedReality.Toolkit.UI;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;

public class BraceletMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject braceletMenu;

    [SerializeField]
    private GameObject prefabButton;

    [SerializeField]
    private GameObject parent;

    public List<string> objFileList = new List<string>();

    private List<GameObject> buttonList = new List<GameObject>();

    void OnDisable()
    {
        objFileList.Clear();
    }

    void OnEnable()
    {
        ////Create a button for each .obj in our resource folder
#if UNITY_EDITOR
        string localPath = "/Resources/Bracelet/";
#else
            string localPath = "/Assets/Resources/Bracelet/";
#endif

        var globalPath = Application.dataPath + localPath;
        Debug.Log(globalPath);


        //Get all files with .obj
        var info = new DirectoryInfo(globalPath);
        var fileInfo = info.GetFiles("*.obj").ToList<FileInfo>();
        int i = 0;
        int j = 0;
        foreach (FileInfo obj in fileInfo)
        {
            string name = Path.GetFileNameWithoutExtension(obj.Name);
            Debug.Log("onEnable " + name);

            objFileList.Add(name); //To use them later

            //Retour à la ligne après 3
            if (i % 3 == 0 && i != 0)
            {
                j++;
            }

            // Changer the position of the new buttons
            Vector3 targetPosition = prefabButton.transform.position;
            targetPosition.x += 0.04f * (i - 3 * j);
            targetPosition.y -= 0.04f * j;

            //Create the new button from a prefab
            GameObject new_btn_prefab = Instantiate(prefabButton, targetPosition, Quaternion.identity, parent.transform);
            new_btn_prefab.GetComponent<ButtonConfigHelper>().MainLabelText = name;
            new_btn_prefab.SetActive(true);

            buttonList.Add(new_btn_prefab); //Used to destroy them later


            //Create a listener to have an action linked to the right file

            Debug.Log("Add Listener on " + i);
            int tempI = i;
            new_btn_prefab.GetComponent<ButtonConfigHelper>().OnClick.AddListener(delegate { onItemClick(tempI); }); // change to temp value to not have the reference but the value only
            i++;

        }

    }
    

    public void OnButtonClick()
    {
        Debug.Log("Debug purpous");
    }


    /// <summary>
    /// Import the selected item at index i
    /// </summary>
    /// <param name="i">index in the list of the file</param>
    public void onItemClick(int i)
    {
        Debug.Log("On itemClick" + i);

#if UNITY_EDITOR
        string localPath = "/Resources/Bracelet/" + objFileList[i] + ".obj";
        var globalPath = Application.dataPath + localPath;
#else
            string localPath = "/Assets/Resources/Bracelet/" + objFileList[i] + ".obj";
            var globalPath = Application.dataPath + localPath;
#endif

        Debug.Log(globalPath);

        OBJLoader objLoader = new OBJLoader();
        //Load the file
        GameObject obj = objLoader.Load(globalPath);

        Utility.addImportantComponent(obj); //Add the important Components

    }

    /// <summary>
    /// On the close of the panel we want to destroy the button and create new one each time
    /// </summary>
    public void onCloseClick()
    {
        foreach (GameObject btn in buttonList)
        {
            Debug.Log("DESTOY");
            Destroy(btn);
        }
        braceletMenu.SetActive(false);

    }


    /// <summary>
    /// This method add quick button for debug purpous
    /// </summary>
    private void OnGUI()
    {
        if (GUI.Button(new Rect(220, 110, 100, 30), "item click bracelet 0"))
        {
            onItemClick(0);

        }
    }
}
    */