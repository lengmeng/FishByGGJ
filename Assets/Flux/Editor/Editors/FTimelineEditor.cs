using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEditorInternal;

using System;
using System.Collections.Generic;

using Flux;

namespace FluxEditor
{
	public class FTimelineEditor : FEditorList<FTrackEditor>
	{
		public const int HEADER_HEIGHT = 25;
		public const int TRACK_HEIGHT = 25;

		public FContainerEditor ContainerEditor { get { return (FContainerEditor)Owner; } }

//		protected FSequenceEditor _sequenceEditor;

		public FTimeline Timeline { get { return (FTimeline)Obj; } }

		public bool _showHeader = true;
		public bool _showTracks = true;

//		public List<FTrackEditor> _trackEditors = new List<FTrackEditor>();

//		[SerializeField]
//		private int[] _trackEditorIds = new int[0];

//		[SerializeField]
//		private AnimVector3 _offsetAnim = new AnimVector3();

//		private FTrackEditor _trackDragged = null;

		public override void Init( FObject obj, FEditor owner )
		{
			base.Init( obj, owner );

			if( Timeline.Owner == null )
				Timeline.Awake();

			Editors.Clear();

			List<FTrack> tracks = Timeline.Tracks;

			for( int i = 0; i < tracks.Count; ++i )
			{
				FTrack track = tracks[i];
				FTrackEditor trackEditor = ContainerEditor.SequenceEditor.GetEditor<FTrackEditor>(track);
				trackEditor.Init( track, this );
				Editors.Add( trackEditor );
			}

			_icon = new GUIContent(FUtility.GetFluxTexture( "Plus.png" ));

//			_offsetAnim.valueChanged.AddListener( ContainerEditor.SequenceEditor.Repaint );
		}

		public void OnStop()
		{
			for( int i = 0; i != Editors.Count; ++i )
				Editors[i].OnStop();
		}

//		public override void RefreshRuntimeObject()
//		{
//			Timeline = (FTimeline)EditorUtility.InstanceIDToObject(Timeline.GetInstanceID());
//		}

		public override float Height {
			get {
				float headerHeight = _showHeader ? HEADER_HEIGHT : 0;
				float tracksHeight = ShowPercentage * Editors.Count * TRACK_HEIGHT;
				return headerHeight + tracksHeight;
			}
		}

		public override float HeaderHeight {
			get {
				return HEADER_HEIGHT;
			}
		}

		protected override string HeaderText {
			get {
				return Timeline.Owner != null ? Timeline.Owner.name : Timeline.name + " (Missing)";//Obj.Owner.name;
			}
		}

		protected override bool IconOnLeft {
			get {
				return false;
			}
		}

		public FTrackEditor GetTrackEditor( Vector2 pos )
		{
			if( !_showTracks 
			   || !Rect.Contains( pos ) 
			   || (_showHeader && pos.y < Rect.yMin + HEADER_HEIGHT) ) return null;

			for( int i = 0; i != Editors.Count; ++i )
			{
				if( Editors[i].Rect.Contains( pos ) )
					return Editors[i];
			}

			return null; // shouldn't happen
		}

		protected override Color BackgroundColor {
			get {
				return FGUI.GetTimelineColor();
			}
		}

		protected override void Delete()
		{
			SequenceEditor.DestroyEditor( this );
		}

		protected override void OnHeaderInput (Rect labelRect, Rect iconRect)
		{
			if( Event.current.type == EventType.MouseDown && Event.current.clickCount > 1 && labelRect.Contains( Event.current.mousePosition ) )
			{
				Selection.activeTransform = Timeline.Owner;
				Event.current.Use();
			}
			base.OnHeaderInput(labelRect, iconRect);

			if( Event.current.type == EventType.MouseDown && iconRect.Contains(Event.current.mousePosition) )
			{
				ShowAddTrackMenu();
			}
		}

