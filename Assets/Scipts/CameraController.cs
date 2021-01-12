using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour {
    public float distance = 50F;
    public float minYAngle = 5F;
    public float minXAngle = 80F;
    private float cameraRotX = 0;
    private float cameraRotY = 0;
    private Vector2 panVec = Vector2.zero;

    public GameObject focus;

    void Update() {
        Vector3 dir = new Vector3(0, 0, -distance);
        cameraRotX += panVec.x * Time.deltaTime * 100f;
        cameraRotY += (-panVec.y) * Time.deltaTime * 100f;
        cameraRotY = Mathf.Clamp(cameraRotY, minYAngle, minXAngle);
        Quaternion rotation = Quaternion.Euler(cameraRotY, cameraRotX, 0);
        this.gameObject.transform.position = this.focus.transform.position + rotation * dir;
        this.gameObject.transform.LookAt(this.focus.transform.position);
    }

    public void OnPan(InputAction.CallbackContext context) {
        if (context.phase.Equals(InputActionPhase.Performed)) {
            this.panVec = context.ReadValue<Vector2>();
        } else {
            this.panVec = Vector2.zero;
        }
    }
}
