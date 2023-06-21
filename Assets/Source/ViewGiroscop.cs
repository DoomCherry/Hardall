using UnityEngine;

public class ViewGiroscop : MonoBehaviour
{
    [SerializeField] private Transform[] _views;
    [SerializeField] private Vector3 _angle;




    public void FixedUpdate()
    {
        foreach (var item in _views)
            item.rotation = Quaternion.Euler(_angle);
    }
}
