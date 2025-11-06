using System.Collections;
using UnityEngine;
using UnityEngine.XR.Management;

public class MXOffset : MonoBehaviour
{
    [System.Serializable] public struct LoaderOffset
    {
        [Tooltip("Will search this string in the active XRLoader name to apply those values")]
        public string loaderName;
        public Vector3 position;
        public Vector3 rotation;
    }

    public LoaderOffset[] offsets = new LoaderOffset[] // default values that works for my hand 3d model 
{
        // Oculus Loader
        new LoaderOffset { loaderName = "Oculus" , position = new Vector3( 0.034f , -0.032f, -0.129f ) , rotation = new Vector3( 11.25f, 0, 0 ) },
        // Open XR Loader
        new LoaderOffset { loaderName = "Open XR" , position = new Vector3( 0, 0.0511f, -0.0476f ) , rotation = new Vector3( 60.36f, 0, 0 ) } ,

        new LoaderOffset { loaderName = "OpenXRLoader" , position = new Vector3( 0, 0.0511f, -0.0476f ) , rotation = new Vector3( 60.36f, 0, 0 ) } ,

    };

    string Normalize(string s) => s.ToLower().Trim();

    IEnumerator Start()
    {
        XRLoader xrloader = null;

        while( xrloader == null )
        {
            yield return new WaitForSeconds( 0.5f );

            xrloader = XRGeneralSettings.Instance?.Manager?.activeLoader ?? null;
        }

        var search = xrloader.name;

        bool found = false;

        foreach( var offset in offsets )
        {
            if( Normalize( search ).IndexOf( Normalize( offset.loaderName ) ) > - 1 )
            {
                transform.localPosition = offset.position;
                transform.localRotation = Quaternion.Euler( offset.rotation );
                found = true;  
                break;
            }
        }

        if( ! found ) Debug.LogError("Missing active loader offset profile, name : " + search , this );
    }

}
