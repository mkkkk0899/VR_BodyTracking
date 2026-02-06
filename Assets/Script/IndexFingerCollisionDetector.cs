using UnityEngine;

/// <summary>
/// 人差し指のコリジョンとの接触判定を行うスクリプト
/// このスクリプトを接触判定したいオブジェクトにアタッチして使用
/// 注意: このオブジェクトと人差し指の両方に予めコリジョンをアタッチしておく必要があります
/// </summary>
public class IndexFingerCollisionDetector : MonoBehaviour
{
    [Header("Index Finger Collision (人差し指コリジョン)")]
    [Tooltip("人差し指に既にアタッチされているコリジョンオブジェクト（既存のものを参照）")]
    public GameObject indexFingerCollider;

    [Header("Collision Status (接触状態)")]
    [Tooltip("現在人差し指と接触しているか")]
    public bool isColliding = false;

    /// <summary>
    /// コリジョン接触時に呼ばれる
    /// </summary>
    private void OnCollisionEnter(Collision collision)
    {
        // 人差し指のコリジョンと接触したか確認
        if (indexFingerCollider != null && collision.gameObject == indexFingerCollider)
        {
            isColliding = true;
            Debug.Log("IndexFingerCollisionDetector: 人差し指と接触しました。");
        }
    }

    /// <summary>
    /// コリジョン接触終了時に呼ばれる
    /// </summary>
    private void OnCollisionExit(Collision collision)
    {
        // 人差し指のコリジョンとの接触が終了したか確認
        if (indexFingerCollider != null && collision.gameObject == indexFingerCollider)
        {
            //isColliding = false;
            Debug.Log("IndexFingerCollisionDetector: 人差し指との接触が終了しました。");
        }
    }

    /// <summary>
    /// トリガー接触時に呼ばれる（Colliderがトリガーの場合）
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        // 人差し指のコリジョンと接触したか確認
        if (indexFingerCollider != null && other.gameObject == indexFingerCollider)
        {
            isColliding = true;
            Debug.Log("IndexFingerCollisionDetector: 人差し指と接触しました（トリガー）。");
        }
    }

    /// <summary>
    /// トリガー接触終了時に呼ばれる（Colliderがトリガーの場合）
    /// </summary>
    private void OnTriggerExit(Collider other)
    {
        // 人差し指のコリジョンとの接触が終了したか確認
        if (indexFingerCollider != null && other.gameObject == indexFingerCollider)
        {
            //isColliding = false;
            Debug.Log("IndexFingerCollisionDetector: 人差し指との接触が終了しました（トリガー）。");
        }
    }
}
