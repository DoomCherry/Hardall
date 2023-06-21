using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum FloreElementType
{
    Hidable,
    Clipping,
    ClippingHide
}

public interface IFloreElement
{
    public void Show();
    public void Hide();
}

public class FloreView : MonoBehaviour
{
    [SerializeField] private List<GameObject> _floreElementGameObjects = new List<GameObject>();
    private IFloreElement[] _floreElement;




    private IFloreElement[] FloreElement
    {
        get => _floreElement = _floreElement ??= _floreElementGameObjects.Select(n => n.GetComponent<IFloreElement>()).ToArray();
    }




    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IPlayerAnimatable playerAnimatable))
        {
            HideForAll();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out IPlayerAnimatable playerAnimatable))
        {
            ShowForAll();
        }
    }

    private void OnValidate()
    {
        for (int i = 0; i < _floreElementGameObjects.Count; i++)
        {
            if (_floreElementGameObjects[i].TryGetComponent(out IFloreElement floreElement))
                continue;

            _floreElementGameObjects.RemoveAt(i);
            i--;
        }
    }

    private void HideForAll()
    {
        foreach (var item in FloreElement)
        {
            item.Hide();
        }
    }

    private void ShowForAll()
    {
        foreach (var item in FloreElement)
        {
            item.Show();
        }
    }
}
