using UnityEngine;

public class Fruit : MonoBehaviour
{
    [Range(0, 9)]
    public int tierIndex; // A=0 ... J=9

    private bool isMerging;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.IsGameOver) return;

        var other = collision.collider.GetComponent<Fruit>();
        if (other == null) return;

        // 必須相同等級才合成
        if (other.tierIndex != tierIndex) return;

        // 避免重複觸發
        if (isMerging || other.isMerging) return;

        // 最後一級 J 不能再合
        if (tierIndex >= 9) return;

        // 只讓其中一顆執行合成（避免兩邊都跑）
        if (GetInstanceID() > other.GetInstanceID()) return;

        isMerging = true;
        other.isMerging = true;

        // 合成位置：兩者中點（也可改用 contact point）
        Vector3 spawnPos = (transform.position + other.transform.position) * 0.5f;

        GameManager.Instance.Merge(tierIndex, spawnPos, this.gameObject, other.gameObject);
    }
}