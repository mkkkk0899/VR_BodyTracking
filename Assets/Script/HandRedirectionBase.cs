using UnityEngine;

namespace VRBodyTracking
{
       public abstract class HandRedirectionBase : MonoBehaviour
    {
        [Header("基本設定")]
        [Tooltip("実際の手の位置（トラッカーまたはコントローラー）")]
        public Transform realHand;

        [Tooltip("仮想の手（表示される手のモデル）")]
        public Transform virtualHand;

        [Tooltip("ワープ原点（リダイレクションを開始する基準点）")]
        public Transform warpOrigin;

        [Header("トリガー設定")]
        [Tooltip("ワープ原点に接触したときからリダイレクションを開始")]
        public bool useContactTrigger = true;

        [Tooltip("接触を検知する距離（メートル）")]
        [Range(0.01f, 0.5f)]
        public float contactDistance = 0.1f;

        [Header("デバッグ")]
        [Tooltip("デバッグ用のGizmosを表示")]
        public bool showDebugGizmos = true;

        // リダイレクションが有効かどうか
        protected bool isRedirectionActive = false;

        // 接触開始時の位置を保存
        protected Vector3 contactStartPosition;

        protected virtual void LateUpdate()
        {
            if (!ValidateReferences())
            {
                return;
            }

            // 接触トリガーを使用する場合
            if (useContactTrigger)
            {
                float distanceToOrigin = Vector3.Distance(realHand.position, warpOrigin.position);

                // 接触判定
                if (!isRedirectionActive && distanceToOrigin <= contactDistance)
                {
                    // リダイレクション開始
                    OnRedirectionStart();
                }
                else if (isRedirectionActive && distanceToOrigin > contactDistance * 1.5f)
                {
                    // リダイレクション終了（ヒステリシスを持たせる）
                    OnRedirectionEnd();
                }

                // リダイレクションが有効な場合のみ処理
                if (isRedirectionActive)
                {
                    Vector3 redirectedPosition = CalculateRedirectedPosition(realHand.position);
                    virtualHand.position = redirectedPosition;
                }
                else
                {
                    // リダイレクション無効時は実際の手の位置と同じにする
                    virtualHand.position = realHand.position;
                }
            }
            else
            {
                // 常にリダイレクションを適用
                Vector3 redirectedPosition = CalculateRedirectedPosition(realHand.position);
                virtualHand.position = redirectedPosition;
            }
        }

        /// <summary>
        /// リダイレクション開始時の処理
        /// </summary>
        protected virtual void OnRedirectionStart()
        {
            isRedirectionActive = true;
            contactStartPosition = realHand.position;
            Debug.Log($"[{GetType().Name}] リダイレクション開始");
        }

        /// <summary>
        /// リダイレクション終了時の処理
        /// </summary>
        protected virtual void OnRedirectionEnd()
        {
            isRedirectionActive = false;
            Debug.Log($"[{GetType().Name}] リダイレクション終了");
        }

        /// <summary>
        /// リダイレクション後の仮想手の位置を計算
        /// </summary>
        protected abstract Vector3 CalculateRedirectedPosition(Vector3 realHandPosition);

        /// <summary>
        /// 必要な参照が設定されているか検証
        /// </summary>
        protected virtual bool ValidateReferences()
        {
            if (realHand == null)
            {
                Debug.LogWarning($"[{GetType().Name}] realHandが設定されていません");
                return false;
            }

            if (virtualHand == null)
            {
                Debug.LogWarning($"[{GetType().Name}] virtualHandが設定されていません");
                return false;
            }

            if (warpOrigin == null)
            {
                Debug.LogWarning($"[{GetType().Name}] warpOriginが設定されていません");
                return false;
            }

            return true;
        }

        protected virtual void OnDrawGizmos()
        {
            if (!showDebugGizmos || !ValidateReferences())
            {
                return;
            }

            // 接触トリガーの範囲を表示
            if (useContactTrigger)
            {
                // 接触距離の球を表示（黄色）
                Gizmos.color = isRedirectionActive ? new Color(1f, 0.5f, 0f, 0.3f) : new Color(1f, 1f, 0f, 0.3f);
                Gizmos.DrawWireSphere(warpOrigin.position, contactDistance);

                // 終了距離の球を表示（薄い黄色）
                Gizmos.color = new Color(1f, 1f, 0f, 0.15f);
                Gizmos.DrawWireSphere(warpOrigin.position, contactDistance * 1.5f);
            }

            // ワープ原点を緑色の球で表示
            Gizmos.color = isRedirectionActive ? Color.green : new Color(0f, 1f, 0f, 0.5f);
            Gizmos.DrawWireSphere(warpOrigin.position, 0.02f);

            // 実際の手を青色で表示
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(realHand.position, 0.015f);

            // 仮想の手を赤色で表示
            Gizmos.color = isRedirectionActive ? Color.red : new Color(1f, 0f, 0f, 0.5f);
            Gizmos.DrawWireSphere(virtualHand.position, 0.015f);

            // 原点から実際の手への線（青）
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(warpOrigin.position, realHand.position);

            // リダイレクションが有効な場合のみ赤い線を表示
            if (isRedirectionActive)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(warpOrigin.position, virtualHand.position);
            }
        }
    }
}