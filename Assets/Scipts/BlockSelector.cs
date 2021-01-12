using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BlockSelector : MonoBehaviour {
    

    public GameObject currentBlock = null;
    public Material highlightMaterial;
    public GameObject localPlayer;

    void OnEnable() {
        Tower.OnTowerBuilt += SelectFirstBlock;
    }

    void OnDisable() {
        Tower.OnTowerBuilt -= SelectFirstBlock;
    }

    void SelectFirstBlock() {
        this.SetCurrentBlock(this.transform.GetChild(0).gameObject);
    }

    private void SetCurrentBlock(GameObject block) {
        MeshRenderer meshRenderer;
        List<Material> matList;
        
        if (this.currentBlock != null) {
            meshRenderer = this.currentBlock.GetComponent<MeshRenderer>();
            matList = new List<Material>(meshRenderer.materials);
            if (matList.Count > 1) {
                matList.RemoveRange(1, matList.Count - 1);
            }
            meshRenderer.materials = matList.ToArray();
        }

        this.currentBlock = block;

        if (this.currentBlock != null) {
            meshRenderer = this.currentBlock.GetComponent<MeshRenderer>();
            matList = new List<Material>(meshRenderer.materials);
            matList.Add(this.highlightMaterial);
            meshRenderer.materials = matList.ToArray();
        }
    }

    private GameObject GetBlock(int level, int pos) {
        for (int i = 0; i < this.transform.childCount; ++i) {
            Block child = this.transform.GetChild(i).GetComponent<Block>();
            if (child != null && child.level == level && child.pos == pos) {
                return child.gameObject;
            }
        }
        return null;
    }

    private Block getCurrent() {
        return this.currentBlock.GetComponent<Block>();
    }

    private void OnUp() {
        Block current = this.getCurrent();
        int newLevel = current.level + 1;

        if (newLevel < this.GetComponent<Tower>().GetLevels()) {
            this.SetCurrentBlock(this.GetBlock(newLevel, current.pos));
        }
    }

    private void OnDown() {
        Block current = this.getCurrent();
        int newLevel = current.level - 1;

        if (newLevel >= 0) {
            this.SetCurrentBlock(this.GetBlock(newLevel, current.pos));
        }
    }

    private void OnLeft() {
        Block current = this.getCurrent();
        int newPos = ((current.pos + 3) % 3) - 1;

        this.SetCurrentBlock(this.GetBlock(current.level, newPos));
    }

    private void OnRight() {
        Block current = this.getCurrent();
        int newPos = ((current.pos + 2) % 3) - 1;

        this.SetCurrentBlock(this.GetBlock(current.level, newPos));
    }

    public void OnNavigate(InputAction.CallbackContext context) {
        if (this.currentBlock == null) return;

        if (context.phase.Equals(InputActionPhase.Started)) {
            Vector2 direction = context.ReadValue<Vector2>();
            Block current = this.currentBlock.GetComponent<Block>();
            if (direction.y > 0) {
                OnUp();
            }

            if (direction.y < 0) {
                OnDown();
            }

            if (direction.x < 0) {
                OnLeft();
            }

            if (direction.x > 0) {
                OnRight();
            }
        }
    }

    public void OnSelect(InputAction.CallbackContext context) {
        if (!context.phase.Equals(InputActionPhase.Performed)) return;

        CameraController cameraController = Camera.main.GetComponent<CameraController>();

        if (currentBlock == null) return;

        if (cameraController.focus != this.currentBlock) {
            cameraController.focus = this.currentBlock;
            cameraController.distance = Block.CAMERA_FOCUS_DISTANCE;

            PlayerInput input = localPlayer.GetComponent<PlayerInput>();
            InputActionMap map = input.actions.FindActionMap("blockPulling", true);
            input.currentActionMap = map;

            BlockMover blockMover = this.currentBlock.GetComponent<BlockMover>();

            InputAction changeAngleInput = map.FindAction("Change Angle", true);
            changeAngleInput.started += blockMover.OnChangeAngle;
            changeAngleInput.performed += blockMover.OnChangeAngle;
            changeAngleInput.canceled += blockMover.OnChangeAngle;

            InputAction applyForceInput = map.FindAction("Pull/Push", true);
            applyForceInput.started += blockMover.OnApplyForce;
            applyForceInput.performed += blockMover.OnApplyForce;
            applyForceInput.canceled += blockMover.OnApplyForce;

            InputAction switchPushPull = map.FindAction("Switch Push/Pull", true);
            switchPushPull.started += blockMover.OnSwitchPushPull;
            switchPushPull.performed += blockMover.OnSwitchPushPull;
            switchPushPull.canceled += blockMover.OnSwitchPushPull;
        }
    }
}
