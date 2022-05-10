using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trace : MonoBehaviour
{
    public float duration = 0.1f;
   
    float createTime;
    public Vector3[] vertices;
    public Trace(Vector3[] vt,float d)
    {
        vertices = vt;
        duration = d;
    }
    // Start is called before the first frame update
    void Start()
    {

        CreateMesh();
        createTime = Time.time;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
        if (Time.time - createTime > duration)
        {
            Destroy(gameObject);
        }
    }

    void CreateMesh()
    {
        Mesh mesh = new Mesh();
        Vector2[] uv = new Vector2[4];
        int[] triangles = new int[6];

        

        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 2;
        
        triangles[3] = 2;
        triangles[4] = 3;
        triangles[5] = 0;
        

        mesh.vertices = this.vertices;
        mesh.triangles = triangles;
        GetComponent<MeshFilter>().mesh = mesh;

    }
}
