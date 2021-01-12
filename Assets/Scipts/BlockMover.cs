using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BlockMover : MonoBehaviour {
    public float forceScale = 1F;
    public float forceDeltaSensitivity = 1F;
    public bool pulling;

    public Vector3 forceVector = Vector3.zero;
    public Vector2 forceDelta = Vector2.zero;
    public float forceInput = 0F;

    void Start() {
        bool xAligned = (this.GetComponent<Block>().level %2).Equals(0);
        this.forceVector = new Vector3(
            xAligned ? 0 : 1,
            0,
            xAligned ? 1 : 0
        );
    }

    void Update() {
        if (!Vector2.zero.Equals(forceDelta)) {
            this.forceVector = Quaternion.AngleAxis(this.forceDeltaSensitivity * this.forceDelta.x, Vector3.up) * this.forceVector;
            Vector2 perp = Vector2.Perpendicular(new Vector2(forceVector.x, forceVector.z));
            this.forceVector = Quaternion.AngleAxis(this.forceDeltaSensitivity * this.forceDelta.y, new Vector3(perp.x, 0, perp.y)) * this.forceVector;
        }

        if (forceInput > 0) {
            Vector3 deltaForce = this.forceVector * Time.deltaTime * forceInput * forceScale;
            Vector3 force = (this.pulling ? -1 : 1) * deltaForce;

            this.GetComponent<Rigidbody>().AddForce(force, ForceMode.Force);
        }
    }

    public void OnChangeAngle(InputAction.CallbackContext context) {
        if (!context.phase.Equals(InputActionPhase.Canceled)) {
            this.forceDelta = context.ReadValue<Vector2>();
        } else {
            this.forceDelta = Vector2.zero;
        }
    }

    public void OnApplyForce(InputAction.CallbackContext context) {
        if (!context.phase.Equals(InputActionPhase.Canceled)) {
            this.forceInput = context.ReadValue<float>();
        } else {
            this.forceInput = 0F;
        }
    }

    public void OnSwitchPushPull(InputAction.CallbackContext context) {
        if (context.phase.Equals(InputActionPhase.Performed)) {
            this.pulling = !this.pulling;
        }
    }
}
