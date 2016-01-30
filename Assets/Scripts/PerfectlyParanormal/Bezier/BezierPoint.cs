using UnityEngine;
using System.Collections;

namespace PerfectlyParanormal.Bezier
{

    [System.Serializable]
    public class BezierPoint
    {

        [SerializeField]
        private Vector3 m_position;
        public Vector3 Position
        {
            get { return m_position; }
            set
            {
                m_position = value;

            }

        }

        public float TimeStamp;
        public AnimationCurve Curve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1,1));

        public float Scale = 1f;

        public BezierPoint()
        {
            m_leftController = new Vector3();
            m_rightController = new Vector3();
        }

        public BezierPoint(Vector3 pos)
        {
            m_leftController = new Vector3();
            m_rightController = new Vector3();
            m_position = pos;
        }

        [SerializeField]
        private Vector3 m_leftController;
        public Vector3 LeftController
        {
            get { return m_leftController + m_position; }
            set
            {

                m_leftController = value - m_position;
                m_rightController = -m_leftController;
            }
        }

        [SerializeField]
        private Vector3 m_rightController;
        public Vector3 RightController
        {
            get { return m_rightController + m_position; }
            set
            {
                m_rightController = value - m_position;
                m_leftController = -m_rightController;
            }
        }



    }

}