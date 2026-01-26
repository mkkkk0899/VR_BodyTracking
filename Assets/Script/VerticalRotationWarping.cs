using UnityEngine;

namespace VRBodyTracking
{
    /// <summary>
    /// 垂直方向の回転ワーピング実装
    /// 論文アルゴリズム: 実際の手の位置を垂直面に投影し、角度オフセットを適用
    /// </summary>
    public class VerticalRotationWarping : HandRedirectionBase
    {
        [Header("垂直回転ワーピング設定")]
        [Tooltip("回転リダイレクション角度（度）- 正の値で上方向")]
        [Range(-30f, 30f)]
        public float rotationAngleDegrees = 0f;

        // Unity座標系での垂直ワープ用ベクトル
        // forward = Z軸 (0,0,1)
        // up = Y軸 (0,1,0)
        private readonly Vector3 forwardVector = Vector3.forward;
        private readonly Vector3 upVector = Vector3.up;

        protected override Vector3 CalculateRedirectedPosition(Vector3 realHandPosition)
        {
            // 1. 水平方向ベクトルを算出: r_hat = f_hat × up_hat
            // ※垂直ワーピングでは水平方向（X軸方向）を保存する
            Vector3 horizontalVector = Vector3.Cross(forwardVector, upVector);
            horizontalVector.Normalize();

            // 2. 原点から手までの相対位置ベクトル
            Vector3 offsetFromOrigin = realHandPosition - warpOrigin.position;

            // 3. 水平方向の成分を保存: horizontal = (p_r - o) · r_hat
            float horizontalComponent = Vector3.Dot(offsetFromOrigin, horizontalVector);

            // 4. 垂直面（YZ平面）への投影: p_proj = p_r - horizontal * r_hat
            Vector3 projectedPosition = realHandPosition - horizontalComponent * horizontalVector;

            // 5. 投影点から原点までの垂直面上のベクトル
            Vector3 verticalOffset = projectedPosition - warpOrigin.position;

            // 6. 現在の角度を算出（X軸周りの回転、YZ平面上）
            float currentAngle = Mathf.Atan2(verticalOffset.y, verticalOffset.z) * Mathf.Rad2Deg;

            // 7. リダイレクション角度を加算
            float redirectedAngle = currentAngle + rotationAngleDegrees;

            // 8. 距離を保持
            float distance = verticalOffset.magnitude;

            // 9. 新しい垂直位置を計算（YZ平面上）
            float redirectedAngleRad = redirectedAngle * Mathf.Deg2Rad;
            Vector3 redirectedVertical = new Vector3(
                0f,
                Mathf.Sin(redirectedAngleRad) * distance,
                Mathf.Cos(redirectedAngleRad) * distance
            );

            // 10. 最終位置: 垂直面上の新位置 + 元の水平成分を復元
            Vector3 redirectedPosition = warpOrigin.position + redirectedVertical + horizontalComponent * horizontalVector;

            return redirectedPosition;
        }

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();

            if (!showDebugGizmos || !ValidateReferences())
            {
                return;
            }

            // 垂直面を視覚化（YZ平面）
            Gizmos.color = new Color(0f, 0f, 1f, 0.2f);
            Vector3 originPos = warpOrigin.position;

            // 垂直面の格子を描画
            float gridSize = 0.5f;
            int gridCount = 5;

            for (int i = -gridCount; i <= gridCount; i++)
            {
                Vector3 start = originPos + new Vector3(0, i * gridSize / gridCount, -gridSize);
                Vector3 end = originPos + new Vector3(0, i * gridSize / gridCount, gridSize);
                Gizmos.DrawLine(start, end);

                start = originPos + new Vector3(0, -gridSize, i * gridSize / gridCount);
                end = originPos + new Vector3(0, gridSize, i * gridSize / gridCount);
                Gizmos.DrawLine(start, end);
            }

            // 角度オフセットを示す円弧を描画
            if (Mathf.Abs(rotationAngleDegrees) > 0.1f)
            {
                Gizmos.color = Color.cyan;
                Vector3 verticalOffset = realHand.position - warpOrigin.position;
                verticalOffset.x = 0; // X成分を除去（垂直面上のみ）
                float radius = verticalOffset.magnitude;

                // 円弧を描画（YZ平面上）
                int segments = 20;
                Vector3 prevPoint = warpOrigin.position + verticalOffset;

                for (int i = 1; i <= segments; i++)
                {
                    float angle = (rotationAngleDegrees * i / segments) * Mathf.Deg2Rad;
                    float cosAngle = Mathf.Cos(angle);
                    float sinAngle = Mathf.Sin(angle);

                    // 回転行列を適用（X軸周りの回転）
                    float newY = verticalOffset.y * cosAngle - verticalOffset.z * sinAngle;
                    float newZ = verticalOffset.y * sinAngle + verticalOffset.z * cosAngle;

                    Vector3 newPoint = warpOrigin.position + new Vector3(0, newY, newZ);
                    Gizmos.DrawLine(prevPoint, newPoint);
                    prevPoint = newPoint;
                }
            }
        }
    }
}