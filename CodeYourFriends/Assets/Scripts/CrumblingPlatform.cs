using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A platform which breaks after a certain amount of steps on it. The mesh will be swapped after each step. " +
/// A step is in this case the character leaving the platform"
/// </summary>
public class CrumblingPlatform : MonoBehaviour
{
    /// <summary>
    /// Used to store all materials for one mesh 
    /// </summary>
    [Serializable]
    private class MaterialWrapper
    {
        public List<Material> materials;
    }
    
    // General Properties
    [Header("Properties")] 
    [SerializeField]
    [Tooltip("How often the character can step on this platform before breaking it")]
    private int numOfStepsAllowed = 1;

    // Animation and Visuals
    [Header("Animations and Visuals")]
    [SerializeField]
    [Tooltip("The meshes assigned with the platform. The order is important, " +
             "since the mesh will be swapped in this order")]
    private List<Mesh> meshes;

    [SerializeField]
    [Tooltip("The materials for the meshes. Please make sure the meshes and the materials are in the same order.")]
    private List<MaterialWrapper> materials;

    // Non Editor fields
    private int _numSteppedOn;
    private MeshFilter _meshFilter;
    private MeshRenderer _meshRenderer;
    private Animation _animation;

    private void Awake()
    {
        _meshFilter = GetComponent<MeshFilter>();
        _meshRenderer = GetComponent<MeshRenderer>();
        _animation = GetComponent<Animation>();
       
        // DEBUG stuff for logging and making sure everything is set up correctly
#if DEBUG
        if (meshes.Count != numOfStepsAllowed)
        {
            Debug.LogError("The amount of meshes and number of steps allowed isn't matching on the object " +
                           gameObject.name);
            Destroy(gameObject);
        }

        if (materials.Count != meshes.Count)
        {
            Debug.LogError($"The amount of materials and meshes isn't matching on the object {gameObject.name}");
            Destroy(gameObject);
        }
        
        if (_meshFilter == null)
        {
            Debug.LogError($"no mesh filter component found on the object {gameObject.name}");
            Destroy(gameObject);
        }

        if (_animation == null)
        {
            Debug.LogError($"No animation component added to the object {gameObject.name}");
        }
#endif
        
        SwapMesh(meshes[0], materials[0]);
    }

    private void OnCollisionExit(Collision other)
    {
        if (!other.gameObject.tag.Equals("Player")) return;
        
        if(++_numSteppedOn < numOfStepsAllowed) return;
        StartCoroutine(Deactivate());
    }

    // Utility methods
    
    private void SwapMesh(Mesh newMesh, MaterialWrapper meshMaterial)
    {
        _meshFilter.sharedMesh = newMesh;
        _meshRenderer.materials = meshMaterial.materials.ToArray();
    }

    private IEnumerator Deactivate()
    {
        _animation.Play("CrumblingTest");
        while (_animation.isPlaying) yield return null;
        //TODO might have to be changed later since platforms should be reused
        Destroy(gameObject);
    }
}
