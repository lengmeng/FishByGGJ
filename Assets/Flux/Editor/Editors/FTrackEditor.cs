using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEditorInternal;
using System;
using System.Collections.Generic;
using System.Reflection;
using Flux;

namespace FluxEditor
{
	public class FTrackEditor : FEditor
	{
		public const int DEFAULT_TRACK_HEIGHT = 25;

		public const int KEYFRAME_WIDTH = 4;
		public const int KEYFRAME_HALF_WIDTH = KEYFRAME_WIDTH / 2;
		
		public FTrack Track { get { return (FTrack)Obj; } }
		
		public List<FEventEditor> _eventEditors = new List<FEventEditor>();
		
//		private GUIStyle _eventStyle = null;
//		private GUIStyle _eventLeftStyle = null;
//		private GUIStyle _eventRightStyle = null;
//		private GUIStyle _eventOutsideStyle = null;

		public FTimelineEditor TimelineEditor { get { return (FTimelineEditor)Owner; } }

		private Texture2D _previewIcon = null;

		private float _headerWidth = 0;

		protected override void OnEnable()
		{
			base.OnEnable ();

			_previewIcon = (Texture2D)AssetDatabase.LoadAssetAtPath(FUtility.GetFluxSkinPath()+"View.png", typeof(Texture2D));
		}

		public override void Init( FObject obj, FEditor owner )
		{
			bool initForFirstTime = Track == null;

			base.Init( obj, owner );
			
			_eventEditors.Clear();
			
			List<FEvent> events = Track.Events;
			
			for( int i = 0; i < events.Count; ++i )
			{
				FEvent evt = events[i];
				FEventEditor evtEditor = SequenceEditor.GetEditor<FEventEditor>( evt );
				evtEditor.Init( evt, this );
				_eventEditors.Add( evtEditor );
			}
			
			if( initForFirstTime && Track.PreviewDirtiesScene )
			{
				Track.CanPreview = false;
			}
		}


		public virtual void OnStop()
		{
		}

//		public override void RefreshRuntimeObject()
//		{
//			Track = (FTrack)EditorUtility.InstanceIDToObject(Track.GetInstanceID());
//		}

		public override float Height { get { return DEFAULT_TRACK_HEIGHT; } }

		public override void ReserveGuiIds()
		{
			base.ReserveGuiIds();
			for( int i = 0; i != _eventEditors.Count; ++i )
			{
				_eventEditors[i].ReserveGuiIds();
			}
		}

		public void SelectEvents( FrameRange range )
		{
			for( int i = 0; i != _eventEditors.Count; ++i )
			{
				if( range.Overlaps( _eventEditors[i].Evt.FrameRange ) )
					SequenceEditor.Select( _eventEditors[i] );
				else if( _eventEditors[i].Evt.Start > range.End )
					break;
			}
		}

		public void DeselectEvents( FrameRange range )
		{
			for( int i = 0; i != _eventEditors.Count; ++i )
			{
				if( range.Overlaps( _eventEditors[i].Evt.FrameRange ) )
					SequenceEditor.Deselect( _eventEditors[i] );
				else if( _eventEditors[i].Evt.Start > range.End )
					break;
			}
		}

