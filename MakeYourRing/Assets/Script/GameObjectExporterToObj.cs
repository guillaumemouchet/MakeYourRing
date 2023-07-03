using UnityEngine;
using System.Text;
using System.Collections.Generic;
using System.IO;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif
/*=============================================================================
 |	    Project:  MakeYourRing Scene GameObject Exporter to obj
 |
 |       Author:  aaro4130 + Guillaume Mouchet ISC3il-b
 |
 |     This code was taken and inspired by the OBJExporter made by aaro4130 in the dummiesman namespace.
 |     It had to be changed to work on this specific project.
 |     Credits are still his (https://assetstore.unity.com/publishers/9173)
 |
 *===========================================================================*/

public class GameObjectExporterToObj
{
    public bool applyPosition = true;
    public bool applyRotation = true;
    public bool applyScale = true;
    public bool generateMaterials = true;
    public bool exportTextures = true;
    public bool splitObjects = true;
    public bool objNameAddIdNum = false;

    private const string versionString = "v2.0";
    private string lastExportFolder;

    private Vector3 RotateAroundPoint(Vector3 point, Vector3 pivot, Quaternion angle)
    {
        return angle * (point - pivot) + pivot;
    }

    private Vector3 MultiplyVec3s(Vector3 v1, Vector3 v2)
    {
        return new Vector3(v1.x * v2.x, v1.y * v2.y, v1.z * v2.z);
    }

    public void Export(GameObject obj, string exportPath)
    {
        try
        {
            Dictionary<string, bool> materialCache = new Dictionary<string, bool>();
            var exportFileInfo = new System.IO.FileInfo(exportPath);
            lastExportFolder = exportFileInfo.Directory.FullName;
            string baseFileName = System.IO.Path.GetFileNameWithoutExtension(exportPath);
            Debug.Log("Exporting OBJ. Please wait... Starting export.");

            MeshFilter[] sceneMeshes = obj.GetComponentsInChildren<MeshFilter>();

            if (Application.isPlaying)
            {
                foreach (MeshFilter mf in sceneMeshes)
                {
                    MeshRenderer mr = mf.gameObject.GetComponent<MeshRenderer>();

                    if (mr != null && mr.isPartOfStaticBatch)
                    {
                        Debug.Log("Error: Static batched object detected. Static batching is not compatible with this exporter. Please disable it before starting the player.");
                        return;
                    }
                }
            }

            StringBuilder sb = new StringBuilder();
            StringBuilder sbMaterials = new StringBuilder();
            sb.AppendLine("# Export of " + SceneManager.GetActiveScene());
            sb.AppendLine("# from modified Aaro4130 OBJ Exporter " + versionString);
            if (generateMaterials)
            {
                sb.AppendLine("mtllib " + baseFileName + ".mtl");
            }
            float maxExportProgress = (float)(sceneMeshes.Length + 1);
            int lastIndex = 0;
            for (int i = 0; i < sceneMeshes.Length; i++)
            {
                MeshFilter mf = sceneMeshes[i];
                MeshRenderer mr = mf.gameObject.GetComponent<MeshRenderer>();
                string meshName = mf.gameObject.name;

                if (splitObjects)
                {
                    string exportName = meshName;
                    if (objNameAddIdNum)
                    {
                        exportName += "_" + i;
                    }
                    sb.AppendLine("g " + exportName);
                }

                if (mr != null && generateMaterials)
                {
                    Material[] mats = mr.sharedMaterials;
                    for (int j = 0; j < mats.Length; j++)
                    {
                        Material m = new Material(mats[j]);

                        if (!materialCache.ContainsKey(m.name))
                        {
                            materialCache[m.name] = true;
                            sbMaterials.Append(MaterialToString(m));
                            sbMaterials.AppendLine();
                        }
                    }
                }

                Mesh msh = mf.sharedMesh;

                foreach (Vector3 vx in msh.vertices)
                {
                    Vector3 v = vx;
                    if (applyScale)
                    {
                        v = MultiplyVec3s(v, mf.gameObject.transform.lossyScale);
                    }
                    if (applyRotation)
                    {
                        v = RotateAroundPoint(v, Vector3.zero, mf.gameObject.transform.rotation);
                    }
                    if (applyPosition)
                    {
                        v += mf.gameObject.transform.position;
                    }
                    v.x *= -1;
                    sb.AppendLine("v " + v.x + " " + v.y + " " + v.z);
                }

                foreach (Vector3 vx in msh.normals)
                {
                    Vector3 v = vx;
                    if (applyScale)
                    {
                        v = MultiplyVec3s(v, mf.gameObject.transform.lossyScale.normalized);
                    }
                    if (applyRotation)
                    {
                        v = RotateAroundPoint(v, Vector3.zero, mf.gameObject.transform.rotation);
                    }
                    v.x *= -1;
                    sb.AppendLine("vn " + v.x + " " + v.y + " " + v.z);
                }

                foreach (Vector2 v in msh.uv)
                {
                    sb.AppendLine("vt " + v.x + " " + v.y);
                }

                for (int j = 0; j < msh.subMeshCount; j++)
                {
                    if (mr != null && j < mr.sharedMaterials.Length)
                    {
                        string matName = mr.sharedMaterials[j].name;
                        sb.AppendLine("usemtl " + matName);
                    }
                    else
                    {
                        sb.AppendLine("usemtl " + meshName + "_sm" + j);
                    }

                    int[] tris = msh.GetTriangles(j);
                    for (int t = 0; t < tris.Length; t += 3)
                    {
                        int idx2 = tris[t] + 1 + lastIndex;
                        int idx1 = tris[t + 1] + 1 + lastIndex;
                        int idx0 = tris[t + 2] + 1 + lastIndex;
                        int faceOrder = (int)Mathf.Clamp((mf.gameObject.transform.lossyScale.x * mf.gameObject.transform.lossyScale.z), -1, 1);

                        if (faceOrder <= 0)
                        {
                            sb.AppendLine("f " + ConstructOBJString(idx2) + " " + ConstructOBJString(idx1) + " " + ConstructOBJString(idx0));
                        }
                        else
                        {
                            sb.AppendLine("f " + ConstructOBJString(idx0) + " " + ConstructOBJString(idx1) + " " + ConstructOBJString(idx2));
                        }
                    }
                }

                lastIndex += msh.vertices.Length;
            }

            int k = 0;
            while (File.Exists(exportPath))
            {
                exportPath = lastExportFolder + "\\" + baseFileName + k + ".obj";
                k++;
            }

            System.IO.File.WriteAllText(exportPath, sb.ToString());

            if (generateMaterials)
            {
                k = 0;
                var matFile = exportFileInfo.Directory.FullName + "\\" + baseFileName + ".mtl";
                while (File.Exists(matFile))
                {
                    matFile = exportFileInfo.Directory.FullName + "\\" + baseFileName + k + ".mtl";
                    k++;
                }
                System.IO.File.WriteAllText(matFile, sbMaterials.ToString());
                Debug.Log(exportFileInfo.Directory.FullName + "\\" + baseFileName + ".mtl");
            }
        }
        catch (System.Exception ex)
        {
            Debug.Log("Error exporting OBJ: " + ex.Message);
        }
    }

