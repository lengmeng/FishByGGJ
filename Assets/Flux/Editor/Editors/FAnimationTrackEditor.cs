using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;
using System;

using Flux;

namespace FluxEditor
{
	[FEditor(typeof(FAnimationTrack))]
	public class FAnimationTrackEditor : FTrackEditor {

		#region Debug tools

		private bool _syncWithAnimationWindow = false;
		private bool SyncWithAnimationWindow { 
			get { return _syncWithAnimationWindow; }
			set {

				if( value )
				{
					SequenceEditor.OnUpdateEvent.AddListener( OnUpdate );
					foreach( FContainerEditor containerEditor in SequenceEditor.Editors )
					{
						List<FTimelineEditor> timelineEditors = containerEditor.Editors;
						foreach( FTimelineEditor timelineEditor in timelineEditors )
						{
							List<FTrackEditor> trackEditors = timelineEditor.Editors;
							foreach( FTrackEditor trackEditor in trackEditors )
							{
								if( trackEditor is FAnimationTrackEditor && ((FAnimationTrackEditor)trackEditor).SyncWithAnimationWindow )
									((FAnimationTrackEditor)trackEditor).SyncWithAnimationWindow = false;
							}
						}
					}

					AnimationWindowProxy.OpenAnimationWindow();
				}
				else
				{
					if( TimelineEditor != null )
						SequenceEditor.OnUpdateEvent.RemoveListener( OnUpdate );

					if( AnimationMode.InAnimationMode() )
						AnimationMode.StopAnimationMode();
				}

				_syncWithAnimationWindow = value;

				if( TimelineEditor != null )
					SequenceEditor.Repaint();
			}
		}

		private bool _showKeyframes = false;
		public bool ShowKeyframes
		{
			get { return _showKeyframes; }
			set { _showKeyframes = value; }
		}
		private bool _showKeyframeTimes = false;
		public bool ShowKeyframeTimes
		{
			get { return _showKeyframeTimes; }
			set { _showKeyframeTimes = value; }
		}


		private bool _showTransformPath = false;
		public bool ShowTransformPath
		{
			get{ return _showTransformPath; }
			set{ _showTransformPath = value; }
		}

		#endregion

        protected override void OnEnable()
        {
            base.OnEnable();

            SceneView.onSceneGUIDelegate += OnSceneGUI;

			// was it syncing anim? hook it up again, because UnityEvent will 
			// lose references on compile
			if( _syncWithAnimationWindow )
			{
				_syncWithAnimationWindow = false;
//				SyncWithAnimationWindow = true;
			}
        }

		private void OnUpdate()
		{
			if( !_syncWithAnimationWindow )
				return;

			if( Selection.activeTransform != Track.Owner )
			{
				Selection.activeTransform = Track.Owner;
			}

			if( !AnimationMode.InAnimationMode() )
			{
//				AnimationMode.StartAnimationMode();
				AnimationWindowProxy.StartAnimationMode();
			}

//			if( _track.IsPreviewing )
			{
				int animWindowFrame = AnimationWindowProxy.GetCurrentFrame();

				FEvent[] evts = new FEvent[2];
				int numEvts = Track.GetEventsAt( SequenceEditor.Sequence.CurrentFrame, ref evts );

				if( numEvts > 0 )
				{
					FPlayAnimationEvent animEvt = (FPlayAnimationEvent)evts[numEvts-1];
					if( animEvt.ControlsAnimation )
					{
						int normCurrentFrame = SequenceEditor.Sequence.CurrentFrame - animEvt.Start;

						if( animWindowFrame > animEvt.Length )
						{
							animWindowFrame = animEvt.Length;
							AnimationWindowProxy.SetCurrentFrame( animWindowFrame, animEvt.LengthTime );
						}

						if( animWindowFrame >= 0 && animWindowFrame != normCurrentFrame )
						{
							SequenceEditor.SetCurrentFrame( animEvt.Start + animWindowFrame );
							SequenceEditor.Repaint();
	//						Debug.Log( "AnimWindow->Flux: " + (animEvt.Start + animWindowFrame) );
						}
					}
				}
			}

		}

        protected override void OnDestroy()
        {
            base.OnDestroy();

            SceneView.onSceneGUIDelegate -= OnSceneGUI;

			if( SyncWithAnimationWindow )
				SyncWithAnimationWindow = false;
        }

        public override void Init( FObject obj, FEditor owner )
        {
            base.Init( obj, owner );

			FAnimationTrack animTrack = (FAnimationTrack)Obj;

			if( animTrack.Owner.GetComponent<Animator>() == null )
			{
				Animator animator = animTrack.Owner.gameObject.AddComponent<Animator>();
				Undo.RegisterCreatedObjectUndo( animator, string.Empty );
			}
        }

		public override void OnTrackChanged()
		{
//			Debug.LogWarning("FAnimationTrackEditor.OnTrackChanged");

			FAnimationTrackInspector.RebuildStateMachine( (FAnimationTrack)Track );
		}

//		public override void OnTogglePreview ()
//		{
////			int currentFrame = Track.Sequence.GetCurrentFrame();
//
//			base.OnTogglePreview();
//
////			if( currentFrame >= 0 )
////				SequenceEditor.SetCurrentFrame( currentFrame );
//		}

