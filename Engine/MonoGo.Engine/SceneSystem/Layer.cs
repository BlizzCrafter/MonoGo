﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGo.Engine.Cameras;
using MonoGo.Engine.Drawing;
using MonoGo.Engine.EC;
using MonoGo.Engine.Utils;
using MonoGo.Engine.Utils.CustomCollections;

namespace MonoGo.Engine.SceneSystem
{
	public delegate void LayerEventDelegate(Layer layer);

	/// <summary>
	/// A layer is a container for entities and components.
	/// </summary>
	public class Layer : IEntityMethods
	{
		/// <summary>
		/// Layer's parent scene.
		/// </summary>
		public readonly Scene Scene;

		/// <summary>
		/// Layer's name. Used for searching.
		/// NOTE: All layers should have unique names!
		/// </summary>
		public readonly string Name;

		/// <summary>
		/// If false, layer won't be rendered.
		/// </summary>
		public bool Visible = true;

		/// <summary>
		/// If false, layer won't be updated.
		/// </summary>
		public bool Enabled = true;


		internal bool _depthListOutdated = false;


		/// <summary>
		/// Priority of a layer. Layers with highest priority are processed first.
		/// </summary>
		public int Priority
		{
			get => _priority;

			set
			{
				_priority = value;
				Scene._layers.Remove(this); // Re-adding element to update its priority.
				Scene._layers.Add(this);
			}
		}
		private int _priority;


		/// <summary>
		/// If true, entities and components will be sorted by their depth.
		/// </summary>
		public bool DepthSorting
		{
			get => _depthSorting;
			set
			{
				_depthSorting = value;
				if (value)
				{
					_depthSortedEntities = new SafeList<Entity>();
					_depthListOutdated = true;
				}
				else
				{
					// Linking "sorted" lists directly to primary lists.
					_depthSortedEntities = _entities;
				}
			}
		}
		private bool _depthSorting;


		/// <summary>
		/// If true, draws everything directly to the backbuffer instead of cameras.
		/// </summary>
		public bool IsGUI = false;


		/// <summary>
		/// List of all layer's entities.
		/// </summary>
		public List<Entity> Entities => _entities.ToList();

		private SafeList<Entity> _entities = new SafeList<Entity>();
		private SafeList<Entity> _depthSortedEntities;
		private EntityDepthComparer _depthComparer = new EntityDepthComparer();
		
		/// <summary>
		/// Shaders applied to the layer.
		/// NOTE: You should enable postprocessing in camera.
		/// NOTE: Shaders won't be applied, if layer is GUI.
		/// </summary>
		public List<Effect> PostprocessorEffects {get; private set;} = new List<Effect>();

		internal Layer(string name, int priority, Scene scene)
		{
			Name = name;
			Scene = scene;
			Priority = priority; // Also adds layer to priority list.
			
			DepthSorting = false;
		}



		#region Entity methods.

		/// <summary>
		/// Returns list of entities of certain type.
		/// </summary>
		public List<T> GetEntityList<T>() where T : Entity
		{
			var entities = new List<T>();
			for (var i = 0; i < _entities.Count; i += 1)
			{
				if (_entities[i] is T)
				{
					entities.Add((T)_entities[i]);
				}
			}
			return entities;
		}
		
