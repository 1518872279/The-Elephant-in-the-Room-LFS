using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhonePageDirect : MonoBehaviour
{
    public GameObject PointedPage;
    public GameObject CurrentPage;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void GoToPointedPage()
    {
        PointedPage.SetActive(true);
        CurrentPage.SetActive(false);
    }
}
