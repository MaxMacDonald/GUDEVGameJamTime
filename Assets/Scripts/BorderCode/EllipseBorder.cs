using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class EllipseBorder : MonoBehaviour
{
    [Header("Ellipse Shape")]
    public float radiusX = 8f;
    public float radiusY = 5f;
    public int pointCount = 64;         // more points = smoother border

    [Header("Border Appearance")]
    public float borderWidth = 0.3f;
    public Color borderColor = new Color(0.2f, 0.8f, 1f, 0.8f);

    [Header("Spring Settings")]
    public float springConstant = 50f;
    public float damping = 5f;
    public float spreadFactor = 0.08f;  // how much impact spreads to neighbours

    [Header("Collision")]
    public float hitForce = 8f;
    public float hitRadius = 1.5f;      // how close something needs to be to cause a ripple

    public SpringPoint[] springs;
    private Mesh mesh;
    private Vector3[] vertices;
    private int[] triangles;

    void Start()
    {
        Shader writeShader = Shader.Find("Custom/StencilWrite");
        Shader voidShader = Shader.Find("Custom/StencilVoid");
        Debug.Log($"StencilWrite found: {writeShader != null}");
        Debug.Log($"StencilVoid found: {voidShader != null}");

        // Initialise springs
        springs = new SpringPoint[pointCount];
        for (int i = 0; i < pointCount; i++)
            springs[i] = new SpringPoint();

        // Set up mesh
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshRenderer>().material = CreateMaterial();

        BuildMesh();
        UpdateCollider();
        
       

    }

    void FixedUpdate()
    {
        // Update all springs
        for (int i = 0; i < pointCount; i++)
            springs[i].Update(springConstant, damping);

        // Spread displacement to neighbours
        for (int i = 0; i < pointCount; i++)
        {
            int left = (i - 1 + pointCount) % pointCount;
            int right = (i + 1) % pointCount;

            springs[left].velocity += spreadFactor * (springs[i].position - springs[left].position);
            springs[right].velocity += spreadFactor * (springs[i].position - springs[right].position);
        }

        UpdateMesh();
    }

    void BuildMesh()
    {
        // 2 vertices per point (inner and outer edge of border)
        vertices = new Vector3[pointCount * 2];
        triangles = new int[pointCount * 6];

        for (int i = 0; i < pointCount; i++)
        {
            float angle = (float)i / pointCount * Mathf.PI * 2f;
            Vector2 basePoint = GetEllipsePoint(angle, 0f);
            Vector2 outerPoint = GetEllipsePoint(angle, borderWidth);

            vertices[i] = basePoint;
            vertices[i + pointCount] = outerPoint;

            // Build triangles
            int next = (i + 1) % pointCount;
            int ti = i * 6;
            triangles[ti] = i;
            triangles[ti + 1] = next;
            triangles[ti + 2] = i + pointCount;
            triangles[ti + 3] = next;
            triangles[ti + 4] = next + pointCount;
            triangles[ti + 5] = i + pointCount;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }

    void UpdateMesh()
    {
        for (int i = 0; i < pointCount; i++)
        {
            float angle = (float)i / pointCount * Mathf.PI * 2f;
            Vector2 basePoint = GetEllipsePoint(angle, springs[i].position);
            Vector2 outerPoint = GetEllipsePoint(angle, springs[i].position + borderWidth);

            vertices[i] = basePoint;
            vertices[i + pointCount] = outerPoint;
        }

        mesh.vertices = vertices;
        mesh.RecalculateNormals();
    }

    void UpdateCollider()
    {
        PolygonCollider2D col = GetComponent<PolygonCollider2D>();
        if (col == null) return;

        Vector2[] points = new Vector2[pointCount];
        for (int i = 0; i < pointCount; i++)
        {
            float angle = (float)i / pointCount * Mathf.PI * 2f;
            points[i] = new Vector2(
                radiusX * Mathf.Cos(angle),
                radiusY * Mathf.Sin(angle)
            );
        }

        col.SetPath(0, points);
    }

    Vector2 GetEllipsePoint(float angle, float displacement)
    {
        // Normal pointing outward from ellipse at this angle
        Vector2 normal = new Vector2(
            Mathf.Cos(angle) / radiusX,
            Mathf.Sin(angle) / radiusY
        ).normalized;

        return new Vector2(
            (radiusX + displacement) * Mathf.Cos(angle),
            (radiusY + displacement) * Mathf.Sin(angle)
        ) + normal * displacement;
    }

    // Call this from bullets or enemies when they hit the border
    public void Ripple(Vector2 hitPosition, float force, bool pushOutward = true)
    {
        for (int i = 0; i < pointCount; i++)
        {
            float angle = (float)i / pointCount * Mathf.PI * 2f;
            Vector2 pointPos = new Vector2(
                radiusX * Mathf.Cos(angle),
                radiusY * Mathf.Sin(angle)
            );

            float distance = Vector2.Distance(hitPosition, pointPos);
            if (distance < hitRadius)
            {
                float impact = force * (1f - distance / hitRadius);
                // pushOutward = true means border pushed out (enemy hitting from outside)
                // pushOutward = false means border pushed in (bullet hitting from inside)
                springs[i].Hit(pushOutward ? impact : -impact);
            }
        }
    }

    Material CreateMaterial()
    {
        Material mat = new Material(Shader.Find("Sprites/Default"));
        mat.color = borderColor;
        return mat;
    }


}