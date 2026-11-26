using UnityEngine;

/// <summary>Samples a random point and direction on a shape, like the Particle System Shape module.</summary>
public class ShapeSampler : MonoBehaviour
{
    public enum Shape { Sphere, Hemisphere, Circle, Cone, Box, Edge }

    private const int ArcSegments = 48;

    [SerializeField] Shape shape = Shape.Circle;
    [SerializeField, Min(0f)] float radius = 1f;
    [Tooltip("1 = fills the whole area/volume, 0 = border only")]
    [SerializeField, Range(0f, 1f)] float radiusThickness = 1f;
    [Tooltip("Portion of the circle that is used (Circle and Cone)")]
    [SerializeField, Range(0f, 360f)] float arc = 360f;
    [Tooltip("Cone opening at the edge of the base")]
    [SerializeField, Range(0f, 90f)] float coneAngle = 25f;
    [SerializeField] Vector3 boxSize = Vector3.one;
    [Tooltip("Box only: 1 = full volume, 0 = surface only")]
    [SerializeField, Range(0f, 1f)] float boxThickness = 1f;
    [Header("Gizmos")]
    [SerializeField] Color shapeColor = Color.cyan;
    [SerializeField] Color directionColor = Color.yellow;
    [SerializeField, Min(0.01f)] float arrowLength = 1f;


    void OnValidate()
    {
        boxSize = Vector3.Max(boxSize, Vector3.zero);
    }

    /// <summary>Returns a random position and direction in world space.</summary>
    public void Sample(out Vector3 position, out Vector3 direction)
    {
        SampleLocal(out Vector3 p, out Vector3 d);
        position = transform.TransformPoint(p);
        direction = transform.TransformDirection(d).normalized;
    }

    private void SampleLocal(out Vector3 p, out Vector3 d)
    {
        float inner = 1f - radiusThickness;

        switch (shape)
        {
            case Shape.Sphere:
            case Shape.Hemisphere:
                {
                    d = Random.onUnitSphere;

                    // The hemisphere faces +Z
                    if (shape == Shape.Hemisphere)
                        d.z = Mathf.Abs(d.z);

                    // Cube root gives a uniform distribution in volume
                    float r = radius * Mathf.Pow(Random.Range(inner * inner * inner, 1f), 1f / 3f);
                    p = d * r;
                    break;
                }

            case Shape.Circle:
            case Shape.Cone:
                {
                    float theta = Random.Range(0f, arc) * Mathf.Deg2Rad;
                    Vector2 radial = new Vector2(Mathf.Cos(theta), Mathf.Sin(theta));

                    // Square root gives a uniform distribution in area
                    float t = Mathf.Sqrt(Random.Range(inner * inner, 1f));
                    p = new Vector3(radial.x, radial.y, 0f) * (radius * t);

                    if (shape == Shape.Circle)
                    {
                        d = new Vector3(radial.x, radial.y, 0f);
                    }
                    else
                    {
                        // Base center points forward; the edge tilts by coneAngle
                        float a = coneAngle * t * Mathf.Deg2Rad;
                        d = new Vector3(radial.x * Mathf.Sin(a), radial.y * Mathf.Sin(a), Mathf.Cos(a));
                    }
                    break;
                }

            case Shape.Box:
                {
                    Vector3 h = boxSize * 0.5f;
                    p = SampleBoxShell(h, BoxInner(h));

                    // Direction is the normal of the closest wall (zero-size axes are ignored)
                    float dx = h.x > 0f ? h.x - Mathf.Abs(p.x) : float.PositiveInfinity;
                    float dy = h.y > 0f ? h.y - Mathf.Abs(p.y) : float.PositiveInfinity;
                    float dz = h.z > 0f ? h.z - Mathf.Abs(p.z) : float.PositiveInfinity;

                    if (dx <= dy && dx <= dz)
                        d = new Vector3(Mathf.Sign(p.x), 0f, 0f);
                    else if (dy <= dz)
                        d = new Vector3(0f, Mathf.Sign(p.y), 0f);
                    else
                        d = new Vector3(0f, 0f, Mathf.Sign(p.z));
                    break;
                }

            default: // Edge
                p = new Vector3(Random.Range(-radius, radius), 0f, 0f);
                d = Vector3.up;
                break;
        }
    }

    // Half-extents of the inner box: a scaled copy of the outer one
    private Vector3 BoxInner(Vector3 h)
    {
        return h * (1f - Mathf.Max(boxThickness, 0.0001f));
    }