		public override void Render( Rect rect, float headerWidth )
		{
			Rect = rect;

			_headerWidth = headerWidth;

			Rect headerRect = rect;
			headerRect.width = headerWidth;

			Rect viewRect = rect;
			viewRect.xMax = rect.xMin+headerWidth;
			viewRect.xMin = viewRect.xMax-16;
			viewRect.height = 16;

			if( Track.CanTogglePreview )
			{
				if( Event.current.type == EventType.MouseDown )
				{
					if( viewRect.Contains( Event.current.mousePosition ) )
					{
						if( Event.current.button == 0 ) // left click?
						{
							if( Event.current.shift ) // turn all?
							{
								SequenceEditor.TurnOnAllPreviews( !Track.CanPreview );
							}
							else
							{
								OnTogglePreview( !Track.CanPreview );
							}
							FUtility.RepaintGameView();
							Event.current.Use();
						}
					}
				}
			}

			Rect trackHeaderRect = rect;
			trackHeaderRect.xMax = viewRect.xMax;//headerWidth;

			Color guiColor = GUI.color;

			bool selected = _isSelected;
			
			if( selected )
			{
				Color c = FGUI.GetSelectionColor();
				c.a = GUI.color.a;
				GUI.color = c;
				GUI.DrawTexture( trackHeaderRect, EditorGUIUtility.whiteTexture );
				GUI.color = guiColor;
			}

			if( !Track.enabled )
			{
				Color c = guiColor;
				c.a = 0.5f;
				GUI.color = c;
			}

			Rect trackLabelRect = trackHeaderRect;
			trackLabelRect.xMin += 8;

			GUI.Label( trackLabelRect, new GUIContent(Track.name), FGUI.GetTrackHeaderStyle() );

			rect.xMin = trackHeaderRect.xMax;

			if( rect.Contains( Event.current.mousePosition ) )
				SequenceEditor.SetMouseHover( Event.current.mousePosition.x - rect.xMin, this );

			FrameRange validKeyframeRange = new FrameRange(0, SequenceEditor.Sequence.Length);

			for( int i = 0; i != _eventEditors.Count; ++i )
			{
				if( i == 0 )
					validKeyframeRange.Start = 0;
				else
					validKeyframeRange.Start = _eventEditors[i-1].Evt.End;

				if( i == _eventEditors.Count-1 )
					validKeyframeRange.End = SequenceEditor.Sequence.Length;
				else
					validKeyframeRange.End = _eventEditors[i+1].Evt.Start;
				_eventEditors[i].Render( rect, SequenceEditor.ViewRange, SequenceEditor.PixelsPerFrame, validKeyframeRange );
			}

			switch( Event.current.type )
			{
			case EventType.ContextClick:
				if( trackHeaderRect.Contains(Event.current.mousePosition) )
				{
					OnHeaderContextClick();
				}
				else if( Rect.Contains( Event.current.mousePosition ) )
				{
					OnBodyContextClick();
				}
				break;
			case EventType.MouseDown:
				if( EditorGUIUtility.hotControl == 0 && trackHeaderRect.Contains(Event.current.mousePosition) )
				{
					if( Event.current.button == 0 ) // selecting
					{
						if( Event.current.control )
						{
							if( IsSelected )
								SequenceEditor.Deselect( this );
							else
								SequenceEditor.Select( this );
						}
						else if( Event.current.shift )
						{
							SequenceEditor.Select( this );
						}
						else
						{
							SequenceEditor.SelectExclusive( this );
						}
						Event.current.Use();
					}
				}
				break;
			case EventType.MouseUp:
//				if( EditorGUIUtility.hotControl == GuiId )
//				{
//					EditorGUIUtility.hotControl = 0;
//					_offsetAnim.value = _offsetAnim.target = Vector2.zero;
//
////					if( TimelineEditor ) TimelineEditor.StopTrackDrag();
//
//					SequenceEditor.Repaint();
//					Event.current.Use();
//				}
				break;

			case EventType.MouseDrag:
//				if( EditorGUIUtility.hotControl == GuiId )
//				{
//					SequenceEditor.Repaint();
//					Event.current.Use();
//				}
				break;
			}

			if( Track.CanTogglePreview )
			{
				GUI.color = GetPreviewIconColor();//FGUI.GetTextColor();
				
				if( !Track.CanPreview )
				{
					Color c = GUI.color;
					c.a = 0.3f;
					GUI.color = c;
				}
				
				GUI.DrawTexture( viewRect, _previewIcon );
				
				GUI.color = Color.white;
			}

			Handles.color = FGUI.GetLineColor();
			Handles.DrawLine( Rect.min, Rect.min + new Vector2(Rect.width, 0) );
			Handles.DrawLine( Rect.max, Rect.max - new Vector2(Rect.width, 0) );

			GUI.color = guiColor;
		}

		public virtual void OnTogglePreview( bool on )
		{
			Track.CanPreview = on;
			if( Track.RequiresEditorCache )
			{
				if( Track.CanPreview )
				{
					Track.CreateCache();
					Track.CanPreview = Track.HasCache;
				}
				else
					Track.ClearCache();
			}
		}

		public override void OnDelete()
		{
			base.OnDelete();

			if( Track.HasCache )
				Track.ClearCache();
		}

		protected virtual Color GetPreviewIconColor()
		{
			return FGUI.GetIconColor();
		}

		public virtual void OnToolsGUI()
		{
		}

		public virtual bool HasTools() { return false; }

		protected virtual void OnHeaderContextClick()
		{
			GenericMenu menu = new GenericMenu();
			menu.AddItem( new GUIContent( "Duplicate" ), false, Duplicate );
			menu.AddItem( new GUIContent( "Delete" ), false, Delete );
			menu.ShowAsContext();

			Event.current.Use();
		}

		protected virtual void OnBodyContextClick()
		{
			int t = SequenceEditor.GetFrameForX( Event.current.mousePosition.x-_headerWidth );

			GUIContent selectAllEvents = new GUIContent("Select All Events");
			GUIContent addEventAtFrame = new GUIContent("Add Event At Frame");
			GUIContent addEventFillGap = new GUIContent("Add Event Fill Gap");

			GenericMenu menu = new GenericMenu();
			menu.AddItem( selectAllEvents, false, SelectAllEvents );
			if( Track.CanAddAt( t ) )
			{
				menu.AddItem( addEventAtFrame, false, AddEventAtPoint, t );
				menu.AddItem( addEventFillGap, false, AddEventFillGap, t );
			}
			else
			{
				menu.AddDisabledItem( addEventAtFrame );
				menu.AddDisabledItem( addEventFillGap );
			}

			menu.ShowAsContext();
			Event.current.Use();
		}

