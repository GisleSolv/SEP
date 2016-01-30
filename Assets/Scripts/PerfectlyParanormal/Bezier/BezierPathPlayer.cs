using UnityEngine;
using System.Collections;
using UnityEngine.Events;

namespace PerfectlyParanormal.Bezier
{
    [RequireComponent(typeof(BezierPath))]
    public class BezierPathPlayer : MonoBehaviour
    {

        private BezierPath m_path;
        public BezierPath Path { get { if (m_path == null) m_path = GetComponent<BezierPath>(); return m_path; } }

        private float m_currentTime;
        private bool m_playing = false;

        public Transform Target;
        public bool PlayOnStart = false;

        public UnityEvent OnComplete;

        public bool AnimateScale=true;

        void Awake()
        {
            m_path = GetComponent<BezierPath>();

            if (m_path == null)
                this.enabled = false;
        }

        void Start()
        {
            if (PlayOnStart)
                PlayPath();
        }

        public void PlayPath(float startPercent = 0)
        {
            m_path.ResetEvents();
            m_currentTime = m_path.PathTotalTime * startPercent;
            m_playing = true;

        }

        public void MoveToPosition(float percent=0)
        {
            Vector3 pos, scale;
            m_path.MoveAlong(percent, out pos, out scale);
            Target.position = pos;
        }

        public void Stop()
        {
            m_playing = false;
        }

        void Update()
        {
            if (!m_playing) return;

            Vector3 pos, scale;
            bool done=m_path.MoveAlong(m_currentTime, out pos, out scale);

            Target.position = pos;
            if (m_path.DoScale && AnimateScale)
                Target.localScale = scale;

            if (done)
            {
                m_playing = false;
                if (OnComplete != null)
                {
                    OnComplete.Invoke();
                }
                
            }

            

            m_currentTime += Time.deltaTime;
        }
    }


}