		public override void Render( Rect rect, float headerWidth )
		{
//			bool isPreviewing = _track.IsPreviewing;

			base.Render( rect, headerWidth );

//			if( isPreviewing != _track.IsPreviewing )
//			{
//				if( Event.current.alt )
//					SyncWithAnimationWindow = true;
//			}

			switch( Event.current.type )
			{
			case EventType.DragUpdated:
				if( rect.Contains(Event.current.mousePosition ) )
				{
					int numAnimationsDragged = FAnimationEventInspector.NumAnimationsDragAndDrop( Track.Sequence.FrameRate );
					int frame = SequenceEditor.GetFrameForX( Event.current.mousePosition.x );

					DragAndDrop.visualMode = numAnimationsDragged > 0 && Track.CanAddAt(frame) ? DragAndDropVisualMode.Copy : DragAndDropVisualMode.Rejected;
					Event.current.Use();
				}
				break;
			case EventType.DragPerform:
				if( rect.Contains(Event.current.mousePosition ) )
				{
					AnimationClip animClip = FAnimationEventInspector.GetAnimationClipDragAndDrop( Track.Sequence.FrameRate );

					if( animClip && Mathf.Approximately(animClip.frameRate, Track.Sequence.FrameRate) )
					{
						int frame = SequenceEditor.GetFrameForX( Event.current.mousePosition.x );
                        int maxLength;

						if( Track.CanAddAt( frame, out maxLength ) )
						{
							FPlayAnimationEvent animEvt = FEvent.Create<FPlayAnimationEvent>( new FrameRange( frame, frame + Mathf.Min(maxLength, Mathf.RoundToInt(animClip.length*animClip.frameRate))  ) );
                            Track.Add( animEvt );
                            FAnimationEventInspector.SetAnimationClip( animEvt, animClip );
							DragAndDrop.AcceptDrag();
						}
					}

					Event.current.Use();
				}
				break;
			}

//			if( _wasPreviewing != _track.IsPreviewing )
//			{
//				if( _wasPreviewing )
//					SequenceEditor.OnUpdateEvent.RemoveListener( OnUpdate );
//				else
//					SequenceEditor.OnUpdateEvent.AddListener( OnUpdate );
//
//				_wasPreviewing = _track.IsPreviewing;
//			}
		}

		public override bool HasTools()
		{
			bool controlsAllAnimations = true;
			foreach( FAnimationEventEditor animEvtEditor in _eventEditors )
			{
				if( !((FPlayAnimationEvent)animEvtEditor.Evt).ControlsAnimation )
				{
					controlsAllAnimations = false;
					break;
				}
			}
			return controlsAllAnimations;
		}

		public override void OnToolsGUI ()
		{
//			bool canSyncWithAnimationWindow = true;
//			foreach( FAnimationEventEditor animEvtEditor in _eventEditors )
//			{
//				if(! ((FPlayAnimationEvent)animEvtEditor._evt).ControlsAnimation )
//				{
//					canSyncWithAnimationWindow = false;
//					break;
//				}
//			}
//
//			if( canSyncWithAnimationWindow )
			{
				bool syncWithAnimationWindow = EditorGUILayout.Toggle( "Sync w/ Animation Window", SyncWithAnimationWindow );
				if( syncWithAnimationWindow != SyncWithAnimationWindow )
					SyncWithAnimationWindow = syncWithAnimationWindow;
			}

			bool showTransformPath = EditorGUILayout.Toggle( "Show Transform Path", ShowTransformPath );
			if( showTransformPath != ShowTransformPath )
				ShowTransformPath = showTransformPath;

			if( !ShowTransformPath )
				GUI.enabled = false;

			bool showKeyframes = EditorGUILayout.Toggle( "Show Keyframes", ShowKeyframes );
			if( showKeyframes != ShowKeyframes )
				ShowKeyframes = showKeyframes;

			bool showKeyframeTimes = EditorGUILayout.Toggle( "Show Keyframe Times", ShowKeyframeTimes );
			if( showKeyframeTimes != ShowKeyframeTimes )
				ShowKeyframeTimes = showKeyframeTimes;

			GUI.enabled = true;
		}

		protected override Color GetPreviewIconColor()
		{
			return _syncWithAnimationWindow ? Color.red : base.GetPreviewIconColor();
		}

        void OnSceneGUI( SceneView sceneView )
		{
			if( Track == null )
				return;

			for( int i = 0; i != _eventEditors.Count; ++i )
			{
				FAnimationEventEditor animEvtEditor = (FAnimationEventEditor)_eventEditors[i];
				FPlayAnimationEvent animEvt = (FPlayAnimationEvent)_eventEditors[i].Evt;
				if( animEvt._animationClip != null && Flux.FUtility.IsAnimationEditable(animEvt._animationClip) && ShowTransformPath /*_track.IsPreviewing*/ )
				{
					animEvtEditor.RenderTransformCurves( animEvt.Sequence.FrameRate );
				}
			}

			SceneView.RepaintAll();
		}

