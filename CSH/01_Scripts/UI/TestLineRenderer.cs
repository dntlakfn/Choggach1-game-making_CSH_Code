using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class TestLineRenderer : MonoBehaviour
{
    public Transform controlPoint;  // 조절점

    public int vertexCount = 30;    // 곡선을 구성할 점의 개수 (많을수록 부드러움)
    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        DrawCurve();
    }

    void DrawCurve()
    {
        lineRenderer.positionCount = vertexCount;

        for (int i = 0; i < vertexCount; i++)
        {
            // 0에서 1 사이의 비율 계산
            float t = i / (float)(vertexCount - 1);

            Vector3 endPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            endPoint.z = 0;

            controlPoint.position = (transform.position + endPoint) / 2 + Vector3.up * 5;

            Vector3 position = CalculateBezierPoint(t, transform.position, controlPoint.position, endPoint);

            lineRenderer.SetPosition(i, position);
        }
    }

    Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        // B(t) = (1-t)^2 * P0 + 2(1-t)t * P1 + t^2 * P2
        float u = 1 - t;
        Vector3 p = u * u * p0; // (1-t)^2 * P0
        p += 2 * u * t * p1;    // 2 * (1-t) * t * P1
        p += t * t * p2;        // t^2 * P2
        return p;
    }
}