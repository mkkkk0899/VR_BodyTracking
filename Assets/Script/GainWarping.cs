using UnityEngine;

namespace VRBodyTracking
{
    /// <summary>
    /// ゲインワーピング（距離伸縮）実装
    /// 論文アルゴリズム: ワープ原点からの移動距離にゲイン係数を適用
    /// </summary>
    public class GainWarping : HandRedirectionBase
    {
        [Header("ゲインワーピング設定")]
        [Tooltip("ゲイン係数 - 1.0より大きいと距離が伸びる、小さいと縮む")]
        [Range(0.5f, 2.0f)]
        public float gain = 1.0f;

        [Header("詳細設定")]
        [Tooltip("ゲインを適用する最小距離（メートル）- これより近い場合はゲインを適用しない")]
        [Range(0f, 0.2f)]
        public float minDistanceThreshold = 0.05f;

        protected override Vector3 CalculateRedirectedPosition(Vector3 realHandPosition)
        {
            // 1. 原点から実際の手までのオフセットベクトルを算出: d_r = p_r - o
            Vector3 offsetFromOrigin = realHandPosition - warpOrigin.position;

            // 距離が最小閾値未満の場合はゲインを適用せず、そのまま返す
            float distance = offsetFromOrigin.magnitude;
            if (distance < minDistanceThreshold)
            {
                return realHandPosition;
            }

            // 2. ゲインを適用: d_v = g * d_r
            Vector3 redirectedOffset = gain * offsetFromOrigin;

            // 3. 最終位置を決定: p_v = o + d_v
            Vector3 redirectedPosition = warpOrigin.position + redirectedOffset;

            return redirectedPosition;
        }

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();

            if (!showDebugGizmos || !ValidateReferences())
            {
                return;
            }

            // ゲイン効果を視覚化
            Vector3 originPos = warpOrigin.position;
            Vector3 realPos = realHand.position;
            Vector3 virtualPos = virtualHand.position;

            // ゲイン適用前と後の距離を表示
            float realDistance = Vector3.Distance(originPos, realPos);
            float virtualDistance = Vector3.Distance(originPos, virtualPos);

            // 距離の比較線を描画
            if (realDistance > minDistanceThreshold)
            {
                // 実際の距離を青で表示
                Gizmos.color = Color.blue;
                DrawDistanceArc(originPos, realPos, realDistance);

                // 仮想の距離を赤で表示
                Gizmos.color = Color.red;
                DrawDistanceArc(originPos, virtualPos, virtualDistance);

                // ゲイン係数をテキストとして表示するための目印
                Gizmos.color = Color.yellow;
                Vector3 midPoint = (realPos + virtualPos) * 0.5f;
                Gizmos.DrawWireSphere(midPoint, 0.01f);
            }

            // 最小距離閾値の球を描画
            Gizmos.color = new Color(1f, 1f, 0f, 0.3f);
            Gizmos.DrawWireSphere(originPos, minDistanceThreshold);

            // ゲインスケールを示す同心円を描画
            Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
            float[] scales = { 0.1f, 0.2f, 0.3f, 0.4f, 0.5f };
            foreach (float scale in scales)
            {
                DrawCircle(originPos, scale, Vector3.up);

                // ゲイン適用後の半径
                float scaledRadius = scale * gain;
                Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
                DrawCircle(originPos, scaledRadius, Vector3.up);
            }
        }

        /// <summary>
        /// 2点間の距離を円弧で表示
        /// </summary>
        private void DrawDistanceArc(Vector3 from, Vector3 to, float radius)
        {
            Vector3 direction = (to - from).normalized;
            Vector3 perpendicular = Vector3.Cross(direction, Vector3.up);
            if (perpendicular.magnitude < 0.001f)
            {
                perpendicular = Vector3.Cross(direction, Vector3.forward);
            }
            perpendicular.Normalize();

            int segments = 10;
            for (int i = 0; i < segments; i++)
            {
                float t0 = (float)i / segments;
                float t1 = (float)(i + 1) / segments;

                Vector3 p0 = Vector3.Lerp(from, to, t0);
                Vector3 p1 = Vector3.Lerp(from, to, t1);

                // 小さな垂直オフセットを追加して円弧のように見せる
                float height0 = Mathf.Sin(t0 * Mathf.PI) * 0.02f;
                float height1 = Mathf.Sin(t1 * Mathf.PI) * 0.02f;

                p0 += perpendicular * height0;
                p1 += perpendicular * height1;

                Gizmos.DrawLine(p0, p1);
            }
        }

        /// <summary>
        /// 指定した中心と半径で円を描画
        /// </summary>
        private void DrawCircle(Vector3 center, float radius, Vector3 normal)
        {
            int segments = 32;
            Vector3 perpendicular = Vector3.Cross(normal, Vector3.forward);
            if (perpendicular.magnitude < 0.001f)
            {
                perpendicular = Vector3.Cross(normal, Vector3.right);
            }
            perpendicular.Normalize();

            Vector3 previousPoint = center + perpendicular * radius;

            for (int i = 1; i <= segments; i++)
            {
                float angle = (float)i / segments * 2f * Mathf.PI;
                Quaternion rotation = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, normal);
                Vector3 newPoint = center + rotation * perpendicular * radius;

                Gizmos.DrawLine(previousPoint, newPoint);
                previousPoint = newPoint;
            }
        }
    }
}