﻿using UnityEngine;
using FairyGUI.Utils;
using DG.Tweening;

namespace FairyGUI
{
	public class GObject : EventDispatcher
	{
		/// <summary>
		/// GObject的id，仅作为内部使用。与name不同，id值是不会相同的。
		/// id is for internal use only.
		/// </summary>
		public string id { get; internal set; }

		/// <summary>
		/// Name of the object.
		/// </summary>
		public string name;

		/// <summary>
		/// User defined data. 
		/// </summary>
		public object data;

		/// <summary>
		/// The source width of the object.
		/// </summary>
		public int sourceWidth { get; internal set; }

		/// <summary>
		/// The source height of the object.
		/// </summary>
		public int sourceHeight { get; internal set; }

		/// <summary>
		/// The initial width of the object.
		/// </summary>
		public int initWidth { get; internal set; }

		/// <summary>
		/// The initial height of the object.
		/// </summary>
		public int initHeight { get; internal set; }

		/// <summary>
		/// Relations Object.
		/// </summary>
		public Relations relations { get; private set; }

		/// <summary>
		/// Group belonging to.
		/// </summary>
		public GGroup group;

		/// <summary>
		/// Restricted range of dragging.
		/// </summary>
		public Rect? dragBounds;

		/// <summary>
		/// Parent object.
		/// </summary>
		public GComponent parent { get; internal set; }

		/// <summary>
		/// Lowlevel display object.
		/// </summary>
		public DisplayObject displayObject { get; protected set; }

		/// <summary>
		/// Gear to display controller.
		/// </summary>
		public GearDisplay gearDisplay { get; private set; }

		/// <summary>
		/// Gear to xy controller.
		/// </summary>
		public GearXY gearXY { get; private set; }

		/// <summary>
		/// Gear to size controller.
		/// </summary>
		public GearSize gearSize { get; private set; }

		/// <summary>
		/// Gear to look controller.
		/// </summary>
		public GearLook gearLook { get; private set; }

		/// <summary>
		/// Dispatched when the object or its child was clicked.
		/// </summary>
		public EventListener onClick { get; private set; }

		/// <summary>
		/// Dispatched when the object or its child was clicked by right mouse button. Web only.
		/// </summary>
		public EventListener onRightClick { get; private set; }

		/// <summary>
		/// Dispatched when the finger touched the object or its child just now.
		/// </summary>
		public EventListener onTouchBegin { get; private set; }

		/// <summary>
		/// Dispatched when the finger was lifted from the screen or from the mouse button. 
		/// </summary>
		public EventListener onTouchEnd { get; private set; }

		/// <summary>
		/// Only available for mouse input: the cursor hovers over an object.
		/// </summary>
		public EventListener onRollOver { get; private set; }

		/// <summary>
		/// Only available for mouse input: the cursor leave an object.
		/// </summary>
		public EventListener onRollOut { get; private set; }

		/// <summary>
		/// Dispatched when the object was added to the stage.
		/// </summary>
		public EventListener onAddedToStage { get; private set; }

		/// <summary>
		/// Dispatched when the object was removed from the stage.
		/// </summary>
		public EventListener onRemovedFromStage { get; private set; }

		/// <summary>
		/// Dispatched on key pressed when the object is in focus.
		/// </summary>
		public EventListener onKeyDown { get; private set; }

		/// <summary>
		/// Dispatched when links in the object or its child was clicked.
		/// </summary>
		public EventListener onClickLink { get; private set; }

		/// <summary>
		/// Dispatched when the object was moved.
		/// </summary>
		public EventListener onPositionChanged { get; private set; }

		/// <summary>
		/// Dispatched when the object was resized.
		/// </summary>
		public EventListener onSizeChanged { get; private set; }

		/// <summary>
		/// Dispatched when drag start. 
		/// </summary>
		public EventListener onDragStart { get; private set; }

		/// <summary>
		/// Dispatched when drag end.
		/// </summary>
		public EventListener onDragEnd { get; private set; }

		float _x;
		float _y;
		float _z;
		float _width;
		float _height;
		float _pivotX;
		float _pivotY;
		bool _pivotAsAnchor;
		float _alpha;
		float _rotation;
		float _rotationX;
		float _rotationY;
		bool _visible;
		int _internalVisible;
		bool _touchable;
		bool _grayed;
		bool _draggable;
		float _scaleX;
		float _scaleY;
		int _sortingOrder;
		bool _focusable;
		string _tooltips;
		bool _pixelSnapping;

		//Size的实现方式，有两种，0-GObject的w/h等于DisplayObject的w/h。1-GObject的sourceWidth/sourceHeight等于DisplayObject的w/h，剩余部分由scale实现
		protected int _sizeImplType;

		internal PackageItem _packageItem;
		internal protected bool underConstruct;
		internal XML constructingData;
		internal float _rawWidth;
		internal float _rawHeight;
		internal bool _gearLocked;

		internal static uint _gInstanceCounter;

