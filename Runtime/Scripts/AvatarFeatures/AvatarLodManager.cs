using AvatarPluginForUnity;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using static AvatarPluginForUnity.AvatarComponent;
using static AvatarPluginForUnity.MaterialCombineOption;
using static AvatarPluginForUnity.MeshCombineOption;

[RequireComponent(typeof(AvatarComponent), typeof(LODGroup))]
public class AvatarLODManager : MonoBehaviour
{
    private LODGroup avatarLODGroup = null;
    private AvatarComponent avatarComponent = null;

    // Start is called before the first frame update
    void Start()
    {
        avatarComponent = GetComponent<AvatarComponent>();
        avatarComponent.OnStatusChangedCallback += InitAvatarLODGroup;
        avatarLODGroup = GetComponent<LODGroup>();
    }

    private void InitAvatarLODGroup(LoadStatus componentLoadStatus)
    {
        if (componentLoadStatus.Equals(LoadStatus.DONE))
        {
            avatarLODGroup.enabled = false;
            LOD[] lods = avatarLODGroup.GetLODs();
            int maxLodLength = System.Enum.GetValues(typeof(TextureResolutionRatio)).Length;
            if (lods.Length > maxLodLength)
            {
                Debug.LogError("LODGroup supports up to 4 levels of LOD!!");
                return;
            }

            int textureResolutionRatio = 1;
            MeshCombineOption meshCombineOption = avatarComponent.meshCombineOption.ToMeshCombineOption();          
            if (!avatarComponent.useMeshCombine || !meshCombineOption.combineFlags.HasFlag(CombineFlags.UseMaterialCombine))
                meshCombineOption.materialCombineOption.customMaterialData = null;

            Renderer[] allChildren = GetComponentsInChildren<Renderer>(true);
            for(int i=0;i< lods.Length; i++)
            {
                int meshIdx = 0;
                List<Renderer> lodRendererGoup = new List<Renderer>();

                foreach (Renderer renderer in allChildren)
                {
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

                    if (i == 0)
                        lodRendererGoup.Add(renderer);
                    else
                    {
                        string lodMeshName = "lodMeshGoup_" + i + "_" + meshIdx;
                        GameObject lodObject = MakeSubNode(lodMeshName, renderer.gameObject.transform);
                        Renderer lodRenderer = null;
                        if (renderer is MeshRenderer)
                        {
                            Mesh mesh = renderer.gameObject.GetComponent<MeshFilter>().mesh;
                            lodObject.AddComponent<MeshFilter>().mesh = mesh;
                            lodRenderer = lodObject.AddComponent<MeshRenderer>();
                            lodRenderer.bounds = renderer.bounds;
                        }
                        else if (renderer is SkinnedMeshRenderer smr) {
                            lodRenderer = lodObject.AddComponent<SkinnedMeshRenderer>();
                            ((SkinnedMeshRenderer)lodRenderer).sharedMesh = smr.sharedMesh;
                            ((SkinnedMeshRenderer)lodRenderer).bones = smr.bones;
                            ((SkinnedMeshRenderer)lodRenderer).rootBone = smr.rootBone;
                            ((SkinnedMeshRenderer)lodRenderer).bounds = smr.bounds;
                            ((SkinnedMeshRenderer)lodRenderer).ResetBounds();
                            ((SkinnedMeshRenderer)lodRenderer).updateWhenOffscreen = true;
                        }
                        meshCombineOption.materialCombineOption.textureResolutionRatio = (TextureResolutionRatio)textureResolutionRatio;
                        lodRenderer.material = AvatarMaterialCombiner.CopyNewMaterial(renderer.material, meshCombineOption);
                        lodRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                        lodRenderer.receiveShadows = false;
                        lodRendererGoup.Add(lodRenderer);
                    }
                    meshIdx++;
                }
                lods[i].renderers = lodRendererGoup.ToArray();
                textureResolutionRatio *= 2;
            }
            avatarLODGroup.SetLODs(lods);
            avatarLODGroup.RecalculateBounds();
            avatarLODGroup.enabled = true;
            AvatarBlendshapeDriver blendshapeDriver = avatarComponent.loadNode.GetComponent<AvatarBlendshapeDriver>();
            if (blendshapeDriver != null)
                blendshapeDriver.InitBlendshapeDriver();
        }
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
