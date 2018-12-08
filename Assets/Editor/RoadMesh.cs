using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Presar.Builder.ProceduralGeneration
{
    public class RoadMesh
    {
        public static Mesh[] GenerateRoadMesh(List<GameObject> nodes)
        {
            if (nodes.Count < 2)
            {
                Debug.LogError("can not generate mesh: nodes should be more than 1");
                return null;
            }

            SplineMesh splineMesh = new SplineMesh(GetSimpleRoadShape());
            Mesh[] meshes = new Mesh[nodes.Count - 1];
            for (int i = 0; i < nodes.Count - 1; i++)
            {
                var points = new Vector3[4];
                var start = nodes[i].transform;
                var end = nodes[i + 1].transform;
                points[0] = start.localPosition;
                if (i == 0)
                {
                    points[1] = start.localPosition + start.forward * start.localScale.z;
                }
                else
                {
                    points[1] = start.localPosition + -start.forward * start.localScale.z;
                }
                points[2] = end.localPosition + end.forward * end.localScale.z;
                points[3] = end.localPosition;
                meshes[i] = splineMesh.GenerateMesh(points);
            }

            return meshes;
        }

        private static ExtrudeShape GetSimpleRoadShape()
        {
            Vector2[] vertices = {
                new Vector2(-1f, 0),
                new Vector2(0, 0),
                new Vector2(1f, 0)
            };
            int[] lines = { 0, 1, 1, 2 };
            var shape = new ExtrudeShape(vertices, lines);
            return shape;
        }

        public static void DrawHandles(List<GameObject> nodes)
        {
            Color color = Color.red;
            float size;
            for (int i = 0; i < nodes.Count - 1; i++)
            {
                var start = nodes[i].transform;
                var end = nodes[i + 1].transform;
                if (i == 0)
                {
                    var startHandle = start.TransformPoint(Vector3.forward);
                    size = HandleUtility.GetHandleSize(startHandle) * 0.1f;
                    Handles.color = Color.grey;
                    Handles.SphereHandleCap(0, start.position, Quaternion.identity, size, EventType.Repaint);
                    Handles.color = color;
                    Handles.SphereHandleCap(0, startHandle, Quaternion.identity, size, EventType.Repaint);
                    Handles.DrawLine(start.position, startHandle);
                }
                else
                {
                    var startHandle = start.TransformPoint(Vector3.back);
                    size = HandleUtility.GetHandleSize(startHandle) * 0.1f;
                    Handles.color = color;
                    Handles.SphereHandleCap(0, startHandle, Quaternion.identity, size, EventType.Repaint);
                    Handles.DrawLine(start.position, startHandle);
                }

                var endHandle = end.TransformPoint(Vector3.forward);
                size = HandleUtility.GetHandleSize(endHandle) * 0.1f;
                Handles.color = Color.grey;
                Handles.SphereHandleCap(0, end.position, Quaternion.identity, size, EventType.Repaint);
                Handles.color = color;
                Handles.SphereHandleCap(0, endHandle, Quaternion.identity, size, EventType.Repaint);
                Handles.DrawLine(end.position, endHandle);
                color = color == Color.red ? Color.blue : Color.red;
            }
        }
    }
}