using UnityEngine;
using Navigation;
using System.Collections.Generic;
using System.Linq;
using System;
using Random = UnityEngine.Random;

namespace View {
    public sealed class EnviromentBuilder : MonoBehaviour {
        [SerializeField]
        private float radius = 10; // set from editor
        [SerializeField]
        private int count = 10; // set from editor
        [SerializeField]
        private EnviromentBarrierObject[] barrierObjectPrefabs; // set from editor

        public float Radius { get { return radius; } }

        public List<ExcludeVolume> GenerateEnviroment() {
            var excludeVolumes = new List<ExcludeVolume>();
            for (int i = 0; i < count; i++) {
                var prefab = barrierObjectPrefabs[Random.Range(0, barrierObjectPrefabs.Length - 1)];
                var normal = Random.onUnitSphere;
                var position = normal * radius;
                var rotation = Quaternion.LookRotation(normal) * Quaternion.Euler(90, 0, 0);
                var volumes = prefab.OccupatedVolumes.Select(volume => new ExcludeVolume(position + rotation * volume.Offset, volume.Size));
                if (IsContains(excludeVolumes, volumes))
                    continue;
                var enviromentObject = Instantiate(prefab);
                enviromentObject.transform.position = position;
                enviromentObject.transform.rotation = rotation;
                enviromentObject.RebuildMeshSpherical(radius);
                if (enviromentObject.OccupatedVolumes == null)
                    continue;
                foreach (var volume in enviromentObject.OccupatedVolumes) {
                    excludeVolumes.Add(new ExcludeVolume(enviromentObject.transform.position + enviromentObject.transform.rotation * volume.Offset, volume.Size));
                }
            }
            return excludeVolumes;
        }

        private bool IsContains(List<ExcludeVolume> excludeVolumes, IEnumerable<ExcludeVolume> volumes) {
            foreach (var volume in volumes) {
                foreach (var exclude in excludeVolumes) {
                    if (Vector3.Distance(volume.Position, exclude.Position) < volume.Radius + exclude.Radius) {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
