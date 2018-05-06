namespace Mapbox.Examples
{
	using UnityEngine;
	using Mapbox.Unity.Map;
	using UnityEngine.UI;
    using Mapbox.Unity.Location;

    public class KittyLoadingPanelController : MonoBehaviour
	{
		[SerializeField]
		GameObject _content;

		[SerializeField]
		Text _text;

		[SerializeField]
		AnimationCurve _curve;

        ILocationProvider locationProvider;

		void Awake()
		{
			var map = FindObjectOfType<AbstractMap>();
			var visualizer = map.MapVisualizer;
            locationProvider = LocationProviderFactory.Instance.DefaultLocationProvider;
            _text.text = "LOADING";
			visualizer.OnMapVisualizerStateChanged += (s) =>
			{
				if (this == null)
					return;

				if (s == ModuleState.Finished)
				{
					_content.SetActive(false);
				}
				else if (s == ModuleState.Working)
				{
					// Uncommment me if you want the loading screen to show again
					// when loading new tiles.
					//Content.SetActive(true);
				}
			};
		}

		void Update()
		{
            if (locationProvider is KittyDeviceLocationProvider && !Input.location.isEnabledByUser)
                _text.text = "GPS NOT ENABLED";
            else
                _text.text = "LOADING";
            var t = _curve.Evaluate(Time.time);
			_text.color = Color.Lerp(Color.clear, Color.white, t);
		}
	}
}