using UnityEngine;
using System.Collections.Generic;

public class ArenaMaskDynamic : MonoBehaviour
{
    [Header("References")]
    public EllipseBorder ellipseBorder;
    public Material voidMaterial; // assign a black Sprites/Default material

    private Mesh maskMesh;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;

    void Start()
    {
        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.material = voidMaterial;
        meshRenderer.sortingOrder = 9; // above game sprites, below UI

        maskMesh = new Mesh();
        meshFilter.mesh = maskMesh;

        UpdateMask();
    }

    void LateUpdate()
    {
        UpdateMask();
    }

    void UpdateMask()
    {
        int pointCount = ellipseBorder.pointCount;
        float s = 100f;

        // Total verts: 4 outer corners + pointCount ellipse points
        int totalVerts = 4 + pointCount;
        Vector3[] verts = new Vector3[totalVerts];

        // Outer corners (indices 0-3)
        verts[0] = new Vector3(-s, -s, 0); // bottom left
        verts[1] = new Vector3(s, -s, 0); // bottom right
        verts[2] = new Vector3(s, s, 0); // top right
        verts[3] = new Vector3(-s, s, 0); // top left

        // Ellipse points (indices 4 to 4+pointCount)
        for (int i = 0; i < pointCount; i++)
        {
            float angle = (float)i / pointCount * Mathf.PI * 2f;
            if (ellipseBorder.springs == null) return; // safety check
            float displacement = ellipseBorder.springs[i].position + ellipseBorder.borderWidth;  // add borderWidth here

            verts[i + 4] = new Vector3(
                (ellipseBorder.radiusX + displacement) * Mathf.Cos(angle),
                (ellipseBorder.radiusY + displacement) * Mathf.Sin(angle),
                0
            );
        }

        // Build triangles
        // We need to fill the area between the outer quad and the ellipse
        // Split into 4 sections (one per quadrant) and connect to corners
        List<int> triList = new List<int>();

        for (int i = 0; i < pointCount; i++)
        {
            int curr = i + 4;
            int next = (i + 1) % pointCount + 4;

            Vector3 currPoint = verts[curr];
            Vector3 nextPoint = verts[next];

            // Find which corner(s) to connect to based on quadrant
            int currCorner = GetCornerIndex(currPoint);
            int nextCorner = GetCornerIndex(nextPoint);

            // Always add triangle from curr to next to their shared corner
            triList.Add(curr);
            triList.Add(next);
            triList.Add(currCorner);

            // If the two points span different corners, fill the corner gap
            if (currCorner != nextCorner)
            {
                triList.Add(currCorner);
                triList.Add(next);
                triList.Add(nextCorner);

                // Fill any corners in between
                int c = currCorner;
                while (c != nextCorner)
                {
                    int nextC = (c + 1) % 4;
                    triList.Add(curr);
                    triList.Add(c);
                    triList.Add(nextC);
                    c = nextC;
                }
            }
        }

        maskMesh.Clear();
        maskMesh.vertices = verts;
        maskMesh.triangles = triList.ToArray();
        maskMesh.RecalculateNormals();
    }

    int GetCornerIndex(Vector3 point)
    {
        // Returns which of the 4 corners (0=BL, 1=BR, 2=TR, 3=TL) this point faces
        if (point.x >= 0 && point.y < 0) return 1;  // bottom right
        if (point.x >= 0 && point.y >= 0) return 2; // top right
        if (point.x < 0 && point.y >= 0) return 3;  // top left
        return 0;                                     // bottom left
    }
}