		/// <summary>
		/// Checks if any instances of an entity exist.
		/// </summary>
		public bool EntityExists<T>() where T : Entity
		{
			for (var i = 0; i < _entities.Count; i += 1)
			{
				if (_entities[i] is T)
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Finds first entity of given type.
		/// </summary>
		public T FindEntity<T>() where T : Entity
		{
			for (var i = 0; i < _entities.Count; i += 1)
			{
				if (_entities[i] is T)
				{
					return (T)_entities[i];
				}
			}
			return null;
		}
		
		/// <summary>
		/// Returns list of entities, which have component - enabled or disabled -  of given type.
		/// </summary>
		public List<Entity> GetEntityListByComponent<T>() where T : Component
		{
			var components = GetComponentList<T>();

			var entityArray = new Entity[components.Count];

			for(var i = 0; i < components.Count; i += 1)
			{
				entityArray[i] = components[i].Owner;
			}

			return new List<Entity>(entityArray);
		}
		
		
		/// <summary>
		/// Finds first entity, which has component of given type.
		/// </summary>
		public Entity FindEntityByComponent<T>() where T : Component
		{
			for (var i = 0; i < _entities.Count; i += 1)
			{
				if (_entities[i].HasComponent<T>())
				{
					return _entities[i];
				}
			}
			return null;
		}


		/// <summary>
		/// Returns list of all components on the layer - enabled and disabled - of given type.
		/// </summary>
		public List<Component> GetComponentList<T>() where T : Component
		{
			var components = new List<Component>();

			for (var i = 0; i < _entities.Count; i += 1)
			{
				if (_entities[i].TryGetComponent<T>(out var component))
				{
					components.Add(component);
				}
			}
			return components;
		}

		#endregion Entity methods.



		#region Ordering.

		/// <summary>
		/// Changes the update order of an entity and places it 
		/// at the specified position of an entity list.
		/// </summary>
		public void ReorderEntity(Entity entity, int index)
		{
			if (!_entities.Contains(entity))
			{
				throw new Exception("Cannot reorder entity - it doesn't belong to this layer.");
			}
			_entities.Remove(entity);
			_entities.Insert(index, entity);
		}


		/// <summary>
		/// Changes the update order of an entity and places it 
		/// at the top of an entity list.
		/// </summary>
		public void ReorderEntityToTop(Entity entity) =>
			ReorderEntity(entity, 0);


		/// <summary>
		/// Changes the update order of an entity and places it 
		/// at the bottom of an entity list.
		/// </summary>
		public void ReorderEntityToBottom(Entity entity)
		{
			if (!_entities.Contains(entity))
			{
				throw new Exception("Cannot reorder entity - it doesn't belong to this layer.");
			}
			_entities.Remove(entity);
			_entities.Add(entity);
		}

		#endregion Ordering.



		#region Events.
		
		/// <summary>
		/// Triggers every frame before all entities perform Update.
		/// </summary>
		public event LayerEventDelegate OnPreUpdate;
		/// <summary>
		/// Triggers every frame after all entities perform Update.
		/// </summary>
		public event LayerEventDelegate OnPostUpdate;
		/// <summary>
		/// Triggers every frame before all entities perform FixedUpdate.
		/// </summary>
		public event LayerEventDelegate OnPreFixedUpdate;
		/// <summary>
		/// Triggers every frame after all entities perform FixedUpdate.
		/// </summary>
		public event LayerEventDelegate OnPostFixedUpdate;
		/// <summary>
		/// Triggers every frame before all entities perform Draw.
		/// </summary>
		public event LayerEventDelegate OnPreDraw;
		/// <summary>
		/// Triggers every frame after all entities perform Draw.
		/// </summary>
		public event LayerEventDelegate OnPostDraw;
		
		internal void FixedUpdate()
		{
			OnPreFixedUpdate?.Invoke(this);
			foreach (var entity in _entities)
			{
				if (entity.Enabled && !entity.Destroyed)
				{
					entity.FixedUpdate();
				}
			}
		  OnPostFixedUpdate?.Invoke(this);
		}

		internal void Update()
		{
			OnPreUpdate?.Invoke(this);
			foreach (var entity in _entities)
			{
				if (entity.Enabled && !entity.Destroyed)
				{
					entity.Update();
				}
			}
			OnPostUpdate?.Invoke(this);
		}

		internal void Draw()
		{
			bool hasPostprocessing = (
				GraphicsMgr.CurrentCamera.PostprocessingMode == PostprocessingMode.CameraAndLayers
				&& PostprocessorEffects.Count > 0
			);

			if (hasPostprocessing)
			{
				Surface.SetTarget(GraphicsMgr.CurrentCamera._postprocessorLayerBuffer, GraphicsMgr.VertexBatch.View);
				GraphicsMgr.Device.Clear(Color.Transparent);
			}

			
			OnPreDraw?.Invoke(this);
			foreach (var entity in _depthSortedEntities)
			{
				if (entity.Visible && !entity.Destroyed)
				{
					entity.Draw();
				}
			}
			OnPostDraw?.Invoke(this);

			if (hasPostprocessing)
			{
				Surface.ResetTarget();

				var oldRasterizer = GraphicsMgr.VertexBatch.RasterizerState;
				GraphicsMgr.VertexBatch.RasterizerState = GraphicsMgr._cameraRasterizerState;
				GraphicsMgr.VertexBatch.PushViewMatrix(Matrix.CreateTranslation(Vector3.Zero));
				ApplyPostprocessing();
				GraphicsMgr.VertexBatch.PopViewMatrix();
				GraphicsMgr.VertexBatch.RasterizerState = oldRasterizer;
			}
		}


		internal void DrawGUI()
		{
			// There is no need for separate DrawGUI events because layer can execute either Draw or DrawGUI once per frame.
			OnPreDraw?.Invoke(this);
			foreach (var entity in _depthSortedEntities)
			{
				if (entity.Visible && !entity.Destroyed)
				{
					entity.Draw();
				}
			}
			OnPostDraw?.Invoke(this);
		}

		#endregion Events.



		/// <summary>
		/// Sorts entites and components by depth, if depth sorting is enabled.
		/// </summary>
		internal void SortByDepth()
		{
			if (DepthSorting)
			{
				if (_depthListOutdated)
				{
					_depthSortedEntities = new SafeList<Entity>(_entities.ToList());
					_depthSortedEntities.Sort(_depthComparer); 
					_depthListOutdated = false;
				}
			}
			else
			{
				_depthSortedEntities = _entities;
			}
		}


		internal void AddEntity(Entity entity)
		{
			_entities.Add(entity);
			_depthListOutdated = true;
		}

		internal void RemoveEntity(Entity entity) =>
			_entities.Remove(entity);


		internal void UpdateEntityList()
		{
			// Clearing main list from destroyed objects.
			foreach (var entity in _entities)
			{
				if (entity.Destroyed)
				{
					_entities.Remove(entity);
				}
			}
			// Clearing main list from destroyed objects.

		}


		/// <summary>
		/// Applies shaders to the camera surface.
		/// </summary>
		private void ApplyPostprocessing()
		{
			var camera = GraphicsMgr.CurrentCamera;
			var oldEffect = GraphicsMgr.VertexBatch.Effect;
			var sufraceChooser = false;

			for (var i = 0; i < PostprocessorEffects.Count - 1; i += 1)
			{
				
				GraphicsMgr.VertexBatch.Effect = PostprocessorEffects[i];
				if (sufraceChooser)
				{
					Surface.SetTarget(camera._postprocessorLayerBuffer);
					GraphicsMgr.Device.Clear(Color.Transparent);
					camera._postprocessorBuffer.Draw(Vector2.Zero, Vector2.Zero, Vector2.One, Angle.Right, Color.White);
				}
				else
				{
					Surface.SetTarget(camera._postprocessorBuffer);
					GraphicsMgr.Device.Clear(Color.Transparent);
					camera._postprocessorLayerBuffer.Draw(Vector2.Zero, Vector2.Zero, Vector2.One, Angle.Right, Color.White);
				}

				Surface.ResetTarget();
				sufraceChooser = !sufraceChooser;
			}


			GraphicsMgr.VertexBatch.Effect = PostprocessorEffects[PostprocessorEffects.Count - 1];
			if ((PostprocessorEffects.Count % 2) != 0)
			{
				camera._postprocessorLayerBuffer.Draw(Vector2.Zero, Vector2.Zero, Vector2.One, Angle.Right, Color.White);
			}
			else
			{
				camera._postprocessorBuffer.Draw(Vector2.Zero, Vector2.Zero, Vector2.One, Angle.Right, Color.White);
			}

			GraphicsMgr.VertexBatch.Effect = oldEffect;
		}



	}
}
