using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

namespace PerfectlyParanormal.Bezier
{
    /// <summary>
    /// Bezierpath primarily used for camera movement together with Flux plugin
    /// </summary>
    public class BezierPath : MonoBehaviour
    {
        [SerializeField]
        private List<BezierPoint> m_pathPoints = new List<BezierPoint>();
        [SerializeField]
        private List<BezierEvent> m_events = new List<BezierEvent>();
        public List<BezierEvent> Events { get { return m_events; } }
        public Action<string> EventCallback;

        public int NumPoints { get { return m_pathPoints.Count; } }

        public float PathResolution = 0.001f;
        public float PathLength=0;

        public float PathTotalTime=0;
        public float PathTotalTimeEditor { get { return GetTotalTime(); } }

        public float PreviewTime;

        //private float m_currentTime;
        //private bool m_playing = false;
        public bool DoScale = false;


        void Awake()
        {
            PathTotalTime = GetTotalTime();
        }

        public void CreatePath()
        {

            m_pathPoints.Add(new BezierPoint(new Vector3(-2f, 0, 0)));
            m_pathPoints.Add(new BezierPoint(new Vector3(2f, 0, 0)));
        }

        public void InsertPointAfter(int index)
        {
            BezierPoint p = new BezierPoint(GetMidpointOnCurveIndex(index));

            m_pathPoints.Insert(index + 1, p);

        }

        public void DeletePoint(int index)
        {
            m_pathPoints.RemoveAt(index);
        }

        public Vector3 GetMidpointOnCurveIndex(int x)
        {
            if(x>=NumPoints-1)
            {
                Vector3 p0 = BezierUtils.CalculateBezierCurve(0.9f, GetPointPosition(x-1), GetPointPosition(x), GetRightControlPosition(x-1), GetLeftControlPosition(x));
                Vector3 p1 = BezierUtils.CalculateBezierCurve(1f, GetPointPosition(x - 1), GetPointPosition(x), GetRightControlPosition(x - 1), GetLeftControlPosition(x));
                Vector3 diff = p1 - p0;
                diff.Normalize();
                diff *= GetDistanceBetweenPoints(x - 1, x)/2;
                return p1+diff;
            }
            else
            {
                return BezierUtils.CalculateBezierCurve(0.5f, GetPointPosition(x), GetPointPosition(x+1), GetRightControlPosition(x), GetLeftControlPosition(x+1));
            }
        }

        public float GetDistanceBetweenPoints(int start, int end)
        {
            if (end <= start) end = start + 1;
            if (end >= NumPoints - 1) end = NumPoints - 1;

            float distance = 0;

            for (int i = start; i < end; i++)
            {
                float t = 0;
                Vector3 prev=BezierUtils.CalculateBezierCurve(t, GetPointPosition(i), GetPointPosition(i + 1), GetRightControlPosition(i), GetLeftControlPosition(i + 1));
                do
                {
                    t += PathResolution;
                    if(t>1f) t=1f;
                    Vector3 p1 = BezierUtils.CalculateBezierCurve(t, GetPointPosition(i), GetPointPosition(i + 1), GetRightControlPosition(i), GetLeftControlPosition(i + 1));
                    Vector3 diff = p1 - prev;
                    distance += diff.magnitude;
                    prev = p1;
                }
                while (t < 1f);
            }

            return distance;
        }

        /*void Update()
        {

            if (!m_playing) return;

            if (MoveAlong(m_currentTime))
            {
                if (OnComplete != null)
                {
                    OnComplete.Invoke();
                }
                m_playing = false;
            }

            m_currentTime += Time.deltaTime;
        }*/

        /*public void PlayPath(float startPercent=0)
        {
            ResetEvents();
            m_currentTime = PathTotalTime*startPercent;
            m_playing = true;

        }*/



        public bool MoveAlong(float time, out Vector3 pos)
        {
            float s;
            pos = GetPositionAtTime(time, out s);
            //obj.transform.position = pos;

            CheckEvents(time);

            if (time >= PathTotalTime)
                return true;
            return false;
        }

        public bool MoveAlong(float time, out Vector3 pos, out Vector3 scale)
        {
            float s;
            pos = GetPositionAtTime(time, out s);
            //obj.transform.position = pos;
            if (DoScale)
                scale = new Vector3(s, s, s);
            else scale = new Vector3();

            CheckEvents(time);

            if (time >= PathTotalTime)
                return true;
            return false;
        }

