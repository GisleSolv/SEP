using UnityEngine;
using System.Collections;
using Fabric;
using UnityEngine.SceneManagement;

public static class AudioManager
{
    private static void playAudio(string eventName)
    {
        //AUDIO: without position
        EventManager.Instance.PostEvent(eventName);
    }

    private static void playAudioAndEvent(string eventName, OnEventNotify cb)
    {
        //AUDIO: without position
        EventManager.Instance.PostEventNotify(eventName, cb);
    }

    private static void playAudioWithPosition(string eventName, GameObject ob)
    {
        //AUDIO: with position
        EventManager.Instance.PostEvent(eventName, ob);
    }

    private static void playAudioWithPositionAndEvent(string eventName, GameObject ob, OnEventNotify cb)
    {
        //AUDIO: with position
        EventManager.Instance.PostEventNotify(eventName, ob, cb);
    }

    public static bool FabricLoaded { get { return EventManager.Instance; } }


    public static void PlaySound(string n)
    {
        LoadFabric();
        if (FabricLoaded)
            playAudio(n);
    }

    public static void PlaySound(string n, OnEventNotify cb)
    {
        LoadFabric();
        if (FabricLoaded)
            playAudioAndEvent(n, cb);
    }

    public static void PlaySound(string n, GameObject ob)
    {
        LoadFabric();
        if (FabricLoaded)
            playAudioWithPosition(n, ob);
    }

    public static void PlaySound(string n, GameObject ob, OnEventNotify cb)
    {
        LoadFabric();
        if (FabricLoaded)
            playAudioWithPositionAndEvent(n, ob, cb);
    }

    public static void StopSound(string n)
    {
        EventManager.Instance.PostEvent(n, EventAction.StopAll);
    }

    public static void FadeOutMusic(string n)
    {
        // fade out the music!
        Fabric.Component component = FabricManager.Instance.GetComponentByName(n);
        if (component != null)
        {
            component.FadeOut(0.1f, 0.5f);
        }
    }

    public static int SetDialogLine(string dialogEvent, string componentName)
    {
        
        EventManager.Instance.PostEvent(componentName, EventAction.SetAudioClipReference, dialogEvent);
        

        Fabric.DialogAudioComponent component = FabricManager.Instance.GetComponentByName("Audio_CharacterGroup_"+componentName) as DialogAudioComponent;
        string pfix = "";
        int inst = component.GetNumActiveComponentInstances();
        return inst;
        /*if (inst > 0)
        {
            return (DialogAudioComponent)FabricManager.Instance.GetComponentByName("Audio_CharacterGroup_" + componentName+"_"+inst.ToString());
        }

        return 0;*/
    }

    public static DialogAudioComponent GetDialogueInstance(string componentName, int instance)
    {

        if (instance > 0)
        {
            Debug.Log("Audio_CharacterGroup_"+componentName+"_Instances_" + componentName + "_" + instance.ToString());
            return (DialogAudioComponent)FabricManager.Instance.GetComponentByName("Audio_CharacterGroup_"+componentName+"_Instances_" + componentName + "_" + instance.ToString());
        }
        else return (DialogAudioComponent)FabricManager.Instance.GetComponentByName("Audio_CharacterGroup_" + componentName); 

    }

    public static void LoadFabric()
    {
        if (FabricLoaded)
        { // || Application.isLoadingLevel) {
            return;
        }
        SceneManager.LoadSceneAsync("Audio", LoadSceneMode.Additive);

    }
    
}
