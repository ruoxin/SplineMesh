using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Presar.Builder.ProceduralGeneration
{
    public class SplineMesh
    {
        private ExtrudeShape shape = null;

        public SplineMesh(ExtrudeShape shape)
        {
            this.shape = shape;
        }

        public Mesh GenerateMesh(Vector3[] points)
        {
            var distance = Vector3.Distance(points[0], points[3]);
            int edgeLoops = (int)distance * 5;
            if (edgeLoops == 0)
            {
                return null;
            }
            int segments = edgeLoops - 1;
            int vertsInShape = shape.Vertices.Length;
            int vertCount = vertsInShape * edgeLoops;
            int triCount = shape.Lines.Length * segments * 3;
            var vertices = new Vector3[vertCount];
            var normals = new Vector3[vertCount];
            var uv = new Vector2[vertCount];
            var triangles = new int[triCount];

            for (int i = 0; i < edgeLoops; i++)
            {
                float t = i / (float)(edgeLoops - 1);
                Vector3 position = GetSplinePoint(points, t);
                Vector3 tangent = GetTangent(points, t);
                Vector3 binormal = Vector3.Cross(Vector3.up, tangent).normalized;
                Vector3 normal = Vector3.Cross(tangent, binormal);
                Quaternion rotation = Quaternion.LookRotation(tangent, normal);

                int offset = i * vertsInShape;
                for (int j = 0; j < vertsInShape; j++)
                {
                    int index = offset + j;
                    vertices[index] = position + rotation * shape.Vertices[j];
                    normals[index] = rotation * shape.Normals[j];
                    uv[index] = new Vector2(shape.UCoord[j], t * distance);
                }
            }

            int ti = 0;
            for (int i = 0; i < segments; i++)
            {
                int offset = i * vertsInShape;
                for (int j = 0; j < shape.Lines.Length; j += 2)
                {
                    int a = offset + shape.Lines[j] + vertsInShape;
                    int b = offset + shape.Lines[j];
                    int c = offset + shape.Lines[j + 1];
                    int d = offset + shape.Lines[j + 1] + vertsInShape;
                    triangles[ti] = a; ti++;
                    triangles[ti] = c; ti++;
                    triangles[ti] = b; ti++;
                    triangles[ti] = a; ti++;
                    triangles[ti] = d; ti++;
                    triangles[ti] = c; ti++;
                }
            }


            Mesh mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.normals = normals;
            mesh.uv = uv;
            return mesh;
        }

        // Optimized GetPoint
        // the point is obtained by lerping of all 4 point recursively 
        // lerping can be expressed as a + t(b-a)
        // add all the lerping together we get 
        // pt = p0((1-t)^3) +
        //      p1(2(1-t^2 * t) + 
        //      p2(3(1-t) * t^2) + 
        //      p3(t^3)
        Vector3 GetSplinePoint(Vector3[] pts, float t)
        {
            float omt = 1f - t;
            float omt2 = omt * omt;
            float t2 = t * t;
            return pts[0] * (omt2 * omt) +
                    pts[1] * (3f * omt2 * t) +
                    pts[2] * (3f * omt * t2) +
                    pts[3] * (t2 * t);
        }

        // Get Tangent
        // Tangent is e - d (different of last 2 points) 
        // then normalizing it 
        Vector3 GetTangent(Vector3[] pts, float t)
        {
            float omt = 1f - t;
            float omt2 = omt * omt;
            float t2 = t * t;
            Vector3 tangent =
                        pts[0] * (-omt2) +
                        pts[1] * (3 * omt2 - 2 * omt) +
                        pts[2] * (-3 * t2 + 2 * t) +
                        pts[3] * (t2);
            return tangent.normalized;
        }

    }
}