		private void ShowAddTrackMenu()
		{
			Event.current.Use();

			GenericMenu menu = new GenericMenu();

			System.Reflection.Assembly fluxAssembly = typeof(FEvent).Assembly;

			Type[] types = typeof(FEvent).Assembly.GetTypes();

			if( fluxAssembly.GetName().Name != "Assembly-CSharp" )
			{
				// if we are in the flux trial, basically allow to get the types in the project assembly
				ArrayUtility.AddRange<Type>( ref types, System.Reflection.Assembly.Load("Assembly-CSharp").GetTypes() );
			}

			List<KeyValuePair<Type, FEventAttribute>> validTypeList = new List<KeyValuePair<Type, FEventAttribute>>();
			
			foreach( Type t in types )
			{
				if( !typeof(FEvent).IsAssignableFrom( t ) )
					continue;
				
				object[] attributes = t.GetCustomAttributes(typeof(FEventAttribute), false);
				if( attributes.Length == 0 || ((FEventAttribute)attributes[0]).menu == null )
					continue;
				
				validTypeList.Add( new KeyValuePair<Type, FEventAttribute>(t, (FEventAttribute)attributes[0]) );
			}
			
			validTypeList.Sort( delegate(KeyValuePair<Type, FEventAttribute> x, KeyValuePair<Type, FEventAttribute> y) 
			                   {
				return x.Value.menu.CompareTo( y.Value.menu );
			});
			
			foreach( KeyValuePair<Type, FEventAttribute> kvp in validTypeList )
			{
				menu.AddItem( new GUIContent(kvp.Value.menu), false, AddTrackMenu, kvp );
			}
			
			menu.ShowAsContext();
		}

//		protected override void RenderHeader( Rect headerRect, float headerWidth )
//		{
//			if( Event.current.type == EventType.Repaint )
//			{
//				GUI.color = FGUI.GetTimelineColor();
//				GUI.DrawTexture( headerRect, EditorGUIUtility.whiteTexture );
//				GUI.color = Color.white;
//			}
//
//			Rect foldoutRect = headerRect;
//			foldoutRect.width = 16;
//			headerRect.xMin += 16;
//
//			ShowEditorList = EditorGUI.Foldout( foldoutRect, ShowEditorList, GUIContent.none );
//
//			string timelineHeaderName = Timeline.Owner != null ? Timeline.Owner.name : Timeline.name + " (Missing)";
//
//			headerRect.width = headerWidth - 16;

//			base.RenderHeader( headerRect, headerWidth );
//
//			Rect addTrackRect = headerRect;
//			addTrackRect.xMin = addTrackRect.xMax - 16;
//			addTrackRect.height = 16;
////			addTrackRect.y += headerRect.height * 0.5f - 8;
//			headerRect.xMax -= 16;

//			GUI.Label( headerRect, new GUIContent(timelineHeaderName), FGUI.GetTimelineHeaderStyle() );

//			GUI.color = FGUI.GetIconColor();
//			GUI.DrawTexture( addTrackRect, FUtility.GetFluxTexture("Plus.png") );
//			GUI.color = Color.white;
//		}

