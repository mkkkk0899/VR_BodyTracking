using UnityEngine;

/// <summary>
/// VRフィンガートラッキングをアバターに適用するスクリプト
/// Oculus/Meta SDKからの指の回転データをアバターのボーンに反映
/// </summary>
public class FingerTracker : MonoBehaviour
{
    [Header("Hand Settings")]
    [Tooltip("左手用のスクリプトかどうか")]
    public bool isLeftHand = true;

    [Header("Coordinate System Conversion")]
    [Tooltip("座標系変換を有効化（OVRとアバターの座標系が異なる場合）")]
    public bool enableCoordinateConversion = true;

    [Tooltip("座標系変換用の回転オフセット")]
    public Vector3 coordinateSystemRotation = new Vector3(0, 180, 0);

    [Header("Thumb (親指)")]
    public Transform thumbProximal;
    public Transform thumbIntermediate;
    public Transform thumbDistal;

    [Header("Index Finger (人差し指)")]
    public Transform indexProximal;
    public Transform indexIntermediate;
    public Transform indexDistal;

    [Header("Middle Finger (中指)")]
    public Transform middleProximal;
    public Transform middleIntermediate;
    public Transform middleDistal;

    [Header("Ring Finger (薬指)")]
    public Transform ringProximal;
    public Transform ringIntermediate;
    public Transform ringDistal;

    [Header("Pinky Finger (小指)")]
    public Transform pinkyProximal;
    public Transform pinkyIntermediate;
    public Transform pinkyDistal;

    [Header("Rotation Offsets - Thumb (親指)")]
    public Vector3 thumbProximalOffset = Vector3.zero;
    public Vector3 thumbIntermediateOffset = Vector3.zero;
    public Vector3 thumbDistalOffset = Vector3.zero;

    [Header("Rotation Offsets - Index (人差し指)")]
    public Vector3 indexProximalOffset = Vector3.zero;
    public Vector3 indexIntermediateOffset = Vector3.zero;
    public Vector3 indexDistalOffset = Vector3.zero;

    [Header("Rotation Offsets - Middle (中指)")]
    public Vector3 middleProximalOffset = Vector3.zero;
    public Vector3 middleIntermediateOffset = Vector3.zero;
    public Vector3 middleDistalOffset = Vector3.zero;

    [Header("Rotation Offsets - Ring (薬指)")]
    public Vector3 ringProximalOffset = Vector3.zero;
    public Vector3 ringIntermediateOffset = Vector3.zero;
    public Vector3 ringDistalOffset = Vector3.zero;

    [Header("Rotation Offsets - Pinky (小指)")]
    public Vector3 pinkyProximalOffset = Vector3.zero;
    public Vector3 pinkyIntermediateOffset = Vector3.zero;
    public Vector3 pinkyDistalOffset = Vector3.zero;

    private Transform[] fingerBones;
    private Vector3[] rotationOffsets;

    [Header("OVR References")]
    [Tooltip("OVRSkeletonコンポーネント（自動検索可能）")]
    public OVRSkeleton ovrSkeleton;

    private bool _isInitialized = false;

    void Start()
    {
        Initialize();
    }

    void Update()
    {
        if (!_isInitialized)
        {
            return;
        }

        ApplyFingerRotations();
    }

    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Initialize()
    {
        // 個別変数から配列を構築
        fingerBones = new Transform[]
        {
            thumbProximal, thumbIntermediate, thumbDistal,
            indexProximal, indexIntermediate, indexDistal,
            middleProximal, middleIntermediate, middleDistal,
            ringProximal, ringIntermediate, ringDistal,
            pinkyProximal, pinkyIntermediate, pinkyDistal
        };

        // オフセット配列を個別変数から構築
        rotationOffsets = new Vector3[]
        {
            thumbProximalOffset, thumbIntermediateOffset, thumbDistalOffset,
            indexProximalOffset, indexIntermediateOffset, indexDistalOffset,
            middleProximalOffset, middleIntermediateOffset, middleDistalOffset,
            ringProximalOffset, ringIntermediateOffset, ringDistalOffset,
            pinkyProximalOffset, pinkyIntermediateOffset, pinkyDistalOffset
        };

        // OVRSkeletonの自動検索
        if (ovrSkeleton == null)
        {
            OVRSkeleton[] skeletons = FindObjectsOfType<OVRSkeleton>();
            foreach (var skeleton in skeletons)
            {
                if ((isLeftHand && skeleton.GetSkeletonType() == OVRSkeleton.SkeletonType.HandLeft) ||
                    (!isLeftHand && skeleton.GetSkeletonType() == OVRSkeleton.SkeletonType.HandRight))
                {
                    ovrSkeleton = skeleton;
                    break;
                }
            }

            if (ovrSkeleton == null)
            {
                Debug.LogError($"OVRSkeleton ({(isLeftHand ? "Left" : "Right")} Hand) が見つかりません");
                return;
            }
        }

        // ボーン配列の検証
        if (fingerBones == null || fingerBones.Length == 0)
        {
            Debug.LogError("fingerBonesが設定されていません");
            return;
        }

        _isInitialized = true;
        Debug.Log($"FingerTracker initialized for {(isLeftHand ? "Left" : "Right")} hand");
    }

