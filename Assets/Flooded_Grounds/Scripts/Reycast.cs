using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Reycast : MonoBehaviour
{

    [Header("Reycast Features")]
    [SerializeField] private float rayLength = 5;
    private Camera _camera;

    private NoteController _noteController;


    [Header("Crosshair")]
    [SerializeField] private Image crosshair;


    [Header("Crosshair")]
    [SerializeField] private KeyCode interactKey;

    void Start()
    {
        _camera = GetComponent<Camera>();
    }

    private void Update()
    {
        if (Physics.Raycast(_camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f)), transform.forward, out RaycastHit hit, rayLength))
        {
            var readebleItem = hit.collider.GetComponent<NoteController>();
            if (readebleItem != null)
            {
                _noteController = readebleItem;
                HighlightCrosshair(true);
            }
            else
            {
                ClearNote();
            }
        }
        else
        {
            ClearNote();
        }

        if (_noteController != null)
        {
            if (Input.GetKeyDown(interactKey))
            {
                _noteController.ShowNote();
            }
        }
    }

    void ClearNote()
    {
        if (_noteController != null)
        {
            HighlightCrosshair(false);
            //notecontoller = null;
        }
    }

    void HighlightCrosshair(bool on)
    {
        if (on)
        {
            crosshair.color = Color.red;
        }
        else
        {
            crosshair.color = Color.white;
        }
    }

}
