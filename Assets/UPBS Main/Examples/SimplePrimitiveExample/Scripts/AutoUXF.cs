using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoUXF : MonoBehaviour
{
    public List<UPBS.Temporary.SimpleAutoMovement> simpleAutos;

    void Start()
    {
        UXF.Session.instance.CreateBlock(3);
        UXF.Session.instance.onSessionBegin.AddListener((x) => StartCoroutine(Runner()));
    }

    private IEnumerator Runner()
    {
        yield return new WaitForSeconds(1.5f);

        for (int i = 0; i < 3; ++i)
        {
            UXF.Session.instance.BeginNextTrial();
            foreach(var a in simpleAutos)
            {
                a.Begin();
                yield return new WaitForSeconds(3.2f);
                if (a.pattern == UPBS.Temporary.SimpleAutoMovement.MovementPattern.Line || a.pattern == UPBS.Temporary.SimpleAutoMovement.MovementPattern.Circle)
                {
                    a.Stop();
                }
            }
        }

        UXF.Session.instance.EndCurrentTrial();
    }
}
