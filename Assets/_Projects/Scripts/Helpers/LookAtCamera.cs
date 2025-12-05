using System;
using UnityEngine;

namespace _Projects.Helpers.Const
{
    public class LookAtCamera : MonoBehaviour
    {
        private void LateUpdate()
        {
            transform.forward = Camera.main.transform.forward;
        }
    }
}