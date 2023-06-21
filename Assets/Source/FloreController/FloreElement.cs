using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FloreElement : MonoBehaviour, IFloreElement
{
    [SerializeField] protected FloreElementType floreElementType;
    [SerializeField] protected List<MeshRenderer> _additionalMeshes = new List<MeshRenderer>();
    [SerializeField] protected float _hideTo = 0.55f;
    [SerializeField] protected float _showHideTime = 1;




    private void Start()
    {
        if (TryGetComponent(out MeshRenderer meshRenderer) && !_additionalMeshes.Contains(meshRenderer))
            _additionalMeshes.Add(meshRenderer);

        if (floreElementType == FloreElementType.Hidable)
            return;

        foreach (var item in _additionalMeshes)
        {
            item.material = new Material(item.material);
        }
    }

    public void Show()
    {
        switch (floreElementType)
        {
            case FloreElementType.Hidable:
                SetViewMeshByHidableType(true);
                break;
            case FloreElementType.Clipping:
                SetViewMeshByClippingType(1, _showHideTime);
                break;
            case FloreElementType.ClippingHide:
                SetViewMeshByClippingHide(1, _showHideTime);
                break;
        }
    }

    public void Hide()
    {
        switch (floreElementType)
        {
            case FloreElementType.Hidable:
                SetViewMeshByHidableType(false);
                break;
            case FloreElementType.Clipping:
                SetViewMeshByClippingType(_hideTo, _showHideTime);
                break;
            case FloreElementType.ClippingHide:
                SetViewMeshByClippingHide(_hideTo, _showHideTime);
                break;
        }
    }

    private void SetViewMeshByHidableType(bool isShow)
    {
        foreach (var item in _additionalMeshes)
        {
            item.shadowCastingMode = isShow ? UnityEngine.Rendering.ShadowCastingMode.On : UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
        }
    }

    private void SetViewMeshByClippingType(float alpha, float time = 0)
    {
        foreach (var item in _additionalMeshes)
        {
            if (time > 0)
                item.material.DOFloat(alpha, "_Alpha", time);
            else
                item.material.SetFloat("_Alpha", alpha);
        }
    }

    private void SetViewMeshByClippingHide(float alpha, float time = 0)
    {
        foreach (var item in _additionalMeshes)
        {
            float currentAlpha = item.material.GetFloat("_Alpha");
            bool isActivate = alpha > currentAlpha;


            if (time > 0)
                DOTween.Sequence().Append(item.material.DOFloat(alpha, "_Alpha", time)).AppendCallback(() =>
                    item.shadowCastingMode = isActivate ? UnityEngine.Rendering.ShadowCastingMode.On : UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly);
            else
                item.shadowCastingMode = isActivate ? UnityEngine.Rendering.ShadowCastingMode.On : UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
        }
    }
}
