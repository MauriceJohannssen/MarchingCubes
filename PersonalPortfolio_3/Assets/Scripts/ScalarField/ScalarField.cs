using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class ScalarField : MonoBehaviour
{
    public float scale;

    private int pixelWidth = 500;
    private int pixelHeight = 500;

    private float xOrigin = 0;
    private float yOrigin = 0;

    private Texture2D _noiseTexture;
    private Color[] _pixels;
    private MeshRenderer _renderer;

    // Start is called before the first frame update
    private void Start()
    {
        _renderer = GetComponent<MeshRenderer>();
        _noiseTexture = new Texture2D(pixelWidth, pixelHeight);
        _pixels = new Color[pixelWidth * pixelHeight];
        _renderer.material.mainTexture = _noiseTexture;
        AssembleNoiseTexture();
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

    private void AssembleScalarField()
    {
        int amountOfScalars = 40;
        
        //NOTE: This is in pixels!
        int materialStepSize = _noiseTexture.width / amountOfScalars;

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
                    newGameObject.transform.localScale *= Mathf.Sqrt(_pixels[(x + y  * _noiseTexture.width) * materialStepSize].maxColorComponent);
                    if(newGameObject.transform.localScale.magnitude < 1f) Destroy(newGameObject);
                    currentZ += vertexStepSizeZ;
                }

                currentZ = 0;
                currentY += vertexStepSizeY;
            }

            currentY = 0;
            currentX += vertexStepSizeX;
        }
    }
}