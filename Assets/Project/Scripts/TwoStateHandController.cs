using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TwoStateHandController : HandController
{
    [SerializeField]
    private Interactor _flatInteractor;
    
    protected override void OnGrabCanceled(InputAction.CallbackContext obj)
    {
        _flatInteractor.Actitvate();
        base.OnGrabCanceled(obj);
    }

    protected override void OnGrabPerformed(InputAction.CallbackContext obj)
    {
        _flatInteractor.Deactivate();
        base.OnGrabPerformed(obj);
    }
}
