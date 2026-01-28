using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Oicaimang
{

    public class ChangeMaterialEffect2 : MonoBehaviour
    {
        public Material[] materials;   // Danh sách material gán trong Inspector
        public float changeInterval = 5f; // Thời gian đổi (giây)

        public Renderer rend;
        private int currentIndex = 0;
        private float timer = 0f;

        void Start()
        {
            if (materials.Length > 0)
            {
                rend.material = materials[0];
            }
        }

        void Update()
        {
            if (materials.Length == 0) return;

            timer += Time.deltaTime;

            if (timer >= changeInterval)
            {
                timer = 0f;
                currentIndex = (currentIndex + 1) % materials.Length;
                rend.material = materials[currentIndex];
            }
        }
    }
}
