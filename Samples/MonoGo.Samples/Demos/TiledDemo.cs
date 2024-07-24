using MonoGo.Engine;
using MonoGo.Engine.EC;
using MonoGo.Engine.Resources;
using MonoGo.Engine.SceneSystem;
using MonoGo.Samples.Misc.Tiled;
using MonoGo.Tiled;
using MonoGo.Tiled.MapStructure;
using System;

namespace MonoGo.Samples.Demos
{
	/// <summary>
	/// Tiled is a free map editor which can be downloaded at
	/// https://mapeditor.org
	/// 
	/// Though, note that not all the Tiled features are
	/// currently supported (like infinite tilemaps or animated tiles.)
	/// </summary>
	public class TiledDemo : Entity
	{
		public static readonly string Description =
			"Build > ${FC:96FF5F}Collision${RESET}: ${FC:FFDB5F}" + BuildCustomMapBuilderButton + "${FC:96FF5F} Default${RESET}: ${FC:FFDB5F}" + BuildDefaultMapBuilderButton + "${RESET}" + " Delete > ${FC:FFDB5F}" + DestroyMapButton + "${RESET}" + Environment.NewLine +
			"Move > ${FC:FFDB5F}WASD${RESET}" + Environment.NewLine +
			"Camera > ${FC:96FF5F}Move${RESET}: ${FC:FFDB5F}" + CameraController.UpButton + "${RESET} / ${FC:FFDB5F}" + CameraController.DownButton + "${RESET} / ${FC:FFDB5F}" + CameraController.LeftButton + "${RESET} / ${FC:FFDB5F}" + CameraController.RightButton + "${RESET}" + Environment.NewLine +
			"Camera > ${FC:96FF5F}Rotate${RESET}: ${FC:FFDB5F}" + CameraController.RotateLeftButton + "${RESET} / ${FC:FFDB5F}" + CameraController.RotateRightButton + "${RESET}" + Environment.NewLine +
			"Camera > ${FC:96FF5F}Zoom${RESET}: ${FC:FFDB5F}" + CameraController.ZoomInButton + "${RESET} / ${FC:FFDB5F}" + CameraController.ZoomOutButton + "${RESET}" + Environment.NewLine;

        MapBuilder _builder;

		public const Buttons BuildCustomMapBuilderButton = Buttons.B;
		public const Buttons BuildDefaultMapBuilderButton = Buttons.N;
		public const Buttons DestroyMapButton = Buttons.U;

		private TiledMap _testMap; 

		public TiledDemo(Layer layer) : base(layer)
		{
			// TiledMap which is loaded from Content, is just a data structure
			// describing the map. We need to make an actual Scene object with entities on it.
			// You can write your own map builder, or use the default one.
			// Default map builder can also be expanded.

			_testMap = ResourceHub.GetResource<TiledMap>("Maps", "Test");

			_builder = new SolidMapBuilder(_testMap);
			_builder.Build();
		}

		public override void Update()
		{
			base.Update();

			if (Input.CheckButtonPress(BuildCustomMapBuilderButton))
			{
				if (_builder != null)
				{
					_builder.Destroy();
					_builder = null;
				}
				_builder = new SolidMapBuilder(_testMap);
				_builder.Build();
			}

			if (Input.CheckButtonPress(BuildDefaultMapBuilderButton))
			{
				if (_builder != null)
				{
					_builder.Destroy();
					_builder = null;
				}
				_builder = new MapBuilder(_testMap);
				_builder.Build();
			}


			if (_builder != null && Input.CheckButtonPress(DestroyMapButton))
			{
				_builder.Destroy();
				_builder = null;
			}
		}


		public override void Destroy()
		{
			base.Destroy();

			if (_builder != null)
			{
				_builder.Destroy();
			}
		}
	}
}
