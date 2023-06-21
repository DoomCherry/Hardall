using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ComplexFloreElements : MonoBehaviour, IFloreElement
{
    [SerializeField] private List<GameObject> _additionalFloreElements = new List<GameObject>();
    private IFloreElement[] _floreElements;




    public IFloreElement[] FloreElements
    {
        get
        {
            IEnumerable<IFloreElement> _additionalElements = _additionalFloreElements.Select(n => n.GetComponent<IFloreElement>())
                                                                                     .ToArray();

            return _floreElements = _floreElements ??= GetComponentsInChildren<IFloreElement>().Union(_additionalElements)
                                                                                               .Where(n => n != GetComponent<IFloreElement>())
                                                                                               .ToArray();
        }
    }




    private void OnValidate()
    {
        for (int i = 0; i < _additionalFloreElements.Count; i++)
        {
            if (_additionalFloreElements[i].TryGetComponent(out IFloreElement floreElement))
                continue;

            _additionalFloreElements.RemoveAt(i);
            i--;
        }
    }

    public void Hide()
    {
        foreach (var item in FloreElements)
        {
            item.Hide();
        }
    }

    public void Show()
    {
        foreach (var item in FloreElements)
        {
            item.Show();
        }
    }
}