		/*public override void Render( Rect rect, float hierarchyWidth, FrameRange viewRange, float pixelsPerFrame )
		{
			if( _timeline == null )
			{
				return;
			}

			rect.y += _offsetAnim.value.y;

			_rect = rect;

			float alpha = GUI.enabled ? 1 : 0.5f;

			if( EditorGUIUtility.hotControl == GuiId )
			{
				rect.xMin += 5;
				rect.xMax -= 5;
				alpha = 0.7f;
				Color c = GUI.color; c.a = alpha;
				GUI.color = c;
			}

			Rect hierarchyHeaderRect = rect; hierarchyHeaderRect.width = hierarchyWidth; hierarchyHeaderRect.height = HEADER_HEIGHT; 
		
			Rect timelineHeaderRect = rect; timelineHeaderRect.height = _showHeader ? HEADER_HEIGHT : 0;

			Rect trackRect = timelineHeaderRect;
			trackRect.yMin = timelineHeaderRect.yMax;
			trackRect.height = TRACK_HEIGHT;

			if( Event.current.type == EventType.Repaint )
			{
				GUI.color = FGUI.GetTimelineColor();
				GUI.DrawTexture( timelineHeaderRect, EditorGUIUtility.whiteTexture );
				GUI.color = new Color(1f, 1f, 1f, alpha);

			}

			if( _showTracks )
			{
				for( int i = 0; i != _trackEditors.Count; ++i )
				{
//					Vector3 upperLeft = trackRect.min;
					Vector3 lowerLeft = new Vector3( trackRect.xMin, trackRect.yMax, 0 );
					Vector3 lowerRight = new Vector3( trackRect.xMax, trackRect.yMax, 0 );

					Handles.color = FGUI.GetLineColor();

					if( _trackDragged != null )
					{
						if( _trackDragged == _trackEditors[i]  )
						{
//							Handles.DrawLine( upperLeft, upperLeft + new Vector3(trackRect.width, 0, 0 ) );
							Handles.DrawLine( lowerLeft, lowerRight );
							trackRect.y += TRACK_HEIGHT;
							continue;
						}

						if( i < _trackDragged.GetRuntimeObject().GetId() && Event.current.mousePosition.y < trackRect.yMax )
							_trackEditors[i].SetOffset( new Vector2(0, TRACK_HEIGHT) );
						else if( i > _trackDragged.GetRuntimeObject().GetId() && Event.current.mousePosition.y > trackRect.yMin )
							_trackEditors[i].SetOffset( new Vector2(0, -TRACK_HEIGHT) );
						else
							_trackEditors[i].SetOffset( Vector2.zero );
					}

					GUI.color = new Color(0.3f, 0.3f, 0.3f, alpha);

					GUI.color = new Color(1f, 1f, 1f, alpha);
					_trackEditors[i].Render( trackRect, hierarchyWidth, viewRange, pixelsPerFrame );

//					Handles.DrawLine( upperLeft, upperLeft + new Vector3(trackRect.width, 0, 0 ) );
					Handles.DrawLine( lowerLeft, lowerRight );

					trackRect.y += TRACK_HEIGHT;
				}

				if( _trackDragged != null )
				{
					Rect r = trackRect;
					r.y = Event.current.mousePosition.y;
					_trackDragged.Render( r, hierarchyWidth, viewRange, pixelsPerFrame );
				}


			}

			if( _showHeader )
			{

				Rect hierarchyLabelRect = hierarchyHeaderRect;
				hierarchyLabelRect.height = 20;
				hierarchyLabelRect.xMax = hierarchyLabelRect.xMax-23;

				Rect foldoutRect = hierarchyLabelRect;
				foldoutRect.width = 16;
				hierarchyLabelRect.xMin += 16;

				string timelineHeaderName = _timeline.Owner != null ? _timeline.Owner.name : _timeline.name + " (Missing)";

				GUI.Label( hierarchyLabelRect, new GUIContent(timelineHeaderName), FGUI.GetTimelineHeaderStyle() );

				Handles.DrawLine( timelineHeaderRect.min, new Vector3( timelineHeaderRect.xMax, timelineHeaderRect.yMin ) );
				Handles.DrawLine( new Vector3( timelineHeaderRect.xMin, timelineHeaderRect.yMax ), timelineHeaderRect.max );

				_showTracks = EditorGUI.Foldout( foldoutRect, _showTracks, GUIContent.none );

				switch( Event.current.type )
				{
				case EventType.ContextClick:
					if( hierarchyHeaderRect.Contains( Event.current.mousePosition ) )
					{
						GenericMenu menu = new GenericMenu();

						if( Selection.activeGameObject == null || PrefabUtility.GetPrefabType(Selection.activeGameObject) == PrefabType.Prefab || PrefabUtility.GetPrefabType(Selection.activeGameObject) == PrefabType.ModelPrefab )
							menu.AddDisabledItem( new GUIContent("Change Owner") );
						else
							menu.AddItem( new GUIContent("Change Owner to " + Selection.activeGameObject.name ), false, ChangeOwner );

						menu.AddItem( new GUIContent("Duplicate Timeline"), false, DuplicateTimeline );
						menu.AddItem( new GUIContent("Delete Timeline"), false, DeleteTimeline );
						
						menu.ShowAsContext();
						Event.current.Use();
					}
					break;
				case EventType.MouseDrag:
					if( EditorGUIUtility.hotControl == GuiId )
					{
						SequenceEditor.Repaint();
						Event.current.Use();
					}
					break;
				case EventType.MouseUp:
					if( EditorGUIUtility.hotControl == GuiId )
					{
						EditorGUIUtility.hotControl = 0;
						_offsetAnim.value = _offsetAnim.target = Vector2.zero;
						SequenceEditor.Repaint();
						
						SequenceEditor.StopTimelineDrag();
						Event.current.Use();
					}
					break;
					
				case EventType.Repaint:
//					Handles.color = Color.red; //FGUI.GetLineColor();
//					Handles.DrawLine( new Vector3(rect.xMin, rect.yMax, 0), new Vector3( rect.xMax, rect.yMax, 0) );
					break;
					
				case EventType.KeyDown:
					if( EditorGUIUtility.hotControl == GuiId && Event.current.keyCode == KeyCode.Escape )
					{
						EditorGUIUtility.hotControl = 0;
						Event.current.Use();
					}
					break;
				}

				Rect timelineOptionsRect = hierarchyHeaderRect;
				timelineOptionsRect.xMin = hierarchyHeaderRect.xMax - 20;
				timelineOptionsRect.yMin = hierarchyHeaderRect.yMin;
				timelineOptionsRect.width = 14;
				timelineOptionsRect.height = 14;

				if( Event.current.type == EventType.MouseDown && timelineOptionsRect.Contains(Event.current.mousePosition) )
				{
	                Event.current.Use();

					GenericMenu menu = new GenericMenu();
					Type[] types = typeof(FEvent).Assembly.GetTypes();
					List<KeyValuePair<Type, FEventAttribute>> validTypeList = new List<KeyValuePair<Type, FEventAttribute>>();

					foreach( Type t in types )
					{
						if( !typeof(FEvent).IsAssignableFrom( t ) )
							continue;

						object[] attributes = t.GetCustomAttributes(typeof(FEventAttribute), false);
						if( attributes.Length == 0 || ((FEventAttribute)attributes[0]).menu == null )
							continue;

						validTypeList.Add( new KeyValuePair<Type, FEventAttribute>(t, (FEventAttribute)attributes[0]) );
					}

					validTypeList.Sort( delegate(KeyValuePair<Type, FEventAttribute> x, KeyValuePair<Type, FEventAttribute> y) 
					{
						return x.Value.menu.CompareTo( y.Value.menu );
					});

					foreach( KeyValuePair<Type, FEventAttribute> kvp in validTypeList )
					{
						menu.AddItem( new GUIContent(kvp.Value.menu), false, AddTrackMenu, kvp );
					}

					menu.ShowAsContext();
				}

//				GUI.color = FGUI.GetTextColor();
				GUI.color = FGUI.GetIconColor();

				GUI.DrawTexture( timelineOptionsRect, (Texture2D)AssetDatabase.LoadAssetAtPath(FUtility.GetFluxSkinPath()+"Plus.png", typeof(Texture2D)) );

				if( Event.current.type == EventType.MouseDown && hierarchyHeaderRect.Contains( Event.current.mousePosition ) )
				{
					if( Event.current.button == 0 ) // dragging
					{
						EditorGUIUtility.hotControl = GuiId;

						_offsetAnim.value = _offsetAnim.target = new Vector2( 0, hierarchyHeaderRect.yMin ) - Event.current.mousePosition;

						SequenceEditor.StartTimelineDrag( this );

						Event.current.Use();
					}
				}
			}
		}*/