		public GObject()
		{
			_width = 0;
			_height = 0;
			_alpha = 1;
			_visible = true;
			_touchable = true;
			_scaleX = 1;
			_scaleY = 1;
			_internalVisible = 1;
			id = "_n" + _gInstanceCounter++;
			name = string.Empty;

			CreateDisplayObject();

			relations = new Relations(this);

			gearDisplay = new GearDisplay(this);
			gearXY = new GearXY(this);
			gearSize = new GearSize(this);
			gearLook = new GearLook(this);

			onClick = new EventListener(this, "onClick");
			onRightClick = new EventListener(this, "onRightClick");
			onTouchBegin = new EventListener(this, "onTouchBegin");
			onTouchEnd = new EventListener(this, "onTouchEnd");
			onRollOver = new EventListener(this, "onRollOver");
			onRollOut = new EventListener(this, "onRollOut");
			onAddedToStage = new EventListener(this, "onAddedToStage");
			onRemovedFromStage = new EventListener(this, "onRemovedFromStage");
			onKeyDown = new EventListener(this, "onKeyDown");
			onClickLink = new EventListener(this, "onClickLink");

			onPositionChanged = new EventListener(this, "onPositionChanged");
			onSizeChanged = new EventListener(this, "onSizeChanged");
			onDragStart = new EventListener(this, "onDragStart");
			onDragEnd = new EventListener(this, "onDragEnd");
		}

		/// <summary>
		/// The x coordinate of the object relative to the local coordinates of the parent.
		/// </summary>
		public float x
		{
			get { return _x; }
			set
			{
				SetPosition(value, _y, _z);
			}
		}

		/// <summary>
		/// The y coordinate of the object relative to the local coordinates of the parent.
		/// </summary>
		public float y
		{
			get { return _y; }
			set
			{
				SetPosition(_x, value, _z);
			}
		}

		/// <summary>
		/// The z coordinate of the object relative to the local coordinates of the parent.
		/// </summary>
		public float z
		{
			get { return _z; }
			set
			{
				SetPosition(_x, _y, value);
			}
		}

		/// <summary>
		/// The x and y coordinates of the object relative to the local coordinates of the parent.
		/// </summary>
		public Vector2 xy
		{
			get { return new Vector2(_x, _y); }
			set { SetPosition(value.x, value.y, _z); }
		}

		/// <summary>
		/// The x,y,z coordinates of the object relative to the local coordinates of the parent.
		/// </summary>
		public Vector3 position
		{
			get { return new Vector3(_x, _y, _z); }
			set { SetPosition(value.x, value.y, value.z); }
		}

		/// <summary>
		/// change the x and y coordinates of the object relative to the local coordinates of the parent.
		/// </summary>
		/// <param name="xv">x value.</param>
		/// <param name="yv">y value.</param>
		public void SetXY(float xv, float yv)
		{
			SetPosition(xv, yv, _z);
		}

		/// <summary>
		/// change the x,y,z coordinates of the object relative to the local coordinates of the parent.
		/// </summary>
		/// <param name="xv">x value.</param>
		/// <param name="yv">y value.</param>
		/// <param name="zv">z value.</param>
		public void SetPosition(float xv, float yv, float zv)
		{
			if (_x != xv || _y != yv || _z != zv)
			{
				float dx = xv - _x;
				float dy = yv - _y;
				_x = xv;
				_y = yv;
				_z = zv;

				HandlePositionChanged();

				if (this is GGroup)
					((GGroup)this).MoveChildren(dx, dy);

				if (gearXY.controller != null)
					gearXY.UpdateState();

				if (parent != null && !(parent is GList))
				{
					parent.SetBoundsChangedFlag();
					onPositionChanged.Call();
				}
			}
		}

		public bool pixelSnapping
		{
			get { return _pixelSnapping; }
			set
			{
				_pixelSnapping = value;
				HandlePositionChanged();
			}
		}

		/// <summary>
		/// Set the object in middle of the parent or GRoot if the parent is not set.
		/// </summary>
		public void Center()
		{
			Center(false);
		}

		/// <summary>
		/// Set the object in middle of the parent or GRoot if the parent is not set.
		/// </summary>
		/// <param name="restraint">Add relations to maintain the center state.</param>
		public void Center(bool restraint)
		{
			GComponent r;
			if (parent != null)
				r = parent;
			else
				r = this.root;

			this.SetXY((int)((r.width - this.width) / 2), (int)((r.height - this.height) / 2));
			if (restraint)
			{
				this.AddRelation(r, RelationType.Center_Center);
				this.AddRelation(r, RelationType.Middle_Middle);
			}
		}

		/// <summary>
		/// The width of the object in pixels.
		/// </summary>
		public float width
		{
			get
			{
				return _width;
			}
			set
			{
				SetSize(value, _rawHeight);
			}
		}

		/// <summary>
		/// The height of the object in pixels.
		/// </summary>
		public float height
		{
			get
			{
				return _height;
			}
			set
			{
				SetSize(_rawWidth, value);
			}
		}

		/// <summary>
		/// The size of the object in pixels.
		/// </summary>
		public Vector2 size
		{
			get { return new Vector2(width, height); }
			set { SetSize(value.x, value.y); }
		}