    // Uniform random point in the shell between the outer and the inner box
    private static Vector3 SampleBoxShell(Vector3 h, Vector3 i)
    {
        // Flat axes (h = 0) count as 1 only when computing the weights
        float Full(float he)
        {
            return he > 0f ? 2f * he : 1f;
        }

        float Span(float ie, float he)
        {
            return he > 0f ? 2f * ie : 1f;
        }

        float RandSign()
        {
            return Random.value < 0.5f ? -1f : 1f;
        }

        // The shell is split into 3 regions that do not overlap:
        // full X walls, Y walls (excluding X) and Z walls (excluding X and Y)
        float vx = 2f * (h.x - i.x) * Full(h.y) * Full(h.z);
        float vy = 2f * (h.y - i.y) * Span(i.x, h.x) * Full(h.z);
        float vz = 2f * (h.z - i.z) * Span(i.x, h.x) * Span(i.y, h.y);
        float total = vx + vy + vz;

        if (total <= 0f) // Degenerate box
            return new Vector3(Random.Range(-h.x, h.x), Random.Range(-h.y, h.y), Random.Range(-h.z, h.z));

        float r = Random.value * total;

        if (r < vx)
            return new Vector3(
                RandSign() * Random.Range(i.x, h.x),
                Random.Range(-h.y, h.y),
                Random.Range(-h.z, h.z));

        if (r < vx + vy)
            return new Vector3(
                Random.Range(-i.x, i.x),
                RandSign() * Random.Range(i.y, h.y),
                Random.Range(-h.z, h.z));

        return new Vector3(
            Random.Range(-i.x, i.x),
            Random.Range(-i.y, i.y),
            RandSign() * Random.Range(i.z, h.z));
    }

    void OnDrawGizmosSelected()
    {
        float inner = radius * (1f - radiusThickness);

        switch (shape)
        {
            case Shape.Sphere:
            case Shape.Hemisphere:
                {
                    bool half = shape == Shape.Hemisphere;

                    Gizmos.color = shapeColor;
                    DrawSphereGizmo(radius, half);
                    if (inner > 0f)
                        DrawSphereGizmo(inner, half);

                    Gizmos.color = directionColor;
                    Vector3[] axes = { Vector3.right, Vector3.left, Vector3.up, Vector3.down, Vector3.forward, Vector3.back };
                    foreach (Vector3 axis in axes)
                    {
                        if (half && axis.z < 0f)
                            continue;

                        DrawArrow(axis * radius, axis);
                    }
                    break;
                }

            case Shape.Circle:
            case Shape.Cone:
                {
                    Gizmos.color = shapeColor;
                    DrawArc(Vector3.zero, Vector3.right, Vector3.up, radius, 0f, arc);
                    if (inner > 0f)
                        DrawArc(Vector3.zero, Vector3.right, Vector3.up, inner, 0f, arc);

                    if (arc < 360f)
                    {
                        // Lines that close the sector
                        foreach (float deg in new[] { 0f, arc })
                        {
                            Vector3 dir = new Vector3(Mathf.Cos(deg * Mathf.Deg2Rad), Mathf.Sin(deg * Mathf.Deg2Rad), 0f);
                            DrawLine(dir * inner, dir * radius);
                        }
                    }

                    Gizmos.color = directionColor;

                    if (shape == Shape.Circle)
                    {
                        // Radial arrows
                        int count = 8;
                        for (int i = 0; i < count; i++)
                        {
                            float t = arc >= 360f ? i / (float)count : i / (float)(count - 1);
                            float th = arc * t * Mathf.Deg2Rad;
                            Vector3 dir = new Vector3(Mathf.Cos(th), Mathf.Sin(th), 0f);
                            DrawArrow(dir * radius, dir);
                        }
                    }
                    else
                    {
                        // Direction from the base center
                        DrawArrow(Vector3.zero, Vector3.forward);

                        // Directions at the edge, tilted by coneAngle
                        float a = coneAngle * Mathf.Deg2Rad;
                        int rays = 4;
                        for (int i = 0; i < rays; i++)
                        {
                            float t = arc >= 360f ? i / (float)rays : i / (float)(rays - 1);
                            float th = arc * t * Mathf.Deg2Rad;
                            Vector3 origin = new Vector3(Mathf.Cos(th), Mathf.Sin(th), 0f) * radius;
                            Vector3 dir = new Vector3(Mathf.Cos(th) * Mathf.Sin(a), Mathf.Sin(th) * Mathf.Sin(a), Mathf.Cos(a));
                            DrawArrow(origin, dir);
                        }

                        // Circle at the arrow tips to show the opening
                        DrawArc(new Vector3(0f, 0f, Mathf.Cos(a) * arrowLength), Vector3.right, Vector3.up,
                                radius + Mathf.Sin(a) * arrowLength, 0f, arc);
                    }
                    break;
                }

            case Shape.Box:
                {
                    Vector3 h = boxSize * 0.5f;

                    Gizmos.color = shapeColor;
                    DrawBoxWire(h);

                    // Inner box (no points are generated inside), dimmer
                    if (boxThickness < 1f)
                    {
                        Gizmos.color = new Color(shapeColor.r, shapeColor.g, shapeColor.b, 0.4f);
                        DrawBoxWire(BoxInner(h));
                    }

                    // One arrow out of the center of each wall
                    Gizmos.color = directionColor;
                    if (h.x > 0f)
                    {
                        DrawArrow(Vector3.right * h.x, Vector3.right);
                        DrawArrow(Vector3.left * h.x, Vector3.left);
                    }
                    if (h.y > 0f)
                    {
                        DrawArrow(Vector3.up * h.y, Vector3.up);
                        DrawArrow(Vector3.down * h.y, Vector3.down);
                    }
                    if (h.z > 0f)
                    {
                        DrawArrow(Vector3.forward * h.z, Vector3.forward);
                        DrawArrow(Vector3.back * h.z, Vector3.back);
                    }
                    break;
                }

            default: // Edge
                {
                    Gizmos.color = shapeColor;
                    DrawLine(Vector3.left * radius, Vector3.right * radius);

                    Gizmos.color = directionColor;
                    for (int i = 0; i < 5; i++)
                        DrawArrow(Vector3.right * Mathf.Lerp(-radius, radius, i / 4f), Vector3.up);
                    break;
                }
        }
    }

