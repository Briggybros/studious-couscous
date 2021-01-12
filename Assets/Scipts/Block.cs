using UnityEngine;

public class Block : MonoBehaviour {
    public static readonly float CAMERA_FOCUS_DISTANCE = 10F;

    public float tolerance = 0.01F;
    public int level;
    public int pos;
    
    void Start() {
        float x, y, z;

        x = this.gameObject.transform.localScale.x + Random.Range(-tolerance, tolerance);
        y = this.gameObject.transform.localScale.y + Random.Range(-tolerance, tolerance);
        z = this.gameObject.transform.localScale.z + Random.Range(-tolerance, tolerance);

        this.gameObject.transform.localScale = new Vector3(x, y, z);
    }
}
