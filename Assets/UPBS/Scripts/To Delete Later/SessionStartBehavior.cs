using UnityEngine.SceneManagement;
using UnityEngine;
using UXF;
using System.Collections;
namespace UPBS.Temporary
{
    public class SessionStartBehavior : MonoBehaviour
    {
        private UXF.Session session;
        private bool sessionActive = false;
        public void HideUXFUI()
        {

        }

        public void InitSession(float trialTime)
        {
            session = UXF.Session.instance;
            if (session)
            {
                session.endAfterLastTrial = true;

                //Prints all the chars individually cuz I'm tired...
                foreach (var x in session.settings.GetString("trial_specification_name"))
                {
                    //print(x);
                }

                //Now we're gonna just manually create some blocks...
                session.CreateBlock(1);

                session.onSessionEnd.AddListener(MarkSessionOver);
                session.FirstTrial.Begin();
                print("Begin First Trial");
                sessionActive = true;
                StartCoroutine(AdvanceTrialsOverTime(trialTime));
            }
        }

        public void MarkSessionOver(Session session)
        {
            sessionActive = false;
        }

        private IEnumerator AdvanceTrialsOverTime(float time)
        {
            while (sessionActive)
            {
                yield return new WaitForSeconds(time);

                if (session)
                {

                    if (session.LastTrial != session.CurrentTrial)
                    {
                        session.CurrentTrial.End();
                        session.BeginNextTrial();
                    }

                    else
                    {
                        session.CurrentTrial.End();
                        session.End();
                    }


                }
            }

        }

        public void HelloDarknessMyOldFriend()
        {

        }

        public void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}