		protected override void PopulateContextMenu( GenericMenu menu )
		{
			base.PopulateContextMenu( menu );
			if( Selection.activeTransform != null && Selection.activeTransform != Timeline.Owner )
				menu.AddItem( new GUIContent("Change Owner to " + Selection.activeTransform.name), false, ChangeOwner, Selection.activeTransform );
			else
				menu.AddDisabledItem( new GUIContent( "Change Owner") );

			menu.AddSeparator(null);
		}

		void AddTrackMenu( object param )
		{
			KeyValuePair<Type, FEventAttribute> kvp = (KeyValuePair<Type, FEventAttribute>)param;

			Undo.RecordObjects( new UnityEngine.Object[]{Timeline, this}, "add Track" );

			FTrack track = (FTrack)typeof(FTimeline).GetMethod("Add", new Type[]{typeof(FrameRange)}).MakeGenericMethod( kvp.Key ).Invoke( Timeline, new object[]{SequenceEditor.ViewRange} );

			string evtName = track.gameObject.name;

			int nameStart = 0;
			int nameEnd = evtName.Length;
			if( nameEnd > 2 && evtName[0] == 'F' && char.IsUpper(evtName[1]) )
				nameStart = 1;
			if( evtName.EndsWith("Event") )
				nameEnd = evtName.Length - "Event".Length;
			evtName = evtName.Substring( nameStart, nameEnd - nameStart );

			track.gameObject.name = ObjectNames.NicifyVariableName( evtName );

			if( !Timeline.Sequence.IsStopped )
				track.Init();

			SequenceEditor.Refresh();

			Undo.RegisterCreatedObjectUndo( track.gameObject, string.Empty );

			SequenceEditor.SelectExclusive( SequenceEditor.GetEditor<FEventEditor>( track.GetEvent(0) ) );
		}