		/*private void PreviewAnimationEvent( FAnimationEventEditor animEvtEditor, int frame )
		{
			FPlayAnimationEvent animEvt = (FPlayAnimationEvent)animEvtEditor._evt;

			if( animEvt._animationClip == null )
				return;

			bool isEditable = Flux.FUtility.IsAnimationEditable(animEvt._animationClip);

			// render path
			if( isEditable )
			{
				TransformCurves transformCurves = new TransformCurves(animEvt.Owner, animEvt._animationClip);

				RenderTransformPath( transformCurves, animEvt.LengthTime, 1f/animEvt.Sequence.FrameRate );

				float t = (float)(frame + animEvt._startOffset - animEvt.Start) / animEvt.Sequence.FrameRate;

				if( animEvt.FrameRange.Contains( frame ) )
				{
//					float t = (float)(frame + animEvt._startOffset - animEvt.Start) / animEvt.Sequence.FrameRate;
					RenderTransformAnimation( transformCurves, t );
				}

//				AnimationClipCurveData[] allCurves = AnimationUtility.GetAllCurves( animEvt._animationClip, true );
//				foreach( AnimationClipCurveData curve in allCurves )
//				{
//
//				}
			}
			else if( animEvt.FrameRange.Contains( frame ) )
			{
				float t = (float)(frame + animEvt._startOffset - animEvt.Start) / animEvt.Sequence.FrameRate;

				bool wasInAnimationMode = AnimationMode.InAnimationMode();

				if( !AnimationMode.InAnimationMode() )
				{
					AnimationMode.StartAnimationMode();
				}
				AnimationMode.BeginSampling();
				AnimationMode.SampleAnimationClip( animEvt.Owner.gameObject, animEvt._animationClip, t );
				AnimationMode.EndSampling();

				if( !wasInAnimationMode )
					AnimationMode.StopAnimationMode();
			}
		}*/


		private void RenderTransformPath( TransformCurves transformCurves, float length, float samplingDelta )
		{
			float t = 0; 

			int numberSamples = Mathf.RoundToInt(length/samplingDelta)+1;

			float delta = length / numberSamples;

			Vector3[] pts = new Vector3[numberSamples];

			int index = 0;

			while( index < numberSamples )
			{
				pts[index++] = transformCurves.GetPosition( t );
				t += delta;
			}

			if( index != pts.Length )
				Debug.LogError("Number of samples doesn't match: " + (index+1) + " instead of " + pts.Length);

			Handles.DrawPolyLine( pts );
		}

		private void RenderTransformAnimation( TransformCurves transformCurves, float time )
        {
	        Vector3 pos = transformCurves.GetPosition(time);//new Vector3( xPos.Evaluate(t), yPos.Evaluate(t), zPos.Evaluate(t) );
	        Quaternion rot = transformCurves.GetRotation(time);
			Vector3 scale = transformCurves.GetScale(time);

			transformCurves.bone.localScale = scale;
			transformCurves.bone.localRotation = rot;
			transformCurves.bone.localPosition = pos;

	        Handles.RectangleCap( 0, pos, rot, 0.1f );
	        Handles.RectangleCap( 0, pos + rot*Vector3.forward, rot, 0.4f );
		}

		public override void UpdateEventsEditor (int frame, float time)
		{
			if( Track.RequiresEditorCache && Track.CanPreview && !Track.HasCache )
			{
				Track.CanPreview = false;
				OnTogglePreview( true );
			}

			base.UpdateEventsEditor (frame, time);
			FEvent[] evts = new FEvent[2];
			int numEvts = Track.GetEventsAt( frame, ref evts );
			if( numEvts > 0 )
			{
//				if( AnimationMode.InAnimationMode() )
				if( _syncWithAnimationWindow )//&& _track.IsPreviewing )
				{
//					Debug.Log( SequenceEditor.OnUpdateEvent );
//					AnimationMode.StartAnimationMode();

//					System.Type t = typeof(EditorWindow).Assembly.GetType("UnityEditor.AnimationWindow");
//					EditorWindow animWindow = EditorWindow.GetWindow( t );
//					
//
//					PropertyInfo stateProperty = t.GetProperty("state", BindingFlags.Instance | BindingFlags.Public);
//					object state = stateProperty.GetValue( animWindow, null );
//
//					Type stateT = typeof(EditorWindow).Assembly.GetType("UnityEditorInternal.AnimationWindowState");
//					FieldInfo timeField = stateT.GetField("m_PlayTime");
//					timeField.SetValue( state, time );
//
//					FieldInfo frameField = stateT.GetField("m_Frame");
//					frameField.SetValue( state, frame );

//					if( !AnimationMode.InAnimationMode() )
//						AnimationMode.StartAnimationMode();

					FPlayAnimationEvent animEvt = (FPlayAnimationEvent)evts[numEvts-1];
					AnimationWindowProxy.SelectAnimationClip( animEvt._animationClip );
					AnimationWindowProxy.SetCurrentFrame( frame - animEvt.Start, time - animEvt.StartTime );
//					Debug.Log("Flux->AnimWindow: " + (frame - animEvt.Start));
				}
			}
		}
    }
}
