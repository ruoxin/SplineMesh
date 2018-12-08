using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadGenerator : MonoBehaviour {


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


    // Get Normal of the tangent 
    // 2D simply rotate it 90 degree 
    Vector3 GetNormal2D(Vector3[] pts, float t)
    {
        Vector3 tng = GetTangent(pts, t);
        return new Vector3(-tng.y, tng.x, 0f);
    }

    // 3D need to have a reference vector for the up direction 
    // cross product of the up vector with the tangent to creat binomal
    // then cross product of tangent and binomal
    Vector3 GetNormal3D(Vector3[] pts, float t, Vector3 up)
    {
        Vector3 tng = GetTangent(pts, t);
        Vector3 binormal = Vector3.Cross(up, tng).normalized;
        return Vector3.Cross(tng, binormal);
    }

    // when we have the point xyz ( binormal, normal, tangent)




    // Get Orientation
    Quaternion GetOrientation2D(Vector3[] pts, float t)
    {
        Vector3 tng = GetTangent(pts, t);
        Vector3 nrm = GetNormal2D(pts, t);
        return Quaternion.LookRotation(tng, nrm);
    }
    

    Quaternion GetOrientation3D(Vector3[] pts, float t, Vector3 up)
    {
        Vector3 tng = GetTangent(pts, t);
        Vector3 nrm = GetNormal3D(pts, t, up);
        return Quaternion.LookRotation(tng, nrm);
    }

    

    public struct OrientedPoint
    {

        public Vector3 position;
        public Quaternion rotation;

        public OrientedPoint(Vector3 position, Quaternion rotation)
        {
            this.position = position;
            this.rotation = rotation;
        }

        public Vector3 LocalToWorld(Vector3 point)
        {
            return position + rotation * point;
        }

        public Vector3 WorldToLocal(Vector3 point)
        {
            return Quaternion.Inverse(rotation) * (point - position);
        }

        public Vector3 LocalToWorldDirection(Vector3 dir)
        {
            return rotation * dir;
        }
    }




//    public void Extrude(Mesh mesh, ExtrudeShape shape, OrientedPoint[] path)
//    {

//        int vertsInShape = shape.vert2Ds.Length;
//        int segments = path.Length - 1;
//        int edgeLoops = path.Length;
//        int vertCount = vertsInShape * edgeLoops;
//        int triCount = shape.lines.Length * segments;
//        int triIndexCount = triCount * 3;
//        int vertsInShape = shape.vert2Ds.Length;
//        int segments = path.Length - 1;
//        int edgeLoops = path.Length;
//        int vertCount = vertsInShape * edgeLoops;
//        int triCount = shape.lines.Length * segments;
//        int triIndexCount = triCount * 3;

//        int[] triangleIndices = new int[triIndexCount];
//        Vector3[] vertices = new Vector3[vertCount];
//        Vector3[] normals = new Vector3[vertCount];
//        Vector2[] uvs = new Vector2[vertCount];

//        /* Generation code goes here */

//        mesh.Clear();
//        mesh.vertices = vertices;
//        mesh.triangles = triangleIndices;
//        mesh.normals = normals;
//        mesh.uv = uvs;
//        /*
//        foreach oriented point in the path
//            foreach vertex in the 2D shape
//                Add the vertex position, based on the oriented point
//                Add the normal direction, based on the oriented point
//                Add the UV. U is based on the shape, V is based on distance along the path
//            end
//        end
//        foreach segment
//            foreach line in the 2D shape
//                Add two triangles with vertex indices based on the line indices
//            end
//        end*/

//        for (int i = 0; i < path.Length; i++)
//        {
//            int offset = i * vertsInShape;
//            for (int j = 0; j < vertsInShape; j++)
//            {
//                int id = offset + j;
//                vertices[id] = path[i].LocalToWorld(shape.vert2Ds[j].point);
//                normals[id] = path[i].LocalToWorldDirection(shape.vert2Ds[j].normal);
//                uvs[id] = new Vector2(vert2Ds[j].uCoord, i / ((float)edgeLoops));
//            }
//        }
//        int ti = 0;
//        for (int i = 0; i < segments; i++)
//        {
//            int offset = i * vertsInShape;
//            for (int l = 0; l < lines.Length; l += 2)
//            {
//                int a = offset + lines[l] + vertsInShape;
//                int b = offset + lines[l];
//                int c = offset + lines[l + 1];
//                int d = offset + lines[l + 1] + vertsInShape;
//                triangleIndices[ti] = a; ti++;
//                triangleIndices[ti] = b; ti++;
//                triangleIndices[ti] = c; ti++;
//                triangleIndices[ti] = c; ti++;
//                triangleIndices[ti] = d; ti++;
//                triangleIndices[ti] = a; ti++;
//            }
//        }


//        // 
//        void CalcLengthTableInto(float[] arr, CubicBezier3D bezier)
//        {
//            arr[0] = 0f;
//            float totalLength = 0f;
//            Vector3 prev = bezier.p0;
//            for (int i = 1; i < arr.Length; i++)
//            {
//                float t = ((float)i) / (arr.Length - 1);
//                Vector3 pt = bezier.GetPoint(t);
//                float diff = (prev - pt).magnitude;
//                totalLength += diff;
//                arr[i] = totalLength;
//                prev = pt;
//            }
//        }

//        // 
//    }

//}

//public static class FloatArrayExtensions
//{
//    public static float Sample(this float[] fArr, float t)
//    {
//        int count = fArr.Length;
//        if (count == 0)
//        {
//            Debug.LogError("Unable to sample array - it has no elements");
//            return 0;
//        }
//        if (count == 1)
//        {
//            return fArr[0];
//            float iFloat = t * (count - 1);
//            int idLower = Mathf.FloorToInt(iFloat);
//            int idUpper = Mathf.FloorToInt(iFloat + 1);
//            if (idUpper >= count)
//                return fArr[count - 1];
//            if (idLower < 0)
//                return fArr[0];
//            return Mathf.Lerp(fArr[idLower], fArr[idUpper], iFloat - idLower);
//        }
//            return 0;
//    }


}