    /// <summary>
    /// SDKから指の回転データを取得してアバターに適用
    /// </summary>
    private void ApplyFingerRotations()
    {
        if (ovrSkeleton == null || ovrSkeleton.Bones == null)
        {
            return;
        }

        // OVRSkeleton.BoneIdを使用して正しいボーンを取得
        // 親指
        ApplyBoneRotation(OVRSkeleton.BoneId.Hand_Thumb1, 0);  // Proximal
        ApplyBoneRotation(OVRSkeleton.BoneId.Hand_Thumb2, 1);  // Intermediate
        ApplyBoneRotation(OVRSkeleton.BoneId.Hand_Thumb3, 2);  // Distal

        // 人差し指
        ApplyBoneRotation(OVRSkeleton.BoneId.Hand_Index1, 3);  // Proximal
        ApplyBoneRotation(OVRSkeleton.BoneId.Hand_Index2, 4);  // Intermediate
        ApplyBoneRotation(OVRSkeleton.BoneId.Hand_Index3, 5);  // Distal

        // 中指
        ApplyBoneRotation(OVRSkeleton.BoneId.Hand_Middle1, 6);  // Proximal
        ApplyBoneRotation(OVRSkeleton.BoneId.Hand_Middle2, 7);  // Intermediate
        ApplyBoneRotation(OVRSkeleton.BoneId.Hand_Middle3, 8);  // Distal

        // 薬指
        ApplyBoneRotation(OVRSkeleton.BoneId.Hand_Ring1, 9);   // Proximal
        ApplyBoneRotation(OVRSkeleton.BoneId.Hand_Ring2, 10);  // Intermediate
        ApplyBoneRotation(OVRSkeleton.BoneId.Hand_Ring3, 11);  // Distal

        // 小指
        ApplyBoneRotation(OVRSkeleton.BoneId.Hand_Pinky1, 12); // Proximal
        ApplyBoneRotation(OVRSkeleton.BoneId.Hand_Pinky2, 13); // Intermediate
        ApplyBoneRotation(OVRSkeleton.BoneId.Hand_Pinky3, 14); // Distal
    }

    /// <summary>
    /// 特定のボーンIDから回転を取得してアバターに適用
    /// </summary>
    private void ApplyBoneRotation(OVRSkeleton.BoneId boneId, int fingerBoneIndex)
    {
        if (fingerBoneIndex >= fingerBones.Length || fingerBones[fingerBoneIndex] == null)
        {
            return;
        }

        // OVRSkeletonから指定されたボーンを検索
        var bones = ovrSkeleton.Bones;
        foreach (var bone in bones)
        {
            if (bone.Id == boneId && bone.Transform != null)
            {
                // SDKからの生の回転データ
                Quaternion rawRotation = bone.Transform.localRotation;

                // 座標系変換を適用（有効な場合）
                Quaternion convertedRotation = rawRotation;
                if (enableCoordinateConversion)
                {
                    Quaternion conversionOffset = Quaternion.Euler(coordinateSystemRotation);
                    convertedRotation = conversionOffset * rawRotation * Quaternion.Inverse(conversionOffset);
                }

                // ボーン固有のオフセットを適用
                Quaternion offsetRotation = Quaternion.Euler(rotationOffsets[fingerBoneIndex]);
                Quaternion finalRotation = convertedRotation * offsetRotation;

                // アバターのボーンに適用
                fingerBones[fingerBoneIndex].localRotation = finalRotation;
                break;
            }
        }
    }

    /// <summary>
    /// 特定のボーンのオフセットを設定
    /// </summary>
    public void SetBoneOffset(int boneIndex, Vector3 offset)
    {
        if (boneIndex >= 0 && boneIndex < rotationOffsets.Length)
        {
            rotationOffsets[boneIndex] = offset;
        }
    }

    /// <summary>
    /// すべてのオフセットをリセット
    /// </summary>
    public void ResetAllOffsets()
    {
        for (int i = 0; i < rotationOffsets.Length; i++)
        {
            rotationOffsets[i] = Vector3.zero;
        }
    }
}