		void ChangeOwner( object newOwnerTransform )
		{
			Transform newOwner = (Transform)newOwnerTransform;
			Undo.RecordObject( Timeline, "Change Timeline Owner" );
			Timeline.SetOwner( newOwner );

			if( !SequenceEditor.Sequence.IsStopped )
				Timeline.Init();
		}

		void DuplicateTimeline()
		{
			UnityEngine.Object[] objsToSave = new UnityEngine.Object[]{ SequenceEditor, Timeline.Container };
			Undo.RecordObjects( objsToSave, string.Empty );
			GameObject duplicateTimeline = (GameObject)Instantiate( Timeline.gameObject );
			duplicateTimeline.name = Timeline.gameObject.name;
			Undo.SetTransformParent( duplicateTimeline.transform, Timeline.Container.transform, string.Empty );
			Undo.RegisterCreatedObjectUndo( duplicateTimeline, "duplicate Timeline" );

			if( !SequenceEditor.Sequence.IsStopped )
				duplicateTimeline.GetComponent<FTimeline>().Init();
		}

		void DeleteTimeline()
		{
			UnityEngine.Object[] objsToSave = new UnityEngine.Object[]{ Timeline.Container, Timeline };
			Undo.RegisterCompleteObjectUndo( objsToSave, string.Empty );
			OnDelete();
			Undo.SetTransformParent( Timeline.transform, null, string.Empty );
			Timeline.Container.Remove( Timeline );
			Undo.DestroyObjectImmediate( Timeline.gameObject );
		}

//		public void StartTrackDrag( FTrackEditor trackEditor )
//		{
//			_trackDragged = trackEditor;
////			_sequenceEditor.StartTimelineDrag( this );
//		}

//		public void StopTrackDrag()
//		{
//			if( _trackDragged == null )
//				return;
//
//			Rect trackRect = Rect;
//			trackRect.yMin += HEADER_HEIGHT;
//			trackRect.height = TRACK_HEIGHT;
//
//			int newPos = -1;
//
//			float mouseY = Event.current.mousePosition.y;
//
//			for( int i = 0; i != Editors.Count; ++i )
//			{
//				if( mouseY >= trackRect.yMin && mouseY <= trackRect.yMax )
//				{
//					newPos = i;
//					break;
//				}
//
//				trackRect.y += TRACK_HEIGHT;
//			}
//
//			string undoMoveTrackStr = "move Track";
//
//			if( newPos == -1 )
//			{
//				Undo.SetTransformParent( _trackDragged.Track.transform, Timeline.transform, undoMoveTrackStr );
//				Editors.RemoveAt( _trackDragged.Obj.GetId() );
//				if( mouseY > trackRect.yMin )
//				{
//					_trackDragged.Track.transform.SetAsLastSibling();
//					Editors.Add( _trackDragged );
//				}
//				else
//				{
//					_trackDragged.Track.transform.SetAsFirstSibling();
//					Editors.Insert( 0, _trackDragged );
//				}
//			}
//			else
//			{
//				if( newPos != _trackDragged.Track.GetId() )
//				{
//					Undo.SetTransformParent( _trackDragged.Track.transform, Timeline.transform, undoMoveTrackStr );
//					_trackDragged.Track.transform.SetSiblingIndex( newPos );
//
//					Editors[_trackDragged.Obj.GetId()] = null;
//					Editors.Insert( newPos, _trackDragged );
//					Editors.Remove( null );
//				}
//			}
//
//			_trackDragged = null;
//
//			for( int i = 0; i != Editors.Count; ++i )
//				Editors[i].SetOffset( Vector2.zero, true );
//
//			ContainerEditor.SequenceEditor.CancelTimelineDrag();
//		}

//		public void CancelTrackDrag()
//		{
//			if( _trackDragged == null )
//				return;
//
//			_trackDragged = null;
//
//			ContainerEditor.SequenceEditor.CancelTimelineDrag();
//		}