    private static Vector3 Corner(int i, Vector3 h)
    {
        return new Vector3((i & 1) == 0 ? -h.x : h.x, (i & 2) == 0 ? -h.y : h.y, (i & 4) == 0 ? -h.z : h.z);
    }

    // Takes local-space points
    private void DrawLine(Vector3 localA, Vector3 localB)
    {
        Gizmos.DrawLine(transform.TransformPoint(localA), transform.TransformPoint(localB));
    }

    private void DrawArc(Vector3 center, Vector3 axisA, Vector3 axisB, float r, float startDeg, float endDeg)
    {
        int steps = Mathf.Max(1, Mathf.CeilToInt(ArcSegments * Mathf.Abs(endDeg - startDeg) / 360f));
        Vector3 prev = center + (axisA * Mathf.Cos(startDeg * Mathf.Deg2Rad) + axisB * Mathf.Sin(startDeg * Mathf.Deg2Rad)) * r;

        for (int i = 1; i <= steps; i++)
        {
            float ang = Mathf.Lerp(startDeg, endDeg, i / (float)steps) * Mathf.Deg2Rad;
            Vector3 next = center + (axisA * Mathf.Cos(ang) + axisB * Mathf.Sin(ang)) * r;
            DrawLine(prev, next);
            prev = next;
        }
    }

    private void DrawSphereGizmo(float r, bool half)
    {
        float end = half ? 180f : 360f;
        DrawArc(Vector3.zero, Vector3.right, Vector3.up, r, 0f, 360f); // XY (equator)
        DrawArc(Vector3.zero, Vector3.right, Vector3.forward, r, 0f, end); // XZ
        DrawArc(Vector3.zero, Vector3.up, Vector3.forward, r, 0f, end); // YZ
    }

    private void DrawBoxWire(Vector3 h)
    {
        for (int i = 0; i < 8; i++)
        {
            Vector3 c = Corner(i, h);
            for (int bit = 1; bit <= 4; bit <<= 1)
            {
                if ((i & bit) == 0)
                    DrawLine(c, Corner(i | bit, h));
            }
        }
    }

    private void DrawArrow(Vector3 localFrom, Vector3 localDir)
    {
        Vector3 from = transform.TransformPoint(localFrom);
        Vector3 dir = transform.TransformDirection(localDir).normalized;
        Vector3 to = from + dir * arrowLength;

        Gizmos.DrawLine(from, to);

        // Arrow head
        Vector3 side = Vector3.Cross(dir, Vector3.up);
        if (side.sqrMagnitude < 0.001f)
            side = Vector3.Cross(dir, Vector3.right);
        side.Normalize();
        Vector3 up = Vector3.Cross(dir, side);

        float head = arrowLength * 0.2f;
        Vector3 back = to - dir * head;
        Gizmos.DrawLine(to, back + side * head * 0.5f);
        Gizmos.DrawLine(to, back - side * head * 0.5f);
        Gizmos.DrawLine(to, back + up * head * 0.5f);
        Gizmos.DrawLine(to, back - up * head * 0.5f);
    }
}
