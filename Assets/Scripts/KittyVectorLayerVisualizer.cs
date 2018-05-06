namespace Mapbox.Unity.MeshGeneration.Interfaces
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Mapbox.VectorTile;
	using UnityEngine;
	using Mapbox.Unity.MeshGeneration.Filters;
	using Mapbox.Unity.MeshGeneration.Data;
	using Mapbox.Unity.MeshGeneration.Modifiers;
	using Mapbox.Unity.Utilities;
	using System.Collections;
    
	[CreateAssetMenu(menuName = "SparkleKitty/Layer Visualizer/Kitty Vector Layer Visualizer")]
	public class KittyVectorLayerVisualizer : VectorLayerVisualizer
	{
		public override void Create(VectorTileLayer layer, UnityTile tile, Action callback)
		{
            PlacesManager.Instance.UpdateTile(tile);
            base.Create(layer, tile, callback);
		}
	}
}