public class Quad
{
    public Vector3[] vertices = new Vector3[]
    {
        new Vector3 (1, 0, 1),
        new Vector3 (-1, 0, 1),
        new Vector3 (1, 0, -1),
        new Vector3 (-1, 0, -1),
    };

    public Vector3[] normals = new Vector3[]
    {
        new Vector3 (0, 1, 0),
        new Vector3 (0, 1, 0),
        new Vector3 (0, 1, 0),
        new Vector3 (0, 1, 0),
    };


    public Vector2[] uvs = new Vector2[]
    {
        new Vector3 (0, 1),
        new Vector3 (0, 0),
        new Vector3 (1, 1),
        new Vector3 (1, 0),
    };

    public int[] triangles = new int []
    {
        // just define the point relationship
        0, 2, 3, // first triangle 
        3, 1, 0, // second triangle 
    };
    

}

public class MyMesh: MonoBehaviour
{
   public Mesh CreateMesh()
    {
        MeshFilter mf = GetComponent<MeshFilter>();
        if (mf.sharedMesh == null)
            mf.sharedMesh = new Mesh();

        Mesh mesh = mf.sharedMesh;
        Quad quad = new Quad();

        mesh.Clear();
        mesh.vertices = quad.vertices;
        mesh.normals = quad.normals;
        mesh.uv = quad.uvs;
        mesh.triangles = quad.triangles;

        return mesh;
    }

}


//public class UniqueMesh: MonoBehaviour
//{
//    [HideInInspector]
//    int ownerId;

//    MeshFilter mf;

//    public MeshFilter Mf
//    {
//        get
//        {
//            mf = mf == null ? GetComponent<MeshFilter>() : mf;
//            mf = mf == null ? gameObject.AddComponent<MeshFilter>() : mf;
//            return mf;
//        }
//    }

//    Mesh mesh;
//    protected Mesh Mesh
//    {
//        get
//        {
//            bool isOwner = ownerId == gameObject.GetInstanceID();
//            if (Mf.sharedMesh == null || !isOwner)
//            {
//                mf.sharedMesh = mesh = new Mesh();
//                ownerId = gameObject.GetInstanceID();
//                mesh.name = "Mesh [" + ownerId + "]";
//            }
//            return mesh;
//        }

//    }


//}