    private string TryExportTexture(string propertyName, Material m)
    {
        if (m.HasProperty(propertyName))
        {
            Texture t = m.GetTexture(propertyName);
            if (t != null)
            {
                return ExportTexture((Texture2D)t);
            }
        }
        return "false";
    }

    private string ExportTexture(Texture2D t)
    {
        try
        {
            string exportName = lastExportFolder + "\\" + t.name + ".jpg";
            int k = 0;
            do
            {
                k++;
                exportName = lastExportFolder + "\\" + t.name + k + ".jpg";
            } while (File.Exists(exportName));

            Texture2D exTexture = new Texture2D(t.width, t.height, TextureFormat.ARGB32, false);
            exTexture.SetPixels(t.GetPixels());
            System.IO.File.WriteAllBytes(exportName, exTexture.EncodeToJPG(95));

#if UNITY_EDITOR

            string localPath = "/Resources/EXAMPLE.txt";
#else
                        string localPath = "/Assets/Resources/EXAMPLE.txt";

#endif            
            string contentMeta = File.ReadAllText(Application.dataPath + localPath);
            string exportNameMeta = exportName + ".meta";
            System.IO.File.WriteAllText(exportNameMeta, contentMeta);

            return t.name + k + ".jpg";
        }
        catch (System.Exception ex)
        {
            Debug.Log(ex + " \nCould not export texture: " + t.name + ". Is it readable?");
            return "null";
        }
    }

    private string ConstructOBJString(int index)
    {
        string idxString = index.ToString();
        return idxString + "/" + idxString + "/" + idxString;
    }

    string MaterialToString(Material m)
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("newmtl " + m.name);

        // Add properties
        if (m.HasProperty("_Color"))
        {
            sb.AppendLine("Kd " + m.color.r.ToString() + " " + m.color.g.ToString() + " " + m.color.b.ToString());
            sb.AppendLine("Tr 0.0"); // Set transparency to fully opaque
            sb.AppendLine("d 1.0");  // Set dissolve to fully opaque
        }
        if (m.HasProperty("_SpecColor"))
        {
            Color sc = m.GetColor("_SpecColor");
            sb.AppendLine("Ks " + sc.r.ToString() + " " + sc.g.ToString() + " " + sc.b.ToString());
        }
        if (exportTextures)
        {
            // Diffuse
            string exResult = TryExportTexture("_MainTex", m);
            if (exResult != "false")
            {
                sb.AppendLine("map_Kd " + exResult);
            }
            // Specular map
            exResult = TryExportTexture("_SpecMap", m);
            if (exResult != "false")
            {
                sb.AppendLine("map_Ks " + exResult);
            }
            // Bump map
            exResult = TryExportTexture("_BumpMap", m);
            if (exResult != "false")
            {
                sb.AppendLine("map_Bump " + exResult);
            }
        }
        sb.AppendLine("illum 2");

        return sb.ToString();
    }
}

