using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlaneWood))]
public class PlaneWoodEditor : Editor
{
    PlaneWood planeWood;

    bool isEditableInEditor = true;

    private void OnEnable()
    {
        if (Application.isPlaying)
        {
            isEditableInEditor = false;
            return;
        }
        
        planeWood = (PlaneWood)target;
        planeWood.Init();
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (!isEditableInEditor)
            return;

        if (GUI.changed)
        {
            planeWood.CreatePlaneWood();
        }

        if (GUILayout.Button("Update"))
        {
            planeWood.CreatePlaneWood();
        }
    }
}