using UnityEngine;
using System.Collections;

public class AnimatedTextureUV : MonoBehaviour {
    public int uvAnimationTileX = 24; //Here you can place the number of columns of your sheet.
                               //The above sheet has 24

    public int uvAnimationTileY = 1; //Here you can place the number of rows of your sheet.
                              //The above sheet has 1

    public float framesPerSecond = 10f;

    public float AnimLength
    {
        set{ framesPerSecond = (float)Mathf.Abs(uvAnimationTileX * uvAnimationTileY) / value; }
        get{ return Mathf.Abs(uvAnimationTileX * uvAnimationTileY) / Mathf.Abs(framesPerSecond);}
    }

    private int index = 0;
    //private float interval = 1f;
    private float time = 0f;

    public bool playOnce = false;
    bool playOnAwake = true;
    bool isPlaying = false;

    void Start()
    {
        isPlaying = playOnAwake;
        index = 0;
        time = 0f;
        //interval = 1f/Mathf.Abs(framesPerSecond);
    }

    public void Stop()
    {
        isPlaying = false;
        index = 0;
        time = 0f;
    }

    public void Play()
    {
        isPlaying = true;
    }

    void OnEnable()
    {
        index = 0;
        time = 0f;
        //interval = 1f/Mathf.Abs(framesPerSecond);
    }

    void Update ()
    {
        if(framesPerSecond != 0 && isPlaying)
        {
            //interval -= (Time.timeScale == 0f) ? 0f : (Time.deltaTime / Time.timeScale);
            time += (Time.timeScale == 0f) ? 0f : (Time.deltaTime / Time.timeScale);
            index = (int)(Mathf.Abs (framesPerSecond) * time) % Mathf.Abs(uvAnimationTileX * uvAnimationTileY);
            if(playOnce && index == (Mathf.Abs(uvAnimationTileX * uvAnimationTileY)-1))
            {
                framesPerSecond = 0;
            }
        }

        // Size of every tile
        Vector2 size = new Vector2 (1f / Mathf.Abs (uvAnimationTileX), 1f / Mathf.Abs (uvAnimationTileY));
        if(uvAnimationTileX<0f) size.x *= -1f;
       
        // split into horizontal and vertical index
        int uIndex = index % Mathf.Abs (uvAnimationTileX);
        int vIndex = index / Mathf.Abs (uvAnimationTileX);

        // build offset
        // v coordinate is the bottom of the image in opengl so we need to invert.
        float offsetX = (1f/uvAnimationTileX) * uIndex;
        if(uvAnimationTileX < 0f) offsetX = (1f - (1f/uvAnimationTileX)) - offsetX;
        Vector2 offset = new Vector2 (offsetX, 1f - size.y - vIndex * size.y);

        GetComponent<Renderer>().material.SetTextureOffset ("_MainTex", offset);
        GetComponent<Renderer>().material.SetTextureScale ("_MainTex", size);
    }
}