		/// <summary>
		/// actualWidth = width * scalex
		/// </summary>
		public float actualWidth
		{
			get { return this.width * _scaleX; }
		}

		/// <summary>
		/// actualHeight = height * scaleY
		/// </summary>
		public float actualHeight
		{
			get { return this.height * _scaleY; }
		}

		/// <summary>
		/// Change size.
		/// </summary>
		/// <param name="wv">Width value.</param>
		/// <param name="hv">Height value.</param>
		public void SetSize(float wv, float hv)
		{
			SetSize(wv, hv, false);
		}

		/// <summary>
		/// Change size.
		/// </summary>
		/// <param name="wv">Width value.</param>
		/// <param name="hv">Height value.</param>
		/// <param name="ignorePivot">If pivot is set, the object's positon will change when its size change. Set ignorePivot=false to keep the position.</param>
		public void SetSize(float wv, float hv, bool ignorePivot)
		{
			if (_rawWidth != wv || _rawHeight != hv)
			{
				_rawWidth = wv;
				_rawHeight = hv;
				if (wv < 0)
					wv = 0;
				if (hv < 0)
					hv = 0;
				float oldWidth = _width;
				float oldHeight = _height;
				_width = wv;
				_height = hv;

				HandleSizeChanged();

				if (_pivotX != 0 || _pivotY != 0)
				{
					if (!_pivotAsAnchor)
					{
						if (!ignorePivot)
							this.SetXY(_x - _pivotX * (_width - oldWidth), _y - _pivotY * (_height - oldHeight));
						else
							this.HandlePositionChanged();
					}
					else
						this.HandlePositionChanged();
				}

				if (gearSize.controller != null)
					gearSize.UpdateState();

				if (parent != null)
				{
					relations.OnOwnerSizeChanged(_width - oldWidth, _height - oldHeight);
					parent.SetBoundsChangedFlag();
				}

				onSizeChanged.Call();
			}
		}

		protected void SetSizeDirectly(float wv, float hv)
		{
			_rawWidth = wv;
			_rawHeight = hv;
			if (wv < 0)
				wv = 0;
			if (hv < 0)
				hv = 0;
			_width = wv;
			_height = hv;
		}

		/// <summary>
		/// The horizontal scale factor. '1' means no scale, cannt be negative.
		/// </summary>
		public float scaleX
		{
			get { return _scaleX; }
			set
			{
				SetScale(value, _scaleY);
			}
		}

		/// <summary>
		/// The vertical scale factor. '1' means no scale, cannt be negative.
		/// </summary>
		public float scaleY
		{
			get { return _scaleY; }
			set
			{
				SetScale(_scaleX, value);
			}
		}

		/// <summary>
		/// The scale factor.
		/// </summary>
		public Vector2 scale
		{
			get { return new Vector2(_scaleX, _scaleY); }
			set { SetScale(value.x, value.y); }
		}

