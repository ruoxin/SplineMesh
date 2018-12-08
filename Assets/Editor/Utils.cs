using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Presar.Builder.ProceduralGeneration
{
    public static class Utils
    {
        private const float MaxHeight = 10000f;

        public static float GetYfromXZ(float x, float z)
        {
            RaycastHit hit;
            if (Physics.Raycast(new Vector3(x, MaxHeight, z), Vector3.down, out hit, Mathf.Infinity, LayerMask.GetMask("Terrain")))
            {
                return hit.point.y;
            }
            else
            {
                Debug.LogWarningFormat("Can not find point of ({0}, {1}) on Terrain Layer (MaxHeight {2})", x, z, MaxHeight);
                return 0;
            }
        }

        public static Rect GetBoundsXZ(List<Vector3> vertices)
        {
            var xMin = vertices[0].x;
            var xMax = vertices[0].x;
            var zMin = vertices[0].z;
            var zMax = vertices[0].z;
            for (int i = 1; i < vertices.Count; i++)
            {
                if (vertices[i].x < xMin) xMin = vertices[i].x;
                if (vertices[i].x > xMax) xMax = vertices[i].x;
                if (vertices[i].z < zMin) zMin = vertices[i].z;
                if (vertices[i].z > zMax) zMax = vertices[i].z;
            }
            return Rect.MinMaxRect(xMin, zMin, xMax, zMax);
        }

        public static bool IsPositionInAreaXZ(Vector3 position, List<Vector3> vertices)
        {
            int intersection = 0;
            for (int i = 0; i < vertices.Count; i++)
            {
                var startPoint = vertices[i];
                var endPoint = (i == vertices.Count - 1) ? vertices[0] : vertices[i + 1];
                if (position.z > Mathf.Min(startPoint.z, endPoint.z)
                    && position.z < Mathf.Max(startPoint.z, endPoint.z)
                    && position.x < Mathf.Max(startPoint.x, endPoint.x))
                {
                    if (startPoint.x == startPoint.z)
                    {
                        intersection++;
                    }
                    else
                    {
                        // y = ax + b, calculate a and b
                        var a = (endPoint.z - startPoint.z) / (endPoint.x - startPoint.x);
                        var b = startPoint.z - a * startPoint.x;
                        var xOnTheLine = (position.z - b) / a;
                        if (position.x < xOnTheLine)
                        {
                            intersection++;
                        }
                    }
                }
            }

            return (intersection % 2 != 0);
        }
    } 
}
