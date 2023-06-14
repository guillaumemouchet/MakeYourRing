using UnityEngine;
using System.Text;
using System.Collections.Generic;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif
/*=============================================================================
 |	    Project:  Unity3D Scene OBJ Exporter
 |
 |		  Notes: Only works with meshes + meshRenderers. No terrain yet
 |
 |       Author:  aaro4130
 |
 |     DO NOT USE PARTS OF THIS CODE, OR THIS CODE AS A WHOLE AND CLAIM IT
 |     AS YOUR OWN WORK. USE OF CODE IS ALLOWED IF I (aaro4130) AM CREDITED
 |     FOR THE USED PARTS OF THE CODE.
 |
 *===========================================================================*/

using UnityEngine;
using System.Text;
using System.Collections.Generic;
using System.IO;

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
            sb.AppendLine("# Export of " + Application.loadedLevelName);
            sb.AppendLine("# from Aaro4130 OBJ Exporter " + versionString);
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

                        Debug.Log("faceOrder " + faceOrder);
                        if (faceOrder <= 0)
                        {
                            Debug.Log("IN ");

                            sb.AppendLine("f " + ConstructOBJString(idx2) + " " + ConstructOBJString(idx1) + " " + ConstructOBJString(idx0));
                        }
                        else
                        {
                            Debug.Log("OUT ");

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
                Debug.Log(exportFileInfo.Directory.FullName + "\\" + baseFileName + ".mtl" + sbMaterials.ToString());
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
            Debug.Log("Could not export texture: " + t.name + ". Is it readable?");
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




/*using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameObjectExporterToObj : MonoBehaviour
{
    public void ExportToOBJ(GameObject gameObject, string globalPath)
    {
        string localPathObj = "\\" + gameObject.name + ".obj";
        string localPathMtl = "\\" + gameObject.name + ".mtl";

        string objFilePath = globalPath + localPathObj;
        string mtlFilePath = globalPath + localPathMtl;

        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> triangles = new List<int>();
        List<int> parentIndices = new List<int>();

        Dictionary<Material, int> materialIndices = new Dictionary<Material, int>();
        materialIndices.Add(new Material(Shader.Find("Standard (Specular setup)")), 0);
        TraverseGameObject(gameObject, vertices, normals, uvs, triangles, materialIndices, parentIndices);

        // Export to MTL file
        using (StreamWriter sw = new StreamWriter(mtlFilePath))
        {
            foreach (var kvp in materialIndices)
            {
                Material material = kvp.Key;
                int materialIndex = kvp.Value;

                string materialName = "Material_" + materialIndex;

                // Write material name
                sw.WriteLine("newmtl " + materialName);

                // Write material color
                Color color = material.color;
                sw.WriteLine("Kd " + color.r + " " + color.g + " " + color.b);

                // Write texture map if available
                if (material.mainTexture != null)
                {
                    string texturePath = Path.GetFileName(material.mainTexture.name);
                    sw.WriteLine("map_Kd " + texturePath);
                }

                // Add a newline between materials
                sw.WriteLine();
            }
        }

        // Export to OBJ file
        using (StreamWriter sw = new StreamWriter(objFilePath))
        {
            // Write MTL file reference
            string mtlFileName = Path.GetFileName(mtlFilePath);
            sw.WriteLine("mtllib " + mtlFileName);

            // Write the vertices
            foreach (Vector3 vertex in vertices)
            {
                string line = string.Format("v {0} {1} {2}", vertex.x, vertex.y, vertex.z);
                sw.WriteLine(line);
            }

            // Write the normals
            foreach (Vector3 normal in normals)
            {
                string line = string.Format("vn {0} {1} {2}", normal.x, normal.y, normal.z);
                sw.WriteLine(line);
            }

            // Write the UVs
            foreach (Vector2 uv in uvs)
            {
                string line = string.Format("vt {0} {1}", uv.x, uv.y);
                sw.WriteLine(line);
            }

            // Write the material assignments and triangles
            Material currentMaterial = null;

            for (int i = 0; i < triangles.Count; i += 3)
            {
                int index0 = triangles[i] + 1;
                int index1 = triangles[i + 1] + 1;
                int index2 = triangles[i + 2] + 1;

                // Get the parent indices for each triangle vertex
                int parentIndex0 = parentIndices[triangles[i]];
                int parentIndex1 = parentIndices[triangles[i + 1]];
                int parentIndex2 = parentIndices[triangles[i + 2]];

                // Write the parent-child relationship as OBJ comments
                if (parentIndex0 != -1)
                {
                    sw.WriteLine("# Parent: " + parentIndex0);
                }
                if (parentIndex1 != -1)
                {
                    sw.WriteLine("# Parent: " + parentIndex1);
                }
                if (parentIndex2 != -1)
                {
                    sw.WriteLine("# Parent: " + parentIndex2);
                }

                // Write material assignment if the material changes
                int materialIndex = triangles[i + 2]; // Using the third vertex index as a reference
                Material triangleMaterial = GetMaterialForIndex(materialIndex, materialIndices);

                if (triangleMaterial != currentMaterial)
                {
                    currentMaterial = triangleMaterial;
                    materialIndices.TryGetValue(currentMaterial, out int materialNumber);
                    string materialName = "Material_" + materialNumber;
                    sw.WriteLine("usemtl " + materialName);
                }

                // Write the triangle indices
                string line = string.Format("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2}", index0, index1, index2);
                sw.WriteLine(line);
            }
        }

        Debug.Log("Exported to OBJ: " + objFilePath);
        Debug.Log("Exported to MTL: " + mtlFilePath);
    }

    private void TraverseGameObject(GameObject gameObject, List<Vector3> vertices, List<Vector3> normals, List<Vector2> uvs, List<int> triangles, Dictionary<Material, int> materialIndices, List<int> parentIndices)
    {
        MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();

        if (meshFilter != null && meshRenderer != null)
        {
            Mesh mesh = meshFilter.sharedMesh;
            Material[] materials = meshRenderer.sharedMaterials;

            if (mesh != null && materials != null)
            {
                int vertexOffset = vertices.Count;
                int[] meshTriangles = mesh.triangles;

                // Get the vertices, normals, and UVs
                vertices.AddRange(mesh.vertices);
                normals.AddRange(mesh.normals);
                uvs.AddRange(mesh.uv);

                // Offset the triangles by the vertex count
                for (int i = 0; i < meshTriangles.Length; i++)
                {
                    triangles.Add(meshTriangles[i] + vertexOffset);
                    parentIndices.Add(GetParentIndex(gameObject));
                }

                // Assign materials to material indices
                for (int i = 0; i < materials.Length; i++)
                {
                    Material material = materials[i];
                    if (!materialIndices.ContainsKey(material))
                    {
                        materialIndices.Add(material, materialIndices.Count);
                    }
                }
            }
            else
            {
                Debug.LogWarning("Mesh or materials are missing for GameObject: " + gameObject.name);
            }
        }

        // Traverse child objects recursively
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            GameObject childObject = gameObject.transform.GetChild(i).gameObject;
            TraverseGameObject(childObject, vertices, normals, uvs, triangles, materialIndices, parentIndices);
        }
    }

    private Material GetMaterialForIndex(int index, Dictionary<Material, int> materialIndices)
    {
        Material defaultMaterial = new Material(Shader.Find("Standard (Specular setup)")); // Assign a default material here

        foreach (var kvp in materialIndices)
        {
            if (kvp.Value == index)
            {
                return kvp.Key;
            }
        }

        // Material not found, return the default material
        return defaultMaterial;
    }

    private int GetParentIndex(GameObject gameObject)
    {
        if (gameObject.transform.parent != null)
        {
            return gameObject.transform.parent.GetSiblingIndex() + 1;
        }
        else
        {
            return -1;
        }
    }
}
*/