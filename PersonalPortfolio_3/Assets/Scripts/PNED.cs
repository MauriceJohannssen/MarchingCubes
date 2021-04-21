using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//@name   PerlinNoiseEnvironmentDesigner
//@author Maurice Johannssen
//@date   4/20/2021

[ExecuteInEditMode]

public class PNED : MonoBehaviour
{
    [SerializeField] private float objectDensity;
    [SerializeField] private float perlinNoiseSampleScale;
    [SerializeField] private bool randomizeNoiseSampleScale;
    [SerializeField] private float OffsetStrength;
    [Range(0.0f, 1.0f)] public float normalLerp;
    [SerializeField] private bool randomizeNormalLerp;
    [SerializeField] private GameObject[] grass;
    [SerializeField] private float grassWeight = 0.4f;
    [SerializeField] private GameObject[] bushes;
    [SerializeField] private float bushWeight = 0.7f;
    [SerializeField] private GameObject[] trees;
    [SerializeField] private float treeWeight = 0.85f;

    private PerlinNoise _perlinNoise;
    private readonly List<GameObject> environmentGameObjects = new List<GameObject>();

    private void Start()
    {
        _perlinNoise = GetComponent<PerlinNoise>();
    }


    public void GenerateEnvironment()
    {
        for (float x = 1; x < 50; x += objectDensity)
        {
            for (float z = 1; z < 50; z += objectDensity)
            {
                do
                {
                    if (randomizeNoiseSampleScale) perlinNoiseSampleScale = Random.Range(0.0f, 200.0f);
                } while (perlinNoiseSampleScale % 1 == 0);

                float perlinNoise = _perlinNoise.PerlinNoise2D(new Vector2(x, z) * perlinNoiseSampleScale);
                Vector3 offsetVector = new Vector3(_perlinNoise.PerlinNoise2D(new Vector2(x,z) * perlinNoiseSampleScale), 0, 
                    _perlinNoise.PerlinNoise2D(new Vector2(z,x) * perlinNoiseSampleScale));
               
                offsetVector *= -0.5f;
                
                if (!Physics.Raycast(new Vector3(x, 100, z) + offsetVector * OffsetStrength, Vector3.down * 150, out var raycastHit)) continue;
                if (!raycastHit.collider.gameObject.CompareTag("Terrain")) continue;
                if (Vector3.Dot(Vector3.up, raycastHit.normal) < 0.55f) continue;
                
                GameObject[] arrayToUse = null;
                if (perlinNoise >= treeWeight)
                    arrayToUse = trees;
                else if (perlinNoise >= bushWeight)
                    arrayToUse = bushes;
                else if (perlinNoise >= grassWeight)
                    arrayToUse = grass;
            
                if(arrayToUse == null) continue;
                float slerpAmount = randomizeNormalLerp ? Random.Range(0.0f, normalLerp) : normalLerp;
                Quaternion rotation = Quaternion.Slerp(Quaternion.Euler(Vector3.up), Quaternion.FromToRotation(Vector3.up, raycastHit.normal), slerpAmount) 
                                      * Quaternion.Euler(0, Random.Range(0,360), 0);
                
                environmentGameObjects.Add(Instantiate(arrayToUse.ElementAt(Random.Range(0, arrayToUse.Length)), raycastHit.point, rotation));
            }
        }
    }

    public void DeleteEnvironment()
    {
        foreach (var environmentObject in environmentGameObjects)
        {
            DestroyImmediate(environmentObject);
        }
    }
}
