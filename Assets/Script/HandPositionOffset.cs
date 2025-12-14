using UnityEngine;

/// <summary>
/// OVRUnityHumanoidSkeletonRetargeterに位置オフセットを適用するスクリプト
/// </summary>
public class HandPositionOffset : MonoBehaviour
{
    [Header("Hand Offset Settings")]
    [Tooltip("位置オフセット (ワールド座標)")]
    public Vector3 positionOffset = Vector3.zero;

    [Header("References")]
    [Tooltip("OVRUnityHumanoidSkeletonRetargeterコンポーネント (自動検索可能)")]
    public OVRUnityHumanoidSkeletonRetargeter skeletonRetargeter;

    private Animator _targetAnimator;
    private Transform _hipsTransform;
    private Vector3 _originalPosition;
    private bool _isInitialized = false;

    void Start()
    {
        InitializeRetargeter();
    }

    void LateUpdate()
    {
        if (!_isInitialized)
        {
            return;
        }

        // Hipsにオフセットを適用（リターゲティング後）
        if (_hipsTransform != null)
        {
            _hipsTransform.position = _hipsTransform.position + positionOffset;
        }
    }

    /// <summary>
    /// OVRUnityHumanoidSkeletonRetargeterを検索して初期化
    /// </summary>
    private void InitializeRetargeter()
    {
        // skeletonRetargeterが未設定の場合、自動検索
        if (skeletonRetargeter == null)
        {
            skeletonRetargeter = FindObjectOfType<OVRUnityHumanoidSkeletonRetargeter>();
            if (skeletonRetargeter == null)
            {
                Debug.LogError("OVRUnityHumanoidSkeletonRetargeterが見つかりません");
                return;
            }
        }

        // ターゲットのAnimatorを取得
        // OVRUnityHumanoidSkeletonRetargeterは同じGameObjectにAnimatorを必要とする
        _targetAnimator = skeletonRetargeter.GetComponent<Animator>();
        if (_targetAnimator == null)
        {
            Debug.LogError("AnimatorコンポーネントがOVRUnityHumanoidSkeletonRetargeterと同じGameObjectに見つかりません");
            return;
        }

        // HumanoidのHipsボーンを取得
        _hipsTransform = _targetAnimator.GetBoneTransform(HumanBodyBones.Hips);
        if (_hipsTransform == null)
        {
            Debug.LogError("Hipsボーンが見つかりません");
            return;
        }

        _isInitialized = true;
        Debug.Log($"HandPositionOffset initialized. Target: {_targetAnimator.gameObject.name}, Hips: {_hipsTransform.name}");
    }

    /// <summary>
    /// オフセットをリアルタイムで変更
    /// </summary>
    public void SetOffset(Vector3 offset)
    {
        positionOffset = offset;
    }

    /// <summary>
    /// オフセットをリセット
    /// </summary>
    public void ResetOffset()
    {
        positionOffset = Vector3.zero;
    }
}
