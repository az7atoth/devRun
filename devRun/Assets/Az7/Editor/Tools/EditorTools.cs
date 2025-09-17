using UnityEngine;
using UnityEditor;

namespace Az7.Editor
{
    public class EditorTools : UnityEditor.Editor
    {
        [MenuItem("Tools/Az7/Clear Player Prefs")]
        public static void ClearPlayerPrefs()
        {
            PlayerPrefs.DeleteAll();
            Debug.Log("Player prefs deleted");
        }
    }
}