		public override FSequenceEditor SequenceEditor { get { return ContainerEditor.SequenceEditor; } }

//		public override FObject GetRuntimeObject()
//		{
//			return Timeline;
//		}

		public void UpdateTracks( int frame, float time )
		{
			for( int i = 0; i != Editors.Count; ++i )
			{
				if( !Editors[i].Track.enabled ) continue;
				Editors[i].UpdateEventsEditor( frame, time );
			}
		}

		/*public void CreatePreviews()
		{
			for( int i = 0; i != Editors.Count; ++i )
			{
				if( Editors[i].Track.PreviewDirtiesScene )
					Editors[i].Track.CreateCache();
//				if( !(_trackEditors[i] is FPreviewableTrackEditor)
//				   && !(_trackEditors[i] is FAnimationTrackEditor) )
//					continue;

//				FPreviewableTrackEditor previewTrackEditor = (FPreviewableTrackEditor)_trackEditors[i];
//				if( previewTrackEditor != null )
//					previewTrackEditor.CreatePreview();

//				FAnimationTrackEditor animTrackEditor = (FAnimationTrackEditor)_trackEditors[i];
//
//				if( animTrackEditor != null )
//					animTrackEditor.PreviewInSceneView = true;
			}
		}*/

		/*public void ClearPreviews()
		{
			for( int i = 0; i != Editors.Count; ++i )
			{
				Editors[i].Track.ClearCache();
//				if( !(_trackEditors[i] is FPreviewableTrackEditor)
//				   && !(_trackEditors[i] is FAnimationTrackEditor) )
//					continue;
				
				//				FPreviewableTrackEditor previewTrackEditor = (FPreviewableTrackEditor)_trackEditors[i];
				//				if( previewTrackEditor != null )
				//					previewTrackEditor.CreatePreview();
				
//				FAnimationTrackEditor animTrackEditor = (FAnimationTrackEditor)_trackEditors[i];
//				
//				if( animTrackEditor != null )
//					animTrackEditor.PreviewInSceneView = false;
			}
		}*/
	}
}
