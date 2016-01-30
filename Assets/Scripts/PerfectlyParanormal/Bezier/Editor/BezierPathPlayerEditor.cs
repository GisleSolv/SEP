using UnityEngine;
using System.Collections;
using UnityEditor;

namespace PerfectlyParanormal.Bezier
{
    [CustomEditor(typeof(BezierPathPlayer))]
    public class BezierPathPlayerEditor : Editor
    {

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            BezierPathPlayer p = (BezierPathPlayer)target;

            float t = p.Path.PreviewTime;
            p.Path.PreviewTime = EditorGUILayout.Slider(p.Path.PreviewTime, 0, p.Path.PathTotalTimeEditor);
            if (t != p.Path.PreviewTime)
            {
                Vector3 pos;
                p.Path.MoveAlong(p.Path.PreviewTime, out pos);
                if (p.Target != null)
                    p.Target.position = pos;

            }
        }
    }

}