using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Presar.Builder.ProceduralGeneration
{
    [SelectionBase]
    public class Road : MonoBehaviour
    {
        public List<GameObject> Nodes = new List<GameObject>();
        public List<GameObject> Meshes = new List<GameObject>();
        public Material material;
        public Transform meshParent;


        public void SetMesh(Mesh[] meshes)
        {
            if (Meshes.Count != meshes.Length)
            {
                Debug.LogError("the number of meshes does not match");
                return;
            }
            for (int i = 0; i < meshes.Length; i++)
            {
                Meshes[i].GetComponent<MeshFilter>().sharedMesh = meshes[i];
            }
        }

    }
}
