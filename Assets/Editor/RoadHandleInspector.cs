using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Presar.Builder.ProceduralGeneration
{
    [CustomEditor(typeof(RoadHandle))]
    public class RoadInspectorHandle : Editor
    {
        private Transform targetTransform;
        private Road road;

        private void OnEnable()
        {
            targetTransform = ((RoadHandle)target).transform;
            road = targetTransform.parent.GetComponent<Road>();
            targetTransform.hasChanged = false;
        }

        private void OnSceneGUI()
        {
            RoadMesh.DrawHandles(road.Nodes);
            if (targetTransform.hasChanged && road.Nodes.Count >= 2)
            {
                var meshes = RoadMesh.GenerateRoadMesh(road.Nodes);
                road.SetMesh(meshes);
                targetTransform.hasChanged = false;
            }
        }

    }

}