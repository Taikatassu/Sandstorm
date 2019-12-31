using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewerChanger : MonoBehaviour
{
    public KeyCode changeViewersKey = KeyCode.Space;
    public Transform[] viewers;
    public FollowTarget cameraController;
    [Tooltip("If false, all given viewers are assigned simultaneously. If true, loops through given viewers, one at a time.")]
    public bool switchBetweenViewers = true;

    private int counter = 0;
    private EndlessTerrainController endlessTerrainController;

    private void Start()
    {
        endlessTerrainController = FindObjectOfType<EndlessTerrainController>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(changeViewersKey))
        {
            if (endlessTerrainController != null && viewers.Length > 0)
            {
                if (switchBetweenViewers)
                {
                    endlessTerrainController.SetViewers(new Transform[1] { viewers[counter] });
                    cameraController.target = viewers[counter];
                    counter++;

                    if (counter >= viewers.Length)
                    {
                        counter = 0;
                    }
                }
                else
                {
                    endlessTerrainController.SetViewers(viewers);
                    cameraController.target = viewers[0];
                }
            }
        }
    }
}
