using System.Collections;
using UnityEngine;

public class Tower : MonoBehaviour {

    public static readonly int BLOCK_COUNT = 54;

    public GameObject block;

    [Header("Builder params")]
    public float dropHeight = 0F;
    public float timeBetweenBlock = 0F;

    public delegate void TowerBuiltAction();
    public static event TowerBuiltAction OnTowerBuilt;

    IEnumerator builder() {
        float width = block.transform.localScale.x;
        float height = block.transform.localScale.y;

        for (int i = 0; i < BLOCK_COUNT; ++i) {
            int pos = (i % 3) - 1;
            int level = i / 3;
            bool xAligned = (level % 2).Equals(0);

            Vector3 position = new Vector3(
                xAligned ? pos * width : 0,
                (level * height) + (this.dropHeight * height/2),
                !xAligned ? pos * width : 0
            );

            Quaternion rotation = Quaternion.Euler(
                0,
                xAligned ? 0 : 90,
                0
            );

            GameObject instance = Instantiate(block, position, rotation);
            instance.transform.parent = this.gameObject.transform;
            instance.name = string.Format("Block {0}", i);
            instance.GetComponent<Block>().level = level;
            instance.GetComponent<Block>().pos = pos;

            yield return new WaitForSeconds(this.timeBetweenBlock);
        }

        if (OnTowerBuilt != null) {
            OnTowerBuilt();
        }
    }

    void Start() {
        StartCoroutine(builder());
    }

    public int GetLevels() {
        return BLOCK_COUNT/3;
    }

    public float GetScaleHeight() {
        float minY = float.PositiveInfinity;
        float maxY = float.NegativeInfinity;

        for (int i = 0; i < this.transform.childCount; ++i) {
            Transform childTransform = this.transform.GetChild(i);
            float bottomY = childTransform.position.y - (childTransform.localScale.y / 2);
            float topY = childTransform.position.y + (childTransform.localScale.y / 2);

            if (topY > maxY) maxY = topY;
            if (bottomY < minY) minY = bottomY;
        }

        return maxY - minY;
    }
}