		private void Duplicate()
		{
			Undo.RecordObjects( new UnityEngine.Object[]{ TimelineEditor, Track.Timeline }, string.Empty );
			GameObject duplicateTrack = (GameObject)Instantiate( Track.gameObject );
			duplicateTrack.name = Track.gameObject.name;
			Undo.SetTransformParent( duplicateTrack.transform, Track.Timeline.transform, string.Empty );
			Undo.RegisterCreatedObjectUndo( duplicateTrack, "duplicate Track" );

			if( !SequenceEditor.Sequence.IsStopped )
				duplicateTrack.GetComponent<FTrack>().Init();
		}

		private void Delete()
		{
			SequenceEditor.DestroyEditor( this );
		}

		private void SelectAllEvents()
		{
			SelectEvents( new FrameRange(0, Track.Sequence.Length) );
		}

		private void AddEventAtPoint( object userData )
		{
			int t = (int)userData;

			TryAddEvent( t );
		}

		private void AddEventFillGap( object userData )
		{
			int t = (int)userData;

			int newT = -1;

			List<FEvent> evts = Track.Events;
			for( int i = 0; i != evts.Count; ++i )
			{
				if( evts[i].FrameRange.ContainsExclusive( t ) )
					return; // can't add
				if( evts[i].FrameRange.Start >= t )
				{
					newT = i == 0 ? 0 : evts[i-1].End;
					break;
				}
			}

			if( newT == -1 )
			{
				newT = evts.Count == 0 ? 0 : evts[evts.Count-1].End;
			}

			TryAddEvent( newT );
		}

		public override FSequenceEditor SequenceEditor { get { return TimelineEditor.SequenceEditor; } }

		private Type[] _fcEventTypes = null;
		
		public void ShowTrackMenu( FTrack track )
		{
			if( _fcEventTypes == null )
			{
				_fcEventTypes = new Type[0];
				Type[] allTypes = typeof(FEvent).Assembly.GetTypes();
				
				foreach( Type type in allTypes )
				{
					if( type.IsSubclassOf( typeof(FEvent) ) && !type.IsAbstract )
					{
						object[] attributes = type.GetCustomAttributes( typeof(FEventAttribute), false );
						if( attributes.Length == 1 )
						{
							ArrayUtility.Add<Type>( ref _fcEventTypes, type );
						}
					}
				}
			}
			
			GenericMenu menu = new GenericMenu();
			foreach( Type t in _fcEventTypes )
			{
				TimelineMenuData param = new TimelineMenuData();
				param.track = track; param.evtType = t;
				object[] attributes = t.GetCustomAttributes(typeof(FEventAttribute), false);
				menu.AddItem( new GUIContent( ((FEventAttribute)attributes[0]).menu ), false, AddEventToTrack, param );
			}
			menu.ShowAsContext();
		}

		private struct TimelineMenuData
		{
			public FTrack track;
			public Type evtType;
		}
		
		private void AddEventToTrack( object obj )
		{
			TimelineMenuData menuData = (TimelineMenuData)obj;
			GameObject go = new GameObject(menuData.evtType.ToString());
			FEvent evt = (FEvent)go.AddComponent(menuData.evtType);
			menuData.track.Add( evt );
			
			SequenceEditor.Refresh();
		}

		public FrameRange GetValidRange( FEventEditor evtEditor )
		{
			int index = 0;
			for( ; index < _eventEditors.Count; ++index )
			{
				if( _eventEditors[index] == evtEditor )
				{
					break;
				}
			}

			FrameRange range = new FrameRange( 0, SequenceEditor.Sequence.Length );

			if( index > 0 )
			{
				range.Start = _eventEditors[index-1].Evt.End+1;
			}
			if( index < _eventEditors.Count-1 )
			{
				range.End = _eventEditors[index+1].Evt.Start-1;
			}

			return range;
		}

		public FEvent TryAddEvent( int t )
		{
			FEvent newEvt = null;
			if( Track.CanAddAt( t ) )
			{
				FEvent evtAfterT = Track.GetEventAfter( t );
				int newEventEndT;
				if( evtAfterT == null )
					newEventEndT = SequenceEditor.Sequence.Length;
				else
					newEventEndT = evtAfterT.Start;

				newEvt = FEvent.Create( Track.GetEventType(), new FrameRange( t, newEventEndT ) );

				Undo.RecordObject( Track, string.Empty );
				Undo.RegisterCreatedObjectUndo( newEvt.gameObject, "create Event" );

				Track.Add( newEvt );
			}
			return newEvt;
		}

		public virtual void OnTrackChanged()
		{
		}

		public virtual void UpdateEventsEditor( int frame, float time )
		{
//			Track.UpdateEventsEditor( frame, time );
		}
	}
}