		/// <summary>
		/// Change the scale factor.
		/// </summary>
		/// <param name="wv">The horizontal scale factor.</param>
		/// <param name="hv">The vertical scale factor</param>
		public void SetScale(float wv, float hv)
		{
			if (_scaleX != wv || _scaleY != hv)
			{
				_scaleX = wv;
				_scaleY = hv;
				HandleScaleChanged();

				if (gearSize.controller != null)
					gearSize.UpdateState();
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public Vector2 skew
		{
			get
			{
				if (displayObject != null)
					return displayObject.skew;
				else
					return Vector2.zero;
			}

			set
			{
				if (displayObject != null)
					displayObject.skew = value;
			}
		}

		/// <summary>
		/// The x coordinate of the object's origin in its own coordinate space.
		/// </summary>
		public float pivotX
		{
			get { return _pivotX; }
			set
			{
				SetPivot(value, _pivotY);
			}
		}

		/// <summary>
		/// The y coordinate of the object's origin in its own coordinate space.
		/// </summary>
		public float pivotY
		{
			get { return _pivotY; }
			set
			{
				SetPivot(_pivotX, value);
			}
		}

		/// <summary>
		/// The x and y coordinates of the object's origin in its own coordinate space.
		/// </summary>
		public Vector2 pivot
		{
			get { return new Vector2(_pivotX, _pivotY); }
			set { SetPivot(value.x, value.y); }
		}

		/// <summary>
		/// Change the x and y coordinates of the object's origin in its own coordinate space.
		/// </summary>
		/// <param name="xv">x value in ratio</param>
		/// <param name="yv">y value in ratio</param>
		public void SetPivot(float xv, float yv)
		{
			SetPivot(xv, yv, false);
		}

		/// <summary>
		///  Change the x and y coordinates of the object's origin in its own coordinate space.
		/// </summary>
		/// <param name="xv">x value in ratio</param>
		/// <param name="yv">y value in ratio</param>
		/// <param name="asAnchor">If use the pivot as the anchor position</param>
		public void SetPivot(float xv, float yv, bool asAnchor)
		{
			if (_pivotX != xv || _pivotY != yv || _pivotAsAnchor != asAnchor)
			{
				_pivotX = xv;
				_pivotY = yv;
				_pivotAsAnchor = asAnchor;
				if (displayObject != null)
					displayObject.pivot = new Vector2(_pivotX, _pivotY);
				if (_sizeImplType == 1 || _pivotAsAnchor) //displayObject的轴心参考宽高与GObject的参看宽高不一样的情况下，需要调整displayObject的位置
					HandlePositionChanged();
			}
		}

		/// <summary>
		/// If the object can touch or click. GImage/GTextField is not touchable even it is true.
		/// </summary>
		public bool touchable
		{
			get
			{
				return _touchable;
			}
			set
			{
				_touchable = value;
				if (displayObject != null)
					displayObject.touchable = _touchable;
			}
		}

		/// <summary>
		/// If true, apply a grayed effect on this object.
		/// </summary>
		public bool grayed
		{
			get
			{
				return _grayed;
			}
			set
			{
				if (_grayed != value)
				{
					_grayed = value;
					HandleGrayedChanged();

					if (gearLook.controller != null)
						gearLook.UpdateState();
				}
			}
		}

		/// <summary>
		/// Enabled is shortcut for grayed and !touchable combination.
		/// </summary>
		public bool enabled
		{
			get
			{
				return !_grayed && _touchable;
			}
			set
			{
				this.grayed = !value;
				this.touchable = value;
			}
		}

		/// <summary>
		/// The rotation around the z axis of the object in degrees.
		/// </summary>
		public float rotation
		{
			get
			{
				return _rotation;
			}
			set
			{
				_rotation = value;
				if (displayObject != null)
					displayObject.rotation = _rotation;

				if (gearLook.controller != null)
					gearLook.UpdateState();
			}
		}

		/// <summary>
		/// The rotation around the x axis of the object in degrees.
		/// </summary>
		public float rotationX
		{
			get
			{
				return _rotationX;
			}
			set
			{
				_rotationX = value;
				if (displayObject != null)
					displayObject.rotationX = _rotationX;
			}
		}

		/// <summary>
		/// The rotation around the y axis of the object in degrees.
		/// </summary>
		public float rotationY
		{
			get
			{
				return _rotationY;
			}
			set
			{
				_rotationY = value;
				if (displayObject != null)
					displayObject.rotationY = _rotationY;
			}
		}

		/// <summary>
		/// The opacity of the object. 0 = transparent, 1 = opaque.
		/// </summary>
		public float alpha
		{

			get
			{
				return _alpha;
			}

			set
			{
				_alpha = value;
				UpdateAlpha();
			}
		}

		virtual protected void UpdateAlpha()
		{
			if (displayObject != null)
				displayObject.alpha = _alpha;

			if (gearLook.controller != null)
				gearLook.UpdateState();
		}

		/// <summary>
		/// The visibility of the object. An invisible object will be untouchable.
		/// </summary>
		public bool visible
		{
			get
			{
				return _visible;
			}

			set
			{
				if (_visible != value)
				{
					_visible = value;
					if (displayObject != null)
						displayObject.visible = _visible;
					if (parent != null)
						parent.ChildStateChanged(this);
				}
			}
		}

		internal int internalVisible
		{
			get { return _internalVisible; }
			set
			{
				if (value < 0)
					value = 0;
				bool oldValue = _internalVisible > 0;
				bool newValue = value > 0;
				_internalVisible = value;
				if (oldValue != newValue)
				{
					if (parent != null)
						parent.ChildStateChanged(this);
				}
			}
		}

		internal bool finalVisible
		{
			get
			{
				return _visible && _internalVisible > 0 && (group == null || group.finalVisible);
			}
		}

		/// <summary>
		/// By default(when sortingOrder==0), object added to component is arrange by the added roder. 
		/// The bigger is the sorting order, the object is more in front.
		/// </summary>
		public int sortingOrder
		{
			get { return _sortingOrder; }
			set
			{
				if (value < 0)
					value = 0;
				if (_sortingOrder != value)
				{
					int old = _sortingOrder;
					_sortingOrder = value;
					if (parent != null)
						parent.ChildSortingOrderChanged(this, old, _sortingOrder);
				}
			}
		}

		/// <summary>
		/// If the object can be focused?
		/// </summary>
		public bool focusable
		{
			get { return _focusable; }
			set { _focusable = value; }
		}

		/// <summary>
		/// If the object is focused. Focused object can receive key events.
		/// </summary>
		public bool focused
		{
			get
			{
				return this.root.focus == this;
			}
		}

		/// <summary>
		/// Request focus on this object.
		/// </summary>
		public void RequestFocus()
		{
			GObject p = this;
			while (p != null && !p._focusable)
				p = p.parent;
			if (p != null)
				this.root.focus = p;
		}

		/// <summary>
		/// Tooltips of this object. UIConfig.tooltipsWin must be set first.
		/// </summary>
		public string tooltips
		{
			get { return _tooltips; }
			set
			{
				if (!string.IsNullOrEmpty(_tooltips))
				{
					this.onRollOver.Remove(__rollOver);
					this.onRollOut.Remove(__rollOut);
				}

				_tooltips = value;
				if (!string.IsNullOrEmpty(_tooltips))
				{
					this.onRollOver.Add(__rollOver);
					this.onRollOut.Add(__rollOut);
				}
			}
		}

		virtual public IFilter filter
		{
			get { return displayObject != null ? displayObject.filter : null; }
			set { if (displayObject != null) displayObject.filter = value; }
		}

		virtual public BlendMode blendMode
		{
			get { return displayObject != null ? displayObject.blendMode : BlendMode.None; }
			set { if (displayObject != null) displayObject.blendMode = value; }
		}

		private void __rollOver()
		{
			this.root.ShowTooltips(tooltips);
		}

		private void __rollOut()
		{
			this.root.HideTooltips();
		}

		/// <summary>
		/// If the object has lowlevel displayobject and the displayobject has a container parent?
		/// </summary>
		public bool inContainer
		{
			get
			{
				return displayObject != null && displayObject.parent != null;
			}
		}

		/// <summary>
		/// If the object is on stage.
		/// </summary>
		public bool onStage
		{
			get
			{
				return displayObject != null && displayObject.stage != null;
			}
		}

		/// <summary>
		/// Resource url of this object.
		/// </summary>
		public string resourceURL
		{
			get
			{
				if (_packageItem != null)
					return UIPackage.URL_PREFIX + _packageItem.owner.id + _packageItem.id;
				else
					return null;
			}
		}

		/// <summary>
		/// Mark the fairy batching state is invalid. 
		/// </summary>
		public void InvalidateBatchingState()
		{
			if (displayObject != null)
				displayObject.InvalidateBatchingState();
			else if ((this is GGroup) && parent != null)
				parent.container.InvalidateBatchingState(true);
		}

		virtual public void HandleControllerChanged(Controller c)
		{
			if (gearDisplay.controller == c)
				gearDisplay.Apply();
			if (gearXY.controller == c)
				gearXY.Apply();
			if (gearSize.controller == c)
				gearSize.Apply();
			if (gearLook.controller == c)
				gearLook.Apply();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="target"></param>
		/// <param name="relationType"></param>
		public void AddRelation(GObject target, RelationType relationType)
		{
			AddRelation(target, relationType, false);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="target"></param>
		/// <param name="relationType"></param>
		/// <param name="usePercent"></param>
		public void AddRelation(GObject target, RelationType relationType, bool usePercent)
		{
			relations.Add(target, relationType, usePercent);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="target"></param>
		/// <param name="relationType"></param>
		public void RemoveRelation(GObject target, RelationType relationType)
		{
			relations.Remove(target, relationType);
		}

		/// <summary>
		/// 
		/// </summary>
		public void RemoveFromParent()
		{
			if (parent != null)
				parent.RemoveChild(this);
		}

		/// <summary>
		/// 
		/// </summary>
		public GRoot root
		{
			get
			{
				if (this is GRoot)
					return (GRoot)this;

				GObject p = parent;
				while (p != null)
				{
					if (p is GRoot)
						return (GRoot)p;
					p = p.parent;
				}
				return GRoot.inst;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		virtual public string text
		{
			get { return null; }
			set { /*override in child*/}
		}

		/// <summary>
		/// 
		/// </summary>
		public bool draggable
		{
			get { return _draggable; }
			set
			{
				if (_draggable != value)
				{
					_draggable = value;
					InitDrag();
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public void StartDrag()
		{
			StartDrag(-1);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="touchId"></param>
		public void StartDrag(int touchId)
		{
			if (displayObject.stage == null)
				return;

			DragBegin(touchId);
		}

		/// <summary>
		/// 
		/// </summary>
		public void StopDrag()
		{
			DragEnd();
		}

		/// <summary>
		/// 
		/// </summary>
		public bool dragging
		{
			get { return sDragging == this; }
		}

		/// <summary>
		/// Transforms a point from the local coordinate system to global (Stage) coordinates.
		/// </summary>
		/// <param name="pt"></param>
		/// <returns></returns>
		public Vector2 LocalToGlobal(Vector2 pt)
		{
			return displayObject.LocalToGlobal(pt);
		}

		/// <summary>
		/// Transforms a point from global (Stage) coordinates to the local coordinate system.
		/// </summary>
		/// <param name="pt"></param>
		/// <returns></returns>
		public Vector2 GlobalToLocal(Vector2 pt)
		{
			return displayObject.GlobalToLocal(pt);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="rect"></param>
		/// <returns></returns>
		public Rect LocalToGlobal(Rect rect)
		{
			Rect ret = new Rect();
			Vector2 v = this.LocalToGlobal(new Vector2(rect.xMin, rect.yMin));
			ret.xMin = v.x;
			ret.yMin = v.y;
			v = this.LocalToGlobal(new Vector2(rect.xMax, rect.yMax));
			ret.xMax = v.x;
			ret.yMax = v.y;
			return ret;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="rect"></param>
		/// <returns></returns>
		public Rect GlobalToLocal(Rect rect)
		{
			Rect ret = new Rect();
			Vector2 v = this.GlobalToLocal(new Vector2(rect.xMin, rect.yMin));
			ret.xMin = v.x;
			ret.yMin = v.y;
			v = this.GlobalToLocal(new Vector2(rect.xMax, rect.yMax));
			ret.xMax = v.x;
			ret.yMax = v.y;
			return ret;
		}

		/// <summary>
		/// Transforms a point from the local coordinate system to GRoot coordinates.
		/// </summary>
		/// <param name="pt"></param>
		/// <param name="r"></param>
		/// <returns></returns>
		public Vector2 LocalToRoot(Vector2 pt, GRoot r)
		{
			pt = displayObject.LocalToGlobal(pt);
			if (r == null || r == GRoot.inst)
			{
				//fast
				pt.x /= GRoot.contentScaleFactor;
				pt.y /= GRoot.contentScaleFactor;
			}
			else
				return r.GlobalToLocal(pt);

			return pt;
		}

		/// <summary>
		/// Transforms a point from the GRoot coordinate  to local coordinates system.
		/// </summary>
		/// <param name="pt"></param>
		/// <param name="r"></param>
		/// <returns></returns>
		public Vector2 RootToLocal(Vector2 pt, GRoot r)
		{
			if (r == null || r == GRoot.inst)
			{
				//fast
				pt.x *= GRoot.contentScaleFactor;
				pt.y *= GRoot.contentScaleFactor;
			}
			else
				pt = r.LocalToGlobal(pt);
			return displayObject.GlobalToLocal(pt);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="pt"></param>
		/// <returns></returns>
		public Vector2 WorldToLocal(Vector3 pt)
		{
			return WorldToLocal(pt, Camera.main);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="pt"></param>
		/// <param name="camera"></param>
		/// <returns></returns>
		public Vector2 WorldToLocal(Vector3 pt, Camera camera)
		{
			Vector3 v = camera.WorldToScreenPoint(pt);
			v.y = Screen.height - v.y;
			v.z = 0;
			return displayObject.GlobalToLocal(v);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="point"></param>
		/// <param name="targetSpace"></param>
		/// <returns></returns>
		public Vector2 TransformPoint(Vector2 point, GObject targetSpace)
		{
			return this.displayObject.TransformPoint(point, targetSpace.displayObject);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="rect"></param>
		/// <param name="targetSpace"></param>
		/// <returns></returns>
		public Rect TransformRect(Rect rect, GObject targetSpace)
		{
			return this.displayObject.TransformRect(rect, targetSpace.displayObject);
		}

		virtual public void Dispose()
		{
			RemoveFromParent();
			RemoveEventListeners();
			relations.Dispose();
			if (displayObject != null && !displayObject.isDisposed)
			{
				displayObject.gOwner = null;
				displayObject.Dispose();
			}
		}

		public GImage asImage
		{
			get { return this as GImage; }
		}

		public GComponent asCom
		{
			get { return this as GComponent; }
		}

		public GButton asButton
		{
			get { return this as GButton; }
		}

		public GLabel asLabel
		{
			get { return this as GLabel; }
		}

		public GProgressBar asProgress
		{
			get { return this as GProgressBar; }
		}

		public GSlider asSlider
		{
			get { return this as GSlider; }
		}

		public GComboBox asComboBox
		{
			get { return this as GComboBox; }
		}

		public GTextField asTextField
		{
			get { return this as GTextField; }
		}

		public GRichTextField asRichTextField
		{
			get { return this as GRichTextField; }
		}

		public GTextInput asTextInput
		{
			get { return this as GTextInput; }
		}

		public GLoader asLoader
		{
			get { return this as GLoader; }
		}

		public GList asList
		{
			get { return this as GList; }
		}

		public GGraph asGraph
		{
			get { return this as GGraph; }
		}

		public GGroup asGroup
		{
			get { return this as GGroup; }
		}

		public GMovieClip asMovieClip
		{
			get { return this as GMovieClip; }
		}

		virtual protected void CreateDisplayObject()
		{
		}

		virtual protected void HandlePositionChanged()
		{
			if (displayObject != null)
			{
				float xv = _x;
				float yv = _y;
				if (!_pivotAsAnchor)
				{
					xv += _width * _pivotX;
					yv += _height * _pivotY;
				}
				if (_pixelSnapping)
				{
					xv = (int)xv;
					yv = (int)yv;
				}
				displayObject.location = new Vector3(xv, yv, _z);
			}
		}

		virtual protected void HandleSizeChanged()
		{
			if (displayObject != null)
			{
				if (_sizeImplType == 0 || sourceWidth == 0 || sourceHeight == 0)
					displayObject.SetSize(_width, _height);
				else
					displayObject.SetScale(_scaleX * _width / sourceWidth, _scaleY * _height / sourceHeight);
			}
		}

		virtual protected void HandleScaleChanged()
		{
			if (displayObject != null)
			{
				if (_sizeImplType == 0 || sourceWidth == 0 || sourceHeight == 0)
					displayObject.SetScale(_scaleX, _scaleY);
				else
					displayObject.SetScale(_scaleX * _width / sourceWidth, _scaleY * _height / sourceHeight);
			}
		}

		virtual protected void HandleGrayedChanged()
		{
			if (displayObject != null)
				displayObject.grayed = _grayed;
		}

		virtual public void ConstructFromResource(PackageItem pkgItem)
		{
			_packageItem = pkgItem;
		}

		virtual public void Setup_BeforeAdd(XML xml)
		{
			string str;
			string[] arr;

			id = xml.GetAttribute("id");
			name = xml.GetAttribute("name");

			arr = xml.GetAttributeArray("xy");
			if (arr != null)
				this.SetXY(int.Parse(arr[0]), int.Parse(arr[1]));

			arr = xml.GetAttributeArray("size");
			if (arr != null)
			{
				initWidth = int.Parse(arr[0]);
				initHeight = int.Parse(arr[1]);
				SetSize(initWidth, initHeight, true);
			}

			arr = xml.GetAttributeArray("scale");
			if (arr != null)
				SetScale(float.Parse(arr[0]), float.Parse(arr[1]));

			arr = xml.GetAttributeArray("skew");
			if (arr != null)
				this.skew = new Vector2(int.Parse(arr[0]), int.Parse(arr[1]));

			str = xml.GetAttribute("rotation");
			if (str != null)
				this.rotation = int.Parse(str);

			arr = xml.GetAttributeArray("pivot");
			if (arr != null)
			{
				float f1 = float.Parse(arr[0]);
				float f2 = float.Parse(arr[1]);
				//处理旧版本的兼容性(旧版本发布的值是坐标值，新版本是比例，一般都小于2）
				if (f1 > 2)
				{
					if (sourceWidth != 0)
						f1 = f1 / sourceWidth;
					else
						f1 = 0;
				}
				if (f2 > 2)
				{
					if (sourceHeight != 0)
						f2 = f2 / sourceHeight;
					else
						f2 = 0;
				}
				this.SetPivot(f1, f2, xml.GetAttributeBool("anchor"));
			}
			else
				this.SetPivot(0, 0, false);

			str = xml.GetAttribute("alpha");
			if (str != null)
				this.alpha = float.Parse(str);

			this.touchable = xml.GetAttributeBool("touchable", true);
			this.visible = xml.GetAttributeBool("visible", true);
			this.grayed = xml.GetAttributeBool("grayed", false);

			str = xml.GetAttribute("blend");
			if (str != null)
				this.blendMode = FieldTypes.ParseBlendMode(str);

			str = xml.GetAttribute("filter");
			if (str != null)
			{
				switch (str)
				{
					case "color":
						ColorFilter cf = new ColorFilter();
						this.filter = cf;
						arr = xml.GetAttributeArray("filterData");
						cf.AdjustBrightness(float.Parse(arr[0]));
						cf.AdjustContrast(float.Parse(arr[1]));
						cf.AdjustSaturation(float.Parse(arr[2]));
						cf.AdjustHue(float.Parse(arr[3]));
						break;
				}
			}

			str = xml.GetAttribute("tooltips");
			if (str != null)
				this.tooltips = str;
		}

		virtual public void Setup_AfterAdd(XML xml)
		{
			XML cxml = null;
			string str;

			str = xml.GetAttribute("group");
			if (str != null)
				group = parent.GetChildById(str) as GGroup;

			cxml = xml.GetNode("gearDisplay");
			if (cxml != null)
				gearDisplay.Setup(cxml);

			cxml = xml.GetNode("gearXY");
			if (cxml != null)
				gearXY.Setup(cxml);

			cxml = xml.GetNode("gearSize");
			if (cxml != null)
				gearSize.Setup(cxml);

			cxml = xml.GetNode("gearLook");
			if (cxml != null)
				gearLook.Setup(cxml);
		}

		#region Drag support
		int _dragTouchId;
		Vector2 _dragTouchStartPos;

		static GObject sDragging;
		static Vector2 sGlobalDragStart = new Vector2();
		static Rect sGlobalRect = new Rect();

		private void InitDrag()
		{
			if (_draggable)
				onTouchBegin.Add(__touchBegin);
			else
				onTouchBegin.Remove(__touchBegin);
		}

		private void DragBegin(int touchId)
		{
			if (sDragging != null)
				sDragging.StopDrag();

			_dragTouchId = touchId;
			sGlobalDragStart = Stage.inst.GetTouchPosition(touchId);
			sGlobalRect = this.LocalToGlobal(new Rect(0, 0, this.width, this.height));

			sDragging = this;
			Stage.inst.onTouchEnd.Add(__touchEnd2);
			Stage.inst.onTouchMove.Add(__touchMove2);
		}

		private void DragEnd()
		{
			if (sDragging == this)
			{
				Stage.inst.onTouchEnd.Remove(__touchEnd2);
				Stage.inst.onTouchMove.Remove(__touchMove2);
				sDragging = null;
			}
		}

		private void Reset()
		{
			Stage.inst.onTouchEnd.Remove(__touchEnd);
			Stage.inst.onTouchMove.Remove(__touchMove);
		}

		private void __touchBegin(EventContext context)
		{
			InputEvent evt = context.inputEvent;
			_dragTouchId = evt.touchId;
			_dragTouchStartPos = evt.position;

			Stage.inst.onTouchEnd.Add(__touchEnd);
			Stage.inst.onTouchMove.Add(__touchMove);
		}

		private void __touchEnd(EventContext context)
		{
			if (_dragTouchId != context.inputEvent.touchId)
				return;

			Reset();
		}

		private void __touchMove(EventContext context)
		{
			InputEvent evt = context.inputEvent;
			if (_dragTouchId != evt.touchId)
				return;

			int sensitivity;
			if (Stage.touchScreen)
				sensitivity = UIConfig.touchDragSensitivity;
			else
				sensitivity = UIConfig.clickDragSensitivity;
			if (Mathf.Abs(_dragTouchStartPos.x - evt.x) < sensitivity
				&& Mathf.Abs(_dragTouchStartPos.y - evt.y) < sensitivity)
				return;

			Reset();

			if (!onDragStart.Call(_dragTouchId))
				DragBegin(evt.touchId);
		}

		private void __touchEnd2(EventContext context)
		{
			InputEvent evt = context.inputEvent;
			if (_dragTouchId != -1 && _dragTouchId != evt.touchId)
				return;

			if (sDragging == this)
			{
				StopDrag();
				onDragEnd.Call();
			}
		}

		private void __touchMove2(EventContext context)
		{
			InputEvent evt = context.inputEvent;
			if (_dragTouchId != -1 && _dragTouchId != evt.touchId || this.parent == null)
				return;

			float xx = evt.x - sGlobalDragStart.x + sGlobalRect.x;
			float yy = evt.y - sGlobalDragStart.y + sGlobalRect.y;

			if (dragBounds != null)
			{
				Rect rect = GRoot.inst.LocalToGlobal((Rect)dragBounds);
				if (xx < rect.x)
					xx = rect.x;
				else if (xx + sGlobalRect.width > rect.xMax)
				{
					xx = rect.xMax - sGlobalRect.width;
					if (xx < rect.x)
						xx = rect.x;
				}

				if (yy < rect.y)
					yy = rect.y;
				else if (yy + sGlobalRect.height > rect.yMax)
				{
					yy = rect.yMax - sGlobalRect.height;
					if (yy < rect.y)
						yy = rect.y;
				}
			}

			Vector2 pt = this.parent.GlobalToLocal(new Vector2(xx, yy));
			if (float.IsNaN(pt.x))
				return;

			this.SetXY(Mathf.RoundToInt(pt.x), Mathf.RoundToInt(pt.y));
		}
		#endregion

		#region Tween Support
		public Tweener TweenMove(Vector2 endValue, float duration)
		{
			return DOTween.To(() => this.xy, x => this.xy = x, endValue, duration)
				.SetOptions(_pixelSnapping)
				.SetUpdate(true)
				.SetTarget(this);
		}

		public Tweener TweenMoveX(float endValue, float duration)
		{
			return DOTween.To(() => this.x, x => this.x = x, endValue, duration)
				.SetOptions(_pixelSnapping)
				.SetUpdate(true)
				.SetTarget(this);
		}

		public Tweener TweenMoveY(float endValue, float duration)
		{
			return DOTween.To(() => this.y, x => this.y = x, endValue, duration)
				.SetOptions(_pixelSnapping)
				.SetUpdate(true)
				.SetTarget(this);
		}

		public Tweener TweenScale(Vector2 endValue, float duration)
		{
			return DOTween.To(() => this.scale, x => this.scale = x, endValue, duration)
				.SetUpdate(true)
				.SetTarget(this);
		}

		public Tweener TweenScaleX(float endValue, float duration)
		{
			return DOTween.To(() => this.scaleX, x => this.scaleX = x, endValue, duration)
				.SetUpdate(true)
				.SetTarget(this);
		}

		public Tweener TweenScaleY(float endValue, float duration)
		{
			return DOTween.To(() => this.scaleY, x => this.scaleY = x, endValue, duration)
				.SetUpdate(true)
				.SetTarget(this);
		}

		public Tweener TweenResize(Vector2 endValue, float duration)
		{
			return DOTween.To(() => this.size, x => this.size = x, endValue, duration)
				.SetOptions(_pixelSnapping)
				.SetUpdate(true)
				.SetTarget(this);
		}

		public Tweener TweenFade(float endValue, float duration)
		{
			return DOTween.To(() => this.alpha, x => this.alpha = x, endValue, duration)
				.SetUpdate(true)
				.SetTarget(this);
		}

		public Tweener TweenRotate(float endValue, float duration)
		{
			return DOTween.To(() => this.rotation, x => this.rotation = x, endValue, duration)
				.SetUpdate(true)
				.SetTarget(this);
		}
		#endregion
	}
}
