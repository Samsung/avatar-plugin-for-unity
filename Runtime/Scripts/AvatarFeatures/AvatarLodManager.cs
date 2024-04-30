using AvatarPluginForUnity;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static AvatarPluginForUnity.MaterialCombineOption;

[RequireComponent(typeof(AvatarComponent), typeof(LODGroup))]
public class AvatarLODManager : MonoBehaviour
{
    private LODGroup avatarLODGroup = null;
    private AvatarComponent avatarComponent = null;

    void Start()
    {
        avatarComponent = GetComponent<AvatarComponent>();
        avatarLODGroup = GetComponent<LODGroup>();
    }

    public void InitAvatarLODGroup()
    {  
        avatarLODGroup.enabled = false;
        LOD[] lods = avatarLODGroup.GetLODs();
        int maxLodLength = System.Enum.GetValues(typeof(TextureResolutionRatio)).Length;
        if (lods.Length > maxLodLength)
        {
            Debug.Log("LODGroup supports up to 4 levels of LOD!!");
            return;
        }

        Renderer[] allChildren = GetComponentsInChildren<Renderer>(true);

        foreach (Renderer renderer in allChildren)
        {
            int textureResolutionRatio = 2;

            if (renderer is MeshRenderer)
            {
                MeshFilter meshFilter = renderer.gameObject.GetComponent<MeshFilter>();
                if (meshFilter == null || meshFilter.mesh == null)
                    continue;
            }
            else if (renderer is SkinnedMeshRenderer smr)
            {
                if (smr.gameObject.name.Equals("LoadNode") || smr.sharedMesh == null)
                    continue;
            }


            List<Renderer> renderers = lods[0].renderers.ToList();
            renderers.Add(renderer);
            lods[0].renderers = renderers.ToArray();

            List<Material> rendererMaterials = renderer.materials.ToList();
            for (int i = 1; i < lods.Length; i++)
            {
                string lodMeshName = renderer.gameObject.name + "_LOD_" + i;
                GameObject lodObject = MakeSubNode(lodMeshName, renderer.gameObject.transform);
                Renderer lodRenderer = null;
                if (renderer is MeshRenderer)
                {
                    Mesh mesh = renderer.gameObject.GetComponent<MeshFilter>().mesh;
                    lodObject.AddComponent<MeshFilter>().mesh = mesh;
                    lodRenderer = lodObject.AddComponent<MeshRenderer>();
                    lodRenderer.bounds = renderer.bounds;
                }
                else if (renderer is SkinnedMeshRenderer smr)
                {
                    lodRenderer = lodObject.AddComponent<SkinnedMeshRenderer>();
                    ((SkinnedMeshRenderer)lodRenderer).sharedMesh = smr.sharedMesh;
                    ((SkinnedMeshRenderer)lodRenderer).bones = smr.bones;
                    ((SkinnedMeshRenderer)lodRenderer).rootBone = smr.rootBone;
                    ((SkinnedMeshRenderer)lodRenderer).bounds = smr.bounds;
                    ((SkinnedMeshRenderer)lodRenderer).ResetBounds();
                    ((SkinnedMeshRenderer)lodRenderer).updateWhenOffscreen = true;
                }
                List<Material> lodMaterials = new List<Material>();

                foreach (var mat in rendererMaterials)
                    lodMaterials.Add(AvatarMaterialCombiner.CopyNewMaterial(mat, (TextureResolutionRatio)textureResolutionRatio));

                lodRenderer.materials = lodMaterials.ToArray();

                lodRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                lodRenderer.receiveShadows = false;


                List<Renderer> _renderers = lods[i].renderers.ToList();
                _renderers.Add(lodRenderer);
                lods[i].renderers = _renderers.ToArray();
                textureResolutionRatio *= 2;
            }
             
        }
        

        avatarLODGroup.SetLODs(lods);
        avatarLODGroup.RecalculateBounds();
        avatarLODGroup.enabled = true;
        AvatarBlendshapeDriver blendshapeDriver = avatarComponent.GetComponentInChildren<AvatarBlendshapeDriver>();
        if (blendshapeDriver != null)
            blendshapeDriver.InitBlendshapeDriver();
        
    }

    private bool IsOnJNTNode(Transform node)
    {
        if(node == null)
            return false;

        if (node.name.Equals("hips_JNT"))
            return true;
        else 
            return IsOnJNTNode(node.parent);
    }

    private GameObject MakeSubNode(string name, Transform parent)
    {
        GameObject subNode = new GameObject(name);
        subNode.transform.parent = parent;
        subNode.transform.localPosition = Vector3.zero;
        subNode.transform.localEulerAngles = Vector3.zero;
        subNode.transform.localScale = Vector3.one;
        subNode.SetActive(true);
        return subNode;
    }

}
