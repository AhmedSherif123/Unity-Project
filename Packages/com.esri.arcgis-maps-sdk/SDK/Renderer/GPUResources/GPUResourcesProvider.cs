// COPYRIGHT 1995-2021 ESRI
// TRADE SECRETS: ESRI PROPRIETARY AND CONFIDENTIAL
// Unpublished material - all rights reserved under the
// Copyright Laws of the United States and applicable international
// laws, treaties, and conventions.
//
// For additional information, contact:
// Attn: Contracts and Legal Department
// Environmental Systems Research Institute, Inc.
// 380 New York Street
// Redlands, California 92373
// USA
//
// email: legal@esri.com
using Esri.GameEngine.RCQ;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Esri.ArcGISMapsSDK.Renderer.GPUResources
{
	internal class GPUResourcesProvider : IGPUResourcesProvider
	{
		private readonly Dictionary<uint, IGPUResourceMaterial> materials = new();
		private readonly Dictionary<uint, IGPUResourceMesh> meshes = new();
		private readonly Dictionary<uint, IGPUResourceTexture2D> textures = new();
		private readonly Dictionary<uint, IGPUResourceRenderTexture> renderTextures = new();

		public IReadOnlyDictionary<uint, IGPUResourceMaterial> Materials => materials;
		public IReadOnlyDictionary<uint, IGPUResourceMesh> Meshes => meshes;
		public IReadOnlyDictionary<uint, IGPUResourceTexture2D> Textures => textures;
		public IReadOnlyDictionary<uint, IGPUResourceRenderTexture> RenderTextures => renderTextures;

		public IGPUResourceMaterial CreateMaterial(uint id, ArcGISRenderableType renderableType, ArcGISMaterialType materialType, Material customMaterial)
		{
			Material material = null;

			if (customMaterial)
			{
				material = new Material(customMaterial);
			}
			else
			{
				string shaderPath;

				if (renderableType == ArcGISRenderableType.Tile)
				{
					shaderPath = "TileSurface";
				}
				else
				{
					switch (materialType)
					{
						case ArcGISMaterialType.Lit:
							shaderPath = "SceneNodeSurface";
							break;
						case ArcGISMaterialType.Unlit:
							shaderPath = "UnlitSceneNodeSurface";
							break;
						default:
							throw new System.Exception("Unexpected material type");
					}
				}

				var isRenderPipelineSupported = GraphicsSettings.defaultRenderPipeline == null;

				if (!isRenderPipelineSupported)
				{
					var renderType = GraphicsSettings.defaultRenderPipeline.GetType().ToString();

					if (renderType == "UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset" || renderType == "UnityEngine.Rendering.HighDefinition.HDRenderPipelineAsset")
					{
						isRenderPipelineSupported = true;
					}
				}

				if (isRenderPipelineSupported)
				{
					material = new Material(Resources.Load<Material>("Materials/Renderables/" + shaderPath));
				}
				else
				{
					Debug.LogError("Unsupported render pipeline.");
				}
			}

			var resourceMaterial = new GPUResourceMaterial(material);
			materials.Add(id, resourceMaterial);

			return resourceMaterial;
		}

		public IGPUResourceMesh CreateMesh(uint id)
		{
			var resourceMesh = new GPUResourceMesh(new Mesh());
			meshes.Add(id, resourceMesh);

			return resourceMesh;
		}

		public IGPUResourceRenderTexture CreateRenderTexture(uint id, uint width, uint height, ArcGISTextureFormat format, bool useMipMaps)
		{
			var rendertextureFormat = GetUnityRendertextureFormatFromInternal(format);

			var nativeRenderTexture = new RenderTexture((int)width, (int)height, 0, rendertextureFormat, RenderTextureReadWrite.Linear)
			{
				enableRandomWrite = true,
				autoGenerateMips = false,
				useMipMap = useMipMaps,
				anisoLevel = 9
			};
			nativeRenderTexture.Create();

			var resourceRenderTexture = new GPUResourceRenderTexture(nativeRenderTexture);
			renderTextures.Add(id, resourceRenderTexture);

			return resourceRenderTexture;
		}

		public IGPUResourceTexture2D CreateTexture(uint id, uint width, uint height, ArcGISTextureFormat format, bool isSRGB)
		{
			var textureFormat = GetUnityTextureFormatFromInternal(format);
			var nativeTexture = new Texture2D((int)width, (int)height, textureFormat, false, !isSRGB)
			{
				wrapMode = TextureWrapMode.Clamp,
				filterMode = FilterMode.Bilinear
			};

			var resourceTexture = new GPUResourceTexture2D(nativeTexture);
			textures.Add(id, resourceTexture);

			return resourceTexture;
		}

		public void DestroyMaterial(uint id)
		{
			Debug.Assert(materials.ContainsKey(id));

			var material = materials[id];
			material.Destroy();
			materials.Remove(id);
		}

		public void DestroyMesh(uint id)
		{
			Debug.Assert(meshes.ContainsKey(id));

			var mesh = meshes[id];
			mesh.Destroy();
			meshes.Remove(id);
		}

		public void DestroyTexture(uint id)
		{
			Debug.Assert(textures.ContainsKey(id));

			var texture = textures[id];
			texture.Destroy();
			textures.Remove(id);
		}

		public void DestroyRenderTexture(uint id)
		{
			Debug.Assert(renderTextures.ContainsKey(id));

			var renderTexture = renderTextures[id];
			renderTexture.Release();
			renderTexture.Destroy();
			renderTextures.Remove(id);
		}

		public void Release()
		{
			foreach (var texture in textures)
			{
				texture.Value.Destroy();
			}

			foreach (var renderTexture in renderTextures)
			{
				renderTexture.Value.Release();
				renderTexture.Value.Destroy();
			}

			textures.Clear();
			renderTextures.Clear();
		}

		private static TextureFormat GetUnityTextureFormatFromInternal(ArcGISTextureFormat textureFormat)
		{
			switch (textureFormat)
			{
				case ArcGISTextureFormat.RGB8UNorm:
					return TextureFormat.RGB24;
				case ArcGISTextureFormat.R32Float:
					return TextureFormat.RFloat;
				case ArcGISTextureFormat.RGBA16UNorm:
					return TextureFormat.RGBAFloat;
				case ArcGISTextureFormat.RGBA8UNorm:
					return TextureFormat.RGBA32;
				case ArcGISTextureFormat.BGRA8UNorm:
					return TextureFormat.BGRA32;
				case ArcGISTextureFormat.DXT1:
					return TextureFormat.DXT1;
				case ArcGISTextureFormat.DXT5:
					return TextureFormat.DXT5;
				case ArcGISTextureFormat.RGBA32Float:
					return TextureFormat.RGBAFloat;
				case ArcGISTextureFormat.ETC2RGB8:
				case ArcGISTextureFormat.ETC2SRGB8:
					return TextureFormat.ETC2_RGB;
				case ArcGISTextureFormat.ETC2RGB8PunchthroughAlpha1:
				case ArcGISTextureFormat.ETC2SRGB8PunchthroughAlpha1:
					return TextureFormat.ETC2_RGBA1;
				case ArcGISTextureFormat.ETC2EacRGBA8:
				case ArcGISTextureFormat.ETC2EacSRGBA8:
					return TextureFormat.ETC2_RGBA8;
				case ArcGISTextureFormat.R32Norm:
					return TextureFormat.RGBA32;
				default:
					Debug.LogError("Texture format not supported!");
					return TextureFormat.RGB24;
			}
		}

		private static RenderTextureFormat GetUnityRendertextureFormatFromInternal(ArcGISTextureFormat textureFormat)
		{
			switch (textureFormat)
			{
				case ArcGISTextureFormat.RGBA8UNorm:
					return RenderTextureFormat.ARGB32;
				case ArcGISTextureFormat.BGRA8UNorm:
					return RenderTextureFormat.BGRA32;
				case ArcGISTextureFormat.R32Float:
					return RenderTextureFormat.RFloat;
				case ArcGISTextureFormat.RGBA16UNorm:
					return RenderTextureFormat.ARGBHalf;
				case ArcGISTextureFormat.RGBA32Float:
					return RenderTextureFormat.ARGBFloat;
				default:
					Debug.LogError("RenderTexture format not supported!");
					return RenderTextureFormat.ARGB32;
			}
		}
	}
}
