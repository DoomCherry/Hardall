using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    void Interact();
}

public interface IInteractableWithOneParameter<T>
{
    void Interact(T parameter);
}
