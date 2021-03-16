using UnityEngine;
using System.Collections.Generic;

public class ScalarField : MonoBehaviour
{
    public float scale;

    [Range(0.0f, 5.0f)] public float SurfaceValue = 1.0f;

    private int pixelWidth = 1024;
    private int pixelHeight = 1024;

    private float xOrigin = 0;
    private float yOrigin = 0;

    private int amountOfScalars = 50;

    public Texture2D _noiseTexture;
    private Color[] _pixels;
    private MeshRenderer _renderer;

    private List<GameObject> _scalarSpheres = new List<GameObject>();

    // Start is called before the first frame update
    private void Start()
    {
        _renderer = GetComponent<MeshRenderer>();
        //_noiseTexture = new Texture2D(pixelWidth, pixelHeight);
        _pixels = new Color[pixelWidth * pixelHeight];
        //_renderer.material.mainTexture = _noiseTexture;
        //AssembleNoiseTexture();
        AssembleScalarField();
    }

    private void AssembleNoiseTexture()
    {
        for (float y = 0; y < _noiseTexture.height; y++)
        {
            for (float x = 0; x < _noiseTexture.width; x++)
            {
                float xCoord = (x / _noiseTexture.width) * scale;
                float yCoord = (y / _noiseTexture.height) * scale;
                float sample = Mathf.PerlinNoise(xCoord, yCoord);
                _pixels[(int) x + (int) y * _noiseTexture.width] = new Color(sample, sample, sample);
            }
        }

        _noiseTexture.SetPixels(_pixels);
        _noiseTexture.Apply();
    }

    private float Get3DPerlinValue(Vector3 sampleCoordinates)
    {
        //Factor is: f(x)= 1 / (x - 1)
        float multiplicationFactor = 1.0f / (amountOfScalars - 1.0f);
        Vector3 normalizedSampleCoordinates = sampleCoordinates * multiplicationFactor;

        Debug.Log($"The normalized vector is {normalizedSampleCoordinates}");

        //float xyColorComponent = _pixels[(int) ((normalizedYValue * _noiseTexture.height) * (normalizedXValue * _noiseTexture.width))].maxColorComponent;
        //float xzColorComponent = _pixels[(int) ((normalizedZValue * _noiseTexture.height) * normalizedXValue * _noiseTexture.width)].maxColorComponent;

        float xzColorComponent = _noiseTexture.GetPixel((int) (normalizedSampleCoordinates.x * _noiseTexture.width), (int) (normalizedSampleCoordinates.z * _noiseTexture.height)).maxColorComponent;
        float xyColorComponent = _noiseTexture.GetPixel((int) (normalizedSampleCoordinates.x * _noiseTexture.width), (int) (normalizedSampleCoordinates.y * _noiseTexture.height)).maxColorComponent;
        float yzColorComponent = _noiseTexture.GetPixel((int) (normalizedSampleCoordinates.z * _noiseTexture.width), (int) (normalizedSampleCoordinates.y * _noiseTexture.height)).maxColorComponent;
        return (xyColorComponent + xzColorComponent + yzColorComponent) / 3.0f;
    }

    private void AssembleScalarField()
    {
        Vector3 currentLocalScale = transform.localScale;
        Vector3 startingPosition = transform.position - currentLocalScale / 2;

        float vertexStepSizeX = currentLocalScale.x / (amountOfScalars - 1);
        float vertexStepSizeY = currentLocalScale.y / (amountOfScalars - 1);
        float vertexStepSizeZ = currentLocalScale.z / (amountOfScalars - 1);

        float currentX = 0;
        float currentY = 0;
        float currentZ = 0;

        //NOTE: Resetting all the values seems cumbersome,
        //      but using floats as iteration variables and having
        //      rounding errors is even more cumbersome (and error prone)
        for (int x = 0; x < amountOfScalars; x++)
        {
            for (int y = 0; y < amountOfScalars; y++)
            {
                for (int z = 0; z < amountOfScalars; z++)
                {
                    GameObject newGameObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    newGameObject.transform.position = startingPosition + new Vector3(currentX, currentY, currentZ);
                    newGameObject.transform.localScale *= Get3DPerlinValue(new Vector3(x, y, z));
                    if (newGameObject.transform.localScale.magnitude < SurfaceValue) Destroy(newGameObject);
                    else _scalarSpheres.Add(newGameObject);
                    currentZ += vertexStepSizeZ;
                }

                currentZ = 0;
                currentY += vertexStepSizeY;
            }

            currentY = 0;
            currentX += vertexStepSizeX;
        }
    }

    public void RebakeScalarField()
    {
        foreach (var currentSphere in _scalarSpheres)
        {
            Destroy(currentSphere);
        }

        _scalarSpheres.Clear();
        AssembleScalarField();
    }
}