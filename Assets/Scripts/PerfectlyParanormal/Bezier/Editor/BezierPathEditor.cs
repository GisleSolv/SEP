using UnityEngine;
using System.Collections;
using UnityEditor;

namespace PerfectlyParanormal.Bezier
{

    [CustomEditor(typeof(BezierPath))]
    public class BezierPathEditor : Editor
    {
        private int se = 0;

        public override void OnInspectorGUI()
        {
            BezierPath p = (BezierPath)target;

            string[] tt = new string[5];
            tt[0] = "Points";
            tt[1] = "Controls";
            tt[2] = "Time";
            tt[3] = "Scale";
            tt[4] = "Events";

            int pse = se;

            se = GUILayout.SelectionGrid(se, tt, 5, "toggle");

            if (se != pse)
                SceneView.RepaintAll();

            if (se == 0)
                displayPoints(p);
            else if (se == 1)
                displayControls(p);
            else if (se == 2)
                displayTime(p);
            else if (se == 3)
                displayScale(p);
            else if (se == 4)
                displayEvents(p);



            if (GUILayout.Button("Approximate Length"))
            {
                p.PathLength=p.GetDistanceBetweenPoints(0, p.NumPoints - 1);
            }
            EditorGUILayout.LabelField(p.PathLength.ToString());
        }

        void displayPoints(BezierPath p)
        {
            for (int i = 0; i < p.NumPoints; i++)
            {
                EditorGUILayout.BeginHorizontal();

                p.SetPointPosition(i, EditorGUILayout.Vector3Field("Point " + i.ToString(), p.GetPointPosition(i)));
                if (GUILayout.Button("Insert After", EditorStyles.miniButton))
                {
                    p.InsertPointAfter(i);
                    SceneView.RepaintAll();
                }
                else if (GUILayout.Button("Delete", EditorStyles.miniButton))
                {
                    p.DeletePoint(i);
                    SceneView.RepaintAll();
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        void displayScale(BezierPath p)
        {
            p.DoScale = EditorGUILayout.Toggle("Enable Scale", p.DoScale);

            for (int i = 0; i < p.NumPoints; i++)
            {
                p.GetPoint(i).Scale = EditorGUILayout.FloatField("Point " + i.ToString() + " scale:", p.GetPoint(i).Scale);
            }
        }

        void displayEvents(BezierPath p)
        {
            for (int i = 0; i < p.Events.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();

                p.Events[i].EventName = EditorGUILayout.TextField("Event", p.Events[i].EventName);
                p.Events[i].Timestamp = EditorGUILayout.FloatField("Time", p.Events[i].Timestamp);
                if (GUILayout.Button("Delete", EditorStyles.miniButton))
                {
                    p.Events.RemoveAt(i);
                }

                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Add Event"))
                p.Events.Add(new BezierEvent());
        }

        void displayControls(BezierPath p)
        {
            for (int i = 0; i < p.NumPoints; i++)
            {
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField("Point " + i.ToString());
                if (GUILayout.Button("Zero Controlpoint", EditorStyles.miniButton))
                {
                    p.SetLeftControlPosition(i, p.GetPointPosition(i));
                    SceneView.RepaintAll();
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        void displayTime(BezierPath p)
        {
            for (int i = 0; i < p.NumPoints; i++)
            {
                EditorGUILayout.BeginHorizontal();
                float prev = p.GetPoint(i).TimeStamp;
                p.GetPoint(i).TimeStamp = EditorGUILayout.FloatField("Point " + i.ToString(), p.GetPoint(i).TimeStamp);
                if(prev!=p.GetPoint(i).TimeStamp)
                    SceneView.RepaintAll();
                p.GetPoint(i).Curve=EditorGUILayout.CurveField(p.GetPoint(i).Curve);
                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Calculate By Length"))
            {
                p.CalculateTimesByLength();
                SceneView.RepaintAll();
            }
        }


        void OnSceneGUI()
        {
            if (se == 0)
                drawPositionHandles();
            if (se == 1)
                drawHandles();
            if (se == 2)
                drawTimes();

            drawPath();
        }

        void drawPositionHandles()
        {
            BezierPath p = (BezierPath)target;

            for (int i = 0; i < p.NumPoints; i++)
            {
                Handles.Label(p.GetPointPosition(i), "Point " + i.ToString());
                p.SetPointPosition(i, Handles.PositionHandle(p.GetPointPosition(i), Quaternion.identity));
                // p.PathPoints[i].RightController = Handles.PositionHandle(p.PathPoints[i].RightController, Quaternion.identity);
            }

        }


        void drawHandles()
        {
            BezierPath p = (BezierPath)target;

            for (int i = 0; i < p.NumPoints; i++)
            {
                Handles.Label(p.GetPointPosition(i), "Point "+i.ToString());
                Handles.DrawLine(p.GetLeftControlPosition(i), p.GetRightControlPosition(i));
                p.SetLeftControlPosition(i, Handles.PositionHandle(p.GetLeftControlPosition(i), Quaternion.identity));
                p.SetRightControlPosition(i, Handles.PositionHandle(p.GetRightControlPosition(i), Quaternion.identity));
            }

        }

        void drawTimes()
        {
            BezierPath p = (BezierPath)target;

            for (int i = 0; i < p.NumPoints; i++)
            {
                Handles.Label(p.GetPointPosition(i), p.GetPoint(i).TimeStamp.ToString());
            }

        }

        void drawPath()
        {
            BezierPath p = (BezierPath)target;

            for (int i = 0; i < p.NumPoints - 1; i++)
            {
                //Handles.Label(p.GetPointPosition(i), "Point " + i.ToString());
                Handles.DrawBezier(p.GetPointPosition(i), p.GetPointPosition(i + 1), p.GetRightControlPosition(i), p.GetLeftControlPosition(i + 1), Color.green, null, 2);
            }
        }

    }

}