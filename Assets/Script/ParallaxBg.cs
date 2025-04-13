using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ParallaxBg : MonoBehaviour
{
    private Transform cam;
    private Vector3 previousCamPos;

    [SerializeField]
    private float parallaxEffect = 0.5f;

    private float texturUnitSizeX;
   
    void Start()
    {
        cam = Camera.main.transform;
        previousCamPos = cam.position;

        Sprite sprite = GetComponent<SpriteRenderer>().sprite;
        Texture2D texture = sprite.texture;
        texturUnitSizeX = texture.width / sprite.pixelsPerUnit * transform.localScale.x;
    }

    private void Update()
    {
        Vector3 delta = cam.position - previousCamPos;
        transform.position += new Vector3(delta.x * parallaxEffect, 0, 0);
        previousCamPos = cam.position;

        float distanceFromCam = Mathf.Abs(cam.position.x - transform.position.x);
        if (distanceFromCam >= texturUnitSizeX)
        {
            float offset = (cam.position.x - transform.position.x) % texturUnitSizeX;
            transform.position = new Vector3(cam.position.x + offset, transform.position.y, transform.position.z);
        }
    }
}
