using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : Character
{
    #region Constants #############################################################

    #endregion

    #region Variables #############################################################

    #endregion

    #region Statics   #############################################################

    #endregion

    #region Inner Types ###########################################################

    #endregion

    #region Properties ############################################################

    #endregion

    #region Public Functions ######################################################

    #endregion

    #region Private Functions #####################################################

    protected override void CalculateDesiredDirection(float deltaTime)
    {
        base.CalculateDesiredDirection(deltaTime);

        if (_camera == null)
        {
            DesiredDirection = Vector3.zero;
            return;
        }
        var gamePad = Gamepad.current;
        if (gamePad == null)
        {
            DesiredDirection = Vector3.zero;
            return;
        }
        if (SuspendInputs)
        {
            DesiredDirection = Vector3.zero;
            return;
        }
        Vector2 inputAxis = gamePad.leftStick.ReadValue();
        Vector3 planedCameraFwd = Vector3.ProjectOnPlane(_camera.transform.forward, transform.up).normalized;
        Vector3 planedCameraRight = Vector3.ProjectOnPlane(_camera.transform.right, transform.up).normalized;
        Vector3 relativeAxis = planedCameraFwd * inputAxis.y + planedCameraRight * inputAxis.x;
        DesiredDirection = relativeAxis;
    }

    protected override void GetActions()
    {
        base.GetActions();

        var gamePad = Gamepad.current;
        if (gamePad == null)
        {
            return;
        }
        if (SuspendInputs)
        {
            return;
        }
        if (gamePad.buttonNorth.wasPressedThisFrame)
            AttackAction?.Invoke();
        if (gamePad.buttonSouth.wasPressedThisFrame)
            JumpAction?.Invoke();
        if (gamePad.buttonWest.wasPressedThisFrame)
            InteractAction?.Invoke();

        DefenseAction?.Invoke(gamePad.leftShoulder.isPressed);
        SprintAction?.Invoke(gamePad.rightTrigger.IsPressed());
    }

    #endregion

    #region Jobs      #############################################################

    #endregion

    #region MonoBehaviours ########################################################

    #endregion
}

