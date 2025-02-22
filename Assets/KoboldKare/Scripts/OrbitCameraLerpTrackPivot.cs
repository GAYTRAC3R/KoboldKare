using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;
using UnityScriptableSettings;

public class OrbitCameraLerpTrackPivot : OrbitCameraPivotBase {
    protected Transform targetTransform;
    protected Ragdoller ragdoller;
    private CharacterControllerAnimator characterControllerAnimator;
    protected SettingFloat fov;
    protected float lerpTrackSpeed = 0.1f;
    private Vector3 lastPosition;
    public virtual void Initialize(Animator targetAnimator, HumanBodyBones bone, float lerpTrackSpeed) {
        targetTransform = targetAnimator.GetBoneTransform(bone);
        transform.SetParent(targetAnimator.GetBoneTransform(HumanBodyBones.Hips), false);
        ragdoller = GetComponentInParent<Ragdoller>();
        characterControllerAnimator = GetComponentInParent<CharacterControllerAnimator>();
        transform.position = targetTransform.position;
        fov = SettingsManager.GetSetting("CameraFOV") as SettingFloat;
        this.lerpTrackSpeed = lerpTrackSpeed;
    }

    public override Vector3 GetPivotPosition(Quaternion camRotation) {
        if (!isActiveAndEnabled) {
            return lastPosition;
        }
        lastPosition = ragdoller.ragdolled || characterControllerAnimator.IsAnimating() ? targetTransform.position : transform.position;
        return lastPosition;
    }

    protected virtual void LateUpdate() {
        Vector3 a = transform.localPosition;
        Vector3 b = transform.parent.InverseTransformPoint(targetTransform.position);
        Vector3 correction = b - a;
        float diff = correction.magnitude;
        transform.localPosition = Vector3.MoveTowards(a, a+correction, (0.1f+diff)*Time.deltaTime*lerpTrackSpeed);
    }

    public void SnapInstant() {
        Vector3 b = transform.parent.InverseTransformPoint(targetTransform.position);
        transform.localPosition = b;
    }

    public override float GetFOV(Quaternion camRotation) {
        return fov.GetValue();
    }
}
