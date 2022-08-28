using System;
using System.Collections.Generic;
using System.Linq;
using AnttiStarterKit.Extensions;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AnttiStarterKit.Animations
{
    public class Clouds : MonoBehaviour
    {
        [SerializeField] private List<SpriteRenderer> clouds;
        [SerializeField] private float diff = 1f;
        [SerializeField] private float speed = -1f;
        [SerializeField] private float limit = 10;

        private List<float> speeds;

        private void Awake()
        {
            clouds.ForEach(c => c.transform.position += Random.Range(-diff, diff) * Vector3.up);
            speeds = clouds.Select(c => (1 + c.transform.position.z) * Random.Range(0.5f, 1.5f)).ToList();
        }

        private void Update()
        {
            clouds.ForEach(MoveCloud);
        }

        private void MoveCloud(SpriteRenderer cloud)
        {
            var t = cloud.transform;
            t.position += speed * Time.deltaTime * Vector3.right / speeds[clouds.IndexOf(cloud)];

            if (t.position.x < -limit || t.position.x > limit)
            {
                t.position = t.position.WhereX(limit * -Mathf.Sign(speed));
            }
        }
    }
}