        public Vector3 GetPositionAtTime(float time)
        {
            for (int i = 1; i < NumPoints; i++)
            {
                if (time <= m_pathPoints[i].TimeStamp && time > m_pathPoints[i - 1].TimeStamp)
                {
                    float p = (time - m_pathPoints[i - 1].TimeStamp) / (m_pathPoints[i].TimeStamp - m_pathPoints[i - 1].TimeStamp);
                    return BezierUtils.CalculateBezierCurve(m_pathPoints[i].Curve.Evaluate(p), GetPointPosition(i - 1), GetPointPosition(i), GetRightControlPosition(i - 1), GetLeftControlPosition(i));
                }
            }

            if (time > GetTotalTime())
                return GetPointPosition(NumPoints - 1);

            return GetPointPosition(0);
        }

        public Vector3 GetPositionAtTime(float time, out float scale)
        {
            for (int i = 1; i < NumPoints; i++)
            {
                if (time <= m_pathPoints[i].TimeStamp && time > m_pathPoints[i - 1].TimeStamp)
                {
                    float p = (time - m_pathPoints[i - 1].TimeStamp) / (m_pathPoints[i].TimeStamp - m_pathPoints[i - 1].TimeStamp);
                    scale = m_pathPoints[i - 1].Scale + (m_pathPoints[i].Scale - m_pathPoints[i - 1].Scale) * m_pathPoints[i].Curve.Evaluate(p);
                    return BezierUtils.CalculateBezierCurve(m_pathPoints[i].Curve.Evaluate(p), GetPointPosition(i - 1), GetPointPosition(i), GetRightControlPosition(i - 1), GetLeftControlPosition(i));
                }
            }

            if (time > GetTotalTime())
            {
                scale = GetPoint(NumPoints - 1).Scale;
                return GetPointPosition(NumPoints - 1);
            }
            scale = GetPoint(0).Scale;
            return GetPointPosition(0);
        }

        void CheckEvents(float t)
        {
            for (int i = 0; i < m_events.Count; i++)
            {
                if (m_events[i].Timestamp >= t && !m_events[i].Called)
                {
                    m_events[i].Called = true;
                    if (EventCallback != null) EventCallback(m_events[i].EventName);
                }
            }
        }

        public void ResetEvents()
        {
            for (int i = 0; i < m_events.Count; i++)
            {
                m_events[i].Called = false;
            }
        }

        public float GetTotalTime()
        {
            return m_pathPoints[NumPoints - 1].TimeStamp;

            /*float time = 0;
            for (int i = 0; i < NumPoints; i++)
            {
                time += m_pathPoints[i].TimeStamp;
            }
            return time;*/
        }

        public void RecalculateTotalTime()
        {
            PathTotalTime = GetTotalTime();
        }

        public void CalculateTimesByLength()
        {
            float totalTime = m_pathPoints[NumPoints - 1].TimeStamp;
            float totalLength = GetDistanceBetweenPoints(0, NumPoints - 1);
            m_pathPoints[0].TimeStamp = 0f;
            float lastTime = 0;

            for (int i = 1; i < NumPoints; i++)
            {
                float dist = GetDistanceBetweenPoints(i-1, i);
                m_pathPoints[i].TimeStamp = totalTime * (dist / totalLength)+lastTime;
                lastTime = m_pathPoints[i].TimeStamp;
            }
        }

        public Vector3 GetPointPosition(int index)
        {
            return m_pathPoints[index].Position + transform.position;
        }

        public void SetPointPosition(int index, Vector3 val)
        {
            m_pathPoints[index].Position = val - transform.position;
        }

        public Vector3 GetLeftControlPosition(int index)
        {
            return m_pathPoints[index].LeftController + transform.position;
        }

        public void SetLeftControlPosition(int index, Vector3 val)
        {
            m_pathPoints[index].LeftController = val - transform.position;
        }

        public Vector3 GetRightControlPosition(int index)
        {
            return m_pathPoints[index].RightController + transform.position;
        }

        public void SetRightControlPosition(int index, Vector3 val)
        {
            m_pathPoints[index].RightController = val - transform.position;
        }

        public BezierPoint GetPoint(int index)
        {
            return m_pathPoints[index];
        }

        public float GetPointScale(int index)
        {
            return m_pathPoints[index].Scale;
        }

    }

}