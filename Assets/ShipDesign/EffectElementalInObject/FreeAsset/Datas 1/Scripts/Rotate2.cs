using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Oicaimang
{
    public class Rotate2 : MonoBehaviour
    {
        public Vector3 rotationSpeed = new Vector3(0f, 100f, 0f); // tốc độ xoay theo từng trục

        void Update()
        {
            transform.Rotate(rotationSpeed * Time.deltaTime);
        }
    }
}