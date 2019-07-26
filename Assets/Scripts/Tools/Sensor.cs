using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Sensor : Tool
{
    /// <summary>
    /// The ray for analyzing the blocks. 
    /// </summary>
    private Ray analyzer;

    /// <summary>
    /// Show the Ray for the client. 
    /// </summary>
    public LineRenderer sensorAnalyzerLine;

    /// <summary>
    /// The image to show on the sensor when analyzing a block. Fills up to 100.
    /// </summary>
    [SerializeField]
    private Image analyzerStatus = null;

    /// <summary>
    /// The image to show on the sensor. 
    /// </summary>
    [SerializeField]
    private Image sensorImage = null;

    /// <summary>
    /// The prefab when nothing is scanned. 
    /// </summary>
    [SerializeField]
    private Sprite sensorStandard = null;

    /// <summary>
    /// The prefab when bones are available in the analyzed block.
    /// </summary>
    [SerializeField]
    private Sprite sensorBones = null;

    /// <summary>
    /// The prefab when bones are not available in the analyzed block.
    /// </summary>
    [SerializeField]
    private Sprite sensorNoBones = null;

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();

        analyzer = new Ray(sensorAnalyzerLine.transform.position, sensorAnalyzerLine.transform.forward);
    }

    private void Update()
    {
        if (hasAuthority)
        {
            if (ToolType == ToolType.SENSOR)
            {
                RaycastHit hit;
                analyzer.origin = sensorAnalyzerLine.transform.position;
                analyzer.direction = sensorAnalyzerLine.transform.forward;

                if (Physics.Raycast(analyzer, out hit, 5))
                {
                    //set length of line renderer to distance of ray until hit -> line renderer won't go through objects
                    sensorAnalyzerLine.SetPosition(1, new Vector3(0, 0, hit.distance*9));
                    //Debug.Log("SensorAnalyzerLine length: " + hit.distance);

                    if (hit.transform.CompareTag("Block"))
                    {
                        Block block = hit.transform.parent.GetComponent<Block>();
                        block.ChangePercentAnalyzed(1);     //+1% analyzed with every tick
                        analyzerStatus.fillAmount = (float)block.PercentAnalyzed / (float)100;

                        //if block is completely analyzed
                        if (block.Analyzed)
                        {
                            if (block.BonesAvailable)
                            {
                                if(sensorImage.sprite != sensorBones)
                                {
                                    sensorImage.sprite = sensorBones;
                                }
                            }
                            else
                            {
                                if(sensorImage.sprite != sensorNoBones)
                                {
                                    sensorImage.sprite = sensorNoBones;
                                }
                            }
                        }
                        else
                        {
                            if(sensorImage.sprite != sensorStandard)
                            {
                                sensorImage.sprite = sensorStandard;
                            }
                        }
                    }
                    else
                    {
                        //if analyzer hits something different than a block it won't show the analyzerStatus
                        analyzerStatus.fillAmount = 0;
                        if (sensorImage.sprite != sensorStandard)
                        {
                            sensorImage.sprite = sensorStandard;
                        }
                    }
                }
                else
                {
                    //set length of line renderer to 10 when not hitting anything with colliders
                    sensorAnalyzerLine.SetPosition(1, new Vector3(0, 0, 10));

                    //hide analyzer when nothing is hit and set sensor image to neutral
                    analyzerStatus.fillAmount = 0;
                    if (sensorImage.sprite != sensorStandard)
                    {
                        sensorImage.sprite = sensorStandard;
                    }
                }
            }
        }
    }
}
