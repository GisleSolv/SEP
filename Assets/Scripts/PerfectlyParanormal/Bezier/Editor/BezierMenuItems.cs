using UnityEngine;
using System.Collections;
using UnityEditor;

namespace PerfectlyParanormal.Bezier
{

    public class BezierMenuItems
    {
        [MenuItem("GameObject/Sam/Create Bezier Path", false, 10)]
        public static void CreateBezierPath(MenuCommand menuCommand)
        {
            GameObject go = new GameObject();
            go.name = "New Bezier Path";
            go.AddComponent<BezierPath>().CreatePath();

            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
        }

    }

}