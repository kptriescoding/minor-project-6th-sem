using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;
using UnityEngine.Video;

public class Video : MonoBehaviour
{
    public GameObject VirtualButton1;
    public GameObject VideoCube1;
    public VideoPlayer video1;
    private VirtualButtonBehaviour VB_Behaviour1;
    public GameObject ButtonCube1;
    
    
    // Start is called before the first frame update
     private void Start()
    {
        video1 = VideoCube1.GetComponent<VideoPlayer>();
        
        VB_Behaviour1 = VirtualButton1.GetComponent<VirtualButtonBehaviour>();
        VB_Behaviour1.RegisterOnButtonPressed(playVideo1);


        
    }

    // Update is called once per frame
    public void playVideo1(VirtualButtonBehaviour vp)
    {
        if(!video1.isPlaying){
           
        video1.Play();
         ButtonCube1.GetComponent<Renderer>().material.color = Color.red;
        }
        else{
        video1.Pause();
        
         ButtonCube1.GetComponent<Renderer>().material.color = Color.green;
        }
    }

    public void stop(){
        video1.Pause();
    }
   
}
