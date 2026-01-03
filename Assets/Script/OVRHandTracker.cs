using UnityEngine;

/// <summary>
/// OVRハンドトラッキングをアバタに適用するスクリプト
/// シーン上のOVRハンドモデルの各ボーンの回転をアバタの手に直接反映
/// </summary>
public class OVRHandTracker : MonoBehaviour
{
    [Header("OVR Thumb (親指)")]
    public Transform ovrThumb0;
    public Transform ovrThumb1;
    public Transform ovrThumb2;

    [Header("OVR Index (人差し指)")]
    public Transform ovrIndex1;
    public Transform ovrIndex2;
    public Transform ovrIndex3;

    [Header("OVR Middle (中指)")]
    public Transform ovrMiddle1;
    public Transform ovrMiddle2;
    public Transform ovrMiddle3;

    [Header("OVR Ring (薬指)")]
    public Transform ovrRing1;
    public Transform ovrRing2;
    public Transform ovrRing3;

    [Header("OVR Pinky (小指)")]
    public Transform ovrPinky0;
    public Transform ovrPinky1;
    public Transform ovrPinky2;
    [Header("Avatar Thumb (親指)")]
    public Transform avatarThumb0;
    public Transform avatarThumb1;
    public Transform avatarThumb2;

    [Header("Avatar Index (人差し指)")]
    public Transform avatarIndex1;
    public Transform avatarIndex2;
    public Transform avatarIndex3;

    [Header("Avatar Middle (中指)")]
    public Transform avatarMiddle1;
    public Transform avatarMiddle2;
    public Transform avatarMiddle3;

    [Header("Avatar Ring (薬指)")]
    public Transform avatarRing1;
    public Transform avatarRing2;
    public Transform avatarRing3;

    [Header("Avatar Pinky (小指)")]
    public Transform avatarPinky0;
    public Transform avatarPinky1;
    public Transform avatarPinky2;

    [Header("Rotation Offsets (回転オフセット)")]
    [Tooltip("親指の付け根の回転オフセット")]
    public Vector3 thumbOffset = Vector3.zero;

    [Tooltip("人差し指の付け根の回転オフセット")]
    public Vector3 indexOffset = Vector3.zero;

    [Tooltip("中指の付け根の回転オフセット")]
    public Vector3 middleOffset = Vector3.zero;

    [Tooltip("薬指の付け根の回転オフセット")]
    public Vector3 ringOffset = Vector3.zero;

    [Tooltip("小指の付け根の回転オフセット")]
    public Vector3 pinkyOffset = Vector3.zero;

    [Header("Invert Rotation (回転反転)")]
    [Tooltip("第2関節と第3関節の回転を反転する")]
    public bool invertSecondAndThirdJoints = false;

    void LateUpdate()
    {
        ApplyHandTracking();
    }

    /// <summary>
    /// OVRハンドの回転をアバタに適用
    /// </summary>
    private void ApplyHandTracking()
    {
        // 親指（付け根にオフセット適用）
        ApplyRotationWithOffset(ovrThumb0, avatarThumb0, thumbOffset);
        ApplyRotation(ovrThumb1, avatarThumb1, invertSecondAndThirdJoints);
        ApplyRotation(ovrThumb2, avatarThumb2, invertSecondAndThirdJoints);

        // 人差し指（付け根にオフセット適用）
        ApplyRotationWithOffset(ovrIndex1, avatarIndex1, indexOffset);
        ApplyRotation(ovrIndex2, avatarIndex2, invertSecondAndThirdJoints);
        ApplyRotation(ovrIndex3, avatarIndex3, invertSecondAndThirdJoints);

        // 中指（付け根にオフセット適用）
        ApplyRotationWithOffset(ovrMiddle1, avatarMiddle1, middleOffset);
        ApplyRotation(ovrMiddle2, avatarMiddle2, invertSecondAndThirdJoints);
        ApplyRotation(ovrMiddle3, avatarMiddle3, invertSecondAndThirdJoints);

        // 薬指（付け根にオフセット適用）
        ApplyRotationWithOffset(ovrRing1, avatarRing1, ringOffset);
        ApplyRotation(ovrRing2, avatarRing2, invertSecondAndThirdJoints);
        ApplyRotation(ovrRing3, avatarRing3, invertSecondAndThirdJoints);

        // 小指（付け根にオフセット適用）
        ApplyRotationWithOffset(ovrPinky0, avatarPinky0, pinkyOffset);
        ApplyRotation(ovrPinky1, avatarPinky1, invertSecondAndThirdJoints);
        ApplyRotation(ovrPinky2, avatarPinky2, invertSecondAndThirdJoints);
    }

    /// <summary>
    /// OVRボーンの回転をアバターボーンに適用
    /// </summary>
    private void ApplyRotation(Transform ovrBone, Transform avatarBone, bool invert = false)
    {
        if (ovrBone != null && avatarBone != null)
        {
            if (invert)
            {
                // 回転を反転（逆回転を適用）
                avatarBone.localRotation = Quaternion.Inverse(ovrBone.localRotation);
            }
            else
            {
                avatarBone.localRotation = ovrBone.localRotation;
            }
        }
    }

    /// <summary>
    /// OVRボーンの回転にオフセットを加えてアバターボーンに適用
    /// </summary>
    private void ApplyRotationWithOffset(Transform ovrBone, Transform avatarBone, Vector3 offset)
    {
        if (ovrBone != null && avatarBone != null)
        {
            Quaternion ovrRotation = ovrBone.localRotation;
            Quaternion offsetRotation = Quaternion.Euler(offset);
            avatarBone.localRotation = ovrRotation * offsetRotation;
        }
    }
}