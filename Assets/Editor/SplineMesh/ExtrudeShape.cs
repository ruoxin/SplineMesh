using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Presar.Builder.ProceduralGeneration
{
    public class ExtrudeShape
    {
        private Vector2[] vertices;
        private Vector2[] normals;
        private int[] lines;
        private float[] uCoord;
        private float shapeLength;

        public ExtrudeShape(Vector2[] vertices, int[] lines)
        {
            if (vertices.Length < 2)
            {
                Debug.LogError("ExtrudeShape can not be less than 2 vertices");
                return;
            }
            this.vertices = vertices;
            this.lines = lines;

            shapeLength = 0f;
            for (int i = 1; i < vertices.Length; i++)
            {
                shapeLength += Vector2.Distance(vertices[i], vertices[i - 1]);
            }

            uCoord = new float[vertices.Length];
            uCoord[0] = 0f;
            float length = 0f;
            for (int i = 1; i < vertices.Length; i++)
            {
                length += Vector2.Distance(vertices[i], vertices[i - 1]);
                uCoord[i] = length / shapeLength;
            }

            normals = new Vector2[vertices.Length];
            for (int i = 0; i < lines.Length; i += 2)
            {
                Vector2 normal = Vector2.Perpendicular(vertices[lines[i + 1]] - vertices[lines[i]]);
                normals[lines[i]] = normal;
                normals[lines[i + 1]] = normal;
            }
        }

        public Vector2[] Vertices
        {
            get
            {
                return vertices;
            }
        }

        public Vector2[] Normals
        {
            get
            {
                return normals;
            }
        }

        public int[] Lines
        {
            get
            {
                return lines;
            }
        }

        public float[] UCoord
        {
            get
            {
                return uCoord;
            }
        }

        public float ShapeLength
        {
            get
            {
                return shapeLength;
            }
        }

    } 
}
