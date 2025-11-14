using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using Unity.Hierarchy;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
    
public class AdvancementManager : MonoBehaviour
{
    public GameObject AdvancementMenu;
    private bool advancementActivated;

    //=========Miral=========//

    public GameObject Bobby;

    [SerializeField]
    public Image BobbyImage;

    [SerializeField]
    public TMP_Text BobbyDescription;

    [SerializeField]
    public TMP_Text BobbyName;

    //=========Miral=========//

    public GameObject Miral;

    [SerializeField]
    public Image MiralImage;

    [SerializeField]
    public TMP_Text MiralDescription;

    [SerializeField]
    public TMP_Text MiralName;


    //=========Blob=========//

    public GameObject Blob;

    [SerializeField]
    public Image BlobImage;

    [SerializeField]
    public TMP_Text BlobDescription;

    [SerializeField]
    public TMP_Text BlobName;

    private void Start()
    {
        
        BobbyImage.enabled = false;
        BobbyDescription.enabled = false;
        BobbyName.enabled = false;


        MiralImage.enabled = false;
        MiralDescription.enabled = false;
        MiralName.enabled = false;

        BlobImage.enabled = false;
        BlobDescription.enabled = false;
        BlobName.enabled = false;

    }
    private void Update()
    {
        if (Bobby == null)
        {
            BobbyImage.enabled = true;
            BobbyDescription.enabled = true;
            BobbyName.enabled = true;

        }

        if (Miral == null)
        {
            MiralImage.enabled = true;
            MiralDescription.enabled = true;
            MiralName.enabled = true;
        }

        if (Blob == null)
        {
            BlobImage.enabled = true;
            BlobDescription.enabled = true;
            BlobName.enabled = true;
        }
    }
}
