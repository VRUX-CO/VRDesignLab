using UnityEngine;
using System.Collections;

//script is for demo purposes only. Shouldn't be used in a production setting as all the UI work should be handled elsewhere.

public class QT_InteractContainer : MonoBehaviour {
    public GameObject ContainerTop;
    public string OpenText = "Press E to Open.";
    public string CloseText = "Press E to Close.";
    public AnimationClip OpenClip, CloseClip;

    private Vector3 centerScreen;
    public GUIText PopUpText;
    private bool isOpen = false;
    private Animator Anim;
   
	// Use this for initialization
	void Start () {
      
       // int sw = Screen.width/2;
       // int sh = Screen.height/2;
        centerScreen = new Vector3(0.5f, 0.5f, 0f);
        Anim = ContainerTop.GetComponent<Animator>();
	}	

    void OnTriggerStay()
    {
   
        Vector3 rayStart = Camera.main.ViewportToWorldPoint(centerScreen);
        Vector3 rayDir = Camera.main.transform.forward;        
        RaycastHit rayHit;
      
        if (Physics.Raycast(rayStart,rayDir,out rayHit,5.0f))
        {
            
            if (rayHit.collider.name.Equals(ContainerTop.name) && !isOpen)
            {                
                    PopUpText.gameObject.SetActive(true);
                    PopUpText.text = OpenText;
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        Anim.Play(OpenClip.name);
                        
                        isOpen = true;                        
                    }
             }
            else if (rayHit.collider.name.Equals(ContainerTop.name) && isOpen)
                {
                    PopUpText.gameObject.SetActive(true);
                    PopUpText.text = CloseText;
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        Anim.Play(CloseClip.name);
                        isOpen = false;
                    }
                }
            
            else            
                PopUpText.gameObject.SetActive(false);
            
        }
    }

    void OnTriggerExit()
    {
        PopUpText.gameObject.SetActive(false);       
    }

}
