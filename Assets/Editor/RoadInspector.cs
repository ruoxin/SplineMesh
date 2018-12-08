using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace Presar.Builder.ProceduralGeneration
{
    [CustomEditor(typeof(Road))]
    public class RoadInspector : Editor
    {
        private Road road;
        private Transform transform;

        private void OnEnable()
        {
            road = (Road)target;
            transform = road.transform;
        }

        private void OnSceneGUI()
        {
            EditorGUI.BeginChangeCheck();
            var nodes = road.Nodes;
            RoadMesh.DrawHandles(nodes);

            EditorGUI.BeginChangeCheck();
            if (Tools.current == Tool.Move)
            {
                foreach (var node in nodes)
                {
                    Vector3 position = Handles.PositionHandle(node.transform.position, node.transform.rotation);
                    node.transform.position = position;
                }
            }
            if (Tools.current == Tool.Rotate)
            {
                foreach (var node in nodes)
                {
                    Quaternion rotation = Handles.RotationHandle(node.transform.rotation, node.transform.position);
                    node.transform.rotation = rotation;
                }
            }
            if (Tools.current == Tool.Scale)
            {
                foreach (var node in nodes)
                {
                    float size = HandleUtility.GetHandleSize(node.transform.position);
                    Vector3 scale = Handles.ScaleHandle(node.transform.localScale, node.transform.position, node.transform.rotation, size);
                    node.transform.localScale = scale;
                }
            }
            if (EditorGUI.EndChangeCheck())
            {
                road.SetMesh(RoadMesh.GenerateRoadMesh(road.Nodes));
                EditorUtility.SetDirty(road);
            }
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            if (GUILayout.Button("(Re)Generate Mesh"))
            {
                var meshes = RoadMesh.GenerateRoadMesh(road.Nodes);
                road.SetMesh(meshes);
            }

            if (GUILayout.Button("Attach To Terrain"))
            {
                AttachToTerrain();
            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Node"))
            {
                AddNode();
            }
            if (GUILayout.Button("Delete Node"))
            {
                DeleteNode();
            }
            EditorGUILayout.EndHorizontal();

        }

        private void AttachToTerrain()
        {
            foreach (var meshObj in road.Meshes)
            {
                var mesh = meshObj.GetComponent<MeshFilter>().sharedMesh;
                var vertices = mesh.vertices;
                for (int i = 0; i < vertices.Length; i++)
                {
                    //var vertex = vertices[i] + meshObj.transform.position;
                    var positionWorld = meshObj.transform.TransformPoint(vertices[i]);
                    positionWorld.y = Utils.GetYfromXZ(positionWorld.x, positionWorld.z) + 0.1f;
                    vertices[i] = meshObj.transform.InverseTransformPoint(positionWorld);
                }
                mesh.vertices = vertices;
                mesh.RecalculateNormals();
            }
        }



        //[DrawGizmo(GizmoType.Active | GizmoType.Selected | GizmoType.Pickable)]
        //public static void DrawGizmos(Road road, GizmoType gizmoType)
        //{
        //    Vector3 position = road.transform.position;
        //    Gizmos.DrawCube(position, Vector3.one * 5);
        //}

        private GameObject GenerateNewChild(string name, Transform parent)
        {
            var gameObj = new GameObject(name);
            Undo.RegisterCreatedObjectUndo(gameObj, "add road child");
            gameObj.transform.SetParent(parent);
            gameObj.transform.localPosition = Vector3.zero;
            gameObj.transform.localRotation = Quaternion.identity;
            gameObj.transform.localScale = Vector3.one;
            return gameObj;
        }

        public void AddNode()
        {
            if (road.meshParent == null)
            {
                road.meshParent = GenerateNewChild("meshes", transform).transform;
            }

            Debug.Log(road.Nodes.Count);
            var nodeObj = GenerateNewChild(string.Format("node{0}", road.Nodes.Count), transform);
            nodeObj.transform.localScale = new Vector3(0f, 0f, 5f);
            nodeObj.AddComponent<RoadHandle>();
            road.Nodes.Add(nodeObj);

            if (road.Nodes.Count >= 2)
            {
                var meshObj = GenerateNewChild(string.Format("mesh{0}{1}", road.Nodes.Count - 2, road.Nodes.Count-1), road.meshParent);
                meshObj.AddComponent<MeshFilter>();
                meshObj.AddComponent<MeshRenderer>().material = road.material;
                road.Meshes.Add(meshObj);
            }
            EditorUtility.SetDirty(road);
        }

        public void DeleteNode()
        {
            if (road.Nodes.Any())
            {
                Undo.DestroyObjectImmediate(road.Nodes.Last());
                road.Nodes.RemoveAt(road.Nodes.Count - 1);
            }
            if (road.Meshes.Any())
            {
                Undo.DestroyObjectImmediate(road.Meshes.Last());
                road.Meshes.RemoveAt(road.Meshes.Count - 1);
            }
            EditorUtility.SetDirty(road);
        }

    }
}