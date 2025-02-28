using Cinemachine;
using KBCore.Refs;
using Unity.VisualScripting;
using UnityEngine;

namespace Climb
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField, Anywhere] CinemachineVirtualCamera _cmVCam;
        float rotationY;

        void Update()
        {
            var state = _cmVCam.State;
            var rotation = state.FinalOrientation;

            var euler = rotation.eulerAngles;

            rotationY = euler.y;

            var roundedRotationY = Mathf.RoundToInt(rotationY);
        }

        public Quaternion flatRotation => Quaternion.Euler(0, rotationY, 0);

    }
}