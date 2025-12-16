using UnityEngine;
using System.Reflection;

/// <summary>
/// 手と腕全体に位置オフセットを適用するスクリプト
/// 手首だけでなく、前腕・上腕にもオフセットを適用することで、腕が伸びる問題を回避します
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
    private Transform _leftHand;
    private Transform _rightHand;
    private Transform _leftLowerArm;
    private Transform _rightLowerArm;
    private Transform _leftUpperArm;
    private Transform _rightUpperArm;
    private bool _isInitialized = false;
    private Vector3 _lastPositionOffset = Vector3.zero;

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

        // オフセットが変更された場合、適用
        if (_lastPositionOffset != positionOffset)
        {
            _lastPositionOffset = positionOffset;
        }

        // 腕全体（上腕・前腕・手）にオフセットを適用
        ApplyArmOffset();
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
        _targetAnimator = skeletonRetargeter.GetComponent<Animator>();
        if (_targetAnimator == null)
        {
            Debug.LogError("AnimatorコンポーネントがOVRUnityHumanoidSkeletonRetargeterと同じGameObjectに見つかりません");
            return;
        }

        // 左右の手・前腕・上腕のボーンを取得
        _leftHand = _targetAnimator.GetBoneTransform(HumanBodyBones.LeftHand);
        _rightHand = _targetAnimator.GetBoneTransform(HumanBodyBones.RightHand);
        _leftLowerArm = _targetAnimator.GetBoneTransform(HumanBodyBones.LeftLowerArm);
        _rightLowerArm = _targetAnimator.GetBoneTransform(HumanBodyBones.RightLowerArm);
        _leftUpperArm = _targetAnimator.GetBoneTransform(HumanBodyBones.LeftUpperArm);
        _rightUpperArm = _targetAnimator.GetBoneTransform(HumanBodyBones.RightUpperArm);

        if (_leftHand == null && _rightHand == null)
        {
            Debug.LogError("左手と右手のボーンが見つかりません");
            return;
        }

        _isInitialized = true;
        _lastPositionOffset = positionOffset;
        Debug.Log($"HandPositionOffset initialized. Target: {_targetAnimator.gameObject.name}");
    }

    /// <summary>
    /// 腕全体にオフセットを適用（リターゲティング後）
    /// 上腕→前腕→手の順で適用することで、腕が伸びずに全体が移動します
    /// </summary>
    private void ApplyArmOffset()
    {
        // 左腕にオフセット適用
        if (_leftUpperArm != null)
        {
            _leftUpperArm.position += positionOffset;
        }
        if (_leftLowerArm != null)
        {
            _leftLowerArm.position += positionOffset;
        }
        if (_leftHand != null)
        {
            _leftHand.position += positionOffset;
        }

        // 右腕にオフセット適用
        if (_rightUpperArm != null)
        {
            _rightUpperArm.position += positionOffset;
        }
        if (_rightLowerArm != null)
        {
            _rightLowerArm.position += positionOffset;
        }
        if (_rightHand != null)
        {
            _rightHand.position += positionOffset;
        }
    }

    /// <summary>
    /// オフセットをリアルタイムで変更
    /// </summary>
    public void SetOffset(Vector3 offset)
    {
        positionOffset = offset;
        _lastPositionOffset = offset;
    }

    /// <summary>
    /// オフセットをリセット
    /// </summary>
    public void ResetOffset()
    {
        positionOffset = Vector3.zero;
        _lastPositionOffset = Vector3.zero;
    }
}
