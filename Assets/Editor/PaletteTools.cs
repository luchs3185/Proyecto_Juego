using UnityEditor;
using UnityEngine;

public class PaletteTools
{
    [MenuItem("Accessibility/Apply Colorable to All Scene SpriteRenderers")]
    public static void ApplyColorableToAll()
    {
        SpriteRenderer[] spriteRenderers = Object.FindObjectsOfType<SpriteRenderer>();
        foreach (SpriteRenderer sr in spriteRenderers)
        {
            if (sr.GetComponent<ColorableSpriteRenderer>() == null)
                sr.gameObject.AddComponent<ColorableSpriteRenderer>();
        }
        EditorUtility.DisplayDialog("Done", $"Applied ColorableSpriteRenderer to {spriteRenderers.Length} sprites.", "OK");
    }

    [MenuItem("Accessibility/Remove Colorable from All Scene SpriteRenderers")]
    public static void RemoveColorableFromAll()
    {
        ColorableSpriteRenderer[] components = Object.FindObjectsOfType<ColorableSpriteRenderer>();
        foreach (ColorableSpriteRenderer comp in components)
            Object.DestroyImmediate(comp);
        EditorUtility.DisplayDialog("Done", $"Removed {components.Length} ColorableSpriteRenderer components.", "OK");
    }

    [MenuItem("Accessibility/Add Colorable to Selected Sprites")]
    public static void AddColorableToSelected()
    {
        foreach (GameObject obj in Selection.gameObjects)
        {
            SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
            if (sr != null && sr.GetComponent<ColorableSpriteRenderer>() == null)
                obj.AddComponent<ColorableSpriteRenderer>();
        }
    }

    [MenuItem("Accessibility/Remove Colorable from Selected Sprites")]
    public static void RemoveColorableFromSelected()
    {
        foreach (GameObject obj in Selection.gameObjects)
        {
            ColorableSpriteRenderer comp = obj.GetComponent<ColorableSpriteRenderer>();
            if (comp != null)
                Object.DestroyImmediate(comp);
        }
    }
}
