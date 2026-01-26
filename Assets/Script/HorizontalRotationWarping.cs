using UnityEngine;

namespace VRBodyTracking
{
    /// <summary>
    /// 水平方向の回転ワーピング実装
    /// 論文アルゴリズム: 実際の手の位置を水平面に投影し、角度オフセットを適用
    /// </summary>
    public class HorizontalRotationWarping : HandRedirectionBase
    {
        [Header("水平回転ワーピング設定")]
        [Tooltip("回転リダイレクション角度（度）- 正の値で時計回り")]
        [Range(-30f, 30f)]
        public float rotationAngleDegrees = 0f;

        // Unity座標系での水平ワープ用ベクトル
        // forward = Z軸 (0,0,1)
        // right = X軸 (1,0,0)
        private readonly Vector3 forwardVector = Vector3.forward;
        private readonly Vector3 rightVector = Vector3.right;

        protected override Vector3 CalculateRedirectedPosition(Vector3 realHandPosition)
        {
            // 1. 高さベクトルを算出: h_hat = f_hat × r_hat
            Vector3 heightVector = Vector3.Cross(forwardVector, rightVector);
            heightVector.Normalize();

            // 2. 原点から手までの相対位置ベクトル
            Vector3 offsetFromOrigin = realHandPosition - warpOrigin.position;

            // 3. 高さを保存: height = (p_r - o) · h_hat
            float height = Vector3.Dot(offsetFromOrigin, heightVector);

            // 4. 水平面への投影: p_proj = p_r - height * h_hat
            Vector3 projectedPosition = realHandPosition - height * heightVector;

            // 5. 投影点から原点までの水平ベクトル
            Vector3 horizontalOffset = projectedPosition - warpOrigin.position;

            // 6. 現在の角度を算出（Y軸周りの回転）
            float currentAngle = Mathf.Atan2(horizontalOffset.x, horizontalOffset.z) * Mathf.Rad2Deg;

            // 7. リダイレクション角度を加算
            float redirectedAngle = currentAngle + rotationAngleDegrees;

            // 8. 距離を保持
            float distance = horizontalOffset.magnitude;

            // 9. 新しい水平位置を計算
            float redirectedAngleRad = redirectedAngle * Mathf.Deg2Rad;
            Vector3 redirectedHorizontal = new Vector3(
                Mathf.Sin(redirectedAngleRad) * distance,
                0f,
                Mathf.Cos(redirectedAngleRad) * distance
            );

            // 10. 最終位置: 水平面上の新位置 + 元の高さを復元
            Vector3 redirectedPosition = warpOrigin.position + redirectedHorizontal + height * heightVector;

            return redirectedPosition;
        }

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();

            if (!showDebugGizmos || !ValidateReferences())
            {
                return;
            }

            // 水平面を視覚化（XZ平面）
            Gizmos.color = new Color(0f, 1f, 0f, 0.2f);
            Vector3 originPos = warpOrigin.position;

            // 水平面の格子を描画
            float gridSize = 0.5f;
            int gridCount = 5;

            for (int i = -gridCount; i <= gridCount; i++)
            {
                Vector3 start = originPos + new Vector3(i * gridSize / gridCount, 0, -gridSize);
                Vector3 end = originPos + new Vector3(i * gridSize / gridCount, 0, gridSize);
                Gizmos.DrawLine(start, end);

                start = originPos + new Vector3(-gridSize, 0, i * gridSize / gridCount);
                end = originPos + new Vector3(gridSize, 0, i * gridSize / gridCount);
                Gizmos.DrawLine(start, end);
            }

            // 角度オフセットを示す円弧を描画
            if (Mathf.Abs(rotationAngleDegrees) > 0.1f)
            {
                Gizmos.color = Color.yellow;
                Vector3 horizontalOffset = realHand.position - warpOrigin.position;
                horizontalOffset.y = 0;
                float radius = horizontalOffset.magnitude;

                // 円弧を描画
                int segments = 20;
                Vector3 prevPoint = warpOrigin.position + horizontalOffset;

                for (int i = 1; i <= segments; i++)
                {
                    float angle = (rotationAngleDegrees * i / segments) * Mathf.Deg2Rad;
                    float cosAngle = Mathf.Cos(angle);
                    float sinAngle = Mathf.Sin(angle);

                    // 回転行列を適用
                    float newX = horizontalOffset.x * cosAngle - horizontalOffset.z * sinAngle;
                    float newZ = horizontalOffset.x * sinAngle + horizontalOffset.z * cosAngle;

                    Vector3 newPoint = warpOrigin.position + new Vector3(newX, 0, newZ);
                    Gizmos.DrawLine(prevPoint, newPoint);
                    prevPoint = newPoint;
                }
            }
        }
    }
}