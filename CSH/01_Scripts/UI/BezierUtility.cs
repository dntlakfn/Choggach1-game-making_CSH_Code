using UnityEngine;

namespace CSH.Scripts.UIs
{
    public static class BezierUtility
    {
        /// <summary>
        /// 2차 베지에 곡선 위의 특정 위치(t)에 있는 점의 좌표를 구합니다.
        /// (p0: 시작점, p1: 제어점, p2: 끝점)
        /// </summary>
        /// <param name="t">0.0 (시작) ~ 1.0 (끝)</param>
        public static Vector2 GetPointOnQuadraticBezier(Vector2 p0, Vector2 p1, Vector2 p2, float t)
        {
            float u = 1f - t;
            float tt = t * t;
            float uu = u * u;

            // 2차 베지에 곡선 공식 계산
            Vector2 point = uu * p0;         // (1-t)^2 * P0
            point += 2f * u * t * p1;        // 2 * (1-t) * t * P1
            point += tt * p2;                // t^2 * P2

            return point;
        }

        /// <summary>
        /// 2차 베지에 곡선을 여러 개의 직선으로 쪼개서 대략적인 총 길이를 구합니다.
        /// </summary>
        /// <param name="resolution">곡선을 몇 개로 쪼갤 것인지 (보통 10~20이면 충분)</param>
        public static float GetQuadraticBezierLength(Vector2 p0, Vector2 p1, Vector2 p2, int resolution = 20)
        {
            float totalLength = 0f;
            Vector2 previousPoint = p0;

            // t 값을 0에서 1까지 서서히 증가시키며 점과 점 사이의 거리를 누적
            for (int i = 1; i <= resolution; i++)
            {
                float t = (float)i / resolution;
                Vector2 currentPoint = GetPointOnQuadraticBezier(p0, p1, p2, t);

                totalLength += Vector2.Distance(previousPoint, currentPoint);
                previousPoint = currentPoint;
            }

            return totalLength;
        }
